using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litclassic
{
    struct Description
    {
        // <title-info>
        private string genre;

        // <author>
        private string firstName;
        private string middleName;
        private string lastName;
        // </author>

        private string bookTitle;
        private string annotation;
        private string date;
        private string coverpage;
        private string lang;
        private string srcLang; 

        // <translator>
        private string translatorFirstName;
        private string translatorMiddleName;
        private string translatorLastName;
        private string translatorNickname;
        private string translatorHomePage;
        private string translatorEmail;
        // </translator>

        private string sequence; // видимо, это номер в серии книг
        // <title-info>

        // <scr-title-info>
        // </scr-title-info>

        // <document-info>

        // <author>
        private string docAuthorFirstName;
        private string docAuthorMiddleName;
        private string docAuthorLastName;
        private string docAuthorNickname;
        private string docAuthorHomePage;
        private string docAuthorEmail;
        // </author>

        private string programUsed;
        private string docDate;
        private string srcURL;
        private string srcOCR;
        private string docID;
        private string version;
        private string history; // история книги; включает в себя все теги, предназначенные для тектса
        // </document-info>

        // <publish-info>
        private string bookName;
        private string publisher;
        private string city;
        private string year;
        private string binary;
        private string fictionBook;
        // </publish-info>

        // <custom-info>
        // </custom-info>



        // title-info
        public void SetGenre(string newGenre)
        {
            genre = newGenre;
        }
        public void SetFirstName(string newFirstName)
        {
            firstName = newFirstName;
        }
        public void SetMiddleName(string newMiddleName)
        {
            middleName = newMiddleName;
        }
        public void SetLastName (string newLastName)
        {
            lastName = newLastName;
        }
        public void SetBookTitle(string newBookTitle)
        {
            bookTitle = newBookTitle;
        }
        public void SetAnnotation(string newAnnotation)
        {
            annotation = newAnnotation;
        }
        public void SetDate(string newDate)
        {
            date = newDate;
        }
        public void SetCoverpage(string newCoverpage)
        {
            coverpage = newCoverpage;
        }
        public void SetLang(string newLang)
        {
            lang = newLang;
        }
        public void SetSrcLang(string newSrcLang)
        {
            srcLang = newSrcLang;
        }
        public void SetTranslatorFirstName (string newTranslatorFirstName)
        {
            translatorFirstName = newTranslatorFirstName;
        }
        public void SetTranslatorMiddleName(string newTranslatorMiddleName)
        {
            translatorMiddleName = newTranslatorMiddleName;
        }
        public void SetTranslatorLastName(string newTranslatorLastName)
        {
            translatorLastName = newTranslatorLastName;
        }
        public void SetTranslatorNickname(string newTranslatorNickname)
        {
            translatorNickname = newTranslatorNickname;
        }
        public void SetTranslatorHomePage(string newTranslatorHomePage)
        {
            translatorHomePage = newTranslatorHomePage;
        }
        public void SetTranslatorEmail(string newTranslatorEmail)
        {
            translatorEmail = newTranslatorEmail;
        }
        public void SetSequence(string newSequence)
        {
            sequence = newSequence;
        }
        // document-info
        public void SetDocAuthorFirstName(string newDocAuthorFirstName)
        {
            docAuthorFirstName = newDocAuthorFirstName;
        }
        public void SetDocAuthorMiddleName(string newDocAuthorMiddleName)
        {
            docAuthorMiddleName = newDocAuthorMiddleName;
        }
        public void SetDocAuthorLastName(string newDocAuthorLastName)
        {
            docAuthorLastName = newDocAuthorLastName;
        }
        public void SetDocAuthorNickname(string newDocAuthorNickname)
        {
            docAuthorNickname = newDocAuthorNickname;
        }
        public void SetDocAuthorHomePage(string newDocAuthorHomePage)
        {
            docAuthorHomePage = newDocAuthorHomePage;
        }
        public void SetDocAuthorEmail(string newDocAuthorEmail)
        {
            docAuthorEmail = newDocAuthorEmail;
        }
        public void SetProgrammUsed(string newProgrammUsed)
        {
            programUsed = newProgrammUsed;
        }
        public void SetDocDate(string newDocDate)
        {
            docDate = newDocDate;
        }
        public void SetSrcURL(string newSrcURL)
        {
            srcURL = newSrcURL;
        }
        public void SetSrcOCR(string newSrcOCR)
        {
            srcOCR = newSrcOCR;
        }
        public void SetDocID(string newDocID)
        {
            docID = newDocID;
        }
        public void SetVersion(string newVersion)
        {
            version = newVersion;
        }
        public void SetHistory(string newHistory)
        {
            history = newHistory;
        }
        // publish-info
        public void SetBookName(string newBookName)
        {
            bookName = newBookName;
        }
        public void SetPublisher(string newPublisher)
        {
            publisher = newPublisher;
        }
        public void SetCity(string newCity)
        {
            city = newCity;
        }
        public void SetYear(string newYear)
        {
            year = newYear;
        }
        public void SetBinary(string newBinary)
        {
            binary = newBinary;
        }
        public void SetFictionBook(string newFictionBook)
        {
            fictionBook = newFictionBook;
        }



        public string GetGenre()
        {
            return genre;
        }
        public string GetFirstName()
        {
            return firstName;
        }
        public string GetMiddleName()
        {
            return middleName;
        }
        public string GetLastName()
        {
            return lastName;
        }
        public string GetBookTitle()
        {
            return bookTitle;
        }
        public string GetAnnotation()
        {
            return annotation;
        }
        public string GetDate()
        {
            return date;
        }
        public string GetCoverpage()
        {
            return coverpage;
        }
        public string GetLang()
        {
            return lang;
        }
        public string GetSrcLang()
        {
            return srcLang;
        }
        public string GetTranslatorFirstName()
        {
            return translatorFirstName;
        }
        public string GetTranslatorMiddleName()
        {
            return translatorMiddleName;
        }
        public string GetTranslatorLastName()
        {
            return translatorLastName;
        }
        public string GetTranslatorNickname()
        {
            return translatorNickname;
        }
        public string GetTranslatorHomePage()
        {
            return translatorHomePage;
        }
        public string GetTranslatorEmail()
        {
            return translatorEmail;
        }
        public string GetSequence()
        {
            return sequence;
        }
        // document-info
        public string GetDocAuthorFirstName()
        {
            return docAuthorFirstName;
        }
        public string GetDocAuthorMiddleName()
        {
            return docAuthorMiddleName;
        }
        public string GetDocAuthorLastName()
        {
            return docAuthorLastName;
        }
        public string GetDocAuthorNickname()
        {
            return docAuthorNickname;
        }
        public string GetDocAuthorHomePage()
        {
            return docAuthorHomePage;
        }
        public string GetDocAuthorEmail()
        {
            return docAuthorEmail;
        }
        public string GetProgrammUsed()
        {
            return programUsed;
        }
        public string GetDocDate()
        {
            return docDate;
        }
        public string GetSrcURL()
        {
            return srcURL;
        }
        public string GetSrcOCR()
        {
            return srcOCR;
        }
        public string GetDocID()
        {
            return docID;
        }
        public string GetVersion()
        {
            return version;
        }
        public string GetHistory()
        {
            return history;
        }
        // publish-info
        public string GetBookName()
        {
            return bookName;
        }
        public string GetPublisher()
        {
            return publisher;
        }
        public string GetCity()
        {
            return city;
        }
        public string GetYear()
        {
            return year;
        }
        public string GetBinary()
        {
            return binary;
        }
        public string GetFictionBook()
        {
            return fictionBook;
        }
    }
}
