using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sitecore.Demo.Model.XConnect.Events;
using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Tasks.Options.DataSources.Search;
using Sitecore.XConnect;
using Sitecore.XConnect.Client.Configuration;

namespace Sitecore.Demo.Cms
{
    public class CortexController : Controller
    {
        private readonly ITaskManager _taskManager;

        public CortexController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task<ActionResult> RegisterRunnersTask(
            string morningHoursEnd = "12", string eveningHoursStart = "17")
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var searchRequest = client.Contacts
                    .Where(c => c.Interactions.Any(i => i.Events.OfType<RunEnded>().Any(x => true)))
                    .WithExpandOptions(new ContactExpandOptions(RunnerFacet.DefaultFacetKey)
                    {
                        Interactions = new RelatedInteractionsExpandOptions
                        {
                            Limit = int.MaxValue
                        }
                    })
                    .GetSearchRequest();

                var dataSourceOptions = new ContactSearchDataSourceOptionsDictionary(searchRequest, 100, 100);

                var workerOptions = new Dictionary<string, string>
                {
                    ["MorningHoursEnd"] = morningHoursEnd,
                    ["EveningHoursStart"] = eveningHoursStart
                };

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "Sitecore.Demo.CortexWorkers.RunnersWorker, Sitecore.Demo.CortexWorkers", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return new ContentResult {Content = taskId.ToString()};
            }
        }

        public async Task<ActionResult> GetRunnersTaskStatus(Guid taskId)
        {
            var processingTaskProgress = await _taskManager.GetTaskProgressAsync(taskId);

            return new ContentResult {Content = processingTaskProgress.Status.ToString()};
        }
    }
}
