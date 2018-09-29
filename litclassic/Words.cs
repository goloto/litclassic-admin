using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace litclassic
{
    class Words
    {
        private MainForm form;
        private WordsDBConnect currentWordsDBConnect;
        private Timer currentTimer;
        private int currentProgressValue;
        string dictionary;
        private List<string> listWordNames = new List<string>();
        private List<string> listWordValues = new List<string>();
        private List<char> listRussianCapitalLetters;
        private List<string> listWordLinks = new List<string>();
        private List<char> listWordFirstLetters = new List<char>();



        public void CreateWords(string newDictionary)
        {
            dictionary = newDictionary;
            // создание таймера с тиком в 1000
            currentTimer = new System.Timers.Timer(1000);
            // добавление события таймера на каждый тик
            currentTimer.Elapsed += OnTimedEvent;

            // запуск таймера
            currentTimer.Start();
            ChangeProgressMaximumValue(4);
            currentProgressValue = 0;

            // заполнение списка заглавных букв русского алфавита
            FillListRussianCapitalLetters();
            currentProgressValue++;
            ChangePhaseName("Составление русского алфавита.");
            // поиск слов и заполнение списка их названий и значений
            FindWords();
            currentProgressValue++;
            ChangePhaseName("Составление списка слов и их значений.");
            // заполнение списка ссылок на слова
            FillListWordLinks();
            currentProgressValue++;
            ChangePhaseName("Составление списка внутренних ссылок.");
            // заполнение списка первых букв слов
            FillListWordFirstLetters();
            currentProgressValue++;
            ChangePhaseName("Составление списка первых букв слов.");
        }
        public List<string> GetListWordNames()
        {
            return listWordNames;
        }
        public List<string> GetListWordValues()
        {
            return listWordValues;
        }
        public List<string> GetListWordLinks()
        {
            return listWordLinks;
        }
        public void SetListWordNames(List<string> newListWordNames)
        {
            listWordNames = newListWordNames;
        }
        public void SetListWordValues(List<string> newListWordValues)
        {
            listWordValues = newListWordValues;
        }
        public void SetListWordLinks(List<string> newListWordLinks)
        {
            listWordLinks = newListWordLinks;
        }
        public void SetForm(MainForm newForm)
        {
            form = newForm;
        }
        public void AddWordsToDB()
        {
            currentWordsDBConnect = new WordsDBConnect();

            ChangeProgressMaximumValue(listWordNames.Count + 1);

            currentProgressValue = 0;

            ChangePhaseName("Установление соединения с базой и удаление предыдущих значений табоицы.");
            currentWordsDBConnect.SetSQLConnectionToAzureDBLitClassicBooks();
            // зачистка таблицы от всех предыдущих значений
            currentWordsDBConnect.DeleteAllFromWords();
            currentProgressValue++;
            ChangePhaseName("Заполнение таблицы данными.");

            for (int i = 0; i < listWordNames.Count; i++)
            {
                currentWordsDBConnect.WriteNewWord(i, listWordNames[i], listWordValues[i], listWordLinks[i], listWordFirstLetters[i]);
                currentProgressValue++;
            }
        }



        private void FillListRussianCapitalLetters()
        {
            listRussianCapitalLetters = new List<char>();

            listRussianCapitalLetters.Add('А');
            listRussianCapitalLetters.Add('Б');
            listRussianCapitalLetters.Add('В');
            listRussianCapitalLetters.Add('Г');
            listRussianCapitalLetters.Add('Д');
            listRussianCapitalLetters.Add('Е');
            listRussianCapitalLetters.Add('Ё');
            listRussianCapitalLetters.Add('Ж');
            listRussianCapitalLetters.Add('З');
            listRussianCapitalLetters.Add('И');
            listRussianCapitalLetters.Add('Й');
            listRussianCapitalLetters.Add('К');
            listRussianCapitalLetters.Add('Л');
            listRussianCapitalLetters.Add('М');
            listRussianCapitalLetters.Add('Н');
            listRussianCapitalLetters.Add('О');
            listRussianCapitalLetters.Add('П');
            listRussianCapitalLetters.Add('Р');
            listRussianCapitalLetters.Add('С');
            listRussianCapitalLetters.Add('Т');
            listRussianCapitalLetters.Add('У');
            listRussianCapitalLetters.Add('Ф');
            listRussianCapitalLetters.Add('Х');
            listRussianCapitalLetters.Add('Ц');
            listRussianCapitalLetters.Add('Ч');
            listRussianCapitalLetters.Add('Ш');
            listRussianCapitalLetters.Add('Щ');
            listRussianCapitalLetters.Add('Ъ');
            listRussianCapitalLetters.Add('Ы');
            listRussianCapitalLetters.Add('Ь');
            listRussianCapitalLetters.Add('Э');
            listRussianCapitalLetters.Add('Ю');
            listRussianCapitalLetters.Add('Я');
        }
        private void FindWords()
        {
            string wordName;
            string wordValue;
            int startPosition;

            for (int iDictionary = 0; iDictionary < dictionary.Length; iDictionary++) 
            {
                if ((dictionary[iDictionary] == ' ') && (dictionary[iDictionary + 1] == ' ') && (dictionary[iDictionary + 2] == ' ') 
                    && (listRussianCapitalLetters.IndexOf(dictionary[iDictionary + 3]) != -1) 
                    && (listRussianCapitalLetters.IndexOf(dictionary[iDictionary + 4]) != -1))
                {
                    wordName = "";
                    // перемещение переменной на начало названия слова
                    iDictionary = iDictionary + 3;
                    // несколько ли слов даётся при одном значении
                    bool severalWords = true;
                    // число слов, которым даётся одно значение
                    int wordsCount = 0;

                    while (severalWords)
                    {
                        // цикл на поиск названия слова
                        while ((listRussianCapitalLetters.IndexOf(dictionary[iDictionary]) != -1) 
                            | (dictionary[iDictionary] == '-') 
                            | (dictionary[iDictionary] == ' '))
                        {
                            wordName += dictionary[iDictionary];
                            iDictionary++;
                        }

                        // добавление вопроса к названию слова
                        if (dictionary[iDictionary] == '?')
                        {
                            wordName += dictionary[iDictionary];
                            iDictionary++;
                        }

                        // проверка на то, есть ли ещё одно название слова после найденного
                        if ((dictionary[iDictionary] == ',') && (dictionary[iDictionary + 1] == ' ') && (listRussianCapitalLetters.IndexOf(dictionary[iDictionary + 2]) != -1))
                        {
                            iDictionary = iDictionary + 2;
                        }
                        else
                        {
                            severalWords = false;
                        }

                        // удаление последнего символа - пробела, если такой имеется
                        if (wordName.Last() == ' ') wordName = wordName.Substring(0, wordName.Length - 1);

                        // добавление названия слова
                        listWordNames.Add(wordName);

                        // обнуление прежнего названия слова
                        wordName = "";
                        // увеличение количества слов
                        wordsCount++;
                    }

                    // начальная позиция - первый символ после конца названия слова
                    startPosition = iDictionary;

                    // если после значения слова есть ещё текст
                    if (dictionary.IndexOf("\r\n   ", startPosition) != -1)
                    {
                        wordValue = dictionary.Substring(startPosition, dictionary.IndexOf("\r\n   ", startPosition) - startPosition);
                        wordValue = wordValue.Replace("\r", "");
                        wordValue = wordValue.Replace("\n", " ");

                        // замена всех двойных, тройных и более пробелов на одинарные
                        while (wordValue.IndexOf("  ") != -1)
                        {
                            wordValue = wordValue.Replace("  ", " ");
                        }

                        // удаление первого и второго символа в начале значения слова, если это ", "
                        if ((wordValue.Length > 1) && (wordValue[0] == ',') & (wordValue[1] == ' ')) wordValue = wordValue.Substring(2);
                        // удаление первого символа в начале значения слова, если это пробел
                        if ((wordValue.Length > 0) && (wordValue[0] == ' ')) wordValue = wordValue.Substring(1);

                        while (wordsCount > 0) 
                        {
                            listWordValues.Add(wordValue);
                            wordsCount--;
                        }

                        iDictionary = dictionary.IndexOf("\r\n   ", startPosition);
                    }
                    // если это слово последнее
                    else
                    {
                        wordValue = dictionary.Substring(startPosition, dictionary.Length - startPosition);
                        wordValue = wordValue.Replace("\r", "");
                        wordValue = wordValue.Replace("\n", " ");

                        // замена всех двойных, тройных и более пробелов на одинарные
                        while (wordValue.IndexOf("  ") != -1)
                        {
                            wordValue = wordValue.Replace("  ", " ");
                        }

                        // удаление первого и второго символа в начале значения слова, если это ", "
                        if ((wordValue.Length > 1) && (wordValue[0] == ',') & (wordValue[1] == ' ')) wordValue = wordValue.Substring(2);
                        // удаление первого символа в начале значения слова, если это пробел
                        if ((wordValue.Length > 0) && (wordValue[0] == ' ')) wordValue = wordValue.Substring(1);

                        while (wordsCount > 0)
                        {
                            listWordValues.Add(wordValue);
                            wordsCount--;
                        }

                        iDictionary = dictionary.Length;
                    }
                }             
            }
        }
        private void FillListWordLinks()
        {
            // заполнение списка "пустыми" значениями
            foreach (string name in listWordNames)
            {
                listWordLinks.Add("-1");
            }

            // нахождение ссылок
            for (int iListWordValues = 0; iListWordValues < listWordValues.Count; iListWordValues++) 
            {
                if (listWordValues[iListWordValues].ToLower().IndexOf("см.") != -1)
                {
                    // 1. построить список индексов всех вхождений "см."
                    // 2. всё до ближайшей точки или закрытой скобки или точки с запятой
                    // 3. затем разделять на запятые и 'и'
                    // 4. убирать повторяющиеся пробелы
                    // 5. убирать слово "также"
                    // 6. не забывать про ToLower()

                    // при выводе слов нужно сравнивать, не одинаковые ли значения выводятся

                    List<int> listIndexes = new List<int>();
                    int searchIndexStart = 0;
                    // окончательный список ссылок
                    string links = "";

                    // 1
                    while (listWordValues[iListWordValues].ToLower().IndexOf("см.",searchIndexStart) != -1)
                    {
                        listIndexes.Add(listWordValues[iListWordValues].ToLower().IndexOf("см.", searchIndexStart) + 3);

                        searchIndexStart = listWordValues[iListWordValues].ToLower().IndexOf("см.", searchIndexStart) + 1;
                    }

                    // 2
                    foreach (int startIndex in listIndexes)
                    {
                        // список индексов 
                        List<int> listFinalSymbolsIndexes = new List<int>();
                        // последний символ
                        char finalSymbol;
                        // индекс последнего символа
                        int finalSymbolIndex;
                        // разделитель нескольких ссылок
                        string separator = "";
                        // список ссылок-слов
                        List<string> listLinks = new List<string>();

                        if (listWordValues[iListWordValues].ToLower().IndexOf(')', startIndex) != -1)
                            listFinalSymbolsIndexes.Add(listWordValues[iListWordValues].ToLower().IndexOf(')', startIndex));
                        if (listWordValues[iListWordValues].ToLower().IndexOf(';', startIndex) != -1)
                            listFinalSymbolsIndexes.Add(listWordValues[iListWordValues].ToLower().IndexOf(';', startIndex));
                        if (listWordValues[iListWordValues].ToLower().IndexOf('.', startIndex) != -1)
                            listFinalSymbolsIndexes.Add(listWordValues[iListWordValues].ToLower().IndexOf('.', startIndex));

                        if (listFinalSymbolsIndexes.Count == 0) finalSymbolIndex = listWordValues[iListWordValues].Length - 1;
                        else finalSymbolIndex = listFinalSymbolsIndexes.Min();

                        finalSymbol = listWordValues[iListWordValues][finalSymbolIndex];

                        // цельная ссылка
                        string linksFullText = listWordValues[iListWordValues].ToLower().Substring(startIndex, finalSymbolIndex - startIndex);

                        if (linksFullText.IndexOf(',') != -1) separator = ",";
                        if (linksFullText.IndexOf(" и ") != -1) separator = " и ";

                        // если в ссылке несколько слов
                        if (separator != "")
                        {
                            foreach (string link in linksFullText.Split(new[] { separator }, StringSplitOptions.None))
                            {
                                // если найденный фрагмент корректен
                                if (link.Length > 0)
                                {
                                    string newLink = link;
                                    // удаление лишнего
                                    newLink = newLink.Replace("см. ", "");
                                    newLink = newLink.Replace("также ", "");

                                    while (newLink.IndexOf("  ") != -1)
                                    {
                                        newLink = newLink.Replace("  ", " ");
                                    }

                                    // удаление первого пробела, если такой есть
                                    if (newLink[0] == ' ') newLink = newLink.Substring(1);
                                    // удаление последнего пробела, если такой есть
                                    if (newLink[newLink.Length - 1] == ' ') newLink = newLink.Substring(0, newLink.Length - 2);

                                    listLinks.Add(newLink);
                                }
                            }
                        }
                        // если в ссылке одно слово
                        else
                        {
                            // удаление лишнего
                            linksFullText = linksFullText.Replace("см. ", "");
                            linksFullText = linksFullText.Replace("также ", "");

                            while (linksFullText.IndexOf("  ") != -1)
                            {
                                linksFullText = linksFullText.Replace("  ", " ");
                            }

                            // удаление первого пробела, если такой есть
                            if (linksFullText[0] == ' ') linksFullText = linksFullText.Substring(1);
                            // удаление последнего пробела, если такой есть
                            if (linksFullText[linksFullText.Length - 1] == ' ') linksFullText = linksFullText.Substring(0, linksFullText.Length - 2);

                            listLinks.Add(linksFullText);
                        }

                        // проверка на то, есть ли такие слова в словаре
                        foreach (string link in listLinks)
                        {
                            // проверка, есть ли такое слово вообще в словаре и добавление его индекса к ссылке
                            if (listWordNames.Contains(link.ToUpper())) links += listWordNames.IndexOf(link.ToUpper()) + ";";
                        }
                    }

                    if (links != "")
                    {
                        // добавление окончательных ссылок/ссылки в список ссылок
                        listWordLinks[iListWordValues] = links;
                    }
                }
            }
        }
        private void FillListWordFirstLetters()
        {
            foreach (string word in listWordNames)
            {
                listWordFirstLetters.Add(word.ToUpper()[0]);
            }
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
        private void ChangePhaseName(string newPhaseName)
        {
            form.SetPhaseName(newPhaseName);
            form.Invoke(form.changePhaseNameDelegate);
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ChangeProgressCurrentValue(currentProgressValue);
        }
    }
}
