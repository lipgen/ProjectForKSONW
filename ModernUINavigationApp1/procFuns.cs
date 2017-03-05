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
        public string a1 { get; set; }//ИдДок
        public string a2 { get; set; }//ДатаСост                     
        public string a3 { get; set; }//ДатаВклМСП                   
        public string a4 { get; set; }//ВидСубМСП                    
        public string a5 { get; set; }//КатСубМСП
        public string a6 { get; set; }//ПризНовМСП                   
        //таб "сведосн"
        public string b1 { get; set; }//ИННФЛ или ИННЮЛ              
        public string b2 { get; set; }//ЮЛ_НаимОрг или ИП_Фамилия   
        public string b3 { get; set; }//ЮЛ_НаимОргСокр или ИП_Имя                   
        public string b4 { get; set; }//ИП_Отчество                 
        //таб "сведмн"
        public string c1 { get; set; }//КодРегион                    
        public string c2 { get; set; }//Регион                      
        public string c3 { get; set; }//Район  
        public string c4 { get; set; }//Город
        public string c5 { get; set; }//НаселПункт 
        //таб "conn_документ-своквэд"
        public InfoRCoEA InfoRCoEAmain;//ОснОКВЭД
        public InfoRCoEA[] InfoRCoEAadd;//ДопСвОКВЭД
        public List<InfoRCoEA> lInfoRCoEA;
        //таб "conn_документ-свлиценз"
        public InfoLic[] InfoLics;
    }
    //Класс для хранения копии данных СвОКВЭД - "Russian Classification of Economic Activities"
    public class InfoRCoEA
    {
        public string a1 { get; set; }//КодОКВЭД
        public string a2 { get; set; }//НаимОКВЭД
        public string a3 { get; set; }//ВерсОКВЭД
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
        //public string pd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        public string pd = Directory.GetCurrentDirectory();
        public void settingParser(string[] sCol)
        {
            string[] data;
            if (File.Exists(pd + @"\setting.ini")) {
                data = File.ReadAllLines(pd + @"\setting.ini");
            } else {
                data = new string[] { "[db_setting]",
                                      "server=localhost",                                            //sCol[0]
                                      "user=root",                                                   //sCol[1]
                                      "database=reg19",                                              //sCol[2]
                                      "port=3306",                                                   //sCol[3]
                                      "password=1234",                                               //sCol[4]
                                      "[data_getting_setting]",
                                      "rsmpURL=https://www.nalog.ru/opendata/7707329152-rsmp",
                                      "lastArchDate=01.01.2016",
                                      "lastArchURL=",
                                      "deletePriv=1",
                                      "[excel_setting]",
                                      "officeVersion =",
                                      "savePath =};"
                };
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

    }
}
