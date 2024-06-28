using DnsServer;
using DnsServer.Exceptions;
using DnsServer.Extensions;
using DnsServer.Messages.Builders;
using DnsServer.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Chaldea.Fate.RhoAias.Dns.Server;

internal class DnsHostedService : BackgroundService
{
    private readonly UdpClient _udpClient;
    private readonly DnsServerOptions _options;
    private readonly ILogger<DnsHostedService> _logger;
    private readonly IDnsRecursiveHandler _recursiveHandler;
    private readonly IDnsAuthoritativeHandler _authoritativeHandler;
    private readonly IDnsRequestFilter _dnsRequestFilter;

    public DnsHostedService(
        ILogger<DnsHostedService> logger,
        IOptions<DnsServerOptions> options,
        IOptions<RhoAiasDnsServerOptions> serverOptions,
        IDnsRecursiveHandler recursiveHandler,
        IDnsAuthoritativeHandler authoritativeHandler,
        IDnsRequestFilter dnsRequestFilter)
    {
        _logger = logger;
        _options = options.Value;
        _recursiveHandler = recursiveHandler;
        _authoritativeHandler = authoritativeHandler;
        _dnsRequestFilter = dnsRequestFilter;
        _udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(serverOptions.Value.Host), serverOptions.Value.Port));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await HandleDnsRequestAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Dns server is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }
        }
        _udpClient.Close();
    }

    private async Task HandleDnsRequestAsync(CancellationToken cancellationToken)
    {
        UdpReceiveResult receiveResult;
        try
        {

            receiveResult = await _udpClient.ReceiveAsync(cancellationToken);
        }
        catch (SocketException)
        {
            return;
        }

        var builder = new DNSResponseMessageBuilder();
        DNSRequestMessage requestMessage = null;
        try
        {
            requestMessage = DNSRequestMessage.Extract(receiveResult.Buffer);
            Validate(requestMessage);
        }
        catch (DNSNotImplementedException)
        {
            var payload = builder.BuildNotImplemented(requestMessage).Serialize();
            await _udpClient.SendAsync(payload.ToArray(), payload.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
            return;
        }
        catch (DNSRefusedException)
        {
            var payload = builder.BuildRefused(requestMessage).Serialize();
            await _udpClient.SendAsync(payload.ToArray(), payload.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
            return;
        }
        catch
        {
            var payload = builder.BuildFormatError(requestMessage).Serialize();
            await _udpClient.SendAsync(payload.ToArray(), payload.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
            return;
        }

        _logger.LogInformation($"Request {requestMessage.Header.Id} received");

        DNSResponseMessage dnsResponseMessage = null;
        try
        {
            if (!_options.ExcludeForwardRequests.Any(r => r.IsMatch(requestMessage.Questions.First().Label)))
            {
                dnsResponseMessage = await _recursiveHandler.Handle(requestMessage, cancellationToken);
            }
            else
            {
                dnsResponseMessage = await _authoritativeHandler.Handle(requestMessage, cancellationToken);
            }
        }
        catch (DNSNameErrorException)
        {
            var payload = builder.BuildNameError(requestMessage).Serialize();
            await _udpClient.SendAsync(payload.ToArray(), payload.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
            return;
        }
        catch
        {
            var payload = builder.BuildServerFailure(requestMessage).Serialize();
            await _udpClient.SendAsync(payload.ToArray(), payload.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
            return;
        }

        var response = dnsResponseMessage.Serialize();
        await _udpClient.SendAsync(response.ToArray(), response.Count(), receiveResult.RemoteEndPoint).WithCancellation(cancellationToken, _options.TimeOutInMilliSeconds);
        _logger.LogInformation($"Response {dnsResponseMessage.Header.Id} sent");
    }

    private void Validate(DNSRequestMessage requestMessage)
    {
        if (requestMessage.Questions.Count() != 1)
        {
            throw new DNSRefusedException();
        }

        foreach (var question in requestMessage.Questions)
        {
            if (!DnsServerConstants.DefaultQuestionTypes.Any(s => s.Equals(question.QType)))
            {
                throw new DNSNotImplementedException();
            }

            if (!DnsServerConstants.DefaultQuestionClasses.Any(s => s.Equals(question.QClass)))
            {
                throw new DNSBadFormatException();
            }
        }
    }
}
