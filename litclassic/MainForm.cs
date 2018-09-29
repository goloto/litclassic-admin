using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace litclassic
{
    public partial class MainForm : Form
    {
        DataContractSerializer bookSerializer = new DataContractSerializer(typeof(Book));
        DataContractSerializer listBooksSerializer = new DataContractSerializer(typeof(List<string>));
        ListBooks listBooks = new ListBooks();

        public MainForm()
        {
            InitializeComponent();





            //ListBooksDeserialize();
            //ListBooksSerialize();

            //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ListBooks));

            //System.IO.FileStream file = System.IO.File.Create(path);
            //writer.Serialize(file, listBooks);
            //file.Close();
        }

        private void ListBooksSerialize()
        {
            string listBooksPath = Application.StartupPath + "//ListBooks.xml";
            FileStream fs = new FileStream(listBooksPath, FileMode.Create);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8);
            listBooksSerializer.WriteObject(xdw, listBooks);
            fs.Close();
        }
        private void ListBooksDeserialize()
        {
            string listBooksPath = Application.StartupPath + "//ListBooks.xml";
            FileStream fs = new FileStream(listBooksPath, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            listBooks = (ListBooks)listBooksSerializer.ReadObject(reader);
            fs.Close();
        }

        private void BookListLoad()
        {
            
        }
        private void BookDiagnostic()
        {

        }
        private void BookAd()
        {
            openFileDialog1.ShowDialog();

            string path = openFileDialog1.FileName;
        }
        private void BookUpdate()
        {

        }

        private void buttonBookAd_Click(object sender, EventArgs e) // добавить книгу 
        {
            Book newBook = new Book();

            openFileDialog1.ShowDialog(); // открытие диалогово окна
            newBook.SetBookPath(openFileDialog1.FileName); // присваивание путя книги
            newBook.BuildingBook();
            // нужно отделить все срединные процессы в книге от добавления результатов в базу

            //newBook.SetBook(path);
            //listBooks.listBooksPath.Add(newBookPath);
            //ListBooksSerialize();
        }

        
    }
}
