using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using SharpCompress;
using SharpCompress.Archive.Rar;
using SharpCompress.Reader;
using SharpCompress.Common;

namespace ProjectForKSONW_XMLParse
{
    class Doc 
    {
        public string idDoc;          //ИдДок
        public string dateState;    //ДатаСост
        public string dateInclude;  //ДатаВклМСП
        public byte typeSubMCP;       //ВидСубМСП
        public byte cateSubMCP;       //КатСубМСП
        public byte specNewMCP;       //ПризНовМСП
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            List<Doc> docs = new List<Doc>();
            string s = @"c:\Users\user\Documents\visual studio 2013\Projects\ProjectForKSONW_XMLParse\ProjectForKSONW_XMLParse\xmlExamples";
            string[] fileEntries = Directory.GetFiles(s);
            foreach (string fileName in fileEntries)
            {
                XmlDocument xd = new XmlDocument();
                System.Console.WriteLine(fileName);
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
            foreach (Doc doc in docs)
            {
                Console.WriteLine(doc.idDoc);
                Console.WriteLine("==ДатаСост:" + doc.dateState);
                Console.WriteLine("==ДатаВклМСП:" + doc.dateInclude);
            }
            Console.WriteLine(docs.Count);

            string s1 = @"c:\Users\user\Documents\visual studio 2013\Projects\ProjectForKSONW_XMLParse\ProjectForKSONW_XMLParse\xmlExamples.rar";
            using (Stream stream = File.OpenRead(s1))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        //reader.WriteEntryToDirectory(@"C:\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }

        }
    }
}
