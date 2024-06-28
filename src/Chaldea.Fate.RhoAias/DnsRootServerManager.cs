namespace Chaldea.Fate.RhoAias;

public interface IDnsRootServerManager
{
    Task CreateDnsRootServerAsync(DnsRootServer entity);
    Task<List<DnsRootServer>> GetDnsRootServerListAsync();
}

internal class DnsRootServerManager : IDnsRootServerManager
{
    private readonly IRepository<DnsRootServer> _repository;

    public DnsRootServerManager(IRepository<DnsRootServer> repository)
    {
        _repository = repository;
    }

    public async Task CreateDnsRootServerAsync(DnsRootServer entity)
    {
        await _repository.InsertAsync(entity);
    }

    public async Task<List<DnsRootServer>> GetDnsRootServerListAsync()
    {
        var entities = await _repository.GetListAsync();
        return entities;
    }
}
