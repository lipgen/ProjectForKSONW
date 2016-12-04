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
    public partial class StatusPage : UserControl
    {
        procFuns pf = new procFuns();
        static string[] sCol = new string[9];
        string lastArch;

        public StatusPage()
        {
            InitializeComponent();
            Main();
        }

        public void Main()
        {
            pf.settingParser(sCol);
            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
            //fill Label1Date
            try {
                conn.Open();
                var cmd = new MySqlCommand("SELECT `value` FROM db_info WHERE paramName = \"dataDate\";", conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    Label1Date.Content = result;
            } catch (Exception ex) {
                textBox.Text += "Невозможно обратиться к базе данных: \n";
                textBox.Text += ex.Message + "\n";
            }
            conn.Close();
            //fill Label2Date
            try {
                using (var wc = new WebClient()) {
                    wc.DownloadFile(sCol[5], pf.pd + @"\lastHtml.html");
                    string[] data = File.ReadAllLines(pf.pd + @"\lastHtml.html");
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i].Contains("Гиперсылка (URL) на набор"))
                            lastArch = data[i + 1].Substring(data[i + 1].IndexOf("a href=") + 8, data[i + 1].IndexOf(".zip") - 12);
                        if (data[i].Contains("Дата последнего внесения изменений"))
                            Label2Date.Content = data[i + 1].Substring(data[i + 1].IndexOf(">", 0) + 1, data[i + 1].IndexOf(">", 0) + 4);
                    }
                }
            } catch (Exception ex) {
                textBox.Text += "Невозможно получить информацию о состоянии данных: \n" ;
                textBox.Text += ex.Message + "\n";
            }
            conn.Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            archProccess();
        }

        void archProccess()
        {
            string[] fileEntries;
            string filePath = pf.pd + @"\xmlExamples.rar";
            Array.ForEach(Directory.GetFiles(pf.pd + @"\temp"), File.Delete);
            try {
                using (Stream stream = File.OpenRead(filePath)) {
                    var reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry()) {
                            reader.WriteEntryToDirectory(pf.pd + @"\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                            fileEntries = Directory.GetFiles(pf.pd + @"\temp");
                            if (fileEntries.Length == 10) {
                                xmlsProcess(fileEntries);
                                Array.ForEach(Directory.GetFiles(pf.pd + @"\temp"), File.Delete);
                            }
                    }
                    fileEntries = Directory.GetFiles(pf.pd + @"\temp");
                    if (fileEntries.Length > 0) {
                        xmlsProcess(fileEntries);
                        Array.ForEach(Directory.GetFiles(pf.pd + @"\temp"), File.Delete);
                    }
                }
                textBox.Text = "Данные были загружены в базу";        
             }
             catch (Exception ex) {
                 textBox.Text = ex.ToString();
             }
        }

        //XMLs Processing
        void xmlsProcess(string[] fileEntries)
        {
            List<Document> docs = new List<Document>();
            foreach (string fileName in fileEntries)
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(fileName);
                XmlNodeList nodes = xd.DocumentElement.SelectNodes("/Файл/Документ");
                foreach (XmlNode node in nodes) 
                {
                    if (node.SelectSingleNode("СведМН").Attributes["КодРегион"].Value == "19") 
                    {
                        var doc = new Document();
                        //таб "документ"
                        doc.a1 = node.Attributes["ИдДок"].Value;
                        doc.a2 = node.Attributes["ДатаСост"].Value;
                        doc.a3 = node.Attributes["ДатаВклМСП"].Value;
                        doc.a4 = node.Attributes["ВидСубМСП"].Value;
                        doc.a5 = node.Attributes["КатСубМСП"].Value;
                        doc.a6 = node.Attributes["ПризНовМСП"].Value;
                        //таб "сведосн"
                        if (doc.a4 == "1")
                        {
                            doc.b1 = node.SelectSingleNode("ОргВклМСП").Attributes["ИННЮЛ"].Value;
                            doc.b2 = node.SelectSingleNode("ОргВклМСП").Attributes["НаимОрг"].Value;
                            doc.b3 = (node.SelectSingleNode("ОргВклМСП").Attributes["НаимОргСокр"] != null) ?
                                node.SelectSingleNode("ОргВклМСП").Attributes["НаимОргСокр"].Value : "-";
                        }
                        else
                        {
                            doc.b1 = node.SelectSingleNode("ИПВклМСП").Attributes["ИННФЛ"].Value;
                            doc.b2 = node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Фамилия"].Value;
                            doc.b3 = node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Имя"].Value;
                            doc.b4 = (node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Отчество"] != null) ?
                                node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Отчество"].Value : "-";
                        }
                        //таб "сведмн"
                        doc.c1 = node.SelectSingleNode("СведМН").Attributes["КодРегион"].Value;
                        doc.c2 = (node.SelectSingleNode("СведМН").SelectSingleNode("Регион") != null) ?
                                 node.SelectSingleNode("СведМН").SelectSingleNode("Регион").Attributes["Тип"].Value + " " +
                                 node.SelectSingleNode("СведМН").SelectSingleNode("Регион").Attributes["Наим"].Value : "-";
                        doc.c3 = (node.SelectSingleNode("СведМН").SelectSingleNode("Район") != null) ?
                            node.SelectSingleNode("СведМН").SelectSingleNode("Район").Attributes["Тип"].Value + " " +
                            node.SelectSingleNode("СведМН").SelectSingleNode("Район").Attributes["Наим"].Value : "-";
                        doc.c4 = (node.SelectSingleNode("СведМН").SelectSingleNode("Город") != null) ?
                             node.SelectSingleNode("СведМН").SelectSingleNode("Город").Attributes["Тип"].Value + " " +
                             node.SelectSingleNode("СведМН").SelectSingleNode("Город").Attributes["Наим"].Value : "-";
                        doc.c5 = (node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт") != null) ?
                            node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт").Attributes["Тип"].Value + " " +
                            node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт").Attributes["Наим"].Value : "-";
                        //таб "conn_документ-своквэд"
                        if (node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн") != null)
                        {
                            doc.InfoRCoEAmain = new InfoRCoEA();
                            doc.InfoRCoEAmain.a1 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["КодОКВЭД"].Value;
                            if (node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["НаимОКВЭД"] != null)
                            {
                                doc.InfoRCoEAmain.a2 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["НаимОКВЭД"].Value;
                                doc.InfoRCoEAmain.a3 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["ВерсОКВЭД"].Value;
                            }
                            else
                            {
                                doc.InfoRCoEAmain.a2 = "-";
                                doc.InfoRCoEAmain.a3 = "-";
                            }
                            XmlNodeList infos = node.SelectSingleNode("СвОКВЭД").ChildNodes;
                            if (infos.Count > 1)
                            {
                                int i = 0;
                                doc.InfoRCoEAadd = Enumerable.Range(0, infos.Count - 1).Select(j => new InfoRCoEA()).ToArray();
                                foreach (XmlNode info in infos)
                                {
                                    if (info != null && info != infos[0] && info.Attributes["КодОКВЭД"] != null)
                                    {
                                        if (info.Attributes["КодОКВЭД"].Value != null)
                                        {
                                            doc.InfoRCoEAadd[i].a1 = info.Attributes["КодОКВЭД"].Value;
                                            doc.InfoRCoEAadd[i].a2 = info.Attributes["НаимОКВЭД"].Value;
                                            doc.InfoRCoEAadd[i].a3 = info.Attributes["ВерсОКВЭД"].Value;
                                        }
                                        else
                                        {
                                            doc.InfoRCoEAadd[i].a1 = "-";
                                            doc.InfoRCoEAadd[i].a2 = "-";
                                            doc.InfoRCoEAadd[i].a3 = "-";
                                        }
                                        i++;
                                    }
                                }
                            }
                        }  
                        //таб "conn_документсвлиценз"
                        if (node.SelectSingleNode("СвЛиценз") != null)
                        {
                            var clipDoc = new XmlDocument();
                            clipDoc.LoadXml(node.OuterXml);
                            XmlNodeList infos = clipDoc.GetElementsByTagName("СвЛиценз");
                            int i = 0;
                            doc.InfoLics = Enumerable.Range(0, infos.Count).Select(j => new InfoLic()).ToArray();
                            foreach (XmlNode info in infos)
                            {
                                string s = "";
                                if (info.Attributes["СерЛиценз"] != null) s += info.Attributes["СерЛиценз"].Value + " ";
                                s += info.Attributes["НомЛиценз"].Value;
                                if (info.Attributes["ВидЛиценз"] != null) s += " " + info.Attributes["ВидЛиценз"].Value;
                                doc.InfoLics[i].a1 = s;
                                doc.InfoLics[i].a2 = (info.Attributes["ДатаЛиценз"] != null)          ? info.Attributes["ДатаЛиценз"].Value : "00.00.0000";
                                doc.InfoLics[i].a3 = (info.Attributes["ДатаНачЛиценз"] != null)       ? info.Attributes["ДатаНачЛиценз"].Value : "00.00.0000";
                                doc.InfoLics[i].a4 = (info.Attributes["ДатаКонЛиценз"] != null)       ? info.Attributes["ДатаКонЛиценз"].Value : "00.00.0000";
                                doc.InfoLics[i].a5 = (info.Attributes["ОргВыдЛиценз"] != null)        ? info.Attributes["ОргВыдЛиценз"].Value : "-";
                                doc.InfoLics[i].a6 = (info.Attributes["ДатаОстЛиценз"] != null)       ? info.Attributes["ДатаОстЛиценз"].Value : "00.00.0000";
                                doc.InfoLics[i].a7 = (info.Attributes["ОргОстЛиценз"] != null)        ? info.Attributes["ОргОстЛиценз"].Value : "-";
                                doc.InfoLics[i].a8 = (info.SelectSingleNode("НаимЛицВД") != null)     ? info.FirstChild.InnerText : "-";
                                doc.InfoLics[i].a9 = (info.SelectSingleNode("СведАдрЛицВД") != null)  ? info.LastChild.InnerText : "-";
                                i++;
                            }
                        }
                        fillDataIntoDB(doc);
                    }
                }
            }
        }

        void fillDataIntoDB(Document doc)
        {
            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
            try
            {
                conn.Open();
                                    
                //таб "документ" и "сведосн"
                if (doc.a4 == "1") { 
                    string[] paramName = { "_ID_ДокИдДок", "_ДатаСост", "_ДатаВклМСП", "_ВидСубМСП", "_КатСубМСП", "_ПризНовМСП", 
                                           "_КодРегион", "_Регион", "_Район", "_Город", "_НаселПункт", 
                                           "_ID_СведОснИНН", "_ЮЛ_НаимОрг", "_ЮЛ_НаимОргСокр" };
                    string[] paramCont = { doc.a1, doc.a2, doc.a3, doc.a4, doc.a5, doc.a6, doc.c1, doc.c2, doc.c3, doc.c4, doc.c5, doc.b1, doc.b2, doc.b3 };
                    pf.execStorProc("insert_документюл", paramName, paramCont, conn);
                    
                } else {
                    string[] paramName = { "_ID_ДокИдДок", "_ДатаСост", "_ДатаВклМСП", "_ВидСубМСП", "_КатСубМСП", "_ПризНовМСП", 
                                           "_КодРегион", "_Регион", "_Район", "_Город", "_НаселПункт", 
                                           "_ID_СведОснИНН", "_ИП_Фамилия", "_ИП_Имя", "_ИП_Отчество" };
                    string[] paramCont = { doc.a1, doc.a2, doc.a3, doc.a4, doc.a5, doc.a6, doc.c1, doc.c2, doc.c3, doc.c4, doc.c5, doc.b1, doc.b2, doc.b3, doc.b4 };
                    pf.execStorProc("insert_документип", paramName, paramCont, conn);
                }

                //таб "dir_районы" и "dir_города" 
                var cmd1 = new MySqlCommand("SELECT * FROM reg19.dir_районы WHERE НаимРайона = '" + doc.c3 + "';", conn);
                if (cmd1.ExecuteScalar() == null)
                {
                    string[] paramName1 = { "_НаимРайона" };
                    string[] paramCont1 = { doc.c3 };
                    pf.execStorProc("insert_dir_районы", paramName1, paramCont1, conn);
                }
                cmd1 = new MySqlCommand("SELECT * FROM reg19.dir_города WHERE НаимГород = '" + doc.c4 + "';", conn);
                if (cmd1.ExecuteScalar() == null)
                {
                    string[] paramName1 = { "_НаимГород" };
                    string[] paramCont1 = { doc.c3 };
                    pf.execStorProc("insert_dir_города", paramName1, paramCont1, conn);
                }

                //таб "своквэд"
                if (doc.InfoRCoEAmain != null)
                {
                    var cmd = new MySqlCommand("SELECT * FROM reg19.dir_своквэд WHERE ID_СвОКВЭДКодОКВЭД = '" + doc.InfoRCoEAmain.a1 + "';", conn);
                    if (cmd.ExecuteScalar() == null)
                    {
                        string[] paramName = { "_КодОКВЭД", "_НаимОКВЭД", "_ВерсОКВЭД" };
                        string[] paramCont = { doc.InfoRCoEAmain.a1, doc.InfoRCoEAmain.a2, doc.InfoRCoEAmain.a3 };
                        pf.execStorProc("insert_свокэд", paramName, paramCont, conn);
                    }
                    
                    cmd = new MySqlCommand("SELECT * FROM reg19.conn_документсвоквэд WHERE Id_ДокИдДок = '" + doc.a1
                                         + "' AND Id_СвОКВЭДКодОКВЭД = '" + doc.InfoRCoEAmain.a1 + "' AND Основ = '1';", conn);

                    if (cmd.ExecuteScalar() == null)
                    {
                        string[] paramName = { "_Id_ДокИдДок", "_Id_СвОКВЭДКодОКВЭД", "_Основ" };
                        string[] paramCont = { doc.a1, doc.InfoRCoEAmain.a1, "1" };
                        pf.execStorProc("insert_conn_документсвоквэд", paramName, paramCont, conn);

                    }

                    if (doc.InfoRCoEAadd != null)
                    {
                        foreach (InfoRCoEA info in doc.InfoRCoEAadd)
                        {

                            cmd = new MySqlCommand("SELECT * FROM reg19.dir_своквэд WHERE ID_СвОКВЭДКодОКВЭД = '" + doc.InfoRCoEAmain.a1 + "';", conn);
                            if (cmd.ExecuteScalar() == null)
                            {
                                string[] paramName = { "_КодОКВЭД", "_НаимОКВЭД", "_ВерсОКВЭД" };
                                string[] paramCont = { info.a1, info.a2, info.a3 };
                                pf.execStorProc("insert_свокэд", paramName, paramCont, conn);
                            }

                            cmd = new MySqlCommand("SELECT * FROM reg19.conn_документсвоквэд WHERE Id_ДокИдДок = '" + doc.a1
                                                 + "' AND Id_СвОКВЭДКодОКВЭД = '" + info.a1 + "' AND Основ = '0';", conn);
                            if (cmd.ExecuteScalar() == null)
                            {
                                string[] paramName = { "_Id_ДокИдДок", "_Id_СвОКВЭДКодОКВЭД", "_Основ" };
                                string[] paramCont = { doc.a1, info.a1, "0" };
                                pf.execStorProc("insert_conn_документсвоквэд", paramName, paramCont, conn);
                            }
                        }
                    }
                }
                //таб "СвЛиценз"
                if (doc.InfoLics != null)
                {
                    foreach (InfoLic info in doc.InfoLics)
                    {
                        var cmd = new MySqlCommand("SELECT * FROM reg19.conn_документсвлиценз WHERE ID_СвЛицензСерНомВидЛиценз = '" + info.a1 + "';", conn);
                        if (cmd.ExecuteScalar() == null)
                        {
                            string[] paramName = { "_ID_СвЛицензСерНомВидЛиценз" , "_ДатаЛиценз", "_ДатаНачЛиценз", "_ДатаКонЛиценз", "_ОргВыдЛиценз", "_ДатаОстЛиценз",
                                                   "_ОргОстЛиценз", "_НаимЛицВД", "_СведАдрЛицВД", "_Id_ДокИдДок" };
                            string[] paramCont = { info.a1, info.a2, info.a3, info.a4, info.a5, info.a6, info.a7, info.a8, info.a9, doc.a1 };
                            pf.execStorProc("`insert_conn_документсвлиценз`", paramName, paramCont, conn);
                        }
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                textBox.Text = "Error " + ex.Number + " has occurred: " + ex.Message;
            }
        }

        static string pd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            pb.Value = e.ProgressPercentage;
            label1.Text = e.ProgressPercentage + "%" + " " + (e.BytesReceived / 1000000) + "/" + (e.TotalBytesToReceive / 1000000);
        }
        void downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else
                MessageBox.Show("Completed!!!");
            button.IsEnabled = true;
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string url = lastArch;
            button.IsEnabled = false;
            WebClient downloader = new WebClient();
            downloader.DownloadFileCompleted += new AsyncCompletedEventHandler(downloader_DownloadFileCompleted);
            downloader.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloader_DownloadProgressChanged);
            downloader.DownloadFileAsync(new Uri(url), pd + @"\temp.zip");
        }
    }
}
