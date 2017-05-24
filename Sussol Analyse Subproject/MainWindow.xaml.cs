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
namespace Sussol_Analyse_Subproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        modelService ms = new modelService();
        string path = "";
        List<String> algorithmsUsed = new List<String>();
        List<String> formats = new List<String>();
        List<String> modellingtypes = new List<String>();
       
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = @"C:\";
                ofd.Title = "Select your dataset";
                 ofd.Filter = "CSV |*.csv";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                 path = ofd.FileName;

            }
            LabelDataSetName.Content = ofd.SafeFileName;
                
            }
       
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //algorithm checkboxes
            if (CheckBoxCanopy.IsChecked.Value) {
                algorithmsUsed.Add("canopy");
            }
            if (CheckBoxSom.IsChecked.Value) {
                algorithmsUsed.Add("som");
            }
            if (CheckBoxXmeans.IsChecked.Value) {

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
            if(CheckBoxNested.IsChecked.Value) {
                modellingtypes.Add("nested");
            }
            foreach (string format in formats)
            {
                foreach (string algo in algorithmsUsed)
                {
                    foreach (string modellingtype in modellingtypes)
                    {
                        ms.GetResultFiles(path, format, algo, modellingtype);
                    }
                }
            }
            
        }

        private void LabelDataSetName_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           // setButtonVisibility();
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
                        ButtonStart.IsEnabled = true;
                    }
                }
            }
          
        }

    }

}
