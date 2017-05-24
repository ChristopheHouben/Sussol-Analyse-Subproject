using System;
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

namespace Sussol_Analyse_Subproject
{
    public class modelService
    {
        List<String> features = new List<String>();
        List<int> results = new List<int>();
        TrainingSet training = new TrainingSet();
        FileWriter writer = new FileWriter();
        static string dedicatedMapPath = @"C:\SussolAnalysis";
        static string dedicatedMapCsvPath = @"C:\SussolAnalysis\CSV results";
        static string dedicatedMapRawDataPath = @"C:\SussolAnalysis\Raw data results";
        com.sussol.web.controller.ServiceModel sus = new com.sussol.web.controller.ServiceModel();

        string feature = "";
        string filename = "";

        DirectoryInfo di = Directory.CreateDirectory(dedicatedMapPath);
        DirectoryInfo diCsv = Directory.CreateDirectory(Path.Combine(dedicatedMapCsvPath));
        DirectoryInfo diRaw = Directory.CreateDirectory(Path.Combine(dedicatedMapRawDataPath));
        
        AlgorithmName algos;
        public void GetResultFiles(string dataset, string format, string algorithmUsed, string modellingtype)
        {



            switch (format) {

            case "csv":
                    switch (modellingtype) {

                        case "nested":
                            AlgorithmName type = (AlgorithmName)Enum.Parse(typeof(AlgorithmName), algorithmUsed.ToUpper(), true);
                            GetNestedResults(type, dataset, format);
                            break;

                        case "varied":
                            break;

                    }
            break;
            case "raw":
                    switch (modellingtype)
                    {

                        case "nested":
                            AlgorithmName type = (AlgorithmName)Enum.Parse(typeof(AlgorithmName), algorithmUsed.ToUpper(), true);
                            GetNestedResults(type, dataset, "text");
                            break;

                        case "varied":
                            break;

                    }
                    break;


            }
                    

             
                
        }
            


        
        static string GetNameOf<T>(Expression<Func<T>> property)
        {
            return (property.Body as MemberExpression).Member.Name;
        }
        //public void GetVariedresults(AlgorithmName algorithm, string dataset, string filetype)
        //{
        //    training.dataSet = resourcepath + dataset;
        //    string datasets = File.ReadAllText(training.dataSet);
        //    JObject jObject = new JObject();
        //    string algotype = ((AlgorithmName)algorithm).ToString();
        //    switch (algorithm)
        //    {
        //        case AlgorithmName.CANOPY:
        //            for (var o = (int)Analyses.MinenMaxWaarden.canopyNmin; o < (int)Analyses.MinenMaxWaarden.canopyNmax + 1; o++)
        //            {
        //                feature = "Number of clusters";
        //                if (o != 0)
        //                {
        //                    var model = sus.canopyModeller(datasets, o.ToString(), "", "", "").ToString();
        //                    jObject = JObject.Parse(model);
        //                    JArray items = (JArray)jObject["clusters"];
        //                    int length = items.Count;
        //                    filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                    switch (filetype)
        //                    {

        //                        case "csv":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            break;
        //                        case "text":
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                        case "both":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                    }
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.canopyMaxCandidatesMin; o < (int)Analyses.MinenMaxWaarden.canopyMaxCandidatesMax + 1; o++)
        //            {
        //                feature = "Maximum number of candidates";

        //                jObject = JObject.Parse(sus.canopyModeller(datasets, "", "", "", o.ToString()).ToString());

        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.canopyT2min; o < (int)Analyses.MinenMaxWaarden.canopyT2max + 1; o++)
        //            {
        //                feature = "T2 distance";
        //                if (o != 0)
        //                {
        //                    jObject = JObject.Parse(sus.canopyModeller(datasets, "", "", o.ToString(), "").ToString());
        //                    JArray items = (JArray)jObject["clusters"];
        //                    int length = items.Count;
        //                    filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";

        //                    switch (filetype)
        //                    {

        //                        case "csv":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            break;
        //                        case "text":
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                        case "both":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                    }
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.canopyT1min; o < (int)Analyses.MinenMaxWaarden.canopyT1max + 1; o++)
        //            {
        //                feature = "T1 distance";

        //                if (o != 0)
        //                {
        //                    jObject = JObject.Parse(sus.canopyModeller(datasets, "", o.ToString(), "", "").ToString());

        //                    JArray items = (JArray)jObject["clusters"];
        //                    int length = items.Count;

        //                    filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";

        //                    switch (filetype)
        //                    {

        //                        case "csv":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            break;
        //                        case "text":
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                        case "both":
        //                            writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                            writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                            break;
        //                    }

        //                }

        //            }
        //            break;
        //        case AlgorithmName.SOM:
        //            for (var o = Analyses.MinenMaxWaarden.somLmin; o < Analyses.MinenMaxWaarden.somLmax + 0.001; o += 0.0899)
        //            {
        //                feature = " learning rate step ";
        //                var oString = o.ToString();
        //                jObject = JObject.Parse(sus.somModeller(datasets, oString.Replace(',', '.'), "", "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype, length);
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }


        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.somHmin; o < (int)Analyses.MinenMaxWaarden.somHmax + 1; o++)
        //            {
        //                feature = "height ";

        //                jObject = JObject.Parse(sus.somModeller(training.dataSet, "", o.ToString(), "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;

        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }

        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.somWmin; o < (int)Analyses.MinenMaxWaarden.somWmax + 1; o++)
        //            {
        //                feature = "width ";

        //                jObject = JObject.Parse(sus.somModeller(training.dataSet, "", "", o.ToString()).ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;

        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }

        //            }
        //            break;
        //        case AlgorithmName.XMEANS:
        //            for (var o = (int)Analyses.MinenMaxWaarden.xMeansImin; o < (int)Analyses.MinenMaxWaarden.xMeansImax + 1; o++)
        //            {
        //                feature = "maximum overal iterations";

        //                jObject = JObject.Parse(sus.xmeansModeller(datasets, o.ToString(), "", "", "", "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.xMeansMmin; o < (int)Analyses.MinenMaxWaarden.xMeansMmax + 1; o += 5)
        //            {
        //                feature = "maximum iterations in the kMeans loop in the Improve-Parameter part";
        //                jObject = JObject.Parse(sus.xmeansModeller(datasets, "", o.ToString(), "", "", "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.xMeansJmin; o < (int)Analyses.MinenMaxWaarden.xMeansJmax + 1; o += 5)
        //            {
        //                feature = "maximum iterations in the kMeans loop in the Improve-Structure part";
        //                jObject = JObject.Parse(sus.xmeansModeller(datasets, "", "", o.ToString(), "", "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.xMeansLmin; o < (int)Analyses.MinenMaxWaarden.xMeansLmax + 1; o++)
        //            {
        //                feature = "minimum number of clusters";
        //                jObject = JObject.Parse(sus.xmeansModeller(datasets, "", "", "", o.ToString(), "").ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            for (var o = (int)Analyses.MinenMaxWaarden.xMeansHmin; o < (int)Analyses.MinenMaxWaarden.xMeansHmax + 1; o++)
        //            {
        //                feature = "maximum number of clusters";
        //                jObject = JObject.Parse(sus.xmeansModeller(datasets, "", "", "", "", o.ToString()).ToString());
        //                JArray items = (JArray)jObject["clusters"];
        //                int length = items.Count;
        //                filename = "analyseRapport__parameter_is_" + feature + "_value_is_" + o + ".txt";
        //                switch (filetype)
        //                {

        //                    case "csv":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        break;
        //                    case "text":
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                    case "both":
        //                        writer.WriteCsv(directoryCsv, feature, o.ToString(), algotype + "results " + "dataset " + dataset + ".csv", length);
        //                        writer.Writetextfile(directory, filename, jObject, o.ToString(), feature);
        //                        break;
        //                }
        //            }
        //            break;
        //    }
        //}
        public void GetNestedResults(AlgorithmName algorithm, string dataset,string format) {
            string set = "";
            if (dataset.Contains("\\"))
            {
                 set = dataset.Split('\\').Last();
            }
                training.dataSet =  dataset;
            string datasets = File.ReadAllText(training.dataSet);
            JObject jObject = new JObject();
            string algorithmtype = ((AlgorithmName)algorithm).ToString();

            string filename = algorithmtype + "results " + "nested parameters " + "dataset " + set ;
            if (format.Equals("text")) { filename += ".txt"; }
            else{
                writer.createCsv(dedicatedMapCsvPath, algorithmtype, filename, dataset);
            }
            

            int counter = 0;
            int length = 0;
            switch (algorithm) {

                case AlgorithmName.CANOPY:


                    for (var paramT1 = MinMaxValues.canopyT1min; paramT1 < MinMaxValues.canopyT1max + 1; paramT1 += 5)
                    {
                        for (var paramT2 = MinMaxValues.canopyT2min; paramT2 < MinMaxValues.canopyT2max + 1; paramT2 += 5)
                        {
                            for (var paramMaxCandidates = MinMaxValues.canopyMaxCandidatesMin; paramMaxCandidates < MinMaxValues.canopyMaxCandidatesMax + 1; paramMaxCandidates += 5)
                            {
                                feature = "Number of clusters: " + "-1" + " Number of candidates: " + paramMaxCandidates + " T2 distance: " + paramT2 + "T1 distance: " + paramT1;
                                var model = sus.canopyModeller(datasets, "-1", paramT1.ToString(), paramT2.ToString(), paramMaxCandidates.ToString()).ToString();
                                jObject = JObject.Parse(model);
                                JArray items = (JArray)jObject["clusters"];
                                length = items.Count;
                                counter++;
                                features.Add(feature);
                                results.Add(length);
                                if (format.Equals("text"))
                                {

                                    feature = "Dataset_" + dataset + "_Raw_data__T1_value_is_" + paramT1 + "_T2_value_is_" + paramT2 + "_maxCandidates_is_" + paramMaxCandidates + ".txt";
                                    writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, counter.ToString(), feature);
                                }
                            }
                        }
                    }
                                   
                      if (format.Equals("csv")) {
                            writer.WriteCsv(dedicatedMapCsvPath, features, filename, results, algorithmtype);
                            string canopyFullPath = Path.Combine(dedicatedMapCsvPath, filename);
                            writer.addGraph(canopyFullPath, counter);
                        }
                    
                    counter = 0;
                    features.Clear();
                    results.Clear();
                    break;
                case AlgorithmName.SOM:
                    for (var paramL = MinMaxValues.somLmin; paramL < MinMaxValues.somLmax + 0.001; paramL += 0.1899)
                    {
                        for (var paramH = MinMaxValues.somHmin; paramH < MinMaxValues.somHmax -10; paramH += 10)
                        {
                            for (var paramW = MinMaxValues.somWmin; paramW < MinMaxValues.somWmax -10; paramW += 5)
                            {
                               
                                    feature = "Learning rate: " + paramL + " Height: " + paramH + " Width: " + paramW;
                                    var paramLtoString = paramL.ToString();
                                    var model = sus.somModeller(datasets, paramLtoString.Replace(',','.'), paramH.ToString(), paramW.ToString()).ToString();
                                    jObject = JObject.Parse(model);
                                    JArray items = (JArray)jObject["clusters"];
                                    length = items.Count;
                                    counter++;
                                    features.Add(feature);
                                    results.Add(length);
                                if (format.Equals("text"))
                                {

                                    feature = "Dataset_" + dataset + "_Raw_data__LearningRate_value_is_" + paramL + "_Height_value_is_" + paramH + "_Width_is_" + paramW + ".txt";
                                    writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, counter.ToString(), feature);
                                }
                            }

                        }
                    }
                    if (format.Equals("csv"))
                    {
                        writer.WriteCsv(dedicatedMapCsvPath, features, filename, results, algorithmtype);
                        string somFullPath = Path.Combine(dedicatedMapCsvPath, filename);
                        writer.addGraph(somFullPath, counter);
                    }
                    counter = 0;
                    features.Clear();
                    results.Clear();
                    break;
                case AlgorithmName.XMEANS:
                      for (var paramI = MinMaxValues.xMeansImin; paramI < MinMaxValues.xMeansImax + 1; paramI += 5)
                    {
                        for (var paramM = MinMaxValues.xMeansMmin; paramM < MinMaxValues.xMeansMmax + 1; paramM += 50)
                        {
                            for (var paramJ = MinMaxValues.xMeansJmin; paramJ < MinMaxValues.xMeansJmax + 1; paramJ += 500)
                            {
                                for (var paramL = MinMaxValues.xMeansLmin; paramL < MinMaxValues.xMeansLmax + 1; paramL++)
                                {
                                    for (var paramH = MinMaxValues.xMeansHmin; paramH < MinMaxValues.xMeansHmax + 1; paramH++)
                                    {
                                     paramI = 1;
                                        feature = "Iteration value: " + paramI + " Max number of iterations in K-means in improveme-parameter part: " + paramM + " Max number of iterations in K-means in improveme-structure part: " + paramJ + "Minimum number of clusters: " + paramL + "Maximum number of clusters: " + paramH;


                                        var model = sus.xmeansModeller(datasets, paramI.ToString(), paramM.ToString(), paramJ.ToString(), paramL.ToString(), paramH.ToString()).ToString();
                                        jObject = JObject.Parse(model);
                                        JArray items = (JArray)jObject["clusters"];
                                         length = items.Count;
                                        counter++;
                                        features.Add(feature);
                                        results.Add(length);
                                        if (format.Equals("text"))
                                        {

                                            feature = "Dataset_" + dataset + "_Raw_data__numberOfIterations_is_" + paramI + "_NumberOfIterationsIntImproveParameterPart_is_" + paramM + "_NumberOfIterationsIntImproveStructurePart_is_" + paramJ + "_minimumNumberOfClusters_is_"+paramL+"_maximumNumberOfClusters_is_"+paramH+".txt";
                                            writer.Writetextfile(dedicatedMapRawDataPath, feature + ".txt", jObject, counter.ToString(), feature);
                                        }

                                    }
                                }
                            }

                        }
                    }
                    if (format.Equals("csv"))
                    {
                        writer.WriteCsv(dedicatedMapCsvPath, features, filename, results, algorithmtype);
                        string xmeansFullPath = Path.Combine(dedicatedMapCsvPath, filename);
                        writer.addGraph(xmeansFullPath, counter);
                    }
                    counter = 0;
                    features.Clear();
                    results.Clear();
                    break;
                   
            }
        }

    }
}


