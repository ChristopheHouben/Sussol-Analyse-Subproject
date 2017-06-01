using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Forms;
using System.Threading;
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
            if (CheckBoxCanopy.IsChecked != null && CheckBoxCanopy.IsChecked.Value)
                {
                    _algorithmsUsed.Add("canopy");
                }
                if (CheckBoxSom.IsChecked != null && CheckBoxSom.IsChecked.Value)
                {
                    _algorithmsUsed.Add("som");
                }
                if (CheckBoxXmeans.IsChecked != null && CheckBoxXmeans.IsChecked.Value)
                {

                    _algorithmsUsed.Add("xmeans");
                }
                //format checkboxes
                if (CheckBoxCsv.IsChecked != null && CheckBoxCsv.IsChecked.Value)
                {
                    _formats.Add("csv");
                }
                if (CheckBoxRawData.IsChecked != null && CheckBoxRawData.IsChecked.Value)
                {
                    _formats.Add("raw");
                }
                //modellingtype checkboxes
                if (CheckBoxVaried.IsChecked != null && CheckBoxVaried.IsChecked.Value)
                {
                    _modellingtypes.Add("varied");
                }
                if (CheckBoxNested.IsChecked != null && CheckBoxNested.IsChecked.Value)
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
            CheckBoxCanopy.IsEnabled = false;
            CheckBoxSom.IsEnabled = false;
            CheckBoxXmeans.IsEnabled = false;
            CheckBoxNested.IsEnabled = false;
            CheckBoxVaried.IsEnabled = false;
            CheckBoxCsv.IsEnabled = false;
            CheckBoxRawData.IsEnabled = false;
            }



        private void SetButtonVisibility(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = false;
            
                if (CheckBoxCanopy.IsChecked.Value || CheckBoxSom.IsChecked.Value || CheckBoxXmeans.IsChecked.Value)
                {
                    if (CheckBoxCsv.IsChecked.Value || CheckBoxRawData.IsChecked.Value)
                    {
                        if (CheckBoxVaried.IsChecked.Value || CheckBoxNested.IsChecked.Value)
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
                CheckBoxCanopy.IsEnabled = true;
                CheckBoxSom.IsEnabled = true;
                CheckBoxXmeans.IsEnabled = true;
                CheckBoxNested.IsEnabled = true;
                CheckBoxVaried.IsEnabled = true;
                CheckBoxCsv.IsEnabled = true;
                CheckBoxRawData.IsEnabled = true;
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
                CheckBoxCanopy.IsEnabled = false;
                CheckBoxSom.IsEnabled = false;
                CheckBoxXmeans.IsEnabled = false;
                CheckBoxNested.IsEnabled = false;
                CheckBoxVaried.IsEnabled = false;
                CheckBoxCsv.IsEnabled = false;
                CheckBoxRawData.IsEnabled = false;
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
