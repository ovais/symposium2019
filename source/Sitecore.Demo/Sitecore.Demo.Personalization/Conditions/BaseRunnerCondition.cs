using Sitecore.Analytics;
using Sitecore.Analytics.XConnect.Facets;
using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.XConnect;

namespace Sitecore.Demo.Personalization.Conditions
{
    public abstract class BaseRunnerCondition<T> : OperatorCondition<T> where T : RuleContext
    {
        public double Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.IsNotNull(Tracker.Current, "Tracker.Current must be not null");
            Assert.IsNotNull(Tracker.Current.Contact, "Tracker.Current.Contact must be not null");

            var runnerFacet = GetRunnerFacet();
            if (runnerFacet != null)
            {
                double runnerValue = GetRunnerValue(runnerFacet);
                switch (GetOperator())
                {
                    case ConditionOperator.Equal:
                        return runnerValue == Value;
                    case ConditionOperator.GreaterThanOrEqual:
                        return runnerValue >= Value;
                    case ConditionOperator.GreaterThan:
                        return runnerValue > Value;
                    case ConditionOperator.LessThanOrEqual:
                        return runnerValue <= Value;
                    case ConditionOperator.LessThan:
                        return runnerValue < Value;
                    case ConditionOperator.NotEqual:
                        return runnerValue != Value;
                    default:
                        return false;
                }
            }

            return false;
        }

        protected abstract double GetRunnerValue(RunnerFacet runnerFacet);

        private RunnerFacet GetRunnerFacet()
        {
            var xconnectFacets = Tracker.Current.Contact.GetFacet<IXConnectFacets>("XConnectFacets");
            Facet runnerFacet = null;

            if (xconnectFacets?.Facets?
                    .TryGetValue(RunnerFacet.DefaultFacetKey, out runnerFacet) ?? false)
                return runnerFacet as RunnerFacet;

            return null;
        }
    }
}
