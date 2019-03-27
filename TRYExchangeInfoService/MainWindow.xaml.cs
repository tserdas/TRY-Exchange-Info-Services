using Microsoft.Win32;
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

namespace TRYExchangeInfoService
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }        

        private void Btn_Info_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.turgayserdas.com");
        }

        private void Btn_Shutdown_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }        

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            dPick_From.SelectedDate = null;
            dPick_End.SelectedDate = null;
            dgrid.ItemsSource = null;
        }

        private void Btn_Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xls)|*.xls";
            saveFileDialog.FileName = DateTime.Now.Date.ToShortDateString() + cBox.Text + "Export";

            if (saveFileDialog.ShowDialog() == true)
            {
                dgrid.SelectAllCells();
                dgrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, dgrid);
                String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                String result = (string)Clipboard.GetData(DataFormats.Text);
                dgrid.UnselectAllCells();
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog.FileName);
                file.WriteLine(result.Replace(',', ' '));
                file.Close();

                MessageBox.Show("Exported to Excel file");
            }
        }

        private void Btn_Get_Click(object sender, RoutedEventArgs e)
        {
            List<bindInfo> bindInfos = new List<bindInfo>();
            bool serviceError = false;
            if (dPick_From.SelectedDate > dPick_End.SelectedDate)
                MessageBox.Show("From Date should be smaller than End Date");
            else if (dPick_From.SelectedDate == null && dPick_End.SelectedDate == null)
                MessageBox.Show("Please select dates");
            else if (dPick_From.SelectedDate == null)
                MessageBox.Show("Please select From Date");
            else if (dPick_End.SelectedDate == null)
                MessageBox.Show("Please select End Date");
            else
            {
                DateTime fromdate = dPick_From.SelectedDate.Value.Date;
                while(fromdate<=dPick_End.SelectedDate.Value.Date)
                {
                    getProcess getProcess = new getProcess(fromdate);
                    if (getProcess.GetExchRate(cBox.Text, ExchRateType.ForexBuying) != 0)
                    {
                        bindInfos.Add(new bindInfo
                        {
                            Date = fromdate.ToShortDateString(),
                            Currency = cBox.Text,
                            Buy = getProcess.GetExchRate(cBox.Text, ExchRateType.ForexBuying).ToString(),
                            Sell = getProcess.GetExchRate(cBox.Text, ExchRateType.ForexSelling).ToString()
                        });
                    }
                    else
                    {
                        bindInfos.Add(new bindInfo
                        {
                            Date = fromdate.ToShortDateString(),
                            Currency = cBox.Text,
                            Buy = String.Empty,
                            Sell = String.Empty
                        });
                        serviceError = true;
                    }
                    fromdate = fromdate.Date.AddDays(1);
                }
                if (serviceError)
                    MessageBox.Show("Data of some days are missing");

                dgrid.ItemsSource = bindInfos;
            }

        }
    }
}
