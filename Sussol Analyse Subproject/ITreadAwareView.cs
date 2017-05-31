namespace Sussol_Analyse_Subproject
{
    public interface IThreadAwareView
    {
        void TaskFinished();

        void ProgressUpdate(int currentModelAmount, int modelsToMake);

        void WritingToCsv();

        void AddingGraph();
    }
}
