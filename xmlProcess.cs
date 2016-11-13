
using System;
using System.IO;
using System.Data;
using System.Xml;
using System.Collections.Generic;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace xmlProcess 
{
    class Doc
    {
        public string idDoc;        //ИдДок
        public string dateState;    //ДатаСост
        public string dateInclude;  //ДатаВклМСП
        public byte typeSubMCP;     //ВидСубМСП
        public byte cateSubMCP;     //КатСубМСП
        public byte specNewMCP;     //ПризНовМСП
    }

    class Program
    {
        static string projDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        static void Main(string[] args)
        {
            List<Doc> docs = new List<Doc>();

            string xml = "VO_RRMSPSV_0000_9965_20160920_000c637d-d3de-4f45-af67-555c5aedbe72.xml";
            string pathToXml = projDir + @"\xmlExamples\" + xml;
            XmlDocument xd = new XmlDocument();
            xd.Load(pathToXml);
            XmlNodeList nodes = xd.DocumentElement.SelectNodes("/Файл/Документ");
            //book.author = node.SelectSingleNode("author").InnerText; 
            foreach (XmlNode node in nodes)
            {
                Doc doc = new Doc();
                doc.idDoc = node.Attributes["ИдДок"].Value;
                doc.dateState = node.Attributes["ДатаСост"].Value;
                doc.dateInclude = node.Attributes["ДатаВклМСП"].Value;
                docs.Add(doc);
            }
            Console.WriteLine(docs.Count);

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;user=root;database=reg19;port=3306;password=1234;";
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                

                foreach (var doc in docs)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "insert_документип";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@_ID_ДокИП", doc.idDoc);
                    cmd.Parameters["@_ID_ДокИП"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_ДатаСост", doc.dateState);
                    cmd.Parameters["@_ДатаСост"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_ДатаВклМСП", doc.dateInclude);
                    cmd.Parameters["@_ДатаВклМСП"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }

        }
    }
}
