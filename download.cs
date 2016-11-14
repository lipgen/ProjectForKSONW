using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace db_xml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        public class Excel
        {
            public const string UID = "Excel.Application";
            object oExcel = null;
            object WorkBooks, WorkBook, WorkSheets, WorkSheet, Range, Interior;

            //КОНСТРУКТОР КЛАССА
            public Excel()
            {
                oExcel = Activator.CreateInstance(Type.GetTypeFromProgID(UID));
            }

            //ВИДИМОСТЬ EXCEL - СВОЙСТВО КЛАССА
            public bool Visible
            {
                set
                {
                    if (false == value)
                        oExcel.GetType().InvokeMember("Visible", BindingFlags.SetProperty,
                            null, oExcel, new object[] { false });

                    else
                        oExcel.GetType().InvokeMember("Visible", BindingFlags.SetProperty,
                            null, oExcel, new object[] { true });
                }
            }


            //ОТКРЫТЬ ДОКУМЕНТ
            public void OpenDocument(string name)
            {
                WorkBooks = oExcel.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, oExcel, null);
                WorkBook = WorkBooks.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, WorkBooks, new object[] { name, true });
                WorkSheets = WorkBook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, WorkBook, null);
                WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { 1 });
                // Range = WorkSheet.GetType().InvokeMember("Range",BindingFlags.GetProperty,null,WorkSheet,new object[1] { "A1" });
            }

            //ЗАКРЫТЬ ДОКУМЕНТ
            public void CloseDocument()
            {
                WorkBook.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, WorkBook, new object[] { true });
            }

            //ЧТЕНИЕ ДАННЫХ ИЗ ВЫБРАННОЙ ЯЧЕЙКИ
            public string GetValue(string range)
            {
                Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty,
                    null, WorkSheet, new object[] { range });
                return Range.GetType().InvokeMember("Value", BindingFlags.GetProperty,
                    null, Range, null).ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            string urlexl = "https://www.nalog.ru/opendata/7707329152-rsmp/7707329152-rsmp.csv";
            string save_path = "C:\\test\\";
            string nameexl = "actualdata.csv";
            wc.DownloadFile(urlexl, save_path + nameexl);
            //Excel doc = new Excel();
           // doc.OpenDocument(save_path+"\\actualdata.csv");
           string[] data = File.ReadAllLines(@"C:\test\actualdata.csv");
           string[] spl = data[8].Split(';');
           string url = spl[2];
           label1.Text = url;
           // doc.CloseDocument();
           // string url = "https://www.nalog.ru/opendata/7707329152-rsmp/data-10122016-structure-08012016.zip";
           string name = "archive.zip";
           wc.DownloadFileAsync(new Uri(url), save_path + name);
           wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
        }

       
       void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                progressBar1.Maximum = 100;
                progressBar1.Value = e.ProgressPercentage;
            }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
