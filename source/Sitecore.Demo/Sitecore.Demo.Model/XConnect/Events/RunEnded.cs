using System;
using System.Collections.Generic;
using System.Text;
using Sitecore.XConnect;

namespace Sitecore.Demo.Model.XConnect.Events
{
    public class RunEnded : Event
    {
        public RunEnded(Guid definitionId, DateTime timestamp) : base(definitionId, timestamp)
        {
        }

        public DateTime Time { get; set; }
    }
}
