using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sussol_Analyse_Subproject
{
    public class Algorithm
    {
        [Key]
        public long Id { get; set; }
        public AlgorithmName AlgorithmName { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}
