using System.ComponentModel.DataAnnotations;

namespace Sussol_Analyse_Subproject.Domain
{
    public class ClusterDistanceCenter
    {
        [Key]
        public long Id { get; set; }
        public long ToClusterId { get; set; }
        public double Distance { get; set; }
    }
}
