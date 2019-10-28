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
    public class DeleteContactWorker : IDistributedWorker<Contact>
    {
        private readonly IXdbContext _xdbContext;
        public DeleteContactWorker(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {
            foreach (var contact in batch)
            {
                _xdbContext.DeleteContact(contact);
            }

            return _xdbContext.SubmitAsync(token);
        }
        public void Dispose() => _xdbContext.Dispose();
    }
}
