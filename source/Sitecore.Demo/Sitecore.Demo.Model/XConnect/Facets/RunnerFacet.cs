using System;
using Sitecore.XConnect;

namespace Sitecore.Demo.Model.XConnect.Facets
{
    [Serializable]
    [FacetKey(DefaultFacetKey)]
    public class RunnerFacet : Facet
    {
        public const string DefaultFacetKey = "RunnerFacet";

        public bool IsMorningRunner { get; set; }
        public bool IsEveningRunner { get; set; }
    }
}