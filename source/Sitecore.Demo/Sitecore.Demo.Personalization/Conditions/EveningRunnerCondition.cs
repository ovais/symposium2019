using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Rules;

namespace Sitecore.Demo.Personalization.Conditions
{
    public class EveningRunnerCondition<T> : BaseRunnerCondition<T> where T : RuleContext
    {
        protected override double GetRunnerValue(RunnerFacet runnerFacet)
        {
            return 0.5;
            //return runnerFacet.IsEveningRunner;
        }
    }
}