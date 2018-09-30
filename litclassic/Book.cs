using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Timers;



namespace litclassic
{
    class Book
    {
        private string book;
        private int bookID;
        private string bookPath;
        private List<string> quotes = new List<string>();
        private Tag fictionBookTag = new Tag();
        private Line emptyLine = new Line();
        private List<Line> listLines = new List<Line>();
        private Description description = new Description();
        private int connection = 0;
        private List<int> listStrongConnections = new List<int>();
        private List<int> listRecommendConnections = new List<int>();
        private Comprehension comprehension = new Comprehension();
        private List<int> listExplanationStrongConnections = new List<int>();
        private List<int> listExplanationRecommendConnections = new List<int>();
        private Partical partical = new Partical();
        private List<Title> listTitles = new List<Title>();
        private List<string> listNewLiterals = new List<string>();
        private int lineIndex = 0;
        private string connectionString;
        private string typeWrite;
        private MainForm form;
        private System.Timers.Timer currentTimer;
        private int currentProgressValue;



        public void CreateBook()
        {
            // установка максимального значения прогресс бара
            ChangeProgressMaximumValue(14);

            currentTimer = new System.Timers.Timer(1000);
            //currentTimer.Elapsed += async (sender, e) => await HandleTimer();
            currentTimer.Elapsed += OnTimedEvent;

            currentTimer.Start();

            ChangePhaseName("Обработка книги");

            EncodeBook(bookPath);
            currentProgressValue = 1;
            //ChangeProgress(1, 14);
            ChangeProgressName("Декодирование книги");

            SymbolsReplacing();
            currentProgressValue++;
            //ChangeProgress(2, 14);
            ChangeProgressName("Замена символов");

            FindNewLiterals();
            currentProgressValue++;
            //ChangeProgress(3, 14);
            ChangeProgressName("Поиск неизвестных символов");

            // нужно еще что-то сделать с тегом ссылок <a>
            fictionBookTag.CreateTag(book);
            currentProgressValue++;
            //ChangeProgress(4, 14);
            ChangeProgressName("Создание дерева тегов");

            ClearSpacesInTags(fictionBookTag);
            currentProgressValue++;
            //ChangeProgress(5, 14);
            ChangeProgressName("Очистка тегов от лишних пробелов");

            CountOrderNumbers(fictionBookTag);
            currentProgressValue++;
            //ChangeProgress(6, 14);
            ChangeProgressName("Подсёт номеров тегов");

            FictionBookIdentification(fictionBookTag);
            currentProgressValue++;
            //ChangeProgress(7, 14);
            ChangeProgressName("Распознование роли тегов");

            //ClearSpacesInLines();

            FillListStrongConnections();
            currentProgressValue++;
            //ChangeProgress(8, 14);
            ChangeProgressName("Заполнение списка строгих связей");

            FillListRecommendConnections();
            currentProgressValue++;
            //ChangeProgress(9, 14);
            ChangeProgressName("Заполнение списка рекомендуемых связей");

            FillListExplanationStrongConnections();
            currentProgressValue++;
            //ChangeProgress(10, 14);
            ChangeProgressName("Заполнение списка объяснения строгих связей");

            FillListExplanationRecommendConnections();
            currentProgressValue++;
            //ChangeProgress(11, 14);
            ChangeProgressName("Заполнение списка объяснения рекомендуемых связей");

            FixListRecommendConnection();
            currentProgressValue++;
            //ChangeProgress(12, 14);
            ChangeProgressName("Исправление пересечений в списке рекомендуемых связей");

            PutConnectionsIntoLines();
            currentProgressValue++;
            //ChangeProgress(13, 14);
            ChangeProgressName("Запись связей в данные строк книги");

            ClearLines();

            partical.BuildParticals(listLines);
            currentProgressValue++;
            //ChangeProgress(14, 14);
            ChangeProgressName("Создание частиц");

            currentTimer.Stop();
        }
        public void AddBookToDB() // добавление информации о книге в таблицу Books 
        {
            // создание таймера с тиком в 1000
            // поменял на 10000 
            currentTimer = new System.Timers.Timer(10000);
            // добавление события таймера на каждый тик
            currentTimer.Elapsed += OnTimedEvent;

            // запуск таймера
            currentTimer.Start();

            //ChangePhaseName("Запись данных в базу");

            BookDBConnect currentDBConnect = new BookDBConnect();

            if (connectionString == "local")
            {
                currentDBConnect.SetSQLConnectionToLocalDBBooks();
            }
            else if (connectionString == "azure")
            {
                currentDBConnect.SetSQLConnectionToAzureDBLitClassicBooks();
            }

            if (typeWrite == "full write")
            {
                // установка максимального значения прогресс бара
                ChangeProgressMaximumValue(listLines.Count());

                currentProgressValue = 0;
                bookID = currentDBConnect.GetUniqIDBook();

                ChangeProgressName("Запись данных книги в базу");
                currentDBConnect.WriteNewBook(bookID, bookPath, description);
                ChangeProgressName("Запись строк книги в базу");

                for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
                {
                    currentDBConnect.WriteNewLine(bookID, listLines[iListLines]);
                    //ChangeProgress(iListLines, listLines.Count());
                    currentProgressValue++;
                }

                currentProgressValue = 0;

                ChangeProgressName("Запись частиц в базу");
                // установка максимального значения прогресс бара
                ChangeProgressMaximumValue(partical.GetListParticals().Count());

                for (int iListParticals = 0; iListParticals < partical.GetListParticals().Count(); iListParticals++)
                {
                    currentDBConnect.WriteNewPartical(bookID,
                        partical.GetListParticalsTitles()[iListParticals],
                        partical.GetListParticals()[iListParticals],
                        partical.GetListIndexLastParticalLines()[iListParticals],
                        partical.GetListSumStrongConnections()[iListParticals]);
                    //ChangeProgress(iListParticals, listLines.Count());
                    currentProgressValue++;
                }

                currentDBConnect.WriteNewStatistic(bookID, partical.GetLinesEntryPercent());
            }
            else if (typeWrite == "only particals")
            {
                // установка максимального значения прогресс бара
                ChangeProgressMaximumValue(partical.GetListParticals().Count());

                currentProgressValue = 0;
                int[] bookIDDescriptionID = new int[2];
                bookIDDescriptionID = currentDBConnect.GetBookIDDescriptionID(description.GetBookTitle(),
                    description.GetFirstName(), description.GetMiddleName(), description.GetLastName());
                bookID = bookIDDescriptionID[0];
                int descriptionID = bookIDDescriptionID[1];

                ChangeProgressName("Запись частиц в базу");

                for (int iListParticals = 0; iListParticals < partical.GetListParticals().Count(); iListParticals++)
                {
                    currentDBConnect.WriteNewPartical(bookID, descriptionID,
                        partical.GetListParticalsTitles()[iListParticals],
                        partical.GetListParticals()[iListParticals],
                        partical.GetListIndexLastParticalLines()[iListParticals],
                        partical.GetListSumStrongConnections()[iListParticals]);
                    //ChangeProgress(iListParticals, listLines.Count());
                    currentProgressValue++;
                }

                currentDBConnect.WriteNewStatistic(bookID, partical.GetLinesEntryPercent());
            }
            else if (typeWrite == "only lines")
            {
                // установка максимального значения прогресс бара
                ChangeProgressMaximumValue(listLines.Count());

                currentProgressValue = 0;

                ChangeProgressName("Запись данных книги в базу");
                currentDBConnect.WriteNewBook(bookID, bookPath, description);
                ChangeProgressName("Запись строк книги в базу");

                for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
                {
                    currentDBConnect.WriteNewLine(bookID, listLines[iListLines]);
                    //ChangeProgress(iListLines, listLines.Count());
                    currentProgressValue++;
                }

                currentDBConnect.WriteNewStatistic(bookID, partical.GetLinesEntryPercent());
            }

            currentTimer.Stop();
        }

        private void EncodeBook(string path) // определение верной кодировки и последущая запись книги в book 
        {
            Encoding win1251 = Encoding.GetEncoding(1251);
            Encoding utf8 = Encoding.GetEncoding(65001);
            string encoding = "";
            string firstLine = "";

            // выявление кодировки
            using (var reader = new StreamReader(path, System.Text.Encoding.Default))
            {
                firstLine = reader.ReadLine();
                string[] arrayFirstLine = firstLine.Split('\"');
                encoding = arrayFirstLine[3];
            }

            if (encoding.ToLower() == "utf-8")
            {
                using (var reader = new StreamReader(path, utf8))
                {
                    book = reader.ReadToEnd();

                    book = book.Replace(firstLine, "");
                    //book = book.Replace("\n", "");
                    //book = book.Replace("\r", "");
                    //book = book.Replace("\0", "");
                }
            }
            else if (encoding.ToLower() == "windows-1251")
            {
                using (var reader = new StreamReader(path, win1251))
                {
                    book = reader.ReadToEnd();

                    book = book.Replace(firstLine, "");
                    //book = book.Replace("\n", "");
                    //book = book.Replace("\r", "");
                    //book = book.Replace("\0", "");
                }
            }
        }
        private void CountOrderNumbers(Tag tag) // подсчет номера встречи тега 
        {
            List<string> listTagsName = new List<string>();
            List<int> listTagsNumberOfMeetings = new List<int>();

            foreach (Tag elementTag in tag.GetListTags())
            {
                // подсчет количестве встреч
                string tagName = elementTag.GetName().ToLower();

                // если имя тега встречено впервые
                if (listTagsName.IndexOf(tagName) == -1)
                {
                    listTagsName.Add(tagName);
                    listTagsNumberOfMeetings.Add(1);
                    elementTag.SetOrderNumber(listTagsNumberOfMeetings[listTagsName.IndexOf(tagName)]);
                }
                // если имя тега уже есть в listTagsName
                else
                {
                    listTagsNumberOfMeetings[listTagsName.IndexOf(tagName)]++;

                    elementTag.SetOrderNumber(listTagsNumberOfMeetings[listTagsName.IndexOf(tagName)]);
                }

                CountOrderNumbers(elementTag);
            }

            
        }
        private void SymbolsReplacing() 
        {
            book.Replace(" \n", "\n");

            book = book.Replace("<strong>", "$$strong-open$$");
            book = book.Replace("<emphasis>", "$$emphasis-open$$");
            book = book.Replace("</strong>", "$$strong-close$$");
            book = book.Replace("</emphasis>", "$$emphasis-close$$");
            book = book.Replace("<empty-line/>", "");
            // закомментированно, потому что иначе возникнут ошибки поиска тегов
            //book = book.Replace("&gt;", ">");
            //book = book.Replace("&lt;", "<");

            book = book.Replace("&#151;", "");
            book = book.Replace("&#180;", "´");
            book = book.Replace("&#186;", "º");
            book = book.Replace("&#188;", "¼");
            book = book.Replace("&#189;", "½");
            book = book.Replace("&#190;", "¾");
            book = book.Replace("&#192;", "À");
            book = book.Replace("&#196;", "Ä");
            book = book.Replace("&#198;", "Æ");

            // латинские символы
            book = book.Replace("&#199;", "Ç");
            book = book.Replace("&#200;", "È");
            book = book.Replace("&#201;", "É");
            book = book.Replace("&#202;", "Ê");
            book = book.Replace("&#203;", "Ë");
            book = book.Replace("&#207;", "Ï");
            book = book.Replace("&#214;", "Ö");
            book = book.Replace("&#215;", "×");
            book = book.Replace("&#220;", "Ü");
            book = book.Replace("&#221;", "Ý");
            book = book.Replace("&#222;", "Þ");
            book = book.Replace("&#223;", "ß");
            book = book.Replace("&#224;", "à");
            book = book.Replace("&#225;", "á");
            book = book.Replace("&#226;", "â");
            book = book.Replace("&#227;", "ã");
            book = book.Replace("&#228;", "ä");           
            book = book.Replace("&#229;", "å");
            book = book.Replace("&#230;", "æ");
            book = book.Replace("&#231;", "ç");
            book = book.Replace("&#232;", "è");
            book = book.Replace("&#233;", "é");
            book = book.Replace("&#234;", "ê");
            book = book.Replace("&#235;", "ë");
            book = book.Replace("&#236;", "ì");
            book = book.Replace("&#237;", "í");
            book = book.Replace("&#238;", "î");
            book = book.Replace("&#239;", "ï");
            book = book.Replace("&#240;", "ð");
            book = book.Replace("&#241;", "ñ");
            book = book.Replace("&#242;", "ò");
            book = book.Replace("&#243;", "ó");
            book = book.Replace("&#244;", "ô");
            book = book.Replace("&#245;", "õ");
            book = book.Replace("&#246;", "ö");
            book = book.Replace("&#249;", "ù");
            book = book.Replace("&#250;", "ú");
            book = book.Replace("&#251;", "û");
            book = book.Replace("&#252;", "ü");
            book = book.Replace("&#253;", "ý");
            book = book.Replace("&#261;", "ą");
            book = book.Replace("&#268;", "Č");
            book = book.Replace("&#269;", "č");
            book = book.Replace("&#281;", "ę");
            book = book.Replace("&#305;", "ı");
            book = book.Replace("&#322;", "ł");
            book = book.Replace("&#328;", "ň");
            book = book.Replace("&#338;", "Œ");
            book = book.Replace("&#339;", "œ");
            book = book.Replace("&#352;", "Š");
            book = book.Replace("&#362;", "Ū");
            book = book.Replace("&#365;", "ŭ");

            book = book.Replace("&#601;", "ə");
            book = book.Replace("&#679;", "ʧ");

            book = book.Replace("&#768;", "̀");
            book = book.Replace("&#769;", "́");
            book = book.Replace("&#771;", "̃");
            book = book.Replace("&#773;", "̅");
            book = book.Replace("&#774;", "̆");

            // греческие символы
            book = book.Replace("&#900;", "΄");
            book = book.Replace("&#901;", "΅");
            book = book.Replace("&#902;", "Ά");
            book = book.Replace("&#903;", "·");
            book = book.Replace("&#910;", "Ύ");
            book = book.Replace("&#911;", "Ώ");
            book = book.Replace("&#912;", "ΐ");
            book = book.Replace("&#913;", "Α");
            book = book.Replace("&#914;", "Β");
            book = book.Replace("&#915;", "Γ");
            book = book.Replace("&#916;", "Δ");
            book = book.Replace("&#917;", "Ε");
            book = book.Replace("&#918;", "Ζ");
            book = book.Replace("&#919;", "Η");
            book = book.Replace("&#920;", "Θ");
            book = book.Replace("&#921;", "Ι");
            book = book.Replace("&#922;", "Κ");
            book = book.Replace("&#923;", "Λ");
            book = book.Replace("&#924;", "Μ");
            book = book.Replace("&#925;", "Ν");
            book = book.Replace("&#926;", "Ξ");
            book = book.Replace("&#927;", "Ο");
            book = book.Replace("&#928;", "Π");
            book = book.Replace("&#929;", "Ρ");
            book = book.Replace("&#930;", "΢");
            book = book.Replace("&#931;", "Σ");
            book = book.Replace("&#932;", "Τ");
            book = book.Replace("&#933;", "Υ");
            book = book.Replace("&#934;", "Φ");
            book = book.Replace("&#935;", "Χ");
            book = book.Replace("&#936;", "Ψ");
            book = book.Replace("&#937;", "Ω");
            book = book.Replace("&#938;", "Ϊ");
            book = book.Replace("&#939;", "Ϋ");
            book = book.Replace("&#940;", "ά");
            book = book.Replace("&#941;", "έ");
            book = book.Replace("&#942;", "ή");
            book = book.Replace("&#943;", "ί");
            book = book.Replace("&#945;", "α");
            book = book.Replace("&#946;", "β");
            book = book.Replace("&#947;", "γ");
            book = book.Replace("&#948;", "δ");
            book = book.Replace("&#949;", "ε");
            book = book.Replace("&#950;", "ζ");
            book = book.Replace("&#951;", "η");
            book = book.Replace("&#952;", "θ");
            book = book.Replace("&#953;", "ι");
            book = book.Replace("&#954;", "κ");
            book = book.Replace("&#955;", "λ");
            book = book.Replace("&#956;", "μ");
            book = book.Replace("&#957;", "ν");
            book = book.Replace("&#958;", "ξ");
            book = book.Replace("&#959;", "ο");
            book = book.Replace("&#960;", "π");
            book = book.Replace("&#961;", "ρ");
            book = book.Replace("&#962;", "ς");
            book = book.Replace("&#963;", "σ");
            book = book.Replace("&#964;", "τ");
            book = book.Replace("&#965;", "υ");
            book = book.Replace("&#966;", "φ");
            book = book.Replace("&#967;", "χ");
            book = book.Replace("&#968;", "ψ");
            book = book.Replace("&#969;", "ω");
            book = book.Replace("&#970;", "ϊ");
            book = book.Replace("&#971;", "ϋ");
            book = book.Replace("&#972;", "ό");
            book = book.Replace("&#973;", "ύ");
            book = book.Replace("&#974;", "ώ");
            book = book.Replace("&#975;", "Ϗ");
            book = book.Replace("&#976;", "ϐ");
            book = book.Replace("&#977;", "ϑ");
            book = book.Replace("&#978;", "ϒ");
            book = book.Replace("&#979;", "ϓ");
            book = book.Replace("&#980;", "ϔ");

            // кириллические символы
            book = book.Replace("&#1122;", "Ѣ");
            book = book.Replace("&#1123;", "ѣ"); 
            book = book.Replace("&#1138;", "Ѳ");
            book = book.Replace("&#1139;", "ѳ");
            book = book.Replace("&#1140;", "Ѵ");
            book = book.Replace("&#1141;", "ѵ");

            book = book.Replace("&#8001;", "ὁ");
            book = book.Replace("&#8050;", "ὲ");
            book = book.Replace("&#8195;", " ");
            book = book.Replace("&#8196;", " ");
            book = book.Replace("&#9674;", "◊");


            book = book.Replace("\n", "");
            book = book.Replace("\r", "");
        }
        private void FindNewLiterals()
        {
            int startIndex = 0;
            List<string> unsortList = new List<string>();

            while (startIndex != -1) 
            {
                int newStartIndex = book.IndexOf("&#", startIndex);

                if (newStartIndex != -1)
                {
                    unsortList.Add(book.Substring(newStartIndex, book.IndexOf(";", newStartIndex) - newStartIndex));

                    startIndex = book.IndexOf("&#", newStartIndex + 1);
                }
                else startIndex = -1;
            }

            foreach (string literal in unsortList)
            {
                if (listNewLiterals.IndexOf(literal) == -1) listNewLiterals.Add(literal);
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ChangeProgressCurrentValue(currentProgressValue);
        }

        private void ChangeProgress(int currentProgress, int maximumProgress)
        {
            form.SetProgressValues(currentProgress, maximumProgress);
            form.Invoke(form.changeProgressValueDelegate);
        }
        private void ChangeProgressCurrentValue(int newProgressCurrentValue)
        {
            form.SetProgressCurrentValue(newProgressCurrentValue);
            form.Invoke(form.changeProgressCurrentValueDelegate);
        }
        private void ChangeProgressMaximumValue(int newProgressMaximumValue)
        {
            form.SetProgressMaximumValue(newProgressMaximumValue);
            form.Invoke(form.changeProgressMaximumValueDelegate);
        }
        private void ChangeProgressName(string newProccessName)
        {
            form.SetProccessName(newProccessName);
            form.Invoke(form.changeProccessNameDelegate);
        }
        private void ChangePhaseName(string newPhaseName)
        {
            form.SetPhaseName(newPhaseName);
            form.Invoke(form.changePhaseNameDelegate);
        }



        // --справка--
        // Add - метод, предполагающий добавление внутри него line в listLines
        // Return - метод, предполагающий лишь сбор целого line из фрагментов, которые в ином случае добавляют line в listLines
        // SetLines используется когда нужно дополнить уже существущую строку
        // присваивание текущему line метода, возвращающего line, используется, когда нужно дополнить существующую строку объединением других line
        // просто вызов метода, не возвращающего line предполагает последующую запись line в listLines
        // line - принимаемый параметр
        // tempLine - копия line, содержащая в себе также данные текущего (возможного) subtitle
        // sendingLine - копия line или tempLine, отправляемая в следующий метод


        private void FictionBookIdentification(Tag tag)
        {
            // задание темы по-умолчанию
            emptyLine.SetThemeType(0);

            foreach (Tag elementFictionBookTag in tag.GetListTags())
            {
                if (elementFictionBookTag.GetName() == "description")
                {
                    DescriptionIdentification(elementFictionBookTag);
                }
                else if (elementFictionBookTag.GetName() == "body")
                {
                    if ((elementFictionBookTag.GetListCommandTypes() == null) || (elementFictionBookTag.GetOrderNumber() == 1)) 
                    {
                        // добавление нового элемента связь в список
                        //emptyLine.AddNewConnection();
                        BodyIdentification(elementFictionBookTag, emptyLine);
                    }
                    else if (elementFictionBookTag.GetListCommandTypes()[0] == "comments")
                    {

                    }
                    else if (elementFictionBookTag.GetListCommandTypes()[0] == "notes")
                    {

                    }
                }
            }
        }
        private void DescriptionIdentification(Tag tag)
        {
            foreach (Tag elementTag in tag.GetListTags())
            {
                if (elementTag.GetName().ToLower() == "title-info")
                {
                    // цикл для обхода всех входящих тегов в тег title-info
                    foreach (Tag elementTitleInfoTag in elementTag.GetListTags())
                    {
                        if (elementTitleInfoTag.GetName().ToLower() == "genre")
                        {
                            description.SetGenre(elementTitleInfoTag.GetContent());
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "author")
                        {
                            // цикл для обхода всех входящих тегов в тег author
                            foreach (Tag elementAuthorTag in elementTitleInfoTag.GetListTags())
                            {
                                if (elementAuthorTag.GetName().ToLower() == "first-name")
                                {
                                    description.SetFirstName(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "middle-name")
                                {
                                    description.SetMiddleName(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "last-name")
                                {
                                    description.SetLastName(elementAuthorTag.GetContent());
                                }
                            }
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "translator")
                        {
                            // цикл для обхода всех входящих тегов в тег translator
                            foreach (Tag elementTranslatorTag in elementTitleInfoTag.GetListTags())
                            {
                                if (elementTranslatorTag.GetName().ToLower() == "first-name")
                                {
                                    description.SetTranslatorFirstName(elementTranslatorTag.GetContent());
                                }
                                else if (elementTranslatorTag.GetName().ToLower() == "middle-name")
                                {
                                    description.SetTranslatorMiddleName(elementTranslatorTag.GetContent());
                                }
                                else if (elementTranslatorTag.GetName().ToLower() == "last-name")
                                {
                                    description.SetTranslatorLastName(elementTranslatorTag.GetContent());
                                }
                                else if (elementTranslatorTag.GetName().ToLower() == "nickname")
                                {
                                    description.SetTranslatorNickname(elementTranslatorTag.GetContent());
                                }
                                else if (elementTranslatorTag.GetName().ToLower() == "homepage")
                                {
                                    description.SetTranslatorHomePage(elementTranslatorTag.GetContent());
                                }
                                else if (elementTranslatorTag.GetName().ToLower() == "email")
                                {
                                    description.SetTranslatorEmail(elementTranslatorTag.GetContent());
                                }
                            }
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "book-title")
                        {
                            description.SetBookTitle(elementTitleInfoTag.GetContent());
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "annotation")
                        {
                            string annotation = "";

                            foreach (Tag elementAnnotationTag in elementTitleInfoTag.GetListTags())
                            {
                                if (elementAnnotationTag.GetName().ToLower() == "p")
                                {
                                    annotation += elementAnnotationTag.GetContent().Replace("\n\r"," ") + "\n\r";
                                }
                            }

                            description.SetAnnotation(annotation);
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "date")
                        {
                            if (elementTitleInfoTag.GetListCommands().Count() > 0)
                            {
                                description.SetDate(elementTitleInfoTag.GetListCommands()[0]);
                            }
                            else description.SetDate(elementTitleInfoTag.GetContent());
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "coverpage")
                        {
                            description.SetCoverpage(elementTitleInfoTag.GetContent());
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "lang")
                        {
                            description.SetLang(elementTitleInfoTag.GetContent());
                        }
                        else if (elementTitleInfoTag.GetName().ToLower() == "sequence")
                        {
                            string sequence = "";
                            sequence += elementTitleInfoTag.GetListCommands()[0];
                            sequence += ", №" + elementTitleInfoTag.GetListCommands()[1];

                            description.SetSequence(sequence);
                        }
                    }
                }
                else if (elementTag.GetName().ToLower() == "document-info")
                {
                    // цикл для обхода всех входящих тегов в тег document-info
                    foreach (Tag elementDocumentInfo in elementTag.GetListTags())
                    {
                        if (elementDocumentInfo.GetName().ToLower() == "author")
                        {
                            // цикл для обхода всех входящих тегов в тег author
                            foreach (Tag elementAuthorTag in elementDocumentInfo.GetListTags())
                            {
                                if (elementAuthorTag.GetName().ToLower() == "first-name")
                                {
                                    description.SetDocAuthorFirstName(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "middle-name")
                                {
                                    description.SetDocAuthorMiddleName(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "last-name")
                                {
                                    description.SetDocAuthorLastName(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "nickname")
                                {
                                    description.SetDocAuthorNickname(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "program-used")
                                {
                                    description.SetProgrammUsed(elementAuthorTag.GetContent());
                                }
                                else if (elementAuthorTag.GetName().ToLower() == "homepage")
                                {
                                    description.SetDocAuthorHomePage(elementAuthorTag.GetContent());
                                }
                            }
                        }                      
                        else if (elementDocumentInfo.GetName().ToLower() == "date")
                        {
                            if (elementDocumentInfo.GetListCommands().Count() > 0)
                            {
                                description.SetDocDate(elementDocumentInfo.GetListCommands()[0]);
                            }
                            else description.SetDocDate(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "src-url")
                        {
                            description.SetSrcURL(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "src-orl")
                        {
                            description.SetSrcOCR(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "id")
                        {
                            description.SetDocID(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "version")
                        {
                            description.SetVersion(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "program-used")
                        {
                            description.SetProgrammUsed(elementDocumentInfo.GetContent());
                        }
                        else if (elementDocumentInfo.GetName().ToLower() == "history")
                        {
                            description.SetHistory(elementDocumentInfo.GetContent());
                        }
                    }
                }
                else if (elementTag.GetName().ToLower() == "publish-info")
                {
                    // цикл для обхода всех входящих тегов в тег publish-info
                    foreach (Tag elementPublishInfoTag in elementTag.GetListTags())
                    {
                        if (elementPublishInfoTag.GetName().ToLower() == "book-name")
                        {
                            description.SetBookName(elementPublishInfoTag.GetContent());
                        }
                        else if (elementPublishInfoTag.GetName().ToLower() == "publisher")
                        {
                            description.SetPublisher(elementPublishInfoTag.GetContent());
                        }
                        else if (elementPublishInfoTag.GetName().ToLower() == "city")
                        {
                            description.SetCity(elementPublishInfoTag.GetContent());
                        }
                        else if (elementPublishInfoTag.GetName().ToLower() == "year")
                        {
                            description.SetYear(elementPublishInfoTag.GetContent());
                        }
                        else if (elementPublishInfoTag.GetName().ToLower() == "binary")
                        {
                            description.SetBinary(elementPublishInfoTag.GetContent());
                        }
                        else if (elementPublishInfoTag.GetName().ToLower() == "fictionbook")
                        {
                            description.SetFictionBook(elementPublishInfoTag.GetContent());
                        }
                    } 
                }
            }
        }
        private void BodyIdentification(Tag tag, Line line)
        {
            // чтение title
            if (tag.GetListTags()[0].GetName().ToLower() == "title")
            {
                // для пропуска определения типа темы у тега body
                line.SetThemeType(-1);

                line = TitleIdentificationAndReturn(tag.GetListTags()[0], line);
            }

            Line tempLine = line;

            connection++;
            //tempLine.UpConnection(0, connection);
            tempLine.SetStrongConnection(connection);

            foreach (Tag elementBodyTag in tag.GetListTags())
            {
                if (elementBodyTag.GetName().ToLower() == "section")
                {
                    SectionIdentification(elementBodyTag, tempLine);
                }
                else if (elementBodyTag.GetName().ToLower() == "epigraph")
                {
                    EpigraphIdentificationAndAdd(elementBodyTag, tempLine);
                }
                else if (elementBodyTag.GetName().ToLower() == "subtitle")
                {
                    // возвращение tempLine в исходное состояние line
                    tempLine = line;

                    tempLine = SubtitleTitleIdentification(elementBodyTag, tempLine);
                }
            }

            //return line;
        }
        private void SectionIdentification(Tag tag, Line line)
        {
            if (tag.GetListTags().Count != 0)
            {
                // чтение title
                if (tag.GetListTags()[0].GetName().ToLower() == "title")
                {
                    line = TitleIdentificationAndReturn(tag.GetListTags()[0], line);
                }
                else line.AddNewPartOfTitleOld(tag.GetOrderNumber() + " раздел");

                Line tempLine = new Line();
                tempLine = line;

                connection++;
                //tempLine.UpConnection(0, connection);
                tempLine.SetStrongConnection(connection);

                foreach (Tag elementSectionTag in tag.GetListTags())
                {
                    if (elementSectionTag.GetName().ToLower() == "section")
                    {
                        SectionIdentification(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "epigraph")
                    {
                        EpigraphIdentificationAndAdd(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "annotation")
                    {
                        AnnotationIdentificationAndAdd(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "cite")
                    {
                        CiteIdentificationAndAdd(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "p")
                    {
                        PIdentification(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "poem")
                    {
                        PoemIdentificationAndAdd(elementSectionTag, tempLine);
                    }
                    else if (elementSectionTag.GetName().ToLower() == "subtitle")
                    {
                        // возвращение tempLine в исходное состояние line
                        tempLine = line;

                        tempLine = SubtitleTitleIdentification(elementSectionTag, tempLine);
                    }
                    // пробная функция
                    else if (elementSectionTag.GetName().ToLower() == "empty-line")
                    {
                        connection++;

                        tempLine.SetStrongConnection(connection);
                    }
                }
            }          
            //return line;
        }
        private Line TitleIdentificationAndReturn(Tag tag, Line line)
        {
            string titleString = "";
            //Title newTitle = new Title();

            foreach (Tag elementTitleTag in tag.GetListTags())
            {
                // нужны ли еще теги?
                // если тег не последний в списке тегов
                if ((elementTitleTag.GetName().ToLower() == "p") & (elementTitleTag != tag.GetListTags().Last()))
                {
                    if (elementTitleTag.GetContent().Count() != 0)
                    {
                        // в конце строки может быть запятая
                        if (elementTitleTag.GetContent().Last() != ',')
                        {
                            titleString += "\"" + elementTitleTag.GetContent() + "\", ";
                        }
                        else titleString += "\"" + elementTitleTag.GetContent() + "\" ";
                    }
                }
                else if (elementTitleTag.GetName().ToLower() == "p")
                {
                    if (elementTitleTag.GetContent().Count() != 0)
                    {
                        titleString += "\"" + elementTitleTag.GetContent() + "\"";
                    }
                }

                // когда-то это зачем-то было нужным, но сейчас, кажется, нет
                // очистка пробелов 
                //while (titleString.IndexOf(" \"") != -1) 
                //{
                //    titleString = titleString.Replace(" \"", "\"");
                //}
            }
            // шансы "выпадения" тем: 20-6-2-1
            // типы тем:
            // "0" - основные произведения
            // "1" - прочие произведения, записки, письма
            // "2" - комментарии, разделы от редакции, биограф.очерки, примечания, приложения
            // "3" - указатели и пр.
            // Распознание и добавление "типа темы"
            if ((titleString != "\"\"") & (titleString != ""))
            {
                if (line.GetThemeType() != -1)
                {
                    if 
                        ( /*Аксаков, 1 т.*/ (titleString.ToLower() == "\"отрывочные воспоминания\"")
                        || /*Аксаков, 1 т.*/ (titleString.ToLower() == "\"последовательные воспоминания\"")
                        || /*Аксаков, 5 т.*/ (titleString.ToLower() == "\"рассказы и воспоминания охотника о разных охотах\"")
                        || /*Аксаков, 5 т.*/ (titleString.ToLower() == "\"новые охотничьи заметки\"")

                        || /*Достоевский, 1 т.*/ (titleString.ToLower() == "\"роман в девяти письмах\"")
                        || /*Достоевский, 4 т.*/ (titleString.ToLower() == "\"зимние заметки о летних впечатлениях\"")
                        || /*Достоевский, 12 т.*/ (titleString.ToLower().IndexOf("дневник писателя") != -1)
                        || /*Достоевский, 12 т.*/ (titleString.ToLower() == "\"полписьма «одного лица»\"")
                        || /*Достоевский, 13 т.*/ (titleString.ToLower().IndexOf("из частного письма") != -1)
                        || /*Достоевский, 14 т.*/ (titleString.ToLower().IndexOf(" русская сатира.") != -1)
                        || /*Достоевский, 14 т.*/ (titleString.ToLower().IndexOf("об анонимных ругательных письмах") != -1))
                    {
                        // сделано для того, чтобы отделить сами произведения с таким названием 
                        // от разделов в примечании с этим же названием
                        if ((line.GetThemeType() == 1) || (line.GetThemeType() == 0))
                            line.SetThemeType(0);
                    }
                    else if  // приложения
                        ((titleString.ToLower().IndexOf("приложение") != -1)
                        || (titleString.ToLower().IndexOf("приложения") != -1)

                        || /*Гоголь, 14 т.*/ (titleString.ToLower() == "\"дополнения\"")

                        // обособливое
                        || /*Гоголь, 14 т.*/ (titleString.ToLower() == "\"примечания к " +
                        "повести «ночь перед рождеством» (отрывок из чернового автографа)\"")

                        // редакции
                        || (titleString.ToLower().IndexOf("другие редакции") != -1)
                        || (titleString.ToLower() == "\"редакционные предисловия, извещения и пр\"")
                        || (titleString.ToLower().IndexOf("из ранних редакций") != -1)

                        // недописанное
                        || (titleString.ToLower().IndexOf("наброски") != -1)
                        || (titleString.ToLower().IndexOf("планы ненаписанных произведений") != -1)
                        || (titleString.ToLower().IndexOf("незавершенные произведения") != -1)
                        || (titleString.ToLower() == "\"мертвые души. том второй\"")
                        || (titleString.ToLower().IndexOf("отрывки") != -1)
                        || /*Достоевский, 10 т.*/ (titleString.ToLower() == "\"братья карамазовы. том 2\"")

                        // заметки
                        || (titleString.ToLower().IndexOf("заметки") != -1)
                        || (titleString.ToLower() == "\"записные книжки\"")
                        || /*Гоголь, 9 т.*/ (titleString.ToLower() == "\"мелочи. биографическое. записные книжки\"")
                        || /*из Пушкина, 10 т.*/ (titleString.ToLower().IndexOf("из черновиков") != -1)

                        // биография
                        || (titleString.ToLower().IndexOf("дневник") != -1)
                        || (titleString.ToLower().IndexOf("письма") != -1)
                        || (titleString.ToLower().IndexOf("воспоминания") != -1)
                        || (titleString.ToLower().IndexOf("история моего знакомства") != -1)
                        || /*из Пушкина, 6 т.*/ (titleString.IndexOf("ПУТЕШЕСТВИЯ") != -1)
                        || /*из Пушкина, 8 т.*/ (titleString.ToLower().IndexOf("автобиографическая проза") != -1)

                        // публицистика
                        || (titleString.ToLower().IndexOf("рецензии") != -1)
                        || (titleString.ToLower().IndexOf("статьи") != -1)
                        || (titleString.ToLower().IndexOf("о современнике") != -1)
                        || (titleString.ToLower().IndexOf("деловые бумаги") != -1)
                        || (titleString.ToLower().IndexOf("материалы по географии") != -1)
                        || (titleString.ToLower().IndexOf("материалы для словаря") != -1)
                        || /*Достоевский, 11 т.*/ (titleString.ToLower() == "\"1860\"")
                        || /*Достоевский, 11 т.*/ (titleString.ToLower() == "\"1861\"")
                        || /*Достоевский, 11 т.*/ (titleString.ToLower() == "\"1862\"")
                        || /*Достоевский, 11 т.*/ (titleString.ToLower() == "\"1863\"")
                        || /*Достоевский, 11 т.*/ (titleString.ToLower() == "\"1864\"")

                        // приписываемое
                        || (titleString.ToLower().IndexOf("приписываемые") != -1)
                        || (titleString.ToLower().IndexOf("приписываемое") != -1)

                        // несерьёзное
                        || (titleString.ToLower() == "\"шуточные стихи, пародии, эпиграммы\"")
                        || (titleString.ToLower().IndexOf("шуточные стихи") != -1)
                        || /*Батюшков*/ (titleString.ToLower().IndexOf("мелкие сатирические") != -1)
                  
                        // коллективное
                        || (titleString.ToLower().IndexOf("коллективные") != -1)
                        || (titleString.ToLower().IndexOf("коллективное") != -1)

                        // переводы
                        || (titleString.ToLower() == "\"переводы\"")
                        || /*Тютчев, 1 т.*/ (titleString.ToLower().IndexOf("переводы стихотворений") != -1)
                        || /*Тютчев, 3 т.*/ (titleString.ToLower().IndexOf("переводы публицистических") != -1)

                        // историческое
                        || /*из Пушкина, 8 т.*/ (titleString.ToLower().IndexOf("историческая проза") != -1)
                        || /*из Пушкина, 8 т.*/ (titleString.ToLower().IndexOf("замечания о бунте") != -1)
                        || /*из Пушкина, 9 т.*/ (titleString.ToLower().IndexOf("история петра") != -1))                                                
                    {
                        // сделано для того, чтобы отделить материал с таким названием 
                        // от разделов в примечании с этим же названием
                        if (line.GetThemeType() != 3) line.SetThemeType(1);
                    }
                    else if // осн.разделы
                        ((titleString.ToLower() == "\"комментарии\"")                       
                        || (titleString.ToLower() == "\"дополнение\"")
                        || (titleString.ToLower().IndexOf("от редакции") != -1)

                        || /*Гоголь, 1 т.*/ (titleString.ToLower() == "\"опечатки\"")

                        // описания составителей
                        || /*Гоголь, 10 т.*/ (titleString.ToLower().IndexOf("даты жизни гоголя") != -1)
                        || /*Одоевский, записки дл.м.правнука*/ (titleString.ToLower().IndexOf("одоевский в критике") != -1)

                        // фамилии редакции
                        || (titleString.ToLower().IndexOf("и.м. семенко") != -1)
                        || (titleString.ToLower().IndexOf("в. мануйлов") != -1)
                        || (titleString.ToLower().IndexOf("а. морозов") != -1)
                        || /*Толстой, 1 т.*/ (titleString.ToLower().IndexOf("и.г. ямпольский") != -1)
                        || /*Тютчев, ед.т.*/ (titleString.ToLower().IndexOf("берковский") != -1)
                        || (titleString.ToLower().IndexOf("машинский") != -1)
                        || (titleString.ToLower().IndexOf("фридман") != -1)
                        || (titleString.ToLower().IndexOf("вас. гиппиус") != -1)
                        || (titleString.ToLower().IndexOf("с. петров") != -1)
                        || (titleString.ToLower().IndexOf("г. фридлендер") != -1)
                        || /*Одоевский, записки дл.м.правнука*/ (titleString.ToLower().IndexOf("всеволод сахаров") != -1)

                        // словари
                        || (titleString.ToLower() == "\"словарь трудных для понимания слов\"")
                        || (titleString.ToLower().IndexOf("словарь мифологических имен") != -1))                 
                    {
                        line.SetThemeType(2);
                    }
                    else if ((titleString.ToLower() == "\"выходные данные\"")

                        // примечания
                        || (titleString.ToLower().IndexOf("примечания") != -1)
                        || (titleString.ToLower() == "\"примечание\"")

                        // иллюстрации
                        || (titleString.ToLower() == "\"иллюстрации\"")
                        || (titleString.ToLower().IndexOf("перечень иллюстраций") != -1)

                        // указатели
                        || (titleString.ToLower().IndexOf("указатель") != -1)
                        || (titleString.ToLower().IndexOf("указатель имён") != -1)
                        || (titleString.ToLower().IndexOf("указатель имен") != -1)
                        || (titleString.ToLower() == "\"именной указатель\"")
                        || (titleString.ToLower() == "\"указатель писем по адресатам\"")
                        || /*Пушкин, 9 т.*/ (titleString.ToLower().IndexOf("алфавитный указатель") != -1)

                        || (titleString.ToLower().IndexOf("архивохранилищ") != -1)                  

                        // списки
                        || (titleString.ToLower().IndexOf("список") != -1)
                        || (titleString.ToLower().IndexOf("алфавитный список") != -1)

                        // сокращения
                        || (titleString.ToLower().IndexOf("сокращений") != -1)
                        || (titleString.ToLower() == "\"сокращенные обозначения источников\"")
                        || (titleString.ToLower() == "\"сокращённые обозначения источников\"")
                        || /*Тютчев, 1 т.*/ (titleString.ToLower() == "условные сокращения")

                        || (titleString.ToLower().IndexOf("переводы") != -1)                     
                        
                        || (titleString.ToLower().IndexOf("содержание") != -1)

                        // авторские заметки, но не читаемые
                        || /*Пушкин, 7 т.*/ (titleString.ToLower().IndexOf("заметки на полях") != -1)                       
                        || /*Пушкин, 10 т.*/ (titleString.ToLower().IndexOf("опечатки") != -1)
                        || /*Тютчев, ед.т.*/ (titleString.ToLower().IndexOf("автограф нац. музея в праге") != -1)                       
                        || /*Тютчев, 2 т. (моё)*/ (titleString.ToLower() == "\"другие редакции (сокр.)\"")
                        || /*Тютчев, 3 т.*/ (titleString.ToLower() == "\"публицистические произведения, написанные на французском языке\""))
                    {
                        line.SetThemeType(3);
                    }
                }
                else line.SetThemeType(1);
                

                line.AddNewPartOfTitleOld(titleString);
            }

            return line;
        }
        private Line SubtitleTitleIdentification(Tag tag, Line line)
        {
            line.AddNewPartOfTitleOld("\"" + tag.GetContent() + "\"");
            connection++;
            //line.UpConnection(0, connection);
            line.SetStrongConnection(connection);

            return line;
        }
        private void EpigraphIdentificationAndAdd(Tag tag, Line line)
        {
            line.AddNewPartOfTitleOld("эпиграф");

            foreach (Tag elementEpigraphTag in tag.GetListTags())
            {
                if (elementEpigraphTag.GetName().ToLower() == "p")
                {
                    line.AddNewPartOfLine(elementEpigraphTag.GetContent());
                }
                else if (elementEpigraphTag.GetName().ToLower() == "poem")
                {
                    line = PoemIdentificationAndReturn(elementEpigraphTag, line);
                }
                else if (elementEpigraphTag.GetName().ToLower() == "cite")
                {
                    line = CiteIdentificationAndReturn(elementEpigraphTag, line);
                }
                else if (elementEpigraphTag.GetName().ToLower() == "emty-line")
                {
                    line.AddNewPartOfLine("/r/n");
                }
                else if (elementEpigraphTag.GetName().ToLower() == "text-author")
                {
                    // т.к. title формируется как список, добавление автора будет новым элементом в нем
                    //line.SetTitle(" (автор: " + elementEpigraphTag.GetContent() + ")");
                    line.AddNewPartOfLine(elementEpigraphTag.GetContent());
                }
            }

            line.SetIndex(lineIndex);
            listLines.Add(line);

            lineIndex++;
        }
        private Line EpigraphIdentificationAndReturn(Tag tag, Line line)
        {
            line.AddNewPartOfTitleOld("эпиграф");

            foreach (Tag elementEpigraphTag in tag.GetListTags())
            {
                if (elementEpigraphTag.GetName().ToLower() == "p")
                {
                    line.AddNewPartOfLine(elementEpigraphTag.GetContent());
                }
                else if (elementEpigraphTag.GetName().ToLower() == "poem")
                {
                    line = PoemIdentificationAndReturn(elementEpigraphTag, line);
                }
                else if (elementEpigraphTag.GetName().ToLower() == "cite")
                {
                    line = CiteIdentificationAndReturn(elementEpigraphTag, line);
                }
                else if (elementEpigraphTag.GetName().ToLower() == "emty-line")
                {
                    line.AddNewPartOfLine("/r/n");
                }
                else if (elementEpigraphTag.GetName().ToLower() == "text-author")
                {
                    line.AddNewPartOfLine(elementEpigraphTag.GetContent());
                }
            }

            listLines.Add(line);

            return line;
        }
        private void AnnotationIdentificationAndAdd(Tag tag, Line line)
        {
            line.AddNewPartOfTitleOld("аннотация");

            foreach (Tag elementAnnotationTag in tag.GetListTags())
            {
                if (elementAnnotationTag.GetName().ToLower() == "p")
                {
                    line.AddNewPartOfLine(elementAnnotationTag.GetContent());
                }
            }

            line.SetIndex(lineIndex);
            listLines.Add(line);

            lineIndex++;
        }
        private void CiteIdentificationAndAdd(Tag tag, Line line)
        {
            // нужен ли номер цитаты?
            line.AddNewPartOfTitleOld(tag.GetOrderNumber() + " цитата");

            //Line tempLine = line;

            // здесь необходимо все содержимое line вернуть обратно
            foreach (Tag elementCiteTag in tag.GetListTags())
            {
                if (elementCiteTag.GetName().ToLower() == "p")
                {
                    line.AddNewPartOfLine(elementCiteTag.GetContent());
                }
                else if (elementCiteTag.GetName().ToLower() == "poem")
                {
                    line = PoemIdentificationAndReturn(elementCiteTag, line);
                }
                else if (elementCiteTag.GetName().ToLower() == "empty-line")
                {
                    line.AddNewPartOfLine("/r/n");
                }
                else if (elementCiteTag.GetName().ToLower() == "text-author")
                {
                    line.AddNewPartOfLine(elementCiteTag.GetContent());
                }
                // нужно ли это?
                //else if (elementCiteTag.GetName().ToLower() == "subtitle")
                //{
                //    // возвращение tempLine в исходное состояние line
                //    tempLine = line;

                //    tempLine = SubtitleTitleIdentification(elementCiteTag, tempLine);
                //}
            }

            line.SetIndex(lineIndex);
            listLines.Add(line);

            lineIndex++;
        }
        private Line CiteIdentificationAndReturn(Tag tag, Line line)
        {
            foreach (Tag elementCiteTag in tag.GetListTags())
            {
                if (elementCiteTag.GetName().ToLower() == "p")
                {
                    line.AddNewPartOfLine(elementCiteTag.GetContent());
                }
                else if (elementCiteTag.GetName().ToLower() == "poem")
                {
                    line = PoemIdentificationAndReturn(elementCiteTag, line);
                }
                else if (elementCiteTag.GetName().ToLower() == "empty-line")
                {
                    line.AddNewPartOfLine("/r/n");
                }
                else if (elementCiteTag.GetName().ToLower() == "text-author")
                {
                    line.AddNewPartOfLine(elementCiteTag.GetContent());
                }
            }

            return line;
        }
        private void PoemIdentificationAndAdd(Tag tag, Line line)
        {           
            // чтение title
            if (tag.GetListTags()[0].GetName().ToLower() == "title")
            {
                line = TitleIdentificationAndReturn(tag.GetListTags()[0], line);
            }
            else line.AddNewPartOfTitleOld(tag.GetOrderNumber() + " стих");

            connection++;
            //line.UpConnection(0, connection);
            // у стихотворных строф своя связь - отрицательная
            line.SetStrongConnection(connection - connection * 2);
            //line.SetStrongConnection(connection);

            foreach (Tag elementPoemTag in tag.GetListTags())
            {
                if (elementPoemTag.GetName().ToLower() == "epigraph")
                {
                    EpigraphIdentificationAndAdd(elementPoemTag, line);
                }
                else if (elementPoemTag.GetName().ToLower() == "stanza")
                {
                    StanzaIdentificationAndAdd(elementPoemTag, line);
                }
                // не будет ли лишним все, что дальше?
                else if (elementPoemTag.GetName().ToLower() == "text-author")
                {
                    line.AddNewPartOfTitleOld(" (автор: " + elementPoemTag.GetContent() + ")");
                    //line.SetLine(elementPoemTag.GetContent());
                }
                else if (elementPoemTag.GetName().ToLower() == "date")
                {
                    // написать, что делать с date
                    //line.SetLine(elementPoemTag.GetContent());
                }
            }
        }
        private Line PoemIdentificationAndReturn(Tag tag, Line line)
        {
            foreach (Tag elementPoemTag in tag.GetListTags())
            {
                if (elementPoemTag.GetName().ToLower() == "epigraph")
                {
                    line = EpigraphIdentificationAndReturn(elementPoemTag, line);
                }
                else if (elementPoemTag.GetName().ToLower() == "stanza")
                {
                    line = StanzaIdentificationAndReturn(elementPoemTag, line);
                }
                else if (elementPoemTag.GetName().ToLower() == "text-author")
                {
                    line.AddNewPartOfLine(elementPoemTag.GetContent());
                }
                else if (elementPoemTag.GetName().ToLower() == "date")
                {
                    // написать, что делать с date
                    //line.SetLine(elementPoemTag.GetContent());
                }
            }

            return line;
        }
        private void StanzaIdentificationAndAdd(Tag tag, Line line)
        {
            // чтение title
            if (tag.GetListTags()[0].GetName().ToLower() == "title")
            {
                line = TitleIdentificationAndReturn(tag.GetListTags()[0], line);
            }
            else line.AddNewPartOfTitleOld(tag.GetOrderNumber() + " строфа");

            //Line tempLine = line;

            foreach (Tag elementStanzaTag in tag.GetListTags())
            {
                // нужно ли это?
                //if (elementStanzaTag.GetName().ToLower() == "subtitle")
                //{
                //    // возвращение tempLine в исходное состояние line
                //    tempLine = line;

                //    tempLine = SubtitleTitleIdentification(elementStanzaTag, tempLine);
                //}
                //else tempLine.SetLine(elementStanzaTag.GetContent());

                line.AddNewPartOfLine(elementStanzaTag.GetContent());
            }

            line.SetIndex(lineIndex);
            listLines.Add(line);

            lineIndex++;
        }
        private Line StanzaIdentificationAndReturn(Tag tag, Line line)
        {
            foreach (Tag elementStanzaTag in tag.GetListTags())
            {
                line.AddNewPartOfLine(elementStanzaTag.GetContent());
            }

            return line;
        }
        private Line VIdentification(Tag tag, Line line)
        {
            // думаю, стихотворным строкам не нужно указывать их номера, т.к. они будут всегда слитны со строфами
            //line.SetTitle(tag.GetOrderNumber() + " строка");
            line.AddNewPartOfLine(tag.GetContent());

            return line;
        }
        private void PIdentification(Tag tag, Line line)
        {
            line.AddNewPartOfTitleOld(tag.GetOrderNumber() + " абзац");
            line.AddNewPartOfLine(tag.GetContent());
            line.SetIndex(lineIndex);
            listLines.Add(line);

            lineIndex++;
        }


        private void ClearSpacesInTags(Tag tag)
        {
            while (tag.GetContent().IndexOf("  ") != -1)
            {
                tag.SetContent(tag.GetContent().Replace("  ", " "));             
            }

            foreach (Tag elementTag in tag.GetListTags())
            {
                ClearSpacesInTags(elementTag);
            }
        }
        private void ClearSpacesInLines()
        {
            foreach (Line line in listLines)
            {
                line.SetLine(line.GetLine().Replace(" \n", "\n"));
            }
        }
        private void FillListStrongConnections()
        {
            foreach (Line elementListLines in listLines)
            {
                listStrongConnections.Add(elementListLines.GetStrongConnection());
            }
        }
        private void FillListRecommendConnections()
        {
            listRecommendConnections = comprehension.Run(listLines);
        }
        private void FillListExplanationStrongConnections()
        {
            for (int iListStrongConnections = 0; iListStrongConnections < listStrongConnections.Count(); iListStrongConnections++)
            {
                // текущее значение связи
                int currentConnection = listStrongConnections[iListStrongConnections];
                // сумма длинн всех строк текущей связи
                int lengthCurrentConnection = 0;
                // индекс начала текущей связи
                int currentStartIndex = iListStrongConnections;

                while (iListStrongConnections < listStrongConnections.Count() && (listStrongConnections[iListStrongConnections] == currentConnection))
                {
                    lengthCurrentConnection += listLines[iListStrongConnections].GetLine().Length;

                    iListStrongConnections++;
                }


                if (lengthCurrentConnection < 1900)
                {
                    for (int i = currentStartIndex; i < iListStrongConnections; i++)
                    {
                        // "0" обозначает то, что все фрагменты текущей связи можно брать вместе с другими для формирования частицы
                        listExplanationStrongConnections.Add(0);
                    }
                }
                else if (lengthCurrentConnection < 3900)
                {
                    for (int i = currentStartIndex; i < iListStrongConnections; i++)
                    {
                        // "1" обозначает то, что все фрагменты текущей связи можно брать без соединения с другими для формирования частицы
                        listExplanationStrongConnections.Add(1);
                    }
                }
                else
                {
                    for (int i = currentStartIndex; i < iListStrongConnections; i++)
                    {
                        // "2" обозначает то, что формировать частицу можно лишь внутри текущей связи
                        listExplanationStrongConnections.Add(2);
                    }
                }

                // потому что после завершения цикла while переменная будет уже иметь индекс следующей связи, а for еще ее увеличит
                iListStrongConnections--;
            }
        }
        private void FillListExplanationRecommendConnections()
        {
            // "0" - без разницы, "1" - должна начинать фрагмент, "2" - должна продолжать фрагмент
            int previousConnection = 0; ;

            foreach (int currentConnection in listRecommendConnections)
            {
                if (currentConnection == 0)
                {
                    listExplanationRecommendConnections.Add(0);
                }
                else if ((currentConnection == previousConnection) | (currentConnection < previousConnection)) 
                {
                    listExplanationRecommendConnections.Add(2);
                }
                else if (currentConnection > previousConnection)
                {
                    listExplanationRecommendConnections.Add(1);
                }
                //else if (currentConnection < previousConnection)
                // если следущая связь меньше текущей?
                // если две "1" подряд?

                previousConnection = currentConnection;
            }
        }
        private void PutConnectionsIntoLines()
        {
            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
            {
                Line tempLine = listLines[iListLines];

                tempLine.SetRecommendConnection(listRecommendConnections[iListLines]);
                tempLine.SetExplanationStrongConnection(listExplanationStrongConnections[iListLines]);
                tempLine.SetExplanationRecommendConnection(listExplanationRecommendConnections[iListLines]);

                listLines[iListLines] = tempLine;
            }
        }
        private void FixListRecommendConnection()
        {
            for (int iListExplanationRecommendConnection = 0; 
                iListExplanationRecommendConnection < listRecommendConnections.Count() - 1; 
                iListExplanationRecommendConnection++)
            {
                if ((listExplanationRecommendConnections[iListExplanationRecommendConnection] == 1) 
                    & (listExplanationRecommendConnections[iListExplanationRecommendConnection + 1] == 1))
                {
                    int iOneConnection = iListExplanationRecommendConnection + 1;

                    while (listExplanationRecommendConnections[iOneConnection] == 1)
                    {
                        listExplanationRecommendConnections[iOneConnection] = 2;

                        iOneConnection++;
                    }
                }
            }
        }
        private void ClearLines()
        {
            
        }


        public void SetBookPath(string newBookPath)
        {
            bookPath = newBookPath;
        }
        public void SetConnectionString(string newConnectionString)
        {
            connectionString = newConnectionString;
        }
        public void SetTypeWrite (string newTypeWrite)
        {
            typeWrite = newTypeWrite;
        }
        public void SetForm(MainForm newForm)
        {
            form = newForm;
        }
    }
}
