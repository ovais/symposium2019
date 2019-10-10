using System;
using Sitecore.XConnect;

namespace Sitecore.Demo.Model.XConnect.Facets
{
    [Serializable]
    [FacetKey(DefaultFacetKey)]
    public class RunnerFacet : Facet
    {
        public const string DefaultFacetKey = "RunnerFacet";

        public double IsMorningRunner { get; set; }
        public double IsEveningRunner { get; set; }
    }
}