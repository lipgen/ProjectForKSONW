using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace ModernUINavigationApp1.Pages
{
    public partial class DataPage : UserControl
    {
        procFuns pf = new procFuns();
        
        static string[] sCol = new string[11];
        int totalOutputs = 0;
        
        public DataPage()
        {
            InitializeComponent();
            
            pf.settingParser(sCol);
            try {
                var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                conn.Open();
                //fill comboBox1
                comboBox1.Items.Add("");
                comboBox1.SelectedIndex = 0;
                var cmd = new MySqlCommand("SELECT * FROM reg19.dir_районы;", conn);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) comboBox1.Items.Add(dataReader["НаимРайона"].ToString());
                dataReader.Close();
                //fill comboBox2
                comboBox2.Items.Add("");
                comboBox2.SelectedIndex = 0;
                cmd = new MySqlCommand("SELECT * FROM reg19.dir_города;", conn);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) comboBox2.Items.Add(dataReader["НаимГород"].ToString());
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            
            List<Document> docs = new List<Document>();
            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
            conn.Open();
            string sql = "SELECT * FROM reg19.документ WHERE ";
            if (checkBox1.IsChecked == false && checkBox2.IsChecked == false)
            {
                sql += " (ВидСубМСП = '1' OR ВидСубМСП = '2') ";
            }
            //ЮЛ
            if (checkBox1.IsChecked == true) {
                sql += (checkBox2.IsChecked == true) ? " (ВидСубМСП = '1' " : "ВидСубМСП = '1' ";
            }
            //ИП
            if (checkBox2.IsChecked == true) {
                sql += (checkBox1.IsChecked == true) ? " OR ВидСубМСП = '2') " : " ВидСубМСП = '2' ";
            }
            if (comboBox1.SelectedValue.ToString() != "")
            {
                sql += " AND Район = '" + comboBox1.SelectedValue.ToString() + "' ";
            }
            else if (comboBox2.SelectedValue.ToString() != "")
            {
                sql += " AND Город = '" + comboBox2.SelectedValue.ToString() + "' ";
            }
            if (datePicker1.SelectedDate != null && datePicker2.SelectedDate != null)
            {
                DateTime dt2 = datePicker1.SelectedDate.Value;
                string data2 = dt2.Year + "-" + dt2.Month + "-" + dt2.Day;
                DateTime dt1 = datePicker2.SelectedDate.Value;
                string data1 = dt1.Year + "-" + dt1.Month + "-" + dt1.Day;
                sql += " AND ДатаВклМСП BETWEEN '" + data1 + "' AND '" + data2 + "' ";
            }
            sql += " ;";
            try
            {
                using (conn)
                {
                    var cmd = new MySqlCommand(sql, conn);
                    var dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        string clipF = "-";
                        if (dataReader["ИП_Фамилия"] != null)
                        {
                            string clipC = dataReader["ИП_Фамилия"].ToString();
                            clipF = (clipC.Length > 1) ? clipC[0].ToString().ToUpper() + clipC.Substring(1) + " " : clipC.ToUpper() + " ";
                            clipC = dataReader["ИП_Имя"].ToString();
                            clipF += (clipC.Length > 1) ? clipC[0].ToString().ToUpper() + clipC.Substring(1) + " " : clipC.ToUpper() + " ";
                            clipC = (dataReader["ИП_Отчество"] != null) ? dataReader["ИП_Отчество"].ToString() : " ";
                            clipF += (clipC.Length > 1) ? clipC[0].ToString().ToUpper() + clipC.Substring(1) : clipC.ToUpper();
                        }

                        docs.Add(new Document()
                        {
                            a1 = dataReader["ID_ДокИдДок"].ToString(),
                            a2 = dataReader["ДатаСост"].ToString(),
                            a3 = dataReader["ДатаВклМСП"].ToString(),
                            a4 = dataReader["ВидСубМСП"].ToString(),
                            a5 = dataReader["КатСубМСП"].ToString(),
                            a6 = dataReader["ПризНовМСП"].ToString(),
                            b1 = dataReader["ID_СведОснИНН"].ToString(),
                            b2 = dataReader["ЮЛ_НаимОрг"].ToString(),
                            b3 = dataReader["ЮЛ_НаимОргСокр"].ToString(),
                            b4 = clipF,
                            c3 = dataReader["Район"].ToString(),
                            c4 = dataReader["Город"].ToString(),
                            c5 = dataReader["НаселПункт"].ToString()
                        });
                        if (checkBox7.IsChecked == true)
                        {
                            sql = "SELECT  * FROM conn_документсвоквэд left join dir_своквэд ON " +
                                  "conn_документсвоквэд.Id_СвОКВЭДКодОКВЭД = dir_своквэд.ID_СвОКВЭДКодОКВЭД " +
                                  "WHERE Id_ДокИдДок = '" + docs[docs.Count - 1].a1 + "';";
                            var conn1 = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                            conn1.Open();
                            var cmd1 = new MySqlCommand(sql, conn1);
                            var dr1 = cmd1.ExecuteReader();
                            if (dr1.HasRows)
                            {
                                docs[docs.Count - 1].lInfoRCoEA = new List<InfoRCoEA>();
                                while (dr1.Read())
                                {
                                    InfoRCoEA info = new InfoRCoEA()
                                    {
                                        a1 = (dr1["НаимОКВЭД"] != null) ? dr1["НаимОКВЭД"].ToString() : "-",
                                        a2 = dr1["Id_СвОКВЭДКодОКВЭД"].ToString(),
                                        a3 = dr1["Основ"].ToString()
                                    };
                                    docs[docs.Count - 1].lInfoRCoEA.Add(info);
                                }
                                dr1.Close();
                                conn1.Close();
                            }
                        }
                        totalOutputs++;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }
            Excel.Workbook wb;
            Excel.Worksheet ws;
            object misValue = System.Reflection.Missing.Value;
            wb = xlApp.Workbooks.Add(misValue);
            ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);

            try
            {
                ws.Cells[1, 1] = "Идентификатор документа";
                ws.Cells[1, 2] = "Дата реестра";
                ws.Cells[1, 3] = "Дата включения";
                ws.Cells[1, 4] = "Вид субъекта";
                ws.Cells[1, 5] = "Категория субъекта";
                ws.Cells[1, 6] = "Новое вхождение";
                ws.Cells[1, 7] = "ИНН";
                ws.Cells[1, 8] = "Наименование организации";
                ws.Cells[1, 9] = "Наименвоание орагнизации сокр ";
                ws.Cells[1, 10] = "ФИО ИП";
                ws.Cells[1, 11] = "Район";
                ws.Cells[1, 12] = "Город";
                ws.Cells[1, 13] = "НаселПункт";
                int j = 1;
                for (int i = 1; i <= totalOutputs; i++)
                {
                    ws.Cells[j + 1, 1] = docs[i - 1].a1;
                    ws.Cells[j + 1, 2] = docs[i - 1].a2;
                    ws.Cells[j + 1, 3] = docs[i - 1].a3;
                    ws.Cells[j + 1, 4] = (docs[i - 1].a4 == "1") ? "ЮЛ" : "ИП";
                    if (docs[i - 1].a5 == "1") ws.Cells[j + 1, 5] = "Микропредприятие";
                    else if (docs[i - 1].a5 == "2") ws.Cells[j + 1, 5] = "Малое предприятие";
                    else ws.Cells[j + 1, 5] = "Среднее предприятие";
                    //ws.Cells[j + 1, 6] = docs[i - 1].a6;
                    ws.Cells[j + 1, 6] = (docs[i - 1].a6 == "1") ? "Да" : "Нет";
                    ws.Cells[j + 1, 7] = docs[i - 1].b1;
                    ws.Cells[j + 1, 8] = docs[i - 1].b2;
                    ws.Cells[j + 1, 9] = docs[i - 1].b3;
                    ws.Cells[j + 1, 10] = docs[i - 1].b4;
                    ws.Cells[j + 1, 11] = docs[i - 1].c3;
                    ws.Cells[j + 1, 12] = docs[i - 1].c4;
                    ws.Cells[j + 1, 13] = docs[i - 1].c5;
                    if (docs[i - 1].a6 == "1" && checkBox6.IsChecked == true)
                    {
                        Excel.Range formatRange;
                        formatRange = ws.get_Range("A" + (j + 1), "M" + (j + 1));
                        formatRange.EntireRow.Interior.ColorIndex = 20;
                        formatRange.EntireRow.Font.Italic = true;
                    }
                    if (docs[i - 1].lInfoRCoEA != null)
                    {
                        foreach (var lic in docs[i - 1].lInfoRCoEA)
                        {
                            j++;
                            ws.Cells[j + 1, 1] = lic.a2;
                            ws.Cells[j + 1, 2] = (lic.a3 == "True") ? "Осн" : "Доп";
                            ws.Cells[j + 1, 3] = lic.a1;
                        }
                    }
                    j++;
                }
                Excel.Range aRange;
                aRange = ws.get_Range("A1", "M1");
                aRange.EntireRow.Font.Bold = true;
                aRange.EntireColumn.AutoFit();
                wb.SaveAs(dialog.SelectedPath + "\\Report.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                wb.Close(true, misValue, misValue);
                xlApp.Quit();
                Marshal.ReleaseComObject(ws);
                Marshal.ReleaseComObject(wb);
                Marshal.ReleaseComObject(xlApp);
                totalOutputs = 0;
                MessageBox.Show(@"Файл был успешно создан!");
            }
            catch (Exception ex)
            {
                xlApp.Quit();
                Marshal.ReleaseComObject(ws);
                Marshal.ReleaseComObject(wb);
                Marshal.ReleaseComObject(xlApp);
                totalOutputs = 0;
            }
        }
       
    }
}
