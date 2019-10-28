using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;
using Sitecore.Demo.Model.XConnect.Events;
using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;

namespace Sitecore.Demo.CortexWorkers
{ 

    public class ExportToBigQueryWorker : IDistributedWorker<Contact>
    {
        private readonly IXdbContext _xdbContext;
        string projectId = "sitecoredemo";
        string datasetId = "sitecoredemo";
        string tableId = "VisitorRuns";

        public ExportToBigQueryWorker(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }
        public Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {           
            var rows = new List<BigQueryInsertRow>();

            foreach (var contact in batch)
            {
                                
                var interactions = contact.Interactions;

                foreach (var interaction in interactions)
                {
                    var runStarted = interaction.Events.OfType<RunStarted>().ToList();

                    if (runStarted.Any())
                    {
                        var row = new BigQueryInsertRow(interaction.Id.ToString())
                        {
                            { "ContactId", contact.Id.ToString() },
                            { "InteractionId", interaction.Id.ToString() },
                            { "RunStart", runStarted.First().Time.ToUniversalTime() }
                        };


                        var runEnded = interaction.Events.OfType<RunEnded>().ToList();

                        if (runEnded.Any())
                        {
                            row.Add("RunEnd", runEnded.First().Time.ToUniversalTime());

                        }
                        rows.Add(row);
                    }
                }
            }
            if(rows.Any())
            {
                var cred = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(@"C:\dev\symposium2019\cred.json");
                BigQueryClient client = BigQueryClient.Create(projectId, cred);
                client.InsertRows(datasetId, tableId, rows);
            }
            
            return Task.Delay(1);
        }

        public void Dispose() => _xdbContext.Dispose();
    }
}