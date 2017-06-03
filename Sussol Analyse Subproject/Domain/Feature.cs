using System.ComponentModel.DataAnnotations;

namespace Sussol_Analyse_Subproject.Domain
{
    public class Feature
    {
        
        [Key]
        public long Id { get; set; }
        //0.4.9 Changed FeatureName to string in order to comply with demand of a database where one can add nw featurenames. 
        public string FeatureName { get; set; }
      //public double Value { get; set; }
      //0.5.0 Changed FeatureValue to double
      public double Value { get; set; }
      //0.5.0 Changed metadata to feature and removed MinMaxValue
   }

   


}
