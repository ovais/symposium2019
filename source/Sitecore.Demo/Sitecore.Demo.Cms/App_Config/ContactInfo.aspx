<%@ Page Language="c#" Inherits="System.Web.UI.Page" CodePage="65001" Debug="true" Async="true"%>
<%@ OutputCache Location="None" VaryByParam="none" %>
<%@ Import Namespace="Sitecore.XConnect" %>
<%@ Import Namespace="Sitecore.XConnect.Client" %>
<%@ Import Namespace="Sitecore.XConnect.Client.Configuration" %>
<%@ Import Namespace="Sitecore.XConnect.Collection.Model" %>
<%@ Import Namespace="Sitecore.Analytics" %>
<%@ Import Namespace="Sitecore.Demo.Model.XConnect.Facets" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Sitecore.XConnect.Client.Serialization" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>


<!DOCTYPE html>

<script runat="server">
    private XConnectClient client = SitecoreXConnectClientConfiguration.GetClient();
    private Contact knownContact;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Tracker.IsActive)
            throw new NotSupportedException("tracker isn't active!");
        if (!Tracker.Enabled)
            throw new NotSupportedException("tracker isn't enabled!");
    }

    protected async void GetContactFacet_Click(object sender, EventArgs e)
    {
        ContactFacet.InnerText = "";
        if (contactIdentificator.Text == "" || contactSource.Text == "")
        {
            ContactFacet.InnerText = "Could not load contact, empty identifiers";
        }
        else
        {
            knownContact = await client.GetContactAsync(
                new IdentifiedContactReference(contactSource.Text, contactIdentificator.Text),
                new ExpandOptions(PersonalInformation.DefaultFacetKey,RunnerFacet.DefaultFacetKey));

            if (knownContact != null)
            {
                var facet = knownContact.GetFacet<RunnerFacet>(RunnerFacet.DefaultFacetKey);
                if (facet != null)
                {
                    ContactFacet.InnerText = JsonConvert.SerializeObject(facet, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ContractResolver = new XdbJsonContractResolver(client.Model, serializeFacets: true, serializeContactInteractions: true),
                        //DefaultValueHandling = DefaultValueHandling.Include
                        // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        // NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    ContactFacet.InnerText = RunnerFacet.DefaultFacetKey + " facet is not found";
                }
            }
            else
            {
                ContactFacet.InnerText = "Contact not found.";
            }
        }
    }

    protected void IdentifyContact_Click(object sender, EventArgs e)
    {
        if (contactIdentificator.Text == "" || contactSource.Text == "")
        {
            ContactFacet.InnerText = "Empty identifiers";
        }
        else
        {
            ContactFacet.InnerText = "";
            Tracker.Current.Session.IdentifyAs(contactSource.Text, contactIdentificator.Text);
            ContactFacet.InnerText = "Contact has been identified as "+ contactIdentificator.Text  ;

            if(contactIdentificator.Text.ToLower().Trim() == "ovais.akhter@sitecore.com")
            {
                SendRequest("Running", 10);
            }
            else
            {
                 SendRequest("Basketball", 10);                
            }

        }
    }
    private static void SendRequest(string sport, int rating)
    {
        SendRequest(sport, rating, "https://habitatfitness.dev.local/sitecore/api/habitatfitness/sports/facet?sc_apikey={EBF6D5C1-EB80-4B15-91AB-DD3845797774}");
        SendRequest(sport, rating, "https://habitatfitness.dev.local/sitecore/api/habitatfitness/sports/profile?sc_apikey={EBF6D5C1-EB80-4B15-91AB-DD3845797774}");

    }

    private static void SendRequest(string sport, int rating,string url)
    {
        WebRequest request = WebRequest.Create(url);
        // Set the Method property of the request to POST.  
        request.Method = "POST";

        // Create POST data and convert it to a byte array.  
        string postData = string.Format("Ratings[{0}]: {1}", sport, rating);
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);

        // Set the ContentType property of the WebRequest.  
        request.ContentType = "application/x-www-form-urlencoded";
        // Set the ContentLength property of the WebRequest.  
        request.ContentLength = byteArray.Length;

        // Get the request stream.  
        Stream dataStream = request.GetRequestStream();
        // Write the data to the request stream.  
        dataStream.Write(byteArray, 0, byteArray.Length);
        // Close the Stream object.  
        dataStream.Close();

        // Get the response.  
        WebResponse response = request.GetResponse();
        // Display the status.  
        Console.WriteLine(((HttpWebResponse)response).StatusDescription);
    }


</script>

<html lang="en">
  <head>
    <title>Working with Contact</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <link href="/default.css" rel="stylesheet" />
  </head>
  <body>
  <form method="post" runat="server" id="mainform">
    <div style="margin: 100px" id="MainPanel">
        <sc:placeholder key="main" runat="server" />

        <b>Contact Identification</b>
        <br>
        Contact Identificator Source: <asp:TextBox ID="contactSource" Width="300" runat="server" /> 
        <br>
        Contact Identificator: <asp:TextBox ID="contactIdentificator" Width="300" runat="server" />
        <br>
        <asp:Button ID="GetContact" runat="server" Text="Get Facet" OnClick="GetContactFacet_Click" />
        <asp:Button ID="IdentifyContact" runat="server" Text="Identify Contact" OnClick="IdentifyContact_Click" />
        <br>
        <p ID="ContactFacet" style="white-space: pre" runat="server" />
    </div>
  </form>
  </body>
</html>
