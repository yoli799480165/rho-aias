using DnsServer.Domains;
using DnsServer.Persistence;

namespace Chaldea.Fate.RhoAias.Dns.Server;

internal class DnsZoneRepository : IDnsZoneRepository
{
    private readonly IDnsZoneManager _dnsZoneManager;

    public DnsZoneRepository(IDnsZoneManager dnsZoneManager)
    {
        _dnsZoneManager = dnsZoneManager;
    }

    public async Task<bool> AddZone(string label, CancellationToken token)
    {
        await _dnsZoneManager.CreateDnsZoneAsync(new DnsZone() { Domain = label });
        return true;
    }

    public async Task<bool> UpdateZone(DNSZone dnsZone)
    {
        var entity = MapFrom(dnsZone);
        await _dnsZoneManager.UpdateDnsZoneAsync(entity);
        return true;
    }

    public async Task<DNSZone> FindDNSZoneByLabel(string label, CancellationToken token)
    {
        var entity = await _dnsZoneManager.GetDnsZoneAsync(label);
        return MapTo(entity);
    }

    public async Task<IEnumerable<DNSZone>> FindDNSZoneByLabels(IEnumerable<string> labels, CancellationToken token)
    {
        var entities = await _dnsZoneManager.GetDnsZoneListAsync(labels);
        return entities.Select(x => MapTo(x));
    }

    public async Task<IEnumerable<DNSZone>> FindAll()
    {
        var entities = await _dnsZoneManager.GetDnsZoneListAsync();
        return entities.Select(x => MapTo(x));
    }

    private DNSZone MapTo(DnsZone entity)
    {
        var newEntity = new DNSZone(entity.Domain);
        newEntity.ResourceRecords = entity.ResourceRecords.Select(x =>
        {
            ResourceRecord record;
            switch (x.ResourceType)
            {
                case DnsRecordType.A:
                    record = new AResourceRecord(x.Ttl, x.SubZoneName, new ResourceClasses(x.ResourceClasses))
                    {
                        Address = x.Address
                    };
                    break;
                case DnsRecordType.AAAA:
                    record = new AAAAResourceRecord(x.Ttl, x.SubZoneName, new ResourceClasses(x.ResourceClasses))
                    {

                    };
                    break;
                default:
                    throw new Exception("");
            }
            return record;
        }).ToList();
        return newEntity;
    }

    private DnsZone MapFrom(DNSZone entity)
    {
        var newEntity = new DnsZone();
        newEntity.Domain = entity.ZoneLabel;
        newEntity.ResourceRecords = entity.ResourceRecords.Select(x =>
        {
            var record = new DnsRecord()
            {
                SubZoneName = x.SubZoneName,
                ResourceType = (DnsRecordType)x.ResourceType.Value,
                Ttl = x.Ttl,
            };
            switch (record.ResourceType)
            {
                case DnsRecordType.A : record.Address = (x as AResourceRecord).Address;
                    break;
                default: throw new Exception("");
            }
            return record;
        }).ToList();

        return newEntity;
    }
}
