using MySql.Data.MySqlClient;
using System;
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
            procFuns pf = new procFuns();
            string[] data = File.ReadAllLines(pd + @"\setting.ini");
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
