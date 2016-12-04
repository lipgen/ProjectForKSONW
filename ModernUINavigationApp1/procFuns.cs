using MySql.Data.MySqlClient;
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
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices; 

namespace ModernUINavigationApp1
{
    //Класс для хранения копии XML узла
    public class Document
    {
        //таб "документ"
        public string a1;//ИдДок
        public string a2;//ДатаСост                     
        public string a3;//ДатаВклМСП                   
        public string a4;//ВидСубМСП                    
        public string a5;//КатСубМСП
        public string a6;//ПризНовМСП                   
        //таб "сведосн"
        public string b1;//ИННФЛ или ИННЮЛ              
        public string b2;//ЮЛ_НаимОрг или ИП_Фамилия   
        public string b3;//ЮЛ_НаимОргСокр или ИП_Имя                   
        public string b4;//ИП_Отчество                 
        //таб "сведмн"
        public string c1;//КодРегион                    
        public string c2;//Регион                      
        public string c3;//Район  
        public string c4;//Город
        public string c5;//НаселПункт 
        //таб "conn_документ-своквэд"
        public InfoRCoEA InfoRCoEAmain;//ОснОКВЭД
        public InfoRCoEA[] InfoRCoEAadd;//ДопСвОКВЭД
        //таб "conn_документ-свлиценз"
        public InfoLic[] InfoLics;
    }
    //Класс для хранения копии данных СвОКВЭД - "Russian Classification of Economic Activities"
    public class InfoRCoEA
    {
        public string a1;//КодОКВЭД
        public string a2;//НаимОКВЭД
        public string a3;//ВерсОКВЭД
    }
    //Класс для хранения копии данных СвЛиценз
    public class InfoLic
    {
        public string a1;//СерНомВидЛиценз
        public string a2;//ДатаЛиценз
        public string a3;//ДатаНачЛиценз
        public string a4;//ДатаКонЛиценз
        public string a5;//ОргВыдЛиценз
        public string a6;//ДатаОстЛиценз
        public string a7;//ОргОстЛиценз
        public string a8;//НаимЛицВД
        public string a9;//СведАдрЛицВД
    }

    class procFuns
    {
        public string pd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        public void settingParser(string[] sCol)
        {
            string[] data;
            if (File.Exists(pd + @"\setting.ini")) {
                data = File.ReadAllLines(pd + @"\setting.ini");
            } else {
                data = new string[] { "[db_setting]", "server=localhost", "user=root", "database=reg19", "port=3306", "password=1234",
                                      "[data_getting_setting]", "rsmpURL=https://www.nalog.ru/opendata/7707329152-rsmp", "lastArchDate=01.01.2016",
                                      "lastArchURL=", "deletePriv=1" };
                File.WriteAllLines(pd + @"\setting.ini", data);
            }
            int j = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (!data[i].Contains("[")) {
                    string[] spl = data[i].Split('=');
                    sCol[j] = spl[1];
                    j++;
                }
            }
        }

        public void execStorProc(string spName, string[] paramName, string[] paramCont, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < paramName.Length; i++)
            {
                cmd.Parameters.AddWithValue("@" + paramName[i], paramCont[i]);
                cmd.Parameters["@" + paramName[i]].Direction = ParameterDirection.Input;
            }
            cmd.ExecuteNonQuery();
        }

        public void CreateSheet()
        {
            try
            {


                Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }


                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet.Cells[1, 1] = "ID";
                xlWorkSheet.Cells[1, 2] = "Name";
                xlWorkSheet.Cells[2, 1] = "1";
                xlWorkSheet.Cells[2, 2] = "One";
                xlWorkSheet.Cells[3, 1] = "2";
                xlWorkSheet.Cells[3, 2] = "Two";

                xlWorkBook.SaveAs(@"c:\tmp\csharp-Excel.xls", Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                MessageBox.Show("Excel file created , you can find the file c:\\csharp-Excel.xls");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
