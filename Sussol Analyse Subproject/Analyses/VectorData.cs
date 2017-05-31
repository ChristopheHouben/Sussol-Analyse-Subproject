namespace Sussol_Analyse_Subproject.Analyses
{
    public class VectorData
    {
        public long Id { get; set; }
        public double Value { get; set; }
        //0.4.9 Removed FeatureName as circular dependency challanges arose after FeatureName type was changed to string instead of enum FeatureName
        //public FeatureName FeatureName { get; set; } 
        public Feature feature { get; set; }
    }
}
