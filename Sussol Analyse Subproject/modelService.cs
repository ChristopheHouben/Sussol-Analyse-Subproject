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
    public class modelService
    {
        IThreadAwareView view;

        List<String> features = new List<String>();
        List<String> featuresDesiredClusters = new List<String>();
        List<int> results = new List<int>();
        List<int> resultsDesiredClusters = new List<int>();
        BackgroundWorker worker = new BackgroundWorker();
        


        TrainingSet training = new TrainingSet();
        FileWriter writer = new FileWriter();

        static string dedicatedMapPath = @"C:\SussolAnalysis";
        static string dedicatedMapCsvPath = @"C:\SussolAnalysis\CSV results";
        static string dedicatedMapRawDataPath = @"C:\SussolAnalysis\Raw data results";
        com.sussol.web.controller.ServiceModel sus = new com.sussol.web.controller.ServiceModel();
        string set = "";
        string feature = "";
        string filename = "";
        DirectoryInfo di = Directory.CreateDirectory(dedicatedMapPath);
        DirectoryInfo diCsv = Directory.CreateDirectory(Path.Combine(dedicatedMapCsvPath));
        DirectoryInfo diRaw = Directory.CreateDirectory(Path.Combine(dedicatedMapRawDataPath));
        int totalModelsToMake;
        int amountOfModels = 0;
        int length = 0;
        int canopyTotalVariedModels = MinMaxValues.canopyNmax + (MinMaxValues.canopyMaxCandidatesMax - 9) + (MinMaxValues.canopyT2max + 1) + (MinMaxValues.canopyT1max+7);
        int somTotalVariedModels = (int)(MinMaxValues.somLmax / 0.09998888) + (MinMaxValues.somHmax - 1) + (MinMaxValues.somWmax - 1);
        int xmeansTotalVariedModels = ((MinMaxValues.xMeansImax + 3) / 4) + ((MinMaxValues.xMeansMmax + 500) / 499) + ((MinMaxValues.xMeansJmax + 500) / 499) + (MinMaxValues.xMeansLmax / 2) + ((MinMaxValues.xMeansHmax + 2) / 5);
        public void QueueGetResults(string dataset, string format, string algorithmUsed, string modellingtype, int desiredClusters, IThreadAwareView view)
        {
            this.view = view;

            AlgorithmName type = (AlgorithmName)Enum.Parse(typeof(AlgorithmName), algorithmUsed.ToUpper(), true);
           
            switch (format)
            {

                case "csv":
                    switch (modellingtype)
                    {

                        case "nested":


                            worker.DoWork += (sender, e) => GetNestedResults(type, dataset, format, desiredClusters);
                     
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            worker.RunWorkerAsync();
                            break;

                        case "varied":
                            worker.DoWork += (sender, e) => GetVariedresults(type, dataset, format, desiredClusters);
                       
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            worker.RunWorkerAsync();

                            break;


                    }
                    break;
                case "raw":
                    switch (modellingtype)
                    {

                        case "nested":
                            worker.DoWork += (sender, e) => GetNestedResults(type, dataset, "text", desiredClusters);
                          
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            worker.RunWorkerAsync();

                            break;

                        case "varied":
                            worker.DoWork += (sender, e) => GetVariedresults(type, dataset, "text", desiredClusters);
                           
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            worker.RunWorkerAsync();

                            break;

                    }
                    break;

            }
        }

       

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            view.TaskFinished();
        }
        
        static string GetNameOf<T>(Expression<Func<T>> property)
        {
            return (property.Body as MemberExpression).Member.Name;
        }
        public void GetVariedresults(AlgorithmName algorithm, string dataset, string format, int desiredClusters)
        {


                featuresDesiredClusters.Clear();
                if (dataset.Contains("\\"))
                {
                    set = dataset.Split('\\').Last();
                }
                string datasetFinal = File.ReadAllText(dataset);
                JObject jObject = new JObject();
                string algorithmtype = ((AlgorithmName)algorithm).ToString();

                filename = algorithmtype + "results " + "varied parameters " + "dataset " + set;
                if (format.Equals("csv"))
                {
                    writer.createCsv(dedicatedMapCsvPath, algorithmtype, filename, dataset);
                }


                switch (algorithm)
                {
                    case AlgorithmName.CANOPY:
                        for (var paramN = MinMaxValues.canopyNmin; paramN < MinMaxValues.canopyNmax + 1; paramN++)
                        {
                            feature = "Number of Clusters = " + paramN.ToString();
                            if (paramN != 0)
                            {

                                var model = sus.canopyModeller(datasetFinal, paramN.ToString(), "", "", "").ToString();
                                jObject = JObject.Parse(model);
                                JArray items = (JArray)jObject["clusters"];
                                length = items.Count;
                                amountOfModels++;
                                if (length == desiredClusters)
                                { featuresDesiredClusters.Add(feature); }
                                results.Add(length);
                                view.ProgressUpdate(amountOfModels, canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {
                                    feature = "Dataset_" + dataset + "_Varied parameters_numberOfClusters_value_is_" + paramN + ".txt";
                                    writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                                }
                                features.Add(feature);

                            }
                        }
                        for (var paramNumberOfCandidates = MinMaxValues.canopyMaxCandidatesMin; paramNumberOfCandidates < MinMaxValues.canopyMaxCandidatesMax + 1; paramNumberOfCandidates++)
                        {

                            feature = "Number of candidates= " + paramNumberOfCandidates.ToString();
                            jObject = JObject.Parse(sus.canopyModeller(datasetFinal, "", "", "", paramNumberOfCandidates.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            results.Add(length);
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, canopyTotalVariedModels);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied_parameters__numberOfCandidates_value_is_" + paramNumberOfCandidates + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        for (var paramT2 = MinMaxValues.canopyT2min; paramT2 < MinMaxValues.canopyT2max + 1; paramT2++)
                        {

                            feature = "T2= " + paramT2.ToString();
                            if (paramT2 != 0)
                            {
                                jObject = JObject.Parse(sus.canopyModeller(datasetFinal, "", "", paramT2.ToString(), "").ToString());
                                JArray items = (JArray)jObject["clusters"];
                                length = items.Count;
                                if (length == desiredClusters)
                                {
                                    featuresDesiredClusters.Add(feature);

                                }
                                results.Add(length);
                                amountOfModels++;
                                view.ProgressUpdate(amountOfModels, canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {

                                    feature = "Dataset_" + dataset + "_Raw_data__T2_value_is_" + paramT2 + ".txt";

                                    writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                                }
                                features.Add(feature);
                            }
                        }
                        for (var paramT1 = MinMaxValues.canopyT1min; paramT1 < MinMaxValues.canopyT1max + 1; paramT1++)
                        {
                            feature = "T1= " + paramT1.ToString();

                            if (paramT1 != 0)
                            {
                                jObject = JObject.Parse(sus.canopyModeller(datasetFinal, "", paramT1.ToString(), "", "").ToString());

                                JArray items = (JArray)jObject["clusters"];
                                length = items.Count;
                                if (length == desiredClusters)
                                {
                                    featuresDesiredClusters.Add(feature);

                                }
                                results.Add(length);
                                amountOfModels++;
                                view.ProgressUpdate(amountOfModels, canopyTotalVariedModels);
                                if (format.Equals("text"))
                                {

                                    feature = "Dataset_" + dataset + "_Variedparameters__T2_value_is_" + paramT1 + ".txt";

                                    writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                                }
                                features.Add(feature);

                            }

                        }
                        break;
                    case AlgorithmName.SOM:
                        for (var paramL = MinMaxValues.somLmin; paramL < MinMaxValues.somLmax + 0.001; paramL += 0.09998888)
                        {
                            feature = " learning rate step= " + paramL;
                            var oString = paramL.ToString();
                            jObject = JObject.Parse(sus.somModeller(datasetFinal, oString.Replace(',', '.'), "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, somTotalVariedModels);
                            results.Add(length);
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_learningRate_value_is_" + paramL + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);

                        }
                        for (var paramH = MinMaxValues.somHmin; paramH < MinMaxValues.somHmax + 1; paramH++)
                        {
                            feature = "height= " + paramH;

                            jObject = JObject.Parse(sus.somModeller(datasetFinal, "", paramH.ToString(), "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, somTotalVariedModels);
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramH + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);

                        }
                        for (var paramW = MinMaxValues.somWmin; paramW < MinMaxValues.somWmax + 1; paramW++)
                        {
                            feature = "width= " + paramW;

                            jObject = JObject.Parse(sus.somModeller(datasetFinal, "", "", paramW.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, somTotalVariedModels);
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramW + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);

                        }
                        break;
                    case AlgorithmName.XMEANS:
                        for (var paramI = MinMaxValues.xMeansImin; paramI < MinMaxValues.xMeansImax + 1; paramI += 4)
                        {
                            feature = "maximum overal iterations= " + paramI;

                            jObject = JObject.Parse(sus.xmeansModeller(datasetFinal, paramI.ToString(), "", "", "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, xmeansTotalVariedModels);
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramI + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        for (var paramM = MinMaxValues.xMeansMmin; paramM < MinMaxValues.xMeansMmax + 1; paramM += 499)
                        {
                            feature = "maximum iterations in the kMeans loop in the Improve-Parameter part= " + paramM;
                            jObject = JObject.Parse(sus.xmeansModeller(datasetFinal, "", paramM.ToString(), "", "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, xmeansTotalVariedModels);
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramM + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        for (var paramJ = MinMaxValues.xMeansJmin; paramJ < MinMaxValues.xMeansJmax + 1; paramJ += 499)
                        {
                            feature = "maximum iterations in the kMeans loop in the Improve-Structure part= " + paramJ;
                            jObject = JObject.Parse(sus.xmeansModeller(datasetFinal, "", "", paramJ.ToString(), "", "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, xmeansTotalVariedModels);
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramJ + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        for (var paramL = MinMaxValues.xMeansLmin; paramL < MinMaxValues.xMeansLmax + 1; paramL += 2)
                        {
                            feature = "minimum number of clusters= " + paramL;
                            jObject = JObject.Parse(sus.xmeansModeller(datasetFinal, "", "", "", paramL.ToString(), "").ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;
                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, xmeansTotalVariedModels);
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_Height_value_is_" + paramL + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        for (var paramH = MinMaxValues.xMeansHmin; paramH < MinMaxValues.xMeansHmax + 1; paramH += 5)
                        {
                            feature = "maximum number of clusters";
                            jObject = JObject.Parse(sus.xmeansModeller(datasetFinal, "", "", "", "", paramH.ToString()).ToString());
                            JArray items = (JArray)jObject["clusters"];
                            length = items.Count;

                            amountOfModels++;
                            view.ProgressUpdate(amountOfModels, xmeansTotalVariedModels);
                            if (length == desiredClusters)
                            {
                                featuresDesiredClusters.Add(feature);

                            }
                            results.Add(length);
                            if (format.Equals("text"))
                            {

                                feature = "Dataset_" + dataset + "_Varied parameters_minimumClusters_value_is_" + paramH + ".txt";

                                writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, view);
                            }
                            features.Add(feature);
                        }
                        break;

                }
                if (format.Equals("csv"))
                {
                    writer.WriteCsv(dedicatedMapCsvPath, features, filename, results, algorithmtype, featuresDesiredClusters, desiredClusters, view);
                    string canopyFullPath = Path.Combine(dedicatedMapCsvPath, filename);
                    writer.addGraph(canopyFullPath, amountOfModels, algorithmtype, view);
                }
                amountOfModels = 0;
                features.Clear();
                results.Clear();
            }

        
            public void GetNestedResults(AlgorithmName algorithm, string dataset,string format, int desiredClusters) {
           
            
            if (dataset.Contains("\\"))
            {
                 set = dataset.Split('\\').Last();
            }
            string datasetFinal = File.ReadAllText(dataset);
            JObject jObject = new JObject();
            string algorithmtype = ((AlgorithmName)algorithm).ToString();
            string txtFileName = "";
            string filename = algorithmtype + "results " + "nested parameters " + "dataset " + set ;
            if (format.Equals("text")) { filename += ".txt"; }
            else{ writer.createCsv(dedicatedMapCsvPath, algorithmtype, filename, dataset);}

            
         
         
            switch (algorithm) {

                case AlgorithmName.CANOPY:
                  for (var paramT1 = MinMaxValues.canopyT1min; paramT1 < MinMaxValues.canopyT1max + 1; paramT1 += 5)
                    {
                        for (var paramT2 = MinMaxValues.canopyT2min; paramT2 < MinMaxValues.canopyT2max + 1; paramT2 += 5)
                        {
                            for (var paramMaxCandidates = MinMaxValues.canopyMaxCandidatesMin; paramMaxCandidates < MinMaxValues.canopyMaxCandidatesMax + 1; paramMaxCandidates += 5)
                            {
                                totalModelsToMake = (((MinMaxValues.canopyT1max+5) / 5)+1) * ((MinMaxValues.canopyT2max / 5)+1) * ((MinMaxValues.canopyMaxCandidatesMax / 5)-1);
                                feature = "No. of clusters: " + desiredClusters + " No. of candidates: " + paramMaxCandidates + " T2 distance: " + paramT2 + "T1 distance: " + paramT1;
                                var model = sus.canopyModeller(datasetFinal, desiredClusters.ToString(), paramT1.ToString(), paramT2.ToString(), paramMaxCandidates.ToString()).ToString();
                                jObject = JObject.Parse(model);
                                JArray items = (JArray)jObject["clusters"];
                                length = items.Count;
                                
                                if (length == desiredClusters) {
                                    featuresDesiredClusters.Add(feature);

                                }
                                features.Add(feature);
                                results.Add(length);
                                amountOfModels++;
                                if (format.Equals("text"))
                                {

                                    txtFileName = "Dataset_" + dataset + "_Raw_data__T1_value_is_" + paramT1 + "_T2_value_is_" + paramT2 + "_maxCandidates_is_" + paramMaxCandidates + ".txt";
                                    writer.Writetextfile(dedicatedMapRawDataPath, txtFileName + ".txt", jObject, view);
                                }

                                //worker.ReportProgress(totalModelsToMake);

                                view.ProgressUpdate(amountOfModels,totalModelsToMake);
                                
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
                                totalModelsToMake = (int)((MinMaxValues.somLmax+0.001) / 0.09998888)  * (MinMaxValues.somHmax-1)  * (MinMaxValues.somWmax-1);

                                if (paramW < paramH)
                                {
                                    amountOfModels++;
                                }
                                else
                                {

                                    feature = "Learning rate: " + paramL + " Height: " + paramH + " Width: " + paramW;
                                    var paramLtoString = paramL.ToString();
                                    var model = sus.somModeller(datasetFinal, paramL.ToString().Replace(',', '.'), paramH.ToString(), paramW.ToString()).ToString();
                                    jObject = JObject.Parse(model);
                                    JArray items = (JArray)jObject["clusters"];
                                    length = items.Count;
                                    if (length == desiredClusters) {
                                        featuresDesiredClusters.Add(feature);
                                        
                                    }
                                    amountOfModels++;
                                    features.Add(feature);
                                    results.Add(length);
                                    if (format.Equals("text"))
                                    {

                                        txtFileName = "Dataset_" + dataset + "_Raw_data__LearningRate_value_is_" + paramL + "_Height_value_is_" + paramH + "_Width_is_" + paramW + ".txt";
                                        writer.Writetextfile(dedicatedMapRawDataPath, txtFileName + ".txt", jObject, view);
                                    }
                                   // view.ProgressUpdate(amountOfModels,totalModelsToMake);
                                }
                                view.ProgressUpdate(amountOfModels, totalModelsToMake);
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

                                         totalModelsToMake = ((MinMaxValues.xMeansImax+3) / 4) * ((MinMaxValues.xMeansMmax+500) / 499)  * ((MinMaxValues.xMeansJmax + 500) / 499) * (MinMaxValues.xMeansLmax / 2) * ((MinMaxValues.xMeansHmax + 2) / 5);
                                        feature = "Iteration value: " + paramI + " Max no. of iterations in improveme-parameter part: " + paramM + " Max no. of iterations in improveme-structure part: " + paramJ + "Minimum no. of clusters: " + paramL + "Maximum no. of clusters: " + paramH;


                                        var model = sus.xmeansModeller(datasetFinal, paramI.ToString(), paramM.ToString(), paramJ.ToString(), paramL.ToString(), paramH.ToString()).ToString();
                                        jObject = JObject.Parse(model);
                                        JArray items = (JArray)jObject["clusters"];
                                         length = items.Count;
                                        if (length == desiredClusters) {
                                           
                                            featuresDesiredClusters.Add(feature);
                                            
                                        }
                                        amountOfModels++;
                                        features.Add(feature);
                                        results.Add(length);
                                        if (format.Equals("text"))
                                        {

                                            txtFileName = "Dataset_" + dataset + "_Raw_data__numberOfIterations_is_" + paramI + "_NumberOfIterationsIntImproveParameterPart_is_" + paramM + "_NumberOfIterationsIntImproveStructurePart_is_" + paramJ + "_minimumNumberOfClusters_is_"+paramL+"_maximumNumberOfClusters_is_"+paramH+".txt";
                                            writer.Writetextfile(dedicatedMapRawDataPath, txtFileName + ".txt", jObject, view);
                                        }

                                        view.ProgressUpdate(amountOfModels, totalModelsToMake);

                                    }
                                }
                            }

                        }
                       
                    }
                   break;
                    
            }
            if (format.Equals("csv"))
            {
                writer.WriteCsv(dedicatedMapCsvPath, features, filename, results, algorithmtype, featuresDesiredClusters, desiredClusters, view);
                string canopyFullPath = Path.Combine(dedicatedMapCsvPath, filename);
                writer.addGraph(canopyFullPath, amountOfModels, algorithmtype, view);
            }
            amountOfModels = 0;
            features.Clear();
            results.Clear();

        }

    }
}


