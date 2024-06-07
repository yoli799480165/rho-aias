namespace Chaldea.Fate.RhoAias.Security.WAF;

internal class WAFDataSeeder : IDataSeeder
{
    private readonly IRepository<Ruleset> _rulesetRepository;

    public WAFDataSeeder(IRepository<Ruleset> rulesetRepository)
    {
        _rulesetRepository = rulesetRepository;
    }

    public async Task SeedAsync()
    {
        await CreateDefaultRulesAsync();
    }

    private async Task CreateDefaultRulesAsync()
    {
        if (await _rulesetRepository.AnyAsync())
        {
            return;
        }

        var rules = new List<Ruleset>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "",
                Rule = new
                {

                }.ToJson()
            }
        };

        await _rulesetRepository.InsertManyAsync(rules);
    }
}