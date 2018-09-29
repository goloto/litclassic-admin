using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace litclassic
{
    [DataContract(Name = "ListBooks")]
    public class ListBooks
    {
        [DataMember(Name = "listBooksPath")]
        public List<string> listBooksPath = new List<string>(); // список "путей" книг
        [DataMember(Name = "listBooks")]
        public List<string> listBooks = new List<string>(); // список книг


        public ListBooks()
        {
            //Book overview = new Book();
            //overview.title = "Serialization Overview";
            //ListBooks listBooks=new ListBooks();
            //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ListBooks));

            //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//ListBooks.xml";
            //System.IO.FileStream file = System.IO.File.Create(path);

            //writer.Serialize(file, listBooks);
            //file.Close();
        }

        // внешние get-функции переменных класса
        public List<string> getListBooks() // получение списка книг
        {
            return listBooks;
        } 
        public List<string> getListBooksPath() // получения списка "путей" книг
        {
            return listBooksPath;
        }


        // внешние set-функции переменных класса
        public void setListBooksPath(List<string> newListBooksPath) // установление списка "путей" книг
        {
            listBooksPath = newListBooksPath;
        }
        public void setListBooks(List<string> newListBooks) // установление списка книг
        {
            listBooks = newListBooks;
        }


        // внешние невозвращающие функции класса
        public void addBook(string bookName, string bookPath) // добавление в списки названия книги и "пути" книги
        {
            listBooks.Add(bookName);
            listBooksPath.Add(bookPath);
        }
        public void deleteBook(int index) // удаление из списков книги и "пути" книги
        {
            listBooks[index].Remove(index,1);
            listBooksPath[index].Remove(index, 1);
        }
    }
}
