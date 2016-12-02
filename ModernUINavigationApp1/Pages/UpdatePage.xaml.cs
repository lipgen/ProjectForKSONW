using MySql.Data.MySqlClient;
using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernUINavigationApp1.Pages
{
    /// <summary>
    /// Interaction logic for UpdatePage.xaml
    /// </summary>



    public partial class UpdatePage : UserControl
    {
        static string pd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            
            pb.Value = e.ProgressPercentage;
            label1.Text = e.ProgressPercentage + "%" + " " + e.BytesReceived +"/" + e.TotalBytesToReceive;
        }
        void downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else
                MessageBox.Show("Completed!!!");
            button.IsEnabled = true;
        }
        public UpdatePage()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] data = File.ReadAllLines(pd + @"\actualdata.csv");
            string[] spl = data[8].Split(';');
            string url = spl[2];
            button.IsEnabled = false;
            WebClient downloader = new WebClient();
            downloader.DownloadFileCompleted += new AsyncCompletedEventHandler(downloader_DownloadFileCompleted);
            downloader.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloader_DownloadProgressChanged);
            downloader.DownloadFileAsync(new Uri(url), pd + @"\temp.zip");
        }
    }
}
