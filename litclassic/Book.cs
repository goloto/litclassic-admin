using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace litclassic
{
    class Book
    {
        // 02.10
        // раз будет отдельная БД с цитатами, то может быть поместить все вызовы работы OutSourcing, Coloring и Comprehension здесь?


        private string bookPath;
        private string[] bookColors;
        private string[] bookContent;
        private string bookName;
        private string bookAuthor;
        private Outsourcing currentOutsourcing = new Outsourcing();

        public void BuildingBook()
        {
            //string sqlConnection = ("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C: \\Users\\chaklimcbosten\\Documents\\Visual Studio 2017\\Projects\\litclassic\\litclassic\\Books.mdf;Integrated Security=True;Connect Timeout=30");
            //string sqlConnection = "AttachDbFilename = C: \\Users\\chaklimcbosten\\Documents\\Visual Studio 2017\\Projects\\litclassic\\litclassic\\Books.mdf; Database = Books;Trusted_Connection = Yes;";
            //string sqlConnection = "Server = (localdb)\v13.0; Integrated Security = true; AttachDbFileName = C: \\Users\\chaklimcbosten\\Documents\\Visual Studio 2017\\Projects\\litclassic\\litclassic\\Books.mdf;";
            //string sqlConnection = "Server =SQLExpress; AttachDbFilename =C: \\Users\\chaklimcbosten\\Documents\\Visual Studio 2017\\Projects\\litclassic\\litclassic\\Books.mdf; Database = Books;Trusted_Connection = Yes;";

            currentOutsourcing.Run(bookPath);

            bookName = currentOutsourcing.GetTitle();
            bookAuthor = currentOutsourcing.GetAuthor();
            bookColors = currentOutsourcing.GetArrayColorTags();

            string sqlConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\chaklimcbosten\\Documents\\Visual Studio 2017\\Projects\\litclassic\\litclassic\\Books.mdf;";
            SqlConnection currentSQLConnection = new SqlConnection(sqlConnection);

            currentSQLConnection.Open();

            SqlCommand insertCommand = new SqlCommand("INSERT INTO Books VALUES(@idBook, @titleBook, @authorBook, @pathBook);", currentSQLConnection);
            SqlCommand selectCommand = new SqlCommand("SELECT MAX(idBook) FROM Books;", currentSQLConnection);
            SqlDataReader currentSelectReader = selectCommand.ExecuteReader();

            object bookId = currentSelectReader.GetValue(0);

            insertCommand.Parameters.Add(new SqlParameter("idBook", bookId));
            insertCommand.Parameters.Add(new SqlParameter("titleBook", bookName));
            insertCommand.Parameters.Add(new SqlParameter("authorBook", bookAuthor));
            insertCommand.Parameters.Add(new SqlParameter("pathBook", bookPath));
            insertCommand.ExecuteNonQuery();
            currentSQLConnection.Close();

            // запись названия, автора и пути книги в БД
        }

        public void BuildingQuotes()
        {

        }

        public void AddingBookToDB()
        {

        }
        public void AddingQuoteToDB()
        {

        }


        public string GetBookPath()
        {
            return bookPath;
        }
        public string[] GetBookColors()
        {
            return bookColors;
        }
        public string[] GetBookContent()
        {
            return bookContent;
        }
        public string GetBookName()
        {
            return bookName;
        }
        public string GetBookAuthor()
        {
            return bookAuthor;
        }


        public void SetBookPath(string newBookPath)
        {
            bookPath = newBookPath;
        }
        public void SetBookColors(string[] newBookColors)
        {
            bookColors = newBookColors;
        }
        public void SetBookContent(string[] newBookContent)
        {
            bookContent = newBookContent;
        }
        public void SetBookName(string newBookName)
        {
            bookName = newBookName;
        }
        public void SetBookAuthor(string newBookAuthor)
        {
            bookAuthor = newBookAuthor;
        }
    }
}
