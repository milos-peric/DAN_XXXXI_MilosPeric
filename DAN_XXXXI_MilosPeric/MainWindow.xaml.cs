using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAN_XXXXI_MilosPeric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dummyPathFileDirectory = @"..\..\";
        private string dummyPathExtension = ".txt";
        private int pageNumber;
        BackgroundWorker backgroundWorker1 = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true,
        };

        /// <summary>
        /// Add methods to delegate invocation list and call InitializeComponent method.
        /// Also disable print button until text is entered.
        /// </summary>
        public MainWindow()
        {
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            InitializeComponent();
            btnPrint.IsEnabled = false;
        }

        /// <summary>
        /// Method does the logic incrementing of progress bar as printing is being done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int x = pageNumber;
            int reporter = 0;
            int temp = 100 / x;
            for (int i = 1; i <= x + 1; i++)
            {
                reporter = i * temp;
                Thread.Sleep(1000);
                // Calling ReportProgress() method raises ProgressChanged event
                // To this method pass the percentage of processing that is complete
                backgroundWorker1.ReportProgress(reporter);
                // Check if the cancellation is requested
                if (backgroundWorker1.CancellationPending)
                {
                    // Set Cancel property of DoWorkEventArgs object to true
                    e.Cancel = true;
                    // Reset progress percentage to ZERO and return
                    backgroundWorker1.ReportProgress(0);
                    return;
                }
            }
            e.Result = reporter;
        }

        /// <summary>
        /// Changes progress bar value and text box tbStatus to match current percentage of task being completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            tbStatus.Text = "Printing: " + e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// Logic of what happens when background worker finishes work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                tbStatus.Text = "Printing cancelled";
            }
            else if (e.Error != null)
            {
                tbStatus.Text = e.Error.Message;
            }
            else
            {
                if ((int)e.Result > 100)
                {
                    tbStatus.Text = "Print completed";
                    MessageBox.Show("Text printed to files in the main project directory.");
                    btnCancel.IsEnabled = false;
                    btnPrint.IsEnabled = true;
                }
                else
                    tbStatus.Text = e.Result.ToString();
            }
        }

        /// <summary>
        /// Logic of what happens when you click on Print button.
        /// Calls Print and RunWorkerAsync methods.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            btnPrint.IsEnabled = false;
            btnCancel.IsEnabled = true;
            int num = int.Parse(tbNumberOfPages.Text);
            pageNumber = num;
            // Check if the backgroundWorker is already busy running the asynchronous operation
            if (!backgroundWorker1.IsBusy)
            {
                // This method will start the execution asynchronously in the background
                backgroundWorker1.RunWorkerAsync();
            }
            Print();
        }

        /// <summary>
        /// Logic of generating a specified number of files with specific name based on page number and print time.
        /// </summary>
        public void Print()
        {
            DateTime dateTime = DateTime.Now;
            int numberOfPages = int.Parse(tbNumberOfPages.Text);
            string textBoxContents = tbTextBoxPrinter.Text;
            for (int i = 1; i <= numberOfPages; i++)
            {
                string day = dateTime.Day.ToString();
                string month = dateTime.Month.ToString();
                string year = dateTime.Year.ToString();
                string hour = dateTime.Hour.ToString();
                string minute = dateTime.Minute.ToString();
                string fullString = string.Format($"{dummyPathFileDirectory}{i}.{day}_{month}_{year}_{hour}_{minute}{dummyPathExtension}");
                File.Create(fullString).Close();
                File.WriteAllText(fullString, textBoxContents);
            }

        }

        /// <summary>
        /// Allows to cancel backgroundWorker by calling CancelAsync method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                // Cancel the asynchronous operation if still in progress
                backgroundWorker1.CancelAsync();
            }
        }

        /// <summary>
        /// Prevents entering charachters other than numbers 0-9 in Text Box Number of pages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNumberOfPages_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(tbNumberOfPages.Text, "[^0-9]"))
            {
                MessageBox.Show("Numbers only please.");
                tbNumberOfPages.Text = tbNumberOfPages.Text.Remove(tbNumberOfPages.Text.Length - 1);
            }
        }

        /// <summary>
        /// Enables or disables Print button depending of status of text within Text Box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbTextBoxPrinter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTextBoxPrinter.Text.Length == 0)
            {
                btnPrint.IsEnabled = false;
            }
            else
            {
                btnPrint.IsEnabled = true;
            }
        }
    }
}
