using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Demo.UtClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            Interaction interaction = new Interaction
            {
                ChannelId = Guid.Parse("27b4e611-a73d-4a95-b20a-811d295bdf77"),
                Initiator = "contact",
                Contact = new Contact
                {
                    Source = "sitecoreextranet",
                    Identifier = "ovais@test.com"
                },

            };
            interaction.Events.Add(new Event
            {
                Type = "RunStarted",
                Time = "2018-01-02T11:11:11.528",
                DefinitionId = "4AC8B092-0747-4E67-A908-4D5B1C6DAE70"
            });

            interaction.Events.Add(new Event
            {
                Type = "RunEnded",
                Time = "2018-01-02T11:11:11.528",
                DefinitionId = "4AC8B092-0747-4E67-A908-4D5B1C6DAE70"
            });
        }

    }





public partial class Interaction

{
    [JsonProperty("ChannelId")]
    public Guid ChannelId { get; set; }

    [JsonProperty("Initiator")]
    public string Initiator { get; set; }

    [JsonProperty("Contact")]
    public Contact Contact { get; set; }

    [JsonProperty("UserAgent")]
    public string UserAgent { get; set; }

    [JsonProperty("Events")]
    public List<Event> Events { get; set; }
}

public partial class Contact
{
    [JsonProperty("Source")]
    public string Source { get; set; }

    [JsonProperty("Identifier")]
    public string Identifier { get; set; }
}

public partial class Event
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("Time")]
    public string Time { get; set; }

    [JsonProperty("Timestamp")]
    public string Timestamp { get; set; }

    [JsonProperty("DefinitionId")]
    public string DefinitionId { get; set; }
}


}