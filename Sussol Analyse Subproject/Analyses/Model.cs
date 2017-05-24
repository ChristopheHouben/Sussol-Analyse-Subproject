using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sussol_Analyse_Subproject
{
    public class Model
    {
       
        public long Id { get; set; }
      
        public string DataSet { get; set; }
        public DateTime Date { get; set; }
        
        public AlgorithmName AlgorithmName { get; set; }
        public string ModelPath { get; set; }

        public int NumberOfSolvents { get; set; }
        public int NumberOfFeatures { get; set; }
        public ICollection<Cluster> Clusters { get; set; }
    }
}
