namespace Chaldea.Fate.RhoAias;

public class DnsRootServer
{
    public Guid Id { get; set; }

    public string HostName { get; set; } = default!;

    public string Manager { get; set; } = default!;

    public string IPV4Address { get; set; } = default!;

    public string IPV6Address { get; set; } = default!;
}
