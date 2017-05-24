using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sussol_Analyse_Subproject
{
    public class Cluster
    {
        [Key]
        public long Id { get; set; }
        public int Number { get; set; }
        public ICollection<ClusterDistanceCenter> DistanceToClusters { get; set; }
        public ICollection<Solvent> Solvents { get; set; }
        public ICollection<VectorData> VectorData { get; set; }

    }
}
