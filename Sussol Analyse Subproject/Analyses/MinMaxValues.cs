using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sussol_Analyse_Subproject
{
    public struct MinMaxValues
    {
        public const int canopyNmin = -1;
        public const int canopyNmax = 25;
        public const int canopyMaxCandidatesMin = 10;
        public const int canopyMaxCandidatesMax = 50;
        public const int canopyT2min = -1;
        public const int canopyT2max = 25;
        public const int canopyT1min = -5;
        public const int canopyT1max = 25;

        public const int xMeansImin = 1;
        public const int xMeansImax = 25;
        public const int xMeansMmin = 1;
        public const int xMeansMmax = 2500;
        public const int xMeansJmin = 1;
        public const int xMeansJmax = 2500;
        public const int xMeansLmin = 2;
        public const int xMeansLmax = 10;
        public const int xMeansHmin = 2;
        public const int xMeansHmax = 23;

        public const double somLmin = 0.001;
        public const double somLmax = 0.9;
        public const int somHmin = 2;
        public const int somHmax = 20;
        public const int somWmin = 2;
        public const int somWmax = 20;

    }
}
