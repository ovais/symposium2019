using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Demo.Model.XConnect.Events;
using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;

namespace Sitecore.Demo.CortexWorkers
{
    public class RunnersWorker : IDistributedWorker<Contact>
    {
        private readonly IXdbContext _xdbContext;
        private readonly int _morningEndHours;
        private readonly int _eveningStartHours;

        public RunnersWorker(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
            _morningEndHours = int.Parse(options["MorningHoursEnd"], CultureInfo.InvariantCulture);
            _eveningStartHours = int.Parse(options["EveningHoursStart"], CultureInfo.InvariantCulture);
        }

        public Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {
            foreach (var contact in batch)
            {
                var runEnded = contact.Interactions.SelectMany(x => x.Events).OfType<RunEnded>()
                    .ToArray();

                var totalRuns = runEnded.Length;

                if (totalRuns == 0)
                {
                    continue;
                }

                var totalRunsReciprocal = 1 / (double) totalRuns;

                var morningRunner = runEnded.Count(x => x.Time.Hour < _morningEndHours) * totalRunsReciprocal;
                var eveningRunner = runEnded.Count(x => x.Time.Hour > _eveningStartHours) * totalRunsReciprocal;

                var runnerFacet = contact.GetFacet<RunnerFacet>();
                var averageMultiplier = 0.5;

                if (runnerFacet == null)
                {
                    runnerFacet = new RunnerFacet();
                    averageMultiplier = 1;
                }

                runnerFacet.IsMorningRunner =
                    (runnerFacet.IsMorningRunner + morningRunner) * averageMultiplier;

                runnerFacet.IsEveningRunner =
                    (runnerFacet.IsEveningRunner + eveningRunner) * averageMultiplier;

                _xdbContext.SetFacet(contact, runnerFacet);
            }

            return _xdbContext.SubmitAsync(token);
        }

        public void Dispose() => _xdbContext.Dispose();
    }
}
