using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ProjectForKSONW_XMLParse 
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
    class archProcess
    {
        static string projDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        static void filesProcess(List<Doc> docs, string[] fileEntries)
        {
            string s = projDir + @"\temp";
            foreach (string fileName in fileEntries)
            {
                XmlDocument xd = new XmlDocument();
                //System.Console.WriteLine(fileName);
                s = fileName;
                xd.Load(s);
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
            }
        }
        static void Main(string[] args)
        {
            List<Doc> docs = new List<Doc>();
            string[] fileEntries;
            string filePath = projDir + @"\xmlExamples.rar";

            //using Stream begin
            using (Stream stream = File.OpenRead(filePath)) {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry()) {
                    if (!reader.Entry.IsDirectory) {
                        reader.WriteEntryToDirectory(projDir + @"\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        fileEntries = Directory.GetFiles(projDir + @"\temp");
                        if (fileEntries.Length >= 10) {
                            filesProcess(docs, fileEntries);
                            Array.ForEach(Directory.GetFiles(projDir + @"\temp"), File.Delete);
                        }
                    } else {
                        fileEntries = Directory.GetFiles(projDir + @"\temp");
                        if (fileEntries.Length > 0) {
                            filesProcess(docs, fileEntries);
                            Array.ForEach(Directory.GetFiles(projDir + @"\temp"), File.Delete);
                        }
                    }
                }
            }
            //using Stream end

            Console.WriteLine(docs.Count);
        }
        //Main End
    }
}
