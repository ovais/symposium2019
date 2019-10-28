using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<ActionResult> RegisterExportToGoogleBigQuery()
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

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "Sitecore.Demo.CortexWorkers.ExportToBigQueryWorker, Sitecore.Demo.CortexWorkers", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return new ContentResult { Content = taskId.ToString() };

            }
        }
        public async Task<ActionResult> RegisterRunnersTask()
        {

            string morningHoursEnd = "12";
            string eveningHoursStart = "17";
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

        public async Task<ActionResult> RegisterDeleteTask()
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

               

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "Sitecore.Demo.CortexWorkers.DeleteContactWorker, Sitecore.Demo.CortexWorkers", new Dictionary<string,string>()),
                    null,
                    TimeSpan.FromHours(1));

                return new ContentResult { Content = taskId.ToString() };
            }
        }


        

        public async Task<ActionResult> GetRunnersTaskStatus(Guid taskId)
        {
            var processingTaskProgress = await _taskManager.GetTaskProgressAsync(taskId);

            return new ContentResult {Content = processingTaskProgress.Status.ToString()};
        }

        public async Task<ActionResult> GenerateData(int amountOfContacts = 10, int amountOfInteractions = 10)
        {
            using (IXdbContext xdbContext = SitecoreXConnectClientConfiguration.GetClient())
            {
                for (var c = 0; c < amountOfContacts; c++)
                {
                    var contact = new Contact(new ContactIdentifier("sitecore.demo", Guid.NewGuid().ToString(),
                        ContactIdentifierType.Known));

                    var currentDate = DateTime.UtcNow;
                    xdbContext.AddContact(contact);

                    for (var i = 1; i <= amountOfInteractions; i++)
                    {
                        // Even are morning runners. 10AM for morning runner and 6PM for evening one. :
                        var startTime = 10 + c % 2 * 8;
                        var startDate = currentDate.Date.AddDays(-i).AddHours(startTime);
                        var endDate = startDate.AddHours(1);
                        var interaction =
                            new Interaction(contact, InteractionInitiator.Contact, /*TODO: Channel ID*/Guid.NewGuid(),
                                "Some Agent")
                            {
                                Events =
                                {
                                    new RunStarted( /*TODO: Definition ID*/Guid.NewGuid(), startDate)
                                        {Time = startDate},
                                    new RunEnded( /*TODO: Definition ID*/Guid.NewGuid(), endDate)
                                        {Time = endDate}
                                }
                            };

                        xdbContext.AddInteraction(interaction);
                    }
                }

                await xdbContext.SubmitAsync();
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> GenerateUsers()
        {
            using (IXdbContext xdbContext = SitecoreXConnectClientConfiguration.GetClient())
            {
                    var ovais = new Contact(new ContactIdentifier("letsplay", "ovais.akhter@sitecore.com",
                        ContactIdentifierType.Known));
                var sumith = new Contact(new ContactIdentifier("letsplay", "sumith.damodaran@sitecore.com",
                        ContactIdentifierType.Known));

                xdbContext.AddContact(ovais);
                xdbContext.AddContact(sumith);


                await xdbContext.SubmitAsync();
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
