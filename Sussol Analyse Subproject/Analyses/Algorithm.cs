using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sussol_Analyse_Subproject.Analyses
{
    public class Algorithm
    {
        [Key]
        public long Id { get; set; }
        public AlgorithmName AlgorithmName { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}
