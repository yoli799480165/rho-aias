namespace Chaldea.Fate.RhoAias
{
    public interface IDnsZoneManager
    {
        Task CreateDnsZoneAsync(DnsZone entity);
        Task UpdateDnsZoneAsync(DnsZone entity);
        Task<DnsZone?> GetDnsZoneAsync(string domain);
        Task<List<DnsZone>> GetDnsZoneListAsync();
        Task<List<DnsZone>> GetDnsZoneListAsync(IEnumerable<string> domains);
    }

    internal class DnsZoneManager : IDnsZoneManager
    {
        private readonly IRepository<DnsZone> _repository;

        public DnsZoneManager(IRepository<DnsZone> repository)
        {
            _repository = repository;
        }

        public async Task CreateDnsZoneAsync(DnsZone entity)
        {
            if (await _repository.AnyAsync(x => x.Domain == entity.Domain))
            {
                return;
            }
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateDnsZoneAsync(DnsZone entity)
        {
            if (await _repository.AnyAsync(x => x.Domain == entity.Domain && x.Id != entity.Id))
            {
                return;
            }
            await _repository.UpdateAsync(entity);
        }

        public async Task<List<DnsZone>> GetDnsZoneListAsync()
        {
            var entities = await _repository.GetListAsync();
            return entities;
        }

        public async Task<DnsZone?> GetDnsZoneAsync(string domain)
        {
            return await _repository.GetAsync(x => domain.EndsWith(x.Domain));
        }

        public async Task<List<DnsZone>> GetDnsZoneListAsync(IEnumerable<string> domains)
        {
            return await _repository.GetListAsync(x => domains.Contains(x.Domain));
        }
    }
}
