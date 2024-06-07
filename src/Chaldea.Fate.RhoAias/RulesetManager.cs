using System.Collections.Concurrent;
using System.Text.Json;

namespace Chaldea.Fate.RhoAias;

public interface IRulesetManager
{
    Task<List<Ruleset>> GetRulesetListAsync();
    List<T> GetRules<T>();
}

internal class RulesetManager : IRulesetManager
{
    private readonly ConcurrentDictionary<Guid, object> _rules = new();
    private readonly IRepository<Ruleset> _rulesetRepository;

    public RulesetManager(IRepository<Ruleset> rulesetRepository)
    {
        _rulesetRepository = rulesetRepository;
    }

    public Task<List<Ruleset>> GetRulesetListAsync()
    {
        return _rulesetRepository.GetListAsync();
    }

    public List<T> GetRules<T>()
    {
        if (_rules.Count == 0)
        {
            var ruleset = GetRulesetListAsync().GetAwaiter().GetResult();
            foreach (var item in ruleset)
            {
                var rule = JsonSerializer.Deserialize<T>(item.Rule);
                if (rule == null) continue;
                _rules[item.Id] = rule;
            }
        }

        return _rules.Values.Cast<T>().ToList();
    }
}