using System;
using Sitecore.XConnect;

namespace Sitecore.Demo.Model.XConnect.Events
{
    public class RunStarted : Event
    {
        public RunStarted(Guid definitionId, DateTime timestamp) : base(definitionId, timestamp)
        {
        }

        public DateTime Time { get; set; }
    }
}