using System;
using System.Collections.Generic;

namespace Sussol_Analyse_Subproject.Domain
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
