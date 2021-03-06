﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Excel = Microsoft.Office.Interop.Excel;


namespace Sussol_Analyse_Subproject.Utils
{
    public class FileWriter 
    {
        // List<String> parameterNames = new List<String>();
        IThreadAwareView _view;
       
        
        public void Writetextfile(string directory, string filename, JObject jObject, IThreadAwareView view)
        {
            this._view = view;
            string onlyName = filename;
            if (filename.Contains("\\"))
            {
                onlyName = filename.Split('\\').Last();
            }
            string path = Path.Combine(directory, onlyName);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {
                    str.Write("===============================================\n" + filename + "\n ===============================================\n");
                    str.Write(jObject);

                }
            }
        }

        public void CreateCsv(string directory, string algorithm, string filename, string setname)
        {
            
            string fullPath = Path.Combine(directory, filename);

            object missing = Type.Missing;
            object misValue = System.Reflection.Missing.Value;

            //create excel
            Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.DisplayAlerts = false;

            //add excel workbook
            Excel.Workbook wb = excel.Workbooks.Add();

            var wsAllClusters = (Excel.Worksheet)wb.Worksheets.Add();
            wsAllClusters.Name = "All models";
            var wsDesiredClusters = (Excel.Worksheet)wb.Worksheets.Add();
            wsDesiredClusters.Name = "Desired clusters";
            //add worksheets to workbook

            //Adjust all columns
            wsAllClusters.Columns.AutoFit();
            wsDesiredClusters.Columns.AutoFit();

            //freeze top row
            // ws.Application.ActiveWindow.FreezePanes = true;
            wsAllClusters.Cells[1, 1] = algorithm + "results";
            wsAllClusters.Cells[1, 2] = "Dataset: " + setname;
            wsAllClusters.Cells[1, 3] = "Generated on: " + DateTime.Now;
            wsAllClusters.Cells[2, 1] = "All models";
            wsAllClusters.Cells[2, 2] = "Number of clusters";
            
            wsDesiredClusters.Cells[1, 1] = algorithm + "results";
            wsDesiredClusters.Cells[1, 2] = "Dataset: " + setname;
            wsDesiredClusters.Cells[1, 3] = "Generated on: " + DateTime.Now;
            wsDesiredClusters.Cells[2, 1] = "All modelling parameters with desired no. of cluster output";
            wsDesiredClusters.Cells[2, 2] = "Number of clusters";
            wb.SaveAs(fullPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            ReleaseObject(wsAllClusters);
            ReleaseObject(wsDesiredClusters);
            ReleaseObject(wb);
            ReleaseObject(excel);
        }
        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public void WriteCsv(string directory, List<string> usedParameter, string excelfilename, List<int> results, string algorithm, List<string> featuresDesiredClusters, int numberOfClusters, IThreadAwareView view)
        {
            this._view = view;
            view.WritingToCsv();
            string fullPath = Path.Combine(directory, excelfilename);
           object misValue = System.Reflection.Missing.Value;
            //create excel
            Excel.Application excel = new Excel.Application();
            //add excel workbook
            Excel.Workbook wb = excel.Workbooks.Add();
            wb = excel.Workbooks.Open(fullPath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            var ws = (Excel.Worksheet)wb.Worksheets.get_Item("All models");
            var wsDesiredClusters = (Excel.Worksheet)wb.Worksheets.get_Item("Desired clusters");

            //add worksheets to workbook
            var totalResults = results.Count();
            for (var o = 0; o < totalResults; o++)
            {
                ws.Cells[o + 3, 1] = usedParameter[o];
                ws.Cells[o + 3, 2] = results[o];
            }
            if (!featuresDesiredClusters.Any())
            {
                wsDesiredClusters.Cells[ 3, 1] = "There are no parameters found for your desired no. of clusters.";
            }
            for (var o = 0; o < featuresDesiredClusters.Count(); o++) {
                wsDesiredClusters.Cells[o + 3, 1] = featuresDesiredClusters[o];
                wsDesiredClusters.Cells[o + 3, 2] = numberOfClusters;
            }
            //Adjust all columns
            ws.Columns.AutoFit();
            wsDesiredClusters.Columns.AutoFit();
            //freeze top row
            // ws.Application.ActiveWindow.FreezePanes = true;
            

            excel.DisplayAlerts = false;

           // excelSheePrint.SaveAs(filename, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);

            wb.SaveAs(fullPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue,
                misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            ReleaseObject(ws);

            ReleaseObject(wb);
            ReleaseObject(excel);


        }


        public void AddGraph(string path, int counter, string algorithm, IThreadAwareView view)
        {
            this._view = view;
            view.AddingGraph();
            object misValue = System.Reflection.Missing.Value;
            //create excel
            Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            //add excel workbook
            Microsoft.Office.Interop.Excel.Workbook wb;
            wb = File.Exists(path) ? excel.Workbooks.Open(path, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0) : excel.Workbooks.Add();

            var ws = (Excel.Worksheet)wb.Worksheets.get_Item("All models");
            //add worksheets to workbook
            Excel.Worksheet ws2;
            bool found = false;
            // Loop through all worksheets in the workbook
            foreach (Excel.Worksheet sheet in wb.Sheets)
            {
                // Check the name of the current sheet
                if (sheet.Name == "Graphical representation")
                {
                    found = true;
                    break; // Exit the loop now
                }
            }

            if (found)
            {
                // Reference it by name
                ws2 = wb.Sheets["Graphical representation"];
            }
            else
            {
                // Create it
                ws2 = (Excel.Worksheet)wb.Worksheets.Add();
                ws2.Name = ("Graphical representation");
            }
            //Adjust all columns
            ws.Columns.AutoFit();
            //insert graph into worsheet 2
            Excel.Range chartRange;

            Excel.ChartObjects xlCharts = (Excel.ChartObjects)ws2.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 2000, 500);
            Excel.Chart chartPage = myChart.Chart;

            chartRange = ws.get_Range("A2", "B" + (counter+2));
            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xlColumnClustered;
            chartPage.HasTitle = true;
            chartPage.ChartTitle.Text = "Graphical representation - " + algorithm + " algorithm";
            chartPage.ChartTitle.Font.Name = "Garamond";
            chartPage.ChartTitle.Font.Size = "20";
            chartPage.ChartArea.RoundedCorners = true;
            chartPage.Legend.Font.Name = "Garamond";
            //  chartPage.SetBackgroundPicture("bg.jpg");
            excel.DisplayAlerts = false;

            wb.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            ReleaseObject(ws);
            ReleaseObject(ws2);
            ReleaseObject(wb);
            ReleaseObject(excel);


        }
    }
}
