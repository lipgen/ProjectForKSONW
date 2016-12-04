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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ModernUINavigationApp1.Pages
{
    public class Docum
    {
        public string ID { get; set; }
        public string Name { get; set; }        
    }

    public partial class DataGridPage : UserControl
    {
        procFuns pf = new procFuns();
        static string[] sCol = new string[9];

        public DataGridPage()
        {
            InitializeComponent();
            try
            {
                pf.settingParser(sCol);
                var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                conn.Open();

                var cmd = new MySqlCommand("SELECT * FROM reg19.документ LIMIT 10;", conn);
                List<Docum> docs = new List<Docum>();
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                    docs.Add(new Docum() { ID = dataReader["ID_ДокИдДок"].ToString(), Name = dataReader["ВидСубМСП"].ToString() });
                dataReader.Close();
                dataGrid.ItemsSource = docs;
            }
            catch (Exception ex) { }
            Main();
        }

        void Main()
        {

        }
    }
}
