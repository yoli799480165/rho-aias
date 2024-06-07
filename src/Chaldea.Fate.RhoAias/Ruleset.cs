namespace Chaldea.Fate.RhoAias;

public class Ruleset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Rule { get; set; } = default!;
    public bool Disabled { get; set; }
}