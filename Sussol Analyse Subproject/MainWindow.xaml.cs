using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Forms;
using System.Threading;
using Sussol_Analyse_Subproject.Analyses;
using Sussol_Analyse_Subproject.Utils;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Sussol_Analyse_Subproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IThreadAwareView
    {
        private readonly SynchronizationContext _syncContext;
       
        public MainWindow()
        {
           
            InitializeComponent();
            var uriBackground = new Uri(@"../../Images/background.png", UriKind.Relative);
            var uriIcon = new Uri(@"../../Images/headericon.jpg", UriKind.Relative);
            GridBackground.ImageSource = new BitmapImage(uriBackground);
            this.Icon = new BitmapImage(uriIcon);
           
            _syncContext = SynchronizationContext.Current; 

        }
        
        ModelService ms = new ModelService();
        string path = "";
        readonly List<string> _algorithmsUsed = new List<string>();
        readonly List<string> _formats = new List<string>();
        readonly List<string> _modellingtypes = new List<string>();
        public bool TextBoxContent = false;
        

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
            {
            var ofd = new OpenFileDialog()
            {
                InitialDirectory = @"C:\",
                Title = "Select your dataset",
                Filter = "CSV |*.csv"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
                TxtBoxDesiredClusters.IsEnabled = true;
                LabelChosenInfo.Content = "You have chosen " + ofd.SafeFileName;
              
            }
            else
            {
                LabelChosenInfo.Content = "Please choose a dataset";
            }
            
                
            }
       
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            LblProgress.Content = "Creating your results! One moment please";
            var value = int.Parse(TxtBoxDesiredClusters.Text);
            //algorithm checkboxes
            if (RdbCanopy.IsChecked != null && RdbCanopy.IsChecked.Value)
                {
                    _algorithmsUsed.Add("canopy");
                }
                if (RdbSom.IsChecked != null && RdbSom.IsChecked.Value)
                {
                    _algorithmsUsed.Add("som");
                }
                if (RdbXmeans.IsChecked != null && RdbXmeans.IsChecked.Value)
                {

                    _algorithmsUsed.Add("xmeans");
                }
                //format checkboxes
                if (RdbCsv.IsChecked != null && RdbCsv.IsChecked.Value)
                {
                    _formats.Add("csv");
                }
                if (RdbRawData.IsChecked != null && RdbRawData.IsChecked.Value)
                {
                    _formats.Add("raw");
                }
                //modellingtype checkboxes
                if (RdbVaried.IsChecked != null && RdbVaried.IsChecked.Value)
                {
                    _modellingtypes.Add("varied");
                }
                if (RdbNested.IsChecked != null && RdbNested.IsChecked.Value)
                {
                    _modellingtypes.Add("nested");
                }


            foreach (var format in _formats)
                {
                    foreach (var algo in _algorithmsUsed)
                    {
                        foreach (var modellingtype in _modellingtypes)
                        {
                        IThreadAwareView view = this;
                        
                            ms.QueueGetResults(path, format, algo, modellingtype, value, view);
                        }
                    }
                }

            ButtonStart.IsEnabled = false;
            BtnOpenFile.IsEnabled = false;
            TxtBoxDesiredClusters.IsEnabled = false;
            RdbCanopy.IsEnabled = false;
            RdbSom.IsEnabled = false;
            RdbXmeans.IsEnabled = false;
            RdbNested.IsEnabled = false;
            RdbVaried.IsEnabled = false;
            RdbCsv.IsEnabled = false;
            RdbRawData.IsEnabled = false;
            }



        private void SetButtonVisibility(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = false;
            
                if (RdbCanopy.IsChecked.Value || RdbSom.IsChecked.Value || RdbXmeans.IsChecked.Value)
                {
                    if (RdbCsv.IsChecked.Value || RdbRawData.IsChecked.Value)
                    {
                        if (RdbVaried.IsChecked.Value || RdbNested.IsChecked.Value)
                        {
                        if (TextBoxContent == true){
                            ButtonStart.IsEnabled = true;
                        }
                        

                    }
                    }
                }
            
          
        }

        private void Setbuttonviisibility(object sender, TextChangedEventArgs e)
        {
            int value;

            if (int.TryParse(TxtBoxDesiredClusters.Text, out value))
            {
                //parsing successful 
                LblDesiredClusters.Foreground = new SolidColorBrush(Colors.White);
                LblDesiredClusters.FontSize = 16;
                TextBoxContent = true;
                RdbCanopy.IsEnabled = true;
                RdbSom.IsEnabled = true;
                RdbXmeans.IsEnabled = true;
                RdbNested.IsEnabled = true;
                RdbVaried.IsEnabled = true;
                RdbCsv.IsEnabled = true;
                RdbRawData.IsEnabled = true;
                LblDesiredClusters.Content = "No. of desired clusters:";
                LblDesiredClusters.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

            }
            else
            {
                //parsing failed. 
                LblDesiredClusters.Foreground = new SolidColorBrush(Colors.Red);
                LblDesiredClusters.FontSize = 16;
                LblDesiredClusters.Content = "Please use a numeric value:";
                LblDesiredClusters.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                TextBoxContent = false;
                RdbCanopy.IsEnabled = false;
                RdbSom.IsEnabled = false;
                RdbXmeans.IsEnabled = false;
                RdbNested.IsEnabled = false;
                RdbVaried.IsEnabled = false;
                RdbCsv.IsEnabled = false;
                RdbRawData.IsEnabled = false;
            }
        }

        public void TaskFinished()
        {

            Dispatcher.Invoke(() =>
            {
                LblProgress.Content = "Modelling finished! You can close the app.";
                LblPercentage.Content = "100%";
                PbLoading.Value = 100000;
            });

        }

        public void ProgressUpdate(int currentModelAmount, int modelsToMake)
        {
            _syncContext.Send(_ => {
                PbLoading.Maximum = modelsToMake;
                if (currentModelAmount > modelsToMake)
                {
                    PbLoading.Value = modelsToMake;
                }
                else
                {
                    PbLoading.Value = currentModelAmount;
                }
               
                double percentage = ((double)currentModelAmount / modelsToMake)*100;
                double roundedPercentage = Math.Round(percentage, 1);
                LblPercentage.Content = roundedPercentage+ "%";
               

            }, null);

        }

        public void WritingToCsv()
        {
            Dispatcher.Invoke(() =>
            {
                LblProgress.Content = "Writing your data to CSV ...";
                LblPercentage.Content = "100%";
            });
        }

        public void AddingGraph()
        {
            Dispatcher.Invoke(() =>
            {
                LblProgress.Content = "Creating your chart ...";
            });
        }

        private void Btn_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Btn_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

       
    }

}
