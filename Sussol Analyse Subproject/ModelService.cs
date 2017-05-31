﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sussol_Analyse_Subproject;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Windows;
using java.awt;

namespace Sussol_Analyse_Subproject
{
    public class ModelService
    {
        private IThreadAwareView _view;

        private readonly List<string> _features = new List<string>();
        private readonly List<string> _featuresDesiredClusters = new List<string>();
        private readonly List<int> _results = new List<int>();
        private readonly List<int> _resultsDesiredClusters = new List<int>();
        readonly BackgroundWorker _worker = new BackgroundWorker();
        


        TrainingSet _training = new TrainingSet();
        private readonly FileWriter _writer = new FileWriter();

        private static readonly string DedicatedMapPath = @"C:\SussolAnalysis";
        private static readonly string DedicatedMapCsvPath = @"C:\SussolAnalysis\CSV results";
        private static readonly string DedicatedMapRawDataPath = @"C:\SussolAnalysis\Raw data results";
        private readonly com.sussol.web.controller.ServiceModel _sus = new com.sussol.web.controller.ServiceModel();
        string _set = "";
        string _feature = "";
        string _filename = "";
        DirectoryInfo _di = Directory.CreateDirectory(DedicatedMapPath);
        DirectoryInfo _diCsv = Directory.CreateDirectory(Path.Combine(DedicatedMapCsvPath));
        DirectoryInfo _diRaw = Directory.CreateDirectory(Path.Combine(DedicatedMapRawDataPath));
        int _totalModelsToMake;
        int _amountOfModels = 0;
        int _length = 0;
        readonly int _canopyTotalVariedModels = MinMaxValues.canopyNmax + (MinMaxValues.canopyMaxCandidatesMax - 9) + (MinMaxValues.canopyT2max + 1) + (MinMaxValues.canopyT1max+7);
        readonly int _somTotalVariedModels = (int)(MinMaxValues.somLmax / 0.09998888) + (MinMaxValues.somHmax - 1) + (MinMaxValues.somWmax - 1);
        readonly int _xmeansTotalVariedModels = ((MinMaxValues.xMeansImax + 3) / 4) + ((MinMaxValues.xMeansMmax + 500) / 499) + ((MinMaxValues.xMeansJmax + 500) / 499) + (MinMaxValues.xMeansLmax / 2) + ((MinMaxValues.xMeansHmax + 2) / 5);
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


                            _worker.DoWork += (sender, e) => GetNestedResults(type, dataset, format, desiredClusters);
                     
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();
                            break;

                        case "varied":
                            _worker.DoWork += (sender, e) => GetVariedresults(type, dataset, format, desiredClusters);
                       
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();

                            break;


                    }
                    break;
                case "raw":
                    switch (modellingtype)
                    {

                        case "nested":
                            _worker.DoWork += (sender, e) => GetNestedResults(type, dataset, "text", desiredClusters);
                          
                            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            _worker.RunWorkerAsync();

                            break;

                        case "varied":
                            _worker.DoWork += (sender, e) => GetVariedresults(type, dataset, "text", desiredClusters);
                           
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
        public void GetVariedresults(AlgorithmName algorithm, string dataset, string format, int desiredClusters)
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
                    _writer.createCsv(DedicatedMapCsvPath, algorithmtype, _filename, dataset);
                }


                switch (algorithm)
                {
                    case AlgorithmName.CANOPY:
                        for (var paramN = MinMaxValues.canopyNmin; paramN < MinMaxValues.canopyNmax + 1; paramN++)
                        {
                            _feature = "Number of Clusters = " + paramN.ToString();
                            if (paramN != 0)
                            {

                                var model = _sus.canopyModeller(datasetFinal, paramN.ToString(), "", "", "").ToString();
                                jObject = JObject.Parse(model);
                                JArray items = (JArray)jObject["clusters"];
                                _length = items.Count;
                                _amountOfModels++;
                                if (_length == desiredClusters)
                                { _featuresDesiredClusters.Add(_feature); }
                                _results.Add(_length);
                                _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {
                                    _feature = "Dataset_" + dataset + "_Varied parameters_numberOfClusters_value_is_" + paramN + ".txt";
                                    _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                                }
                                _features.Add(_feature);

                            }
                        }
                        for (var paramNumberOfCandidates = MinMaxValues.canopyMaxCandidatesMin; paramNumberOfCandidates < MinMaxValues.canopyMaxCandidatesMax + 1; paramNumberOfCandidates++)
                        {

                            _feature = "Number of candidates= " + paramNumberOfCandidates.ToString();
                            jObject = JObject.Parse(_sus.canopyModeller(datasetFinal, "", "", "", paramNumberOfCandidates.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _results.Add(_length);
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied_parameters__numberOfCandidates_value_is_" + paramNumberOfCandidates + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        for (var paramT2 = MinMaxValues.canopyT2min; paramT2 < MinMaxValues.canopyT2max + 1; paramT2++)
                        {

                            _feature = "T2= " + paramT2.ToString();
                            if (paramT2 != 0)
                            {
                                jObject = JObject.Parse(_sus.canopyModeller(datasetFinal, "", "", paramT2.ToString(), "").ToString());
                                JArray items = (JArray)jObject["clusters"];
                                _length = items.Count;
                                if (_length == desiredClusters)
                                {
                                    _featuresDesiredClusters.Add(_feature);

                                }
                                _results.Add(_length);
                                _amountOfModels++;
                                _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {

                                    _feature = "Dataset_" + dataset + "_Raw_data__T2_value_is_" + paramT2 + ".txt";

                                    _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                                }
                                _features.Add(_feature);
                            }
                        }
                        for (var paramT1 = MinMaxValues.canopyT1min; paramT1 < MinMaxValues.canopyT1max + 1; paramT1++)
                        {
                            _feature = "T1= " + paramT1.ToString();

                            if (paramT1 != 0)
                            {
                                jObject = JObject.Parse(_sus.canopyModeller(datasetFinal, "", paramT1.ToString(), "", "").ToString());

                                JArray items = (JArray)jObject["clusters"];
                                _length = items.Count;
                                if (_length == desiredClusters)
                                {
                                    _featuresDesiredClusters.Add(_feature);

                                }
                                _results.Add(_length);
                                _amountOfModels++;
                                _view.ProgressUpdate(_amountOfModels, _canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {

                                    _feature = "Dataset_" + dataset + "_Variedparameters__T2_value_is_" + paramT1 + ".txt";

                                    _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                                }
                                _features.Add(_feature);

                            }

                        }
                        break;
                    case AlgorithmName.SOM:
                        for (var paramL = MinMaxValues.somLmin; paramL < MinMaxValues.somLmax + 0.001; paramL += 0.09998888)
                        {
                            _feature = " learning rate step= " + paramL;
                            var oString = paramL.ToString();
                            jObject = JObject.Parse(_sus.somModeller(datasetFinal, oString.Replace(',', '.'), "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _somTotalVariedModels);
                            _results.Add(_length);
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_learningRate_value_is_" + paramL + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);

                        }
                        for (var paramH = MinMaxValues.somHmin; paramH < MinMaxValues.somHmax + 1; paramH++)
                        {
                            _feature = "height= " + paramH;

                            jObject = JObject.Parse(_sus.somModeller(datasetFinal, "", paramH.ToString(), "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _somTotalVariedModels);
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramH + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);

                        }
                        for (var paramW = MinMaxValues.somWmin; paramW < MinMaxValues.somWmax + 1; paramW++)
                        {
                            _feature = "width= " + paramW;

                            jObject = JObject.Parse(_sus.somModeller(datasetFinal, "", "", paramW.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _somTotalVariedModels);
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramW + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);

                        }
                        break;
                    case AlgorithmName.XMEANS:
                        for (var paramI = MinMaxValues.xMeansImin; paramI < MinMaxValues.xMeansImax + 1; paramI += 4)
                        {
                            _feature = "maximum overal iterations= " + paramI;

                            jObject = JObject.Parse(_sus.xmeansModeller(datasetFinal, paramI.ToString(), "", "", "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramI + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        for (var paramM = MinMaxValues.xMeansMmin; paramM < MinMaxValues.xMeansMmax + 1; paramM += 499)
                        {
                            _feature = "maximum iterations in the kMeans loop in the Improve-Parameter part= " + paramM;
                            jObject = JObject.Parse(_sus.xmeansModeller(datasetFinal, "", paramM.ToString(), "", "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramM + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        for (var paramJ = MinMaxValues.xMeansJmin; paramJ < MinMaxValues.xMeansJmax + 1; paramJ += 499)
                        {
                            _feature = "maximum iterations in the kMeans loop in the Improve-Structure part= " + paramJ;
                            jObject = JObject.Parse(_sus.xmeansModeller(datasetFinal, "", "", paramJ.ToString(), "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramJ + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        for (var paramL = MinMaxValues.xMeansLmin; paramL < MinMaxValues.xMeansLmax + 1; paramL += 2)
                        {
                            _feature = "minimum number of clusters= " + paramL;
                            jObject = JObject.Parse(_sus.xmeansModeller(datasetFinal, "", "", "", paramL.ToString(), "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;
                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramL + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        for (var paramH = MinMaxValues.xMeansHmin; paramH < MinMaxValues.xMeansHmax + 1; paramH += 5)
                        {
                            _feature = "maximum number of clusters";
                            jObject = JObject.Parse(_sus.xmeansModeller(datasetFinal, "", "", "", "", paramH.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            _length = items.Count;

                            _amountOfModels++;
                            _view.ProgressUpdate(_amountOfModels, _xmeansTotalVariedModels);
                            if (_length == desiredClusters)
                            {
                                _featuresDesiredClusters.Add(_feature);

                            }
                            _results.Add(_length);
                            if (format.Equals("text"))
                            {

                                _feature = "Dataset_" + dataset + "_Varied parameters_minimumClusters_value_is_" + paramH + ".txt";

                                _writer.Writetextfile(DedicatedMapRawDataPath, _feature + ".txt", jObject, _view);
                            }
                            _features.Add(_feature);
                        }
                        break;

                }
                if (format.Equals("csv"))
                {
                    _writer.WriteCsv(DedicatedMapCsvPath, _features, _filename, _results, algorithmtype, _featuresDesiredClusters, desiredClusters, _view);
                    string canopyFullPath = Path.Combine(DedicatedMapCsvPath, _filename);
                    _writer.addGraph(canopyFullPath, _amountOfModels, algorithmtype, _view);
                }
                _amountOfModels = 0;
                _features.Clear();
                _results.Clear();
            }

        
            public void GetNestedResults(AlgorithmName algorithm, string dataset,string format, int desiredClusters) {
           
            
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
            else{ _writer.createCsv(DedicatedMapCsvPath, algorithmtype, filename, dataset);}

            
         
         
            switch (algorithm) {

                case AlgorithmName.CANOPY:
                  for (var paramT1 = MinMaxValues.canopyT1min; paramT1 < MinMaxValues.canopyT1max + 1; paramT1 += 5)
                    {
                        for (var paramT2 = MinMaxValues.canopyT2min; paramT2 < MinMaxValues.canopyT2max + 1; paramT2 += 5)
                        {
                            for (var paramMaxCandidates = MinMaxValues.canopyMaxCandidatesMin; paramMaxCandidates < MinMaxValues.canopyMaxCandidatesMax + 1; paramMaxCandidates += 5)
                            {
                                _totalModelsToMake = (((MinMaxValues.canopyT1max+5) / 5)+1) * ((MinMaxValues.canopyT2max / 5)+1) * ((MinMaxValues.canopyMaxCandidatesMax / 5)-1);
                                _feature = "No. of clusters: " + desiredClusters + " No. of candidates: " + paramMaxCandidates + " T2 distance: " + paramT2 + "T1 distance: " + paramT1;
                                var model = _sus.canopyModeller(datasetFinal, desiredClusters.ToString(), paramT1.ToString(), paramT2.ToString(), paramMaxCandidates.ToString()).ToString();
                                jObject = JObject.Parse(model);
                                JArray items = (JArray)jObject["clusters"];
                                _length = items.Count;
                                
                                if (_length == desiredClusters) {
                                    _featuresDesiredClusters.Add(_feature);

                                }
                                _features.Add(_feature);
                                _results.Add(_length);
                                _amountOfModels++;
                                if (format.Equals("text"))
                                {

                                    txtFileName = "Dataset_" + dataset + "_Raw_data__T1_value_is_" + paramT1 + "_T2_value_is_" + paramT2 + "_maxCandidates_is_" + paramMaxCandidates + ".txt";
                                    _writer.Writetextfile(DedicatedMapRawDataPath, txtFileName + ".txt", jObject, _view);
                                }

                                //worker.ReportProgress(totalModelsToMake);

                                _view.ProgressUpdate(_amountOfModels,_totalModelsToMake);
                                
                            }
                           
                        }

                          

                            
                    }
                   break;
                case AlgorithmName.SOM:
                  for (var paramL = MinMaxValues.somLmin; paramL < MinMaxValues.somLmax + 0.001; paramL += 0.09998888)
                    {
                        for (var paramH = MinMaxValues.somHmin; paramH < MinMaxValues.somHmax+1; paramH += 1)
                        {
                            for (var paramW = MinMaxValues.somWmin; paramW < MinMaxValues.somWmax+1; paramW += 1)
                            {
                                _totalModelsToMake = (int)((MinMaxValues.somLmax+0.001) / 0.09998888)  * (MinMaxValues.somHmax-1)  * (MinMaxValues.somWmax-1);

                                if (paramW < paramH)
                                {
                                    _amountOfModels++;
                                }
                                else
                                {

                                    _feature = "Learning rate: " + paramL + " Height: " + paramH + " Width: " + paramW;
                                    var paramLtoString = paramL.ToString();
                                    var model = _sus.somModeller(datasetFinal, paramL.ToString().Replace(',', '.'), paramH.ToString(), paramW.ToString()).ToString();
                                    jObject = JObject.Parse(model);
                                    JArray items = (JArray)jObject["clusters"];
                                    _length = items.Count;
                                    if (_length == desiredClusters) {
                                        _featuresDesiredClusters.Add(_feature);
                                        
                                    }
                                    _amountOfModels++;
                                    _features.Add(_feature);
                                    _results.Add(_length);
                                    if (format.Equals("text"))
                                    {

                                        txtFileName = "Dataset_" + dataset + "_Raw_data__LearningRate_value_is_" + paramL + "_Height_value_is_" + paramH + "_Width_is_" + paramW + ".txt";
                                        _writer.Writetextfile(DedicatedMapRawDataPath, txtFileName + ".txt", jObject, _view);
                                    }
                                   // view.ProgressUpdate(amountOfModels,totalModelsToMake);
                                }
                                _view.ProgressUpdate(_amountOfModels, _totalModelsToMake);
                            }
                      
                        }
                    }
                   break;
                case AlgorithmName.XMEANS:
                  for (var paramI = MinMaxValues.xMeansImin; paramI < MinMaxValues.xMeansImax + 1; paramI += 4)
                    {
                        for (var paramM = MinMaxValues.xMeansMmin; paramM < MinMaxValues.xMeansMmax + 1; paramM +=499)
                        {
                            for (var paramJ = MinMaxValues.xMeansJmin; paramJ < MinMaxValues.xMeansJmax + 1; paramJ += 499)
                            {
                                for (var paramL = MinMaxValues.xMeansLmin; paramL < MinMaxValues.xMeansLmax + 1; paramL+=2)
                                {
                                    for (var paramH = MinMaxValues.xMeansHmin; paramH < MinMaxValues.xMeansHmax + 1; paramH+=5)
                                    {

                                         _totalModelsToMake = ((MinMaxValues.xMeansImax+3) / 4) * ((MinMaxValues.xMeansMmax+500) / 499)  * ((MinMaxValues.xMeansJmax + 500) / 499) * (MinMaxValues.xMeansLmax / 2) * ((MinMaxValues.xMeansHmax + 2) / 5);
                                        _feature = "Iteration value: " + paramI + " Max no. of iterations in improveme-parameter part: " + paramM + " Max no. of iterations in improveme-structure part: " + paramJ + "Minimum no. of clusters: " + paramL + "Maximum no. of clusters: " + paramH;


                                        var model = _sus.xmeansModeller(datasetFinal, paramI.ToString(), paramM.ToString(), paramJ.ToString(), paramL.ToString(), paramH.ToString()).ToString();
                                        jObject = JObject.Parse(model);
                                        JArray items = (JArray)jObject["clusters"];
                                         _length = items.Count;
                                        if (_length == desiredClusters) {
                                           
                                            _featuresDesiredClusters.Add(_feature);
                                            
                                        }
                                        _amountOfModels++;
                                        _features.Add(_feature);
                                        _results.Add(_length);
                                        if (format.Equals("text"))
                                        {

                                            txtFileName = "Dataset_" + dataset + "_Raw_data__numberOfIterations_is_" + paramI + "_NumberOfIterationsIntImproveParameterPart_is_" + paramM + "_NumberOfIterationsIntImproveStructurePart_is_" + paramJ + "_minimumNumberOfClusters_is_"+paramL+"_maximumNumberOfClusters_is_"+paramH+".txt";
                                            _writer.Writetextfile(DedicatedMapRawDataPath, txtFileName + ".txt", jObject, _view);
                                        }

                                        _view.ProgressUpdate(_amountOfModels, _totalModelsToMake);

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
                _writer.addGraph(canopyFullPath, _amountOfModels, algorithmtype, _view);
            }
            _amountOfModels = 0;
            _features.Clear();
            _results.Clear();

        }

    }
}


