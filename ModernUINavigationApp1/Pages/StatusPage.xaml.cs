using MySql.Data.MySqlClient;
using SharpCompress.Archive;
using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using WMPLib;

namespace ModernUINavigationApp1.Pages
{
    public partial class StatusPage : UserControl
    {
        procFuns pf = new procFuns();
        BackgroundWorker bw = new BackgroundWorker();
        MySqlConnection conn = new MySqlConnection();
        WindowsMediaPlayer wmp = new WindowsMediaPlayer();

        static string[] sCol = new string[11];
        string lastArchURL;
        string lastArchName;

        public StatusPage() {
            InitializeComponent();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            pf.settingParser(sCol);
            conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");

            Main();
        }

        public void Main() {
            //fill Label1Date
            wmp.settings.volume = 20;
            try {
                conn.Open();
                var cmd = new MySqlCommand("SELECT `value` FROM reg19.db_info WHERE paramName = \"dataDate\";", conn);
                Label1Date.Content = cmd.ExecuteScalar();
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Подключение к базе выполнено успешно \n";
                wmp.URL = @"sounds\fanfare.wav";
                wmp.controls.play();
            } catch (Exception ex) {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Невозможно обратиться к базе данных: \n";
                textBox.Text += ex.Message + "\n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();
            }
            conn.Close();
            //fill Label2Date
            try {
                using (var wc = new WebClient()) {
                    wc.DownloadFile(sCol[5], pf.pd + @"\lastHtml.html");
                    string[] data = File.ReadAllLines(pf.pd + @"\lastHtml.html");
                    for (int i = 0; i < data.Length; i++) {
                        if (data[i].Contains("Гиперсылка (URL) на набор"))
                        {
                            lastArchURL = data[i + 1].Substring(data[i + 1].IndexOf("a href=") + 8, data[i + 1].IndexOf(".zip") - 12);
                            string[] spl = lastArchURL.Split('/');
                            lastArchName = spl[spl.Length - 1];
                        }
                        if (data[i].Contains("Дата последнего внесения изменений"))
                            Label2Date.Content = data[i + 1].Substring(data[i + 1].IndexOf(">", 0) + 1, data[i + 1].IndexOf(">", 0) + 4);
                    }
                }
            } catch (Exception ex) {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Невозможно получить информацию о состоянии данных: \n";
                textBox.Text += ex.Message + "\n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();
            }

        }
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e) {
            if (bw.IsBusy != true)
                bw.RunWorkerAsync();
        }
        private void ButtonCancle_Click(object sender, RoutedEventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
                bw.CancelAsync();
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                string[] fileEntries;
                string filePath = pf.pd + @"\" + lastArchName;
                var archive = ArchiveFactory.Open(filePath);
                long l = archive.TotalSize;
                archive.Dispose();
                Array.ForEach(Directory.GetFiles(pf.pd + @"\temp"), File.Delete);
                int i = 0;
                using (Stream stream = File.OpenRead(filePath))
                {
                    var reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        if ((worker.CancellationPending == true))
                        {
                            e.Cancel = true;
                            return;
                        }
                        else
                        {
                            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                            conn.Open();
                            var cmd = new MySqlCommand("UPDATE reg19.db_info SET db_info.value = '" + (i++) + "' WHERE paramName = 'procesedXml';", conn);
                            cmd.ExecuteScalar();

                            reader.WriteEntryToDirectory(pf.pd + @"\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                            fileEntries = Directory.GetFiles(pf.pd + @"\temp");
                            xmlsProcess(fileEntries);
                            worker.ReportProgress(i);
                            Array.ForEach(Directory.GetFiles(pf.pd + @"\temp"), File.Delete);
                        }
                    }
                }
                var conn1 = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
                conn1.Open();
                var cmd1 = new MySqlCommand("UPDATE db_info SET `value`= '01.05.2016' WHERE paramName = '" + Label2Date.Content + "';", conn1);
                cmd1.ExecuteScalar();
            }
            catch (Exception ex) {

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Произошла ошибка во время портирования данных: \n";
                    textBox.Text += ex.Message + "\n";
                }));
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var archive = ArchiveFactory.Open(pf.pd + @"\" + lastArchName);
            pb.Value = ((e.ProgressPercentage * 100) / (double)archive.Entries.Count());
            label1.Text = string.Format("{0:N2}%", (pb.Value)) + " " + e.ProgressPercentage + "/" + archive.Entries.Count() + " XMLs"; ;
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Портирование было отменено \n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();
            }
            else if (!(e.Error == null))
            {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + (" Произошла ошибка во время портирования данных: \n");
                textBox.Text += e.Error.Message + "\n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();
            }
            else
            {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Данные были загружены в базу \n";
                wmp.URL = @"sounds\success.wav";
                wmp.controls.play();
            }
        }
        private void buttonDownload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                buttonDownload.IsEnabled = false;
                WebClient dl = new WebClient();
                dl.DownloadFileCompleted += new AsyncCompletedEventHandler(dl_DownloadFileCompleted);
                dl.DownloadProgressChanged += new DownloadProgressChangedEventHandler(dl_DownloadProgressChanged);
                dl.DownloadFileAsync(new Uri(lastArchURL), pf.pd + @"\" + lastArchName);
            }
            catch (Exception ex)
            {
                textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " Невозможно загрузить архив: \n";
                textBox.Text += ex.Message + "\n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();
            }
        }
        void dl_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pb.Value = e.ProgressPercentage;
            label1.Text = pb.Value + "%" + " " + (pb.Value / 1000000) + "/" + (pb.Value / 1000000) + " Mb";
        }
        void dl_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.Message);
            else MessageBox.Show("Процесс получения архива завершен");
            buttonDownload.IsEnabled = true;
        }
        void xmlsProcess(string[] fileEntries)
        {
            List<Document> docs = new List<Document>();
            foreach (string fileName in fileEntries)
            {
                try
                { 
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileName);
                    XmlNodeList nodes = xd.DocumentElement.SelectNodes("/Файл/Документ");
                    foreach (XmlNode node in nodes) {
                        if (node.SelectSingleNode("СведМН").Attributes["КодРегион"].Value == "19") {
                            //таб "документ"
                            var doc = new Document() {
                                a1 = node.Attributes["ИдДок"].Value,
                                a2 = DateTime.ParseExact(node.Attributes["ДатаСост"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                                a3 = DateTime.ParseExact(node.Attributes["ДатаВклМСП"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                                a4 = node.Attributes["ВидСубМСП"].Value,
                                a5 = node.Attributes["КатСубМСП"].Value, a6 = node.Attributes["ПризНовМСП"].Value };
                            //таб "сведосн"
                            if (doc.a4 == "1") {
                                doc.b1 = node.SelectSingleNode("ОргВклМСП").Attributes["ИННЮЛ"].Value;
                                doc.b2 = node.SelectSingleNode("ОргВклМСП").Attributes["НаимОрг"].Value;
                                doc.b3 = (node.SelectSingleNode("ОргВклМСП").Attributes["НаимОргСокр"] != null) ?
                                    node.SelectSingleNode("ОргВклМСП").Attributes["НаимОргСокр"].Value : "-";
                            } else {
                                doc.b1 = node.SelectSingleNode("ИПВклМСП").Attributes["ИННФЛ"].Value;
                                doc.b2 = node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Фамилия"].Value.ToLower();
                                doc.b3 = node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Имя"].Value.ToLower();
                                doc.b4 = (node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Отчество"] != null) ?
                                    node.SelectSingleNode("ИПВклМСП").SelectSingleNode("ФИОИП").Attributes["Отчество"].Value.ToLower() : "-";
                            }
                            //таб "сведмн"
                            doc.c1 = node.SelectSingleNode("СведМН").Attributes["КодРегион"].Value;
                            doc.c2 = (node.SelectSingleNode("СведМН").SelectSingleNode("Регион") != null) ?
                                node.SelectSingleNode("СведМН").SelectSingleNode("Регион").Attributes["Тип"].Value.ToLower() + " " +
                                node.SelectSingleNode("СведМН").SelectSingleNode("Регион").Attributes["Наим"].Value.ToLower() : "-";
                            doc.c3 = (node.SelectSingleNode("СведМН").SelectSingleNode("Район") != null) ?
                                node.SelectSingleNode("СведМН").SelectSingleNode("Район").Attributes["Тип"].Value.ToLower() + " " +
                                node.SelectSingleNode("СведМН").SelectSingleNode("Район").Attributes["Наим"].Value.ToLower() : "-";
                            doc.c4 = (node.SelectSingleNode("СведМН").SelectSingleNode("Город") != null) ?
                                node.SelectSingleNode("СведМН").SelectSingleNode("Город").Attributes["Тип"].Value.ToLower() + " " +
                                node.SelectSingleNode("СведМН").SelectSingleNode("Город").Attributes["Наим"].Value.ToLower() : "-";
                            doc.c5 = (node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт") != null) ?
                                node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт").Attributes["Тип"].Value.ToLower() + " " +
                                node.SelectSingleNode("СведМН").SelectSingleNode("НаселПункт").Attributes["Наим"].Value.ToLower() : "-";
                            //таб "conn_документ-своквэд"
                            if (node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн") != null) {
                                doc.InfoRCoEAmain = new InfoRCoEA();
                                doc.InfoRCoEAmain.a1 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["КодОКВЭД"].Value;
                                if (node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["НаимОКВЭД"] != null) {
                                    doc.InfoRCoEAmain.a2 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["НаимОКВЭД"].Value;
                                    doc.InfoRCoEAmain.a3 = node.SelectSingleNode("СвОКВЭД").SelectSingleNode("СвОКВЭДОсн").Attributes["ВерсОКВЭД"].Value;
                                } else {
                                    doc.InfoRCoEAmain.a2 = "-";
                                    doc.InfoRCoEAmain.a3 = "-";
                                }
                                XmlNodeList infos = node.SelectSingleNode("СвОКВЭД").ChildNodes;
                                if (infos.Count > 1) {
                                    int i = 0;
                                    doc.InfoRCoEAadd = Enumerable.Range(0, infos.Count - 1).Select(j => new InfoRCoEA()).ToArray();
                                    foreach (XmlNode info in infos) {
                                        if (info != null && info != infos[0] && info.Attributes["КодОКВЭД"] != null) {
                                            if (info.Attributes["КодОКВЭД"].Value != null) {
                                                doc.InfoRCoEAadd[i].a1 = info.Attributes["КодОКВЭД"].Value;
                                                doc.InfoRCoEAadd[i].a2 = info.Attributes["НаимОКВЭД"].Value;
                                                doc.InfoRCoEAadd[i].a3 = info.Attributes["ВерсОКВЭД"].Value;
                                            } else {
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
                            if (node.SelectSingleNode("СвЛиценз") != null) {
                                var clipDoc = new XmlDocument();
                                clipDoc.LoadXml(node.OuterXml);
                                XmlNodeList infos = clipDoc.GetElementsByTagName("СвЛиценз");
                                int i = 0;
                                doc.InfoLics = Enumerable.Range(0, infos.Count).Select(j => new InfoLic()).ToArray();
                                foreach (XmlNode info in infos) {
                                    string s = "";
                                    if (info.Attributes["СерЛиценз"] != null) s += info.Attributes["СерЛиценз"].Value + " ";
                                    s += info.Attributes["НомЛиценз"].Value;
                                    if (info.Attributes["ВидЛиценз"] != null) s += " " + info.Attributes["ВидЛиценз"].Value;
                                    doc.InfoLics[i].a1 = s;
                                    doc.InfoLics[i].a2 = (info.Attributes["ДатаЛиценз"] != null) ?
                                        DateTime.ParseExact(info.Attributes["ДатаЛиценз"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : "0000-00-00";
                                    doc.InfoLics[i].a3 = (info.Attributes["ДатаНачЛиценз"] != null) ?
                                        DateTime.ParseExact(info.Attributes["ДатаНачЛиценз"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : "0000-00-00";
                                    doc.InfoLics[i].a4 = (info.Attributes["ДатаКонЛиценз"] != null) ?
                                        DateTime.ParseExact(info.Attributes["ДатаКонЛиценз"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : "0000-00-00";
                                    doc.InfoLics[i].a5 = (info.Attributes["ОргВыдЛиценз"] != null) ? info.Attributes["ОргВыдЛиценз"].Value : "-";
                                    doc.InfoLics[i].a6 = (info.Attributes["ДатаОстЛиценз"] != null) ?
                                        DateTime.ParseExact(info.Attributes["ДатаОстЛиценз"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : "0000-00-00";
                                    doc.InfoLics[i].a7 = (info.Attributes["ОргОстЛиценз"] != null) ? info.Attributes["ОргОстЛиценз"].Value : "-";
                                    doc.InfoLics[i].a8 = (info.SelectSingleNode("НаимЛицВД") != null) ? info.FirstChild.InnerText : "-";
                                    doc.InfoLics[i].a9 = (info.SelectSingleNode("СведАдрЛицВД") != null) ? info.LastChild.InnerText : "-";
                                    i++;
                                }
                            }
                            fillDataIntoDB(doc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        void fillDataIntoDB(Document doc)
        {
            var conn = new MySqlConnection("server=" + sCol[0] + ";user=" + sCol[1] + ";database=" + sCol[2] + ";port=" + sCol[3] + ";password=" + sCol[4] + ";");
            try
            {
                conn.Open();
                var cmdDoc = new MySqlCommand("SELECT * FROM reg19.документ WHERE ID_ДокИдДок = '" + doc.a1 +
                                               "' AND ДатаСост = '" + doc.a2 + "' AND ПризНовМСП = '" + doc.a6 + "';", conn);
                if (cmdDoc.ExecuteScalar() == null)
                {
                    //таб "документ"
                    if (doc.a4 == "1")
                    {
                        string[] paramName = { "_ID_ДокИдДок", "_ДатаСост", "_ДатаВклМСП", "_ВидСубМСП", "_КатСубМСП", "_ПризНовМСП",
                                           "_КодРегион", "_Регион", "_Район", "_Город", "_НаселПункт",
                                           "_ID_СведОснИНН", "_ЮЛ_НаимОрг", "_ЮЛ_НаимОргСокр" };
                        string[] paramCont = { doc.a1, doc.a2, doc.a3, doc.a4, doc.a5, doc.a6, doc.c1, doc.c2, doc.c3, doc.c4, doc.c5, doc.b1, doc.b2, doc.b3 };
                        pf.execStorProc("insert_документюл", paramName, paramCont, conn);

                    }
                    else
                    {
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
                        string[] paramCont1 = { doc.c4 };
                        pf.execStorProc("insert_dir_города", paramName1, paramCont1, conn);
                    }
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
                conn.Close();
            }
            catch (Exception ex)
            {
                /*textBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + (" Произошла ошибка во время портирования данных: \n");
                textBox.Text += ex.Message + "\n";
                wmp.URL = @"sounds\error.wav";
                wmp.controls.play();*/
            }
        }
 
    }
}
