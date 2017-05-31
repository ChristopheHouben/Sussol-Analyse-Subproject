using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Windows.Forms;
using System.Threading;
using WpfAnimatedGif;
using System.ComponentModel;

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
            var uriBackground = new Uri(@"../Content/backgrounder.png", UriKind.Relative);
            var uriIcon = new Uri(@"../Content/headericon.png", UriKind.Relative);
            GridBackground.ImageSource = new BitmapImage(uriBackground);
            this.Icon = new BitmapImage(uriIcon);
            
            _syncContext = SynchronizationContext.Current; 

        }
        
        modelService ms = new modelService();
        string path = "";
        List<String> algorithmsUsed = new List<String>();
        List<String> formats = new List<String>();
        List<String> modellingtypes = new List<String>();
        bool textBoxContent = false;
        

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = @"C:\";
                ofd.Title = "Select your dataset";
                 ofd.Filter = "CSV |*.csv";
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
            int value = int.Parse(TxtBoxDesiredClusters.Text);
            //algorithm checkboxes
            if (CheckBoxCanopy.IsChecked.Value)
                {
                    algorithmsUsed.Add("canopy");
                }
                if (CheckBoxSom.IsChecked.Value)
                {
                    algorithmsUsed.Add("som");
                }
                if (CheckBoxXmeans.IsChecked.Value)
                {

                    algorithmsUsed.Add("xmeans");
                }
                //format checkboxes
                if (CheckBoxCsv.IsChecked.Value)
                {
                    formats.Add("csv");
                }
                if (CheckBoxRawData.IsChecked.Value)
                {
                    formats.Add("raw");
                }
                //modellingtype checkboxes
                if (CheckBoxVaried.IsChecked.Value)
                {
                    modellingtypes.Add("varied");
                }
                if (CheckBoxNested.IsChecked.Value)
                {
                    modellingtypes.Add("nested");
                }


            foreach (string format in formats)
                {
                    foreach (string algo in algorithmsUsed)
                    {
                        foreach (string modellingtype in modellingtypes)
                        {
                        IThreadAwareView view = this;
                            ms.QueueGetResults(path, format, algo, modellingtype, value, view);
                        }
                    }
                }

            ButtonStart.IsEnabled = false;


        }



        private void setButtonVisibility(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = false;
            
                if (CheckBoxCanopy.IsChecked.Value || CheckBoxSom.IsChecked.Value || CheckBoxXmeans.IsChecked.Value)
                {
                    if (CheckBoxCsv.IsChecked.Value || CheckBoxRawData.IsChecked.Value)
                    {
                        if (CheckBoxVaried.IsChecked.Value || CheckBoxNested.IsChecked.Value)
                        {
                        if (textBoxContent == true){
                            ButtonStart.IsEnabled = true;
                        }
                        

                    }
                    }
                }
            
          
        }

        private void setbuttonviisibility(object sender, TextChangedEventArgs e)
        {
            int value;

            if (int.TryParse(TxtBoxDesiredClusters.Text, out value))
            {
                //parsing successful 
                LblDesired_clusters.Foreground = new SolidColorBrush(Colors.White);
                LblDesired_clusters.FontSize = 14;
                textBoxContent = true;
                CheckBoxCanopy.IsEnabled = true;
                CheckBoxSom.IsEnabled = true;
                CheckBoxXmeans.IsEnabled = true;
                CheckBoxNested.IsEnabled = true;
                CheckBoxVaried.IsEnabled = true;
                CheckBoxCsv.IsEnabled = true;
                CheckBoxRawData.IsEnabled = true;
                LblDesired_clusters.Content = "No. of desired clusters:";
                LblDesired_clusters.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

            }
            else
            {
                //parsing failed. 
                LblDesired_clusters.Foreground = new SolidColorBrush(Colors.Red);
                LblDesired_clusters.FontSize = 14;
                LblDesired_clusters.Content = "Please use a numeric value:";
                LblDesired_clusters.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                textBoxContent = false;
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
                btnOpenFile.IsEnabled = false;
                TxtBoxDesiredClusters.IsEnabled = false;
                CheckBoxCanopy.IsEnabled = false;
                CheckBoxSom.IsEnabled = false;
                CheckBoxXmeans.IsEnabled = false;
                CheckBoxNested.IsEnabled = false;
                CheckBoxVaried.IsEnabled = false;
                CheckBoxCsv.IsEnabled = false;
                CheckBoxRawData.IsEnabled = false;
                lblPercentage.Content = "100%";
                pbLoading.Value = 100000;
            });

        }

        public void ProgressUpdate(int currentModelAmount, int modelsToMake)
        {
            _syncContext.Send(_ => {
                pbLoading.Maximum = modelsToMake;
                pbLoading.Value = currentModelAmount;
                double percentage = ((double)currentModelAmount / modelsToMake)*100;
                double roundedPercentage = Math.Round(percentage, 1);
                lblPercentage.Content = roundedPercentage+ "%";
              

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

    }

}
