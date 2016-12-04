using MySql.Data.MySqlClient;
using SharpCompress.Common;
using SharpCompress.Reader;
using Excel = Microsoft.Office.Interop.Excel; 
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
    public partial class DataPage : UserControl
    {
        procFuns pf = new procFuns();
        static string[] sCol = new string[9];
        
        public DataPage()
        {
            InitializeComponent();
            
            pf.settingParser(sCol);
            try {
                var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                conn.Open();
                //fill comboBox1
                comboBox1.Items.Add("");
                var cmd = new MySqlCommand("SELECT * FROM reg19.dir_районы;", conn);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) comboBox1.Items.Add(dataReader["НаимРайона"].ToString());
                dataReader.Close();
                //fill comboBox2
                comboBox2.Items.Add("");
                cmd = new MySqlCommand("SELECT * FROM reg19.dir_города;", conn);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) comboBox2.Items.Add(dataReader["НаимГород"].ToString());
                conn.Close();
            } catch (Exception ex) {
                textBox.Text += ex.ToString();
            }
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
            conn.Open();
            string sql = "SELECT * FROM reg19.документ WHERE";
            //ЮЛ
            if (checkBox1.IsChecked == true) {
                try {
                    sql += " ВидСубМСП = '1' ";
                    var cmd = new MySqlCommand(sql, conn);
                    var dataReader = cmd.ExecuteReader();
                    while (dataReader.Read()) textBox.Text += dataReader["ID_ДокИдДок"].ToString() + "\n";
                    dataReader.Close();
                }
                catch (Exception ex) { textBox.Text += ex.ToString(); }
            }
            //ИП
            if (checkBox2.IsChecked == true) {
                try {
                    sql += (checkBox1.IsChecked == true) ? " AND ВидСубМСП = '2' " : " ВидСубМСП = '2' ";
                    var cmd = new MySqlCommand(sql, conn);
                    var dataReader = cmd.ExecuteReader();
                    while (dataReader.Read()) textBox.Text += dataReader["ID_ДокИдДок"].ToString() + "\n";
                    dataReader.Close();
                }
                catch (Exception ex) { textBox.Text += ex.ToString(); }
            }
            sql += ";";
            pf.CreateSheet();
        }
      
    }
}
