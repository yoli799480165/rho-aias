using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Chaldea.Fate.RhoAias.Security.WAF;

internal class WAFMiddleware
{
    private readonly Func<WebContext, bool> _compiledRule;
    private readonly ILogger<WAFMiddleware> _logger;
    private readonly RequestDelegate _next;

    public WAFMiddleware(RequestDelegate next, ILogger<WAFMiddleware> logger, IRulesetManager rulesetManager)
    {
        _next = next;
        _logger = logger;
        _compiledRule = new MicroRuleEngine().CompileRule<WebContext>(new Rule
        {
            Operator = "OrElse",
            Rules = rulesetManager.GetRules<Rule>()
        });
    }

    public async Task Invoke(HttpContext context)
    {
        var wc = new WebContext(context);
        if (_compiledRule(wc))
        {
            _logger.LogWarning($"Forbidden request from {wc}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next.Invoke(context);
    }
}