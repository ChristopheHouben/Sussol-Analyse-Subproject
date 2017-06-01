using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq.Expressions;
using System.ComponentModel;
using Sussol_Analyse_Subproject.Analyses;
using Sussol_Analyse_Subproject.Utils;

namespace Sussol_Analyse_Subproject
{
    public class ModelService
    {
        private IThreadAwareView _view;

        private readonly List<string> _features = new List<string>();
        private readonly List<string> _featuresDesiredClusters = new List<string>();
        private readonly List<int> _results = new List<int>();
        private readonly List<int> _resultsDesiredClusters = new List<int>();
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        


        TrainingSet _training = new TrainingSet();
        private readonly FileWriter _writer = new FileWriter();

        private const string DedicatedMapPath = @"C:\SussolAnalysis";
        private const string DedicatedMapCsvPath = @"C:\SussolAnalysis\CSV results";
        private const string DedicatedMapRawDataPath = @"C:\SussolAnalysis\Raw data results";
        private readonly com.sussol.web.controller.ServiceModel _sus = new com.sussol.web.controller.ServiceModel();
        private string _set = "";
        private string _feature = "";
        private string _filename = "";
        DirectoryInfo _di = Directory.CreateDirectory(DedicatedMapPath);
        DirectoryInfo _diCsv = Directory.CreateDirectory(Path.Combine(DedicatedMapCsvPath));
        DirectoryInfo _diRaw = Directory.CreateDirectory(Path.Combine(DedicatedMapRawDataPath));
        private int _totalModelsToMake;
        private int _amountOfModels = 0;
        private int _length = 0;
        private readonly int _canopyTotalVariedModels;
        private readonly int _somTotalVariedModels;
        private readonly int _xmeansTotalVariedModels;

        public ModelService()
        {
            _canopyTotalVariedModels = MinMaxValues.CanopyNmax + (MinMaxValues.CanopyMaxCandidatesMax - 9) + (MinMaxValues.CanopyT2Max + 1) + (MinMaxValues.CanopyT1Max + 7);
            _somTotalVariedModels = (int)(MinMaxValues.SomLmax / 0.09998888) + (MinMaxValues.SomHmax - 1) + (MinMaxValues.SomWmax - 1);
            _xmeansTotalVariedModels = ((MinMaxValues.XMeansImax + 3) / 4) + ((MinMaxValues.XMeansMmax + 500) / 499) + ((MinMaxValues.XMeansJmax + 500) / 499) + (MinMaxValues.XMeansLmax / 2) + ((MinMaxValues.XMeansHmax + 2) / 5);
        }

        public void QueueGetResults(string dataset, string format, string algorithmUsed, string modellingtype, int desiredClusters, IThreadAwareView view)
        {
            this._view = view;

            var type = (AlgorithmName)Enum.Parse(typeof(AlgorithmName), algorithmUsed.ToUpper(), true);
           
            switch (format)
            {

                case "csv":
                    switch (modellingtype)
                    {

                        case "nested":


                            _worker.DoWork += (sender, e) => GetNestedResults(modellingtype, type, dataset, format, desiredClusters);
                     
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();
                            break;

                        case "varied":
                            _worker.DoWork += (sender, e) => GetVariedresults(modellingtype,type, dataset, format, desiredClusters);
                       
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();

                            break;


                    }
                    break;
                case "raw":
                    switch (modellingtype)
                    {

                        case "nested":
                            _worker.DoWork += (sender, e) => GetNestedResults(modellingtype, type, dataset, "text", desiredClusters);
                          
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();

                            break;

                        case "varied":
                            _worker.DoWork += (sender, e) => GetVariedresults(modellingtype, type, dataset, "text", desiredClusters);
                           
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();

                            break;

                    }
                    break;

            }
        }
        
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            _view.TaskFinished();
        }
        
        static string GetNameOf<T>(Expression<Func<T>> property)
        {
            return (property.Body as MemberExpression).Member.Name;
        }
        public void GetVariedresults(string modellingtype,AlgorithmName algorithm, string dataset, string format, int desiredClusters)
        {
                _featuresDesiredClusters.Clear();
                if (dataset.Contains("\\"))
                {
                    _set = dataset.Split('\\').Last();
                }
                string datasetFinal = File.ReadAllText(dataset);
                JObject jObject = new JObject();
                string algorithmtype = ((AlgorithmName)algorithm).ToString();

                _filename = algorithmtype + "results " + "varied parameters " + "dataset " + _set;
                if (format.Equals("csv"))
                {
                    _writer.CreateCsv(DedicatedMapCsvPath, algorithmtype, _filename, dataset);
                }


                switch (algorithm)
                {
                    case AlgorithmName.CANOPY:
                        for (var paramN = MinMaxValues.CanopyNmin; paramN < MinMaxValues.CanopyNmax + 1; paramN++)
                        {
                            _feature = "Number of Clusters = " + paramN;
                            if (paramN != 0)
                        {
                            ModelVariedOrNestedData(modellingtype,"canopy", dataset, format, desiredClusters, datasetFinal, paramN.ToString(),"","","","");

                        }
                    }
                        for (var paramNumberOfCandidates = MinMaxValues.CanopyMaxCandidatesMin; paramNumberOfCandidates < MinMaxValues.CanopyMaxCandidatesMax + 1; paramNumberOfCandidates++)
                        {

                            _feature = "Number of candidates= " + paramNumberOfCandidates.ToString();
                        ModelVariedOrNestedData(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", "", paramNumberOfCandidates.ToString(), "");
                    }
                        for (var paramT2 = MinMaxValues.CanopyT2Min; paramT2 < MinMaxValues.CanopyT2Max + 1; paramT2++)
                        {

                            _feature = "T2= " + paramT2.ToString();
                            if (paramT2 != 0)
                            {
                            ModelVariedOrNestedData(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", paramT2.ToString(), "", "");

                        }
                    }
                        for (var paramT1 = MinMaxValues.CanopyT1Min; paramT1 < MinMaxValues.CanopyT1Max + 1; paramT1++)
                        {
                            _feature = "T1= " + paramT1.ToString();

                            if (paramT1 != 0)
                            {
                             

                            ModelVariedOrNestedData(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", paramT1.ToString(), "", "", "");

                        }

                        }
                        break;
                    case AlgorithmName.SOM:
                        for (var paramL = MinMaxValues.SomLmin; paramL < MinMaxValues.SomLmax + 0.001; paramL += 0.09998888)
                        {
                            _feature = " learning rate step= " + paramL;
                            var oString = paramL.ToString();
                          
                        ModelVariedOrNestedData(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, oString.Replace(',', '.'), "", "", "", "");

                    }
                        for (var paramH = MinMaxValues.SomHmin; paramH < MinMaxValues.SomHmax + 1; paramH++)
                        {
                            _feature = "height= " + paramH;

                            
                        ModelVariedOrNestedData(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, "", paramH.ToString(), "", "", "");


                    }
                    for (var paramW = MinMaxValues.SomWmin; paramW < MinMaxValues.SomWmax + 1; paramW++)
                        {
                            _feature = "width= " + paramW;
                        
                        ModelVariedOrNestedData(modellingtype, "som", dataset, format, desiredClusters, datasetFinal, "", "", paramW.ToString(), "", "");


                    }
                        break;
                    case AlgorithmName.XMEANS:
                        for (var paramI = MinMaxValues.XMeansImin; paramI < MinMaxValues.XMeansImax + 1; paramI += 4)
                        {
                            _feature = "maximum overal iterations= " + paramI;

                        ModelVariedOrNestedData(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, paramI.ToString(), "", "", "", "");

                    }
                        for (var paramM = MinMaxValues.XMeansMmin; paramM < MinMaxValues.XMeansMmax + 1; paramM += 499)
                        {
                            _feature = "maximum iterations in the kMeans loop in the Improve-Parameter part= " + paramM;
                        ModelVariedOrNestedData(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", paramM.ToString(), "", "", "");

                    }
                        for (var paramJ = MinMaxValues.XMeansJmin; paramJ < MinMaxValues.XMeansJmax + 1; paramJ += 499)
                        {
                            _feature = "maximum iterations in the kMeans loop in the Improve-Structure part= " + paramJ;
                        ModelVariedOrNestedData(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", "", paramJ.ToString(), "", "");

                    }
                        for (var paramL = MinMaxValues.XMeansLmin; paramL < MinMaxValues.XMeansLmax + 1; paramL += 2)
                        {
                            _feature = "minimum number of clusters= " + paramL;
                            ModelVariedOrNestedData(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, "", "", "", paramL.ToString(), "");

                        }
                        for (var paramH = MinMaxValues.XMeansHmin; paramH < MinMaxValues.XMeansHmax + 1; paramH += 5)
                        {
                            _feature = "maximum number of clusters";
                            ModelVariedOrNestedData(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, "", "", "", "", paramH.ToString());

                    }
                        break;

                }
                if (format.Equals("csv"))
                {
                    _writer.WriteCsv(DedicatedMapCsvPath, _features, _filename, _results, algorithmtype, _featuresDesiredClusters, desiredClusters, _view);
                    string canopyFullPath = Path.Combine(DedicatedMapCsvPath, _filename);
                    _writer.AddGraph(canopyFullPath, _amountOfModels, algorithmtype, _view);
                }
                _amountOfModels = 0;
                _features.Clear();
                _results.Clear();
            }

        private JObject ModelVariedOrNestedData(string modellingtype, string algorithm, string dataset, string format, int desiredClusters, string datasetFinal, string param1, string param2, string param3, string param4, string param5)
        {
            JObject jObject = ModelData(algorithm, datasetFinal, param1, param2, param3, param4, param5);
            JArray items = (JArray)jObject["clusters"];
            _length = items.Count;
            _amountOfModels++;
            _results.Add(_length);
            if (_length == desiredClusters)
            { _featuresDesiredClusters.Add(_feature); }
            if (algorithm == "canopy")
            {
                _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
            } else if (algorithm == "som")
            {
                _view.ProgressUpdate(_amountOfModels, _somTotalVariedModels);
            }
            else
            {
                _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
            }





            if (format.Equals("text"))
                if (modellingtype.Equals("varied"))
                {
                    WriteRawData(jObject, "Dataset_" + dataset + "_Raw_data__algorithm_" + algorithm + "_Varied_parameter_" + _feature + ".txt");
                }
                else
                {
                    WriteRawData(jObject, "Dataset_" + dataset + "_Raw_data__algorithm_" + algorithm + "_Nested _parameter_" + _feature + ".txt");
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

        private void WriteRawData(JObject jObject, string feature)
        {
            _writer.Writetextfile(DedicatedMapRawDataPath, feature + ".txt", jObject, _view);
        }

        public void GetNestedResults(string modellingtype, AlgorithmName algorithm, string dataset,string format, int desiredClusters) {
           
            
            if (dataset.Contains("\\"))
            {
                 _set = dataset.Split('\\').Last();
            }
            string datasetFinal = File.ReadAllText(dataset);
            JObject jObject = new JObject();
            string algorithmtype = ((AlgorithmName)algorithm).ToString();
            string txtFileName = "";
            string filename = algorithmtype + "results " + "nested parameters " + "dataset " + _set ;
            if (format.Equals("text")) { filename += ".txt"; }
            else{ _writer.CreateCsv(DedicatedMapCsvPath, algorithmtype, filename, dataset);}

            
         
         
            switch (algorithm) {

                case AlgorithmName.CANOPY:
                  for (var paramT1 = MinMaxValues.CanopyT1Min; paramT1 < MinMaxValues.CanopyT1Max + 1; paramT1 += 5)
                    {
                        for (var paramT2 = MinMaxValues.CanopyT2Min; paramT2 < MinMaxValues.CanopyT2Max + 1; paramT2 += 5)
                        {
                            for (var paramMaxCandidates = MinMaxValues.CanopyMaxCandidatesMin; paramMaxCandidates < MinMaxValues.CanopyMaxCandidatesMax + 1; paramMaxCandidates += 5)
                            {
                                _totalModelsToMake = (((MinMaxValues.CanopyT1Max+5) / 5)+1) * ((MinMaxValues.CanopyT2Max / 5)+1) * ((MinMaxValues.CanopyMaxCandidatesMax / 5)-1);
                                _feature = "No. of clusters= " + desiredClusters + " No. of candidates= " + paramMaxCandidates + " T2 distance= " + paramT2 + "T1 distance= " + paramT1;
                                ModelVariedOrNestedData(modellingtype, "canopy", dataset, format, desiredClusters, datasetFinal, desiredClusters.ToString(), paramT1.ToString(), paramT2.ToString(), paramMaxCandidates.ToString(), "");
                            }
                        }
                    }
                   break;
                case AlgorithmName.SOM:
                  for (var paramL = MinMaxValues.SomLmin; paramL < MinMaxValues.SomLmax + 0.001; paramL += 0.09998888)
                    {
                        for (var paramH = MinMaxValues.SomHmin; paramH < MinMaxValues.SomHmax+1; paramH += 1)
                        {
                            for (var paramW = MinMaxValues.SomWmin; paramW < MinMaxValues.SomWmax+1; paramW += 1)
                            {
                                _totalModelsToMake = (int)((MinMaxValues.SomLmax+0.001) / 0.09998888)  * (MinMaxValues.SomHmax-1)  * (MinMaxValues.SomWmax-1);

                                if (paramW < paramH)
                                {
                                    _amountOfModels++;
                                   // _view.ProgressUpdate(_amountOfModels, _totalModelsToMake);
                                }
                                else
                                {

                                    _feature = "Learning rate= " + paramL + " Height= " + paramH + " Width= " + paramW;
                                    var paramLtoString = paramL.ToString();
                                 
                                    ModelVariedOrNestedData(modellingtype, "som", dataset, format, desiredClusters, datasetFinal,paramLtoString.Replace(',', '.'), paramH.ToString(), paramW.ToString(), "", "");
                                }
                            
                            }
                      
                        }
                    }
                   break;
                case AlgorithmName.XMEANS:
                  for (var paramI = MinMaxValues.XMeansImin; paramI < MinMaxValues.XMeansImax + 1; paramI += 4)
                    {
                        for (var paramM = MinMaxValues.XMeansMmin; paramM < MinMaxValues.XMeansMmax + 1; paramM +=499)
                        {
                            for (var paramJ = MinMaxValues.XMeansJmin; paramJ < MinMaxValues.XMeansJmax + 1; paramJ += 499)
                            {
                                for (var paramL = MinMaxValues.XMeansLmin; paramL < MinMaxValues.XMeansLmax + 1; paramL+=2)
                                {
                                    for (var paramH = MinMaxValues.XMeansHmin; paramH < MinMaxValues.XMeansHmax + 1; paramH+=5)
                                    {

                                         _totalModelsToMake = ((MinMaxValues.XMeansImax+3) / 4) * ((MinMaxValues.XMeansMmax+500) / 499)  * ((MinMaxValues.XMeansJmax + 500) / 499) * (MinMaxValues.XMeansLmax / 2) * ((MinMaxValues.XMeansHmax + 2) / 5);
                                        _feature = "Iteration value= " + paramI + " Max no. of iterations in IP part= " + paramM + " Max no. of iterations in IS part= " + paramJ + "Minimum no. of clusters= " + paramL + "Maximum no. of clusters= " + paramH;
                                        ModelVariedOrNestedData(modellingtype, "xmeans", dataset, format, desiredClusters, datasetFinal, paramI.ToString(), paramM.ToString(), paramJ.ToString(), paramL.ToString(), paramH.ToString());

                                    }
                                }
                            }

                        }
                       
                    }
                   break;
                    
            }
            if (format.Equals("csv"))
            {
                _writer.WriteCsv(DedicatedMapCsvPath, _features, filename, _results, algorithmtype, _featuresDesiredClusters, desiredClusters, _view);
                string canopyFullPath = Path.Combine(DedicatedMapCsvPath, filename);
                _writer.AddGraph(canopyFullPath, _amountOfModels, algorithmtype, _view);
            }
            _amountOfModels = 0;
            _features.Clear();
            _results.Clear();

        }

    }
}


