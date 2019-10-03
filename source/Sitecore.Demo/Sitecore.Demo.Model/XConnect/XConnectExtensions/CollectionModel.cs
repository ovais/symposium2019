using Sitecore.Demo.Model.XConnect.Events;
using Sitecore.Demo.Model.XConnect.Facets;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;

namespace Sitecore.Demo.Model.XConnect.XConnectExtensions
{
    public class CollectionModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var modelBuilder = new XdbModelBuilder("CollectionModel", new XdbModelVersion(1, 0));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);

            modelBuilder.DefineFacet<Contact, RunnerFacet>(RunnerFacet.DefaultFacetKey);
            modelBuilder.DefineEventType<RunStarted>(false);
            modelBuilder.DefineEventType<RunEnded>(false);

            return modelBuilder.BuildModel();
        }
    }
}