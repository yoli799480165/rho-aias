namespace Chaldea.Fate.RhoAias;

public class DnsZone
{
    public Guid Id { get; set; }
    public string Domain { get; set; } = default!;
    public ICollection<DnsRecord>? ResourceRecords { get; set; }
}

public enum DnsRecordType
{
    A = 1,
    NS = 2,
    CNAME = 5,
    SOA = 6,
    MB = 7,
    MG = 8,
    MR = 9,
    NULL = 10,
    WKS = 11,
    PTR = 12,
    HINFO = 13,
    MINFO = 14,
    MX = 15,
    TXT = 16,
    AAAA = 28,
}

public class DnsRecord
{
    public Guid Id { get; set; }

    public string SubZoneName { get; set; } = default!;

    public DnsRecordType ResourceType { get; set; }

    public int Ttl { get; set; } = 3600;

    public ushort ResourceClasses { get; set; } = 1;

    public string? Address { get; set; }
}
