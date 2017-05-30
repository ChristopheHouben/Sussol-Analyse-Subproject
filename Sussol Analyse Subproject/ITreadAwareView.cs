using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sussol_Analyse_Subproject
{
    public interface IThreadAwareView
    {
        void TaskFinished();

        void ProgressUpdate();

        void WritingToCsv();

        void AddingGraph();
    }
}
