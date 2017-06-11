using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sussol_Analyse_Subproject.Domain;
using Sussol_Analyse_Subproject.Utils;

namespace Sussol_Analyse_Subproject.Analyses
{
    public class ModelService
    {
        private IThreadAwareView _view;
        private readonly List<string> _features = new List<string>();
        private readonly List<string> _featuresDesiredClusters = new List<string>();
        private readonly List<int> _results = new List<int>();
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private readonly FileWriter _writer = new FileWriter();
        private const string DedicatedMapCsvPath = @"C:\SussolAnalysis\CSV results";
        private const string DedicatedMapRawDataPath = @"C:\SussolAnalysis\Raw data results";
        private readonly com.sussol.web.controller.ServiceModel _sus = new com.sussol.web.controller.ServiceModel();
        private string _set = "";
        private string _feature = "";
        private string _filename = "";
        private int _totalModelsToMake;
        private int _amountOfModels;
        private int _length;
        private readonly int _canopyTotalVariedModels;
        private readonly int _somTotalVariedModels;
        private readonly int _xmeansTotalVariedModels;

        public ModelService()
        {
            _canopyTotalVariedModels = MinMaxValue.CanopyNmax + (MinMaxValue.CanopyMaxCandidatesMax - 9) + (MinMaxValue.CanopyT2Max + 1) + (MinMaxValue.CanopyT1Max + 7);
            _somTotalVariedModels = (int)(MinMaxValue.SomLmax / 0.09998888) + (MinMaxValue.SomHmax - 1) + (MinMaxValue.SomWmax - 1);
            _xmeansTotalVariedModels = ((MinMaxValue.XMeansImax + 3) / 4) + ((MinMaxValue.XMeansMmax + 500) / 499) + ((MinMaxValue.XMeansJmax + 500) / 499) + (MinMaxValue.XMeansLmax / 2) + ((MinMaxValue.XMeansHmax + 2) / 5);
        }


        public void QueueGetResults(string dataset, string format, string algorithmUsed, string modellingtype, int desiredClusters, IThreadAwareView view)
        {
            this._view = view;
            var algorithm = (AlgorithmName)Enum.Parse(typeof(AlgorithmName), algorithmUsed.ToUpper(), true);
            //check  modellintgype
            DoNestedOrVaried(modellingtype, algorithm, dataset, format, desiredClusters);

        }

        //check modellingtype
        private void DoNestedOrVaried(string modellingtype, AlgorithmName algorithm, string dataset, string format, int desiredClusters)
        {
            if (modellingtype.Equals("nested"))
            {
                _worker.DoWork +=
                    (sender, e) => GetNestedResults(modellingtype, algorithm, dataset, format, desiredClusters);

            }
            else
            {
                _worker.DoWork +=
                    (sender, e) => GetVariedresults(modellingtype, algorithm, dataset, format, desiredClusters);
            }
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            _worker.RunWorkerAsync();
        }

        //when worker is completed
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _view.TaskFinished();
        }
        
        
        public void GetVariedresults(string modellingtype,AlgorithmName algorithm, string dataset, string format, int desiredClusters)
        {
            //clear list of models with desired clusters
            _featuresDesiredClusters.Clear();
            //get name of dataset
            if (dataset.Contains("\\"))
            {
                _set = dataset.Split('\\').Last();
            }
            string datasetFinal = File.ReadAllText(dataset);
            JObject jObject = new JObject();
            string algorithmtype = ((AlgorithmName)algorithm).ToString();
            //filename for excel files
            _filename = algorithmtype + "results " + "varied parameters " + "dataset " + _set;
            switch (algorithm)
            {
                //vary the canopy algorithm by parameter
                case AlgorithmName.CANOPY:
                    for (var paramN = MinMaxValue.CanopyNmin; paramN < MinMaxValue.CanopyNmax + 1; paramN++)
                    {
                        _feature = "Number of Clusters = " + paramN;
                        if (paramN != 0)
                        {
                            ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, paramN.ToString(), "", "", "", "");
                        }
                    }
                    for (var paramNumberOfCandidates = MinMaxValue.CanopyMaxCandidatesMin; paramNumberOfCandidates < MinMaxValue.CanopyMaxCandidatesMax + 1; paramNumberOfCandidates++)
                    {

                        _feature = "Number of candidates= " + paramNumberOfCandidates.ToString();
                        ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", "", paramNumberOfCandidates.ToString(), "");
                    }
                    for (var paramT2 = MinMaxValue.CanopyT2Min; paramT2 < MinMaxValue.CanopyT2Max + 1; paramT2++)
                    {

                        _feature = "T2= " + paramT2.ToString();
                        if (paramT2 != 0)
                        {
                            ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", paramT2.ToString(), "", "");

                        }
                    }
                    for (var paramT1 = MinMaxValue.CanopyT1Min; paramT1 < MinMaxValue.CanopyT1Max + 1; paramT1++)
                    {
                        _feature = "T1= " + paramT1.ToString();

                        if (paramT1 != 0)
                        {


                            ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", paramT1.ToString(), "", "", "");

                        }

                    }
                    break;
                //vary the SOM algorithm by parameter
                case AlgorithmName.SOM:
                    for (var paramL = MinMaxValue.SomLmin; paramL < MinMaxValue.SomLmax + 0.001; paramL += 0.1)
                    {
                        _feature = " learning rate step= " + paramL;
                        var oString = paramL.ToString();
                        ProcessModelling(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, oString.Replace(',', '.'), "", "", "", "");
                    }
                    for (var paramH = MinMaxValue.SomHmin; paramH < MinMaxValue.SomHmax + 1; paramH++)
                    {
                        _feature = "height= " + paramH;
                        ProcessModelling(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, "", paramH.ToString(), "", "", "");
                    }
                    for (var paramW = MinMaxValue.SomWmin; paramW < MinMaxValue.SomWmax + 1; paramW++)
                    {
                        _feature = "width= " + paramW;
                        ProcessModelling(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, "", "", paramW.ToString(), "", "");
                    }
                    break;
                //vary the X-means algorithm by parameter
                case AlgorithmName.XMEANS:
                    for (var paramI = MinMaxValue.XMeansImin; paramI < MinMaxValue.XMeansImax + 1; paramI += 4)
                    {
                        _feature = "Max overall it= " + paramI;
                        ProcessModelling(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, paramI.ToString(), "", "", "", "");

                    }
                    for (var paramM = MinMaxValue.XMeansMmin; paramM < MinMaxValue.XMeansMmax + 2; paramM += 500)
                    {
                        _feature = "Max it in the kMeans loop in IP part= " + paramM;
                        ProcessModelling(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", paramM.ToString(), "", "", "");
                    }
                    for (var paramJ = MinMaxValue.XMeansJmin; paramJ < MinMaxValue.XMeansJmax + 2; paramJ += 500)
                    {
                        _feature = "Max it in the kMeans loop in IS part=" + paramJ;
                        ProcessModelling(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", "", paramJ.ToString(), "", "");
                    }
                    for (var paramL = MinMaxValue.XMeansLmin; paramL < MinMaxValue.XMeansLmax + 1; paramL += 2)
                    {
                        _feature = "Min clusters= " + paramL;
                        ProcessModelling(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", "", "", paramL.ToString(), "");
                    }
                    for (var paramH = MinMaxValue.XMeansHmin; paramH < MinMaxValue.XMeansHmax + 1; paramH += 5)
                    {
                        _feature = "Max clusters= " + paramH;
                        ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", "", "", paramH.ToString());
                    }
                    break;
            }

            if (format.Equals("csv"))
            {
                //create excel workbook with headers in worksheets
                //write data into excel file adn add graph
                _writer.CreateCsv(DedicatedMapCsvPath, algorithmtype, _filename, dataset);
                WriteExcelFile(desiredClusters, algorithmtype, _filename);
            }
            Clearlists();
        }
        
        public void GetNestedResults(string modellingtype, AlgorithmName algorithm, string dataset, string format, int desiredClusters)
        {

            //get name of dataset
            if (dataset.Contains("\\"))
            {
                _set = dataset.Split('\\').Last();
            }
            string datasetFinal = File.ReadAllText(dataset);
            string algorithmtype = ((AlgorithmName)algorithm).ToString();
            string _filename = algorithmtype + "results " + "nested parameters " + "dataset " + _set;
            //add txt as extension if text file is needed, else make excel workbook
            if (format.Equals("text")) { _filename += ".txt"; }
            switch (algorithm)
            {
                //model the algorithm nested for canopy
                case AlgorithmName.CANOPY:
                    for (var paramN = MinMaxValue.CanopyNmin; paramN < MinMaxValue.CanopyNmax; paramN++)
                    {
                        for (var paramT1 = MinMaxValue.CanopyT1Min; paramT1 < MinMaxValue.CanopyT1Max + 1; paramT1 += 5)
                        {
                            for (var paramT2 = MinMaxValue.CanopyT2Min; paramT2 < MinMaxValue.CanopyT2Max + 1; paramT2 += 5)
                            {
                                for (var paramMaxCandidates = MinMaxValue.CanopyMaxCandidatesMin; paramMaxCandidates < MinMaxValue.CanopyMaxCandidatesMax + 1; paramMaxCandidates += 5)
                                {
                                    if (paramN != 0)
                                    {
                                        _totalModelsToMake = (MinMaxValue.CanopyNmax + 1) * (((MinMaxValue.CanopyT1Max + 5) / 5) + 1) * ((MinMaxValue.CanopyT2Max / 5) + 1) * ((MinMaxValue.CanopyMaxCandidatesMax / 5) - 1);
                                        _feature = "No. of clusters= " + paramN + " No. of candidates= " + paramMaxCandidates + " T2= " + paramT2 + "T1= " + paramT1;
                                        ProcessModelling(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, paramN.ToString(), paramT1.ToString(), paramT2.ToString(), paramMaxCandidates.ToString(), "");

                                    }


                                }
                            }
                        }
                    }
                    break;
                case AlgorithmName.SOM:
                    //model the algorithm nested for SOM
                    for (var paramL = MinMaxValue.SomLmin; paramL < MinMaxValue.SomLmax + 0.001; paramL += 0.1)
                    {
                        for (var paramH = MinMaxValue.SomHmin; paramH < MinMaxValue.SomHmax + 1; paramH += 1)
                        {
                            for (var paramW = MinMaxValue.SomWmin; paramW < MinMaxValue.SomWmax + 1; paramW += 1)
                            {
                                _totalModelsToMake = (int)(((MinMaxValue.SomLmax + 0.001) / 0.09998888) + 1) * (MinMaxValue.SomHmax - 1) * (MinMaxValue.SomWmax - 1);

                                if (paramW < paramH)
                                {
                                    _amountOfModels++;
                                }
                                else
                                {
                                    _feature = "Learning rate= " + paramL + " Height= " + paramH + " Width= " + paramW;
                                    var paramLtoString = paramL.ToString();
                                    ProcessModelling(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, paramLtoString.Replace(',', '.'), paramH.ToString(), paramW.ToString(), "", "");
                                }
                            }
                        }
                    }
                    break;
                case AlgorithmName.XMEANS:
                    //model the algorithm nested for X-means
                    for (var paramI = MinMaxValue.XMeansImin; paramI < MinMaxValue.XMeansImax + 1; paramI += 4)
                    {
                        for (var paramM = MinMaxValue.XMeansMmin; paramM < MinMaxValue.XMeansMmax + 2; paramM += 500)
                        {
                            for (var paramJ = MinMaxValue.XMeansJmin; paramJ < MinMaxValue.XMeansJmax + 2; paramJ += 500)
                            {
                                for (var paramL = MinMaxValue.XMeansLmin; paramL < MinMaxValue.XMeansLmax + 1; paramL += 2)
                                {
                                    for (var paramH = MinMaxValue.XMeansHmin; paramH < MinMaxValue.XMeansHmax + 1; paramH += 5)
                                    {

                                        _totalModelsToMake = ((MinMaxValue.XMeansImax + 3) / 4) * ((MinMaxValue.XMeansMmax + 500) / 499) * ((MinMaxValue.XMeansJmax + 500) / 499) * (MinMaxValue.XMeansLmax / 2) * ((MinMaxValue.XMeansHmax + 2) / 5);
                                        _feature = "It value= " + paramI + " Max no. of it in IP part= " + paramM + " Max no. of it in IS part= " + paramJ + "Minimum clusters= " + paramL + "Maximum clusters= " + paramH;
                                        ProcessModelling(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, paramI.ToString(), paramM.ToString(), paramJ.ToString(), paramL.ToString(), paramH.ToString());
                                    }
                                }
                            }

                        }

                    }
                    break;

            }
            if (format.Equals("csv"))
            {
                _writer.CreateCsv(DedicatedMapCsvPath, algorithmtype, _filename, dataset);
                WriteExcelFile(desiredClusters, algorithmtype, _filename);
            }
            Clearlists();
        }

        private JObject ProcessModelling(string modellingtype, string algorithm, string dataset, string format, int desiredClusters, string datasetFinal, string param1, string param2, string param3, string param4, string param5)
        {
            JObject jObject = ModelData(algorithm, datasetFinal, param1, param2, param3, param4, param5);
            JArray items = (JArray)jObject["clusters"];
            _length = items.Count;
            _amountOfModels++;
            _results.Add(_length);
            if (_length == desiredClusters)
            { _featuresDesiredClusters.Add(_feature); }
            UpdateProgressBar(modellingtype, algorithm);

            if (format.Equals("text"))
            {
                WriteVariedOrNestedText(modellingtype, algorithm, dataset, jObject);
            }


            _features.Add(_feature);
            return jObject;
        }

        private JObject ModelData(string algorithm, string datasetFinal, string param1, string param2, string param3, string param4, string param5)
        {
            switch (algorithm)
            {
                case "canopy":
                    return JObject.Parse(_sus.canopyModeller(datasetFinal, param1, param2, param3, param4).ToString());
                    break;
                case "som":
                    return JObject.Parse(_sus.somModeller(datasetFinal, param1, param2, param3).ToString());

                    break;
                case "xmeans":
                    return JObject.Parse(_sus.xmeansModeller(datasetFinal, param1, param2, param3, param4, param5).ToString());

                    break;
            }
            return null;
        }

        private void Clearlists()
        {
            _amountOfModels = 0;
            _features.Clear();
            _results.Clear();
        }

        private void WriteExcelFile(int desiredClusters, string algorithmtype, string filename)
        {
            
            _writer.WriteCsv(DedicatedMapCsvPath, _features, filename, _results, algorithmtype, _featuresDesiredClusters, desiredClusters, _view);
            string fullPath = Path.Combine(DedicatedMapCsvPath, filename);
            _writer.AddGraph(fullPath, _amountOfModels, algorithmtype, _view);
        }

      
        private void WriteVariedOrNestedText(string modellingtype, string algorithm, string dataset, JObject jObject)
        {
            if (modellingtype.Equals("varied"))
            {
                WriteRawData(jObject, "Dataset_" + dataset + "_Raw_data__algorithm_" + algorithm + "_Varied_parameter_" + _feature + ".txt");
            }
            else
            {
                WriteRawData(jObject, "Dataset_" + dataset + "_Raw_data__algorithm_" + algorithm + "_Nested _parameter_" + _feature + ".txt");
            }
        }

        private void UpdateProgressBar(string modellingtype, string algorithm)
        {
            if (modellingtype.Equals("varied"))
            {
                switch (algorithm)
                {
                    case "canopy":
                        _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
                        break;
                    case "som":
                        _view.ProgressUpdate(_amountOfModels, _somTotalVariedModels);
                        break;
                    default:
                        _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                        break;
                }
            }
            else
            {
                _view.ProgressUpdate(_amountOfModels, _totalModelsToMake);
            }
        }

        

        private void WriteRawData(JObject jObject, string feature)
        {
            _writer.Writetextfile(DedicatedMapRawDataPath, feature + ".txt", jObject, _view);
        }

    }
}


