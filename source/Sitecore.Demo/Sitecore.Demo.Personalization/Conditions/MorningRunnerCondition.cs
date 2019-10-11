using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Rules;

namespace Sitecore.Demo.Personalization.Conditions
{
    public class MorningRunnerCondition<T> : BaseRunnerCondition<T> where T : RuleContext
    {
        protected override double GetRunnerValue(RunnerFacet runnerFacet)
        {
            return runnerFacet.IsMorningRunner;
        }
    }
}