using System.ComponentModel.DataAnnotations;

namespace Sussol_Analyse_Subproject.Analyses
{
    public class TrainingSet
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string dataSet { get; set; }
    }
}
