using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;


namespace Sussol_Analyse_Subproject
{
    public class FileWriter
    {
        public void Writetextfile(String directory, String filename, JObject jObject, String counter, String featureInfo)
        {
            string onlyName = "";
            if (filename.Contains("\\"))
            {
                onlyName= filename.Split('\\').Last();
            }
                string path = Path.Combine(directory, onlyName);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {
                    str.Write("===============================================\n" + featureInfo + "\n ===============================================\n");
                    str.Write(" --------------------------------------\nParameter value " + counter + "\n --------------------------------------\n");
                    str.Write(jObject);

                }

            }
        }
        
        public void createCsv(String directory, string algorithm, string filename, string setname)
        {
            
         

            //filename = algo.ToString().ToUpper() + "results " + "dataset= " + setname + ".csv";
                     
            string fullPath = Path.Combine(directory, filename);
         
            object missing = Type.Missing;
            object misValue = System.Reflection.Missing.Value;

            //create excel
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            //add excel workbook
            Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Add();
         
            Excel.Worksheet ws;
            ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
            //add worksheets to workbook
           
            //Adjust all columns
            ws.Columns.AutoFit();

            //freeze top row
            // ws.Application.ActiveWindow.FreezePanes = true;
            ws.Cells[1, 1] = algorithm + "results";
            ws.Cells[1, 2] = "Dataset: " + setname;
            ws.Cells[1, 3] = "Generated on: " + DateTime.Now;
            ws.Cells[2, 2] = "Number of clusters";
            
            wb.SaveAs(fullPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            releaseObject(ws);
            releaseObject(wb);
            releaseObject(excel);
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            
            }
            finally
            {
                GC.Collect();
            }
        }
        public void WriteCsv(String directory, List<String> usedParameter,String excelfilename, List<int> results, string algorithm)
        {
           
            
            string fullPath = Path.Combine(directory, excelfilename);
        

                object missing = Type.Missing;

            object misValue = System.Reflection.Missing.Value;

            //create excel
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            //add excel workbook
            Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Add();
             
                wb = excel.Workbooks.Open(fullPath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            

            Excel.Worksheet ws;
            ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
            //add worksheets to workbook
            var totalResults = results.Count();
            for (var o = 0; o < totalResults; o++) {
                ws.Cells[o+3, 1] = usedParameter[o];
                ws.Cells[o+3, 2] = results[o];
            }
            //Adjust all columns
            ws.Columns.AutoFit();

            //freeze top row
            // ws.Application.ActiveWindow.FreezePanes = true;


            wb.SaveAs(fullPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            releaseObject(ws);
        
            releaseObject(wb);
            releaseObject(excel);


        }

     
        public void addGraph(string path, int counter) {
            object missing = Type.Missing;

            object misValue = System.Reflection.Missing.Value;

            //create excel
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            //add excel workbook
            Microsoft.Office.Interop.Excel.Workbook wb;
            if (File.Exists(path))
            {
                wb = excel.Workbooks.Open(path, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            }
            else { wb = excel.Workbooks.Add(); }
           
            Excel.Worksheet ws;
            ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
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

            //freeze top row
            // ws.Application.ActiveWindow.FreezePanes = true;

            //insert graph into worsheet 2
            Excel.Range chartRange;

            Excel.ChartObjects xlCharts = (Excel.ChartObjects)ws2.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 2000, 500);
            Excel.Chart chartPage = myChart.Chart;
            
            chartRange = ws.get_Range("A2", "B" + counter);
            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xlColumnClustered;

            wb.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            wb.Close(true, misValue, misValue);
            excel.Quit();

            releaseObject(ws);
            releaseObject(ws2);
            releaseObject(wb);
            releaseObject(excel);
         

        }
    }
}
