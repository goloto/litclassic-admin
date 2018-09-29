using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace litclassic
{
    class Book
    {
        // 23.09
        // может быть сделать этот класс лишь хранилищем всех массивов и списков, работу же всю разделить по другим классам?
        //
        // особенно мне нравится то, что работать можно всегда с неформатируемыми фрагментами текста, т.е. с тегами,
        // а дальнейшее преобразование и "очистки" проводить отдельно, независимо
        //
        // мне кажется, нужно сделать так, чтобы этот класс свободно сохранялся на диске, имея в себе только данные,
        // но при этом была бы возможность его "пересоздать", проведя все те махинации, которые обновляют его списки и массивы,
        // но, пока в этом нет надобности (например, не обновились эти механизмы), их при запуске не загружать

        private string fb2=""; // строка файла fb2
        private string body; // "тело" книги
        private string description; // описание книги
        private List<string> listTags = new List<string>(); // список тегов
        private List<string> listSequenceTags = new List<string>(); // список последовательности тегов
        private List<int> listCountTags = new List<int>(); // список количества тегов
        //private List<string> listParagraphs = new List<string>(); // список параграфов книги
        private List<int> listCountEnclosedTags = new List<int>(); // список длинн каждого тега
        private List<double> listAverageCountEnclosedTags = new List<double>(); // список среднего количества использования тега в книге
        //private List<string> listBody = new List<string>();
        private List<string> listClosingTags = new List<string>(); // список "закрывающих" тегов
        private string[] tagsContent; // массив со строками тегов
        private List<int> listTagsContentLength = new List<int>(); // список количества символов в строке тега
        private List<int> listTagsContentLengthAverage = new List<int>(); // список среднего количества символов в строках тегов
        private List<int> listTreeTags = new List<int>(); // "настоящее" дерево; список индексов тега, включающего выбранный
        private List<int> listTreeInterruptTags = new List<int>(); // улучшенный предыдущий список
        private List<string> listTagsColor = new List<string>(); // список "цветов" тега
        private List<int> listTagsContentSpaceCount = new List<int>(); // список количества пробелов в строке тега
        // сделать два массива, содержащих в индексе число встречаемых таких индексов в listIncludedTags и listIncludedTagsPlus
        // а также два массива, в индексе которого сумма символов строк, значения которых в listIncludedTags и listIncludedTagsPlus - этот индекс
        private int[] includedTagsCount;
        private int[] includedTagsPlusCount;
        private int[] includedTagsSymbolCount;
        private int[] includedTagsPlusSymbolCount;
        // список разницы количества символов в теге впереди и список разницы количества символов в теге сзади
        private string[] arrayColorTags; // список последовательности цветов тегов

        public Book()
        {
           
        }


        // get и set функции класса
        public void GetBook()
        {

        }
        public void SetBook(string path) // добавление новой книги
        {
            Encoding win1251 = Encoding.GetEncoding(1251);

            using (var reader = new StreamReader(path, win1251))
            {
                fb2 = reader.ReadToEnd();
            }

            // последовательность ниже важна
            BodySeparate(); // поиск "тела"
            TagsReplacing(); // замена тегов в "теле"           
            TagsSearch(); // поиск тегов
            DeleteUniqueTags(); // удаление уникальных тегов
            CreatingListTreeTags(); // построение списка последовательности тегов
            TagTreeAverage(); // построение списка среднего значения каждого тега в дереве
            ClosingTagSearch(); // построение списка закрывающих тегов
            ClosingTagRemove(); // удаление закрывающих тегов из списков
            BodyStringTagsSeparate(); // разделение тела на строки тегов
            TagsContentLength(); // подсчет количества символов и пробелов в каждой строке тега
            TagsContentLengthAverage(); // подсчет среднего количества символов в строке для каждого тега
            CreatingListTreeInterruptTags(); // построение улучшенного списка последовательности тегов
            MassivesCalculations(); // что-там много и не очень нужного
            //TagsColors(); // разукрашивание тегов
            //DeletingTags();

            DescriptionSeparate(); // формирование описания

            //Coloring coloringBook = new Coloring();
            //coloringBook.Initialize(this);
            //arrayColorTags = coloringBook.GetColorTags();

            // дерево строится неверно! (или верно)
            // считается количество вхождений закрывающих тегов и тегов уникальных
        }


        // внешние get функции переменных класса
        public List<string> GetListTags() // запрос списка тегов книги
        {
            return listTags;
        }
        public List<int> GetListCountTags() // запрос списка количества тегов книги
        {
            return listCountTags;
        }
        public string GetDescription() // запрос описания книги
        {
            return description;
        }
        public List<string> GetListSequenceTags()
        {
            return listSequenceTags;
        } // запрос списка последовательности тегов книги
        public List<int> GetListCountEnclosedTags() // запрос списка длинн каждого тега
        {
            return listCountEnclosedTags;
        }
        public List<double> GetListAverageCountEnclosedTags() // запрос на список среднего количества подтегов для тега
        {
            return listAverageCountEnclosedTags;
        }
        public List<int> GetListParagraphCount() // запрос списка
        {
            return listTagsContentLength;
        }
        public List<int> GetListParagraphCountAverage() // запрос списка средней длины строк тегов
        {
            return listTagsContentLengthAverage;
        }
        public string[] GetParagraphs() // запрос массива параграфов
        {
            return tagsContent;
        }
        public List<int> GetListTreeTags() // запрос "настоящего" дерева (списка включения тегов)
        {
            return listTreeTags;
        }
        public List<int> GetListTreeInterruptTags() // запрос улучшенного "настоящего" дерева
        {
            return listTreeInterruptTags;
        }
        public List<string> GetListTagsColor() // запрос списка "цветов" тегов
        {
            return listTagsColor;
        }
        public string[] GetArrayColorTags() // запрос на массив цветов последовательности тегов
        {
            return arrayColorTags;
        }
        
        // четыре сомнительные по нужности get функции
        // исключение - индекс вне массива/'
        public int GetIncludedTagsCount(int index)
        {
            //if (includedTagsCount[index] != 0)
            //{
                return includedTagsCount[index];
            //}
            //else return -100;
        }
        public int GetIncludedTagsPlusCount(int index)
        {
            //if (includedTagsPlusCount[index] != 0)
            //{
                return includedTagsPlusCount[index];
            //}
            //else return -100;
        }
        public int GetIncludedTagsSymbolCount(int index)
        {
            //if (includedTagsSymbolCount[index] != 0)
            //{
                return includedTagsSymbolCount[index];
            //}
            //else return -100;
        }
        public int GetIncludedTagsPlusSymbolCount(int index)
        {
            //if (includedTagsPlusSymbolCount[index] != 0)
            //{
                return includedTagsPlusSymbolCount[index];
            //}
            //else return -100;
        }



        // внутренние set функции для переменных класса
        private bool GetTagUnique(string tag)
        {
            bool unique = false;
            return unique;
        }
        private int GetTagCount(string tag)
        {
            int tagCount = 1;
            return tagCount;
        }
        private void SetTag(string tag) // создание тега и всего того, что потребуется для работы с ним
        {
            bool tagExist = false; // записан ли уже найденный тег
            int numCountTag = 0; // номер тега в списке тегов
            for (int iListTags = 0; iListTags < listTags.Count(); iListTags++) // проверка, есть ли уже данный тег
            {
                if (tag == listTags[iListTags])
                {
                    tagExist = true;
                    numCountTag = iListTags; // запись номера в списке тегов найденного тега
                }
            }
            if (tagExist != true) // действие, если тег найден впервые
            {
                listTags.Add(tag); // добавление тега в список тегов
                listCountTags.Add(0); // добавление нового счетчика количества повторений тега
                //listTagsColor.Add("yellow"); // добавление цвета "по-умолчанию" тегу
                //if (tag[1] == '/') listClosingTags.Add(tag);
            }
            else listCountTags[numCountTag]++; // прибавление единицы к количеству встреч найденного тега, если тег был уже до этого найден
        }



        // внутренние невозвращающие принимающие функции класса
        private void DeletingTags(string paragraph) // удаление тегов из получаемой функцией строки
        {
            while (paragraph.IndexOf('>') != -1) // цикл действует, пока есть хоть один символ ">"
            {
                paragraph = paragraph.Remove(paragraph.IndexOf('<'), paragraph.IndexOf('>') - paragraph.IndexOf('<') + 1); // удаление всего между "<" и ">"
            }
            //return paragraph;
        }



        // внутренние невозвращающие не принимающие функции класса
        private void BodySeparate() // выделение "тела" книги
        {
            // что делать, если тега "тела" нет?

            int bodyStart = fb2.IndexOf("<body>", 0);

            if (bodyStart!=-1)
            {
                int bodyEnd = fb2.IndexOf("</body>", bodyStart);

                body = fb2.Substring(bodyStart, bodyEnd - bodyStart + 8); // "+8" - длинна тега </body>
            }
            else body = fb2;           
        }
        private void DescriptionSeparate() // выделение описания книги
        {
            int descriptionStart = fb2.IndexOf("<description>", 0);
            int descriptionEnd = fb2.IndexOf("</description>", descriptionStart);

            description = fb2.Substring(descriptionStart, descriptionEnd-descriptionStart+14); // "+14" - длинна тега </description>
        }
        private void TagsSearch() // создание списка тегов книги
        {
            for (int lastPosition = 0; lastPosition<body.Length;lastPosition++)
            {
                int tagStart = body.IndexOf("<", lastPosition); // индекс начала первого встречного тега
                int tagFinish = body.IndexOf(">", tagStart); // индекс последнего символов первого встречного тега

                if (tagFinish != -1) // часто tagFinish возвращал "-1"
                {
                    SetTag(body.Substring(tagStart, tagFinish - tagStart + 1)); // создание тега
                    listSequenceTags.Add(body.Substring(tagStart, tagFinish - tagStart + 1)); // доабвление в список строки, содержащей название этого тега

                    lastPosition = tagFinish + 1; // перемещение начальной позиции поиска на следующий за найденным тегом индекс
                }               
            }
        }
        private void TagsReplacing() // всяческие замены тегов
        {
            body = body.Replace("<empty-line />", "<p></p>"); // замена пустой строки пустым абзацем
            body = body.Replace("<empty-line/>", "<p></p>");
            body = body.Replace("<strong>", ""); // замена тега жирного шрифта пустотой
            body = body.Replace("</strong>", "");
            body = body.Replace("<emphasis>", ""); // замена тега наклонного шрифта пустотой
            body = body.Replace("</emphasis>", "");
            body = body.Replace("<stanza>", "<p>"); // замена строфы пустой строкой
            body = body.Replace("</stanza>", "</p>");
            body = body.Replace("<poem>", ""); // замена обозначения начала стихов пустотой
            body = body.Replace("</poem>", "");

            // может стоит заменять <poem> на что-нибудь, что будет неким "средним" заголовком в книге? например, <subtitle>
            // при этом <v> замениться на <p>
            // или вообще сделать уникальный тег, все вхождения в который алгоритм никогда не будет делить
        }
        private void CreatingListTreeTags()// подсчет длинн каждого тега, а также списка индексов "вышестоящих" тегов
        {
            // заполнение списка включенных тегов "пустышками"
            for (int iListIncludedTags = 0; iListIncludedTags < listSequenceTags.Count(); iListIncludedTags++) 
            {
                listTreeTags.Add(-1);
                listTreeInterruptTags.Add(-1);
            }

            for (int iListSequenceTags = 0; iListSequenceTags < listSequenceTags.Count(); iListSequenceTags++)
            {
                int countTree = listSequenceTags.IndexOf("</" + listSequenceTags[iListSequenceTags].Substring(1), iListSequenceTags + 1) - iListSequenceTags - 1;
                // количество тегов до первого встречного тега, закрывающего выбранный;
                
                for (int iIncludedTags = iListSequenceTags + 1; iIncludedTags <= (countTree + iListSequenceTags); iIncludedTags++)
                // заполнение списка индекса тега, включающего выбранный
                {
                    listTreeTags[iIncludedTags] = iListSequenceTags;
                    listTreeInterruptTags[iIncludedTags] = iListSequenceTags;
                }               
                listCountEnclosedTags.Add(countTree);
               
                // заполнение улучшенного списка индекса тега, включающего выбранный, только теперь будет считаться индекс того тега, который прерывает
                // цепочку из одинаковых тегов
            }
        }
        private void CreatingListTreeInterruptTags() // подсчет улучшенного списка индексов тегов, включающий выбранный
        {
            for (int iListSequenceTags = 0; iListSequenceTags < listSequenceTags.Count() - 1; iListSequenceTags++) // цикл на каждый элемент дерева
            {
                if (listSequenceTags[iListSequenceTags + 1] != listSequenceTags[iListSequenceTags]) // условие является ли следующий элемент таким же как и предыдущий
                {
                    listTreeInterruptTags[iListSequenceTags + 1] = iListSequenceTags; // если нет, то ему присваивается индекс предыдущего
                }
                else listTreeInterruptTags[iListSequenceTags + 1] = listTreeInterruptTags[iListSequenceTags]; // если да, то присваивается значение предыдущего
            }
        }
        private void DeleteUniqueTags() // удаление уникальных тегов из списка тегов, списка количества тегов и списка последовательности тегов
        {
            for (int iListTags=0;iListTags<listTags.Count();iListTags++)
            {
                if (listCountTags[iListTags]==0)
                {
                    //listClosingTags.RemoveAt(listClosingTags.IndexOf(listTags[iListTags]));
                    body = body.Replace(listTags[iListTags],""); // удаление тега из "тела" книги

                    listSequenceTags.Remove(listTags[iListTags]);
                    listTags.RemoveAt(iListTags);
                    listCountTags.RemoveAt(iListTags);
                    //listTagsColor.RemoveAt(iListTags);

                    iListTags--;
                }
            }
        }
        private void TagTreeAverage() // подсчет среднего количества вхождений тегов в тег
        {
            for (int iListTags=0;iListTags<listTags.Count();iListTags++)
            {
                int sumAvrTag = 0;
                int iListCountTags = 0;
                int lastITag = 0; // начальная позиция для поиска тега

                while (iListCountTags<listCountTags[iListTags])
                {
                    sumAvrTag+=listCountEnclosedTags[listSequenceTags.IndexOf(listTags[iListTags], lastITag)];
                    iListCountTags++;
                    lastITag = listSequenceTags.IndexOf(listTags[iListTags], lastITag) + 1;
                }

                listAverageCountEnclosedTags.Add(sumAvrTag / listCountTags[iListTags]);
            }
        }
        private void BodyStringTagsSeparate() // разделение "тела" на список строк всего включенного в каждый тег
        {
            tagsContent = new string[listSequenceTags.Count];
            for (int iListTags=0;iListTags<listTags.Count();iListTags++)
            {
                int firstIndexTagOpen = 0;
                int firstIndexTagClose = 0;
                int firstIndexListSequenceTag = 0;

                for (int iListCountTags=0;iListCountTags<listCountTags[iListTags];iListCountTags++)
                {
                    int currentIndexListSequenceTag = listSequenceTags.IndexOf(listTags[iListTags], firstIndexListSequenceTag); // индек искомого тега
                    string tagOpen = listTags[iListTags]; // открывающий тег
                    string tagClose = "</" + listTags[iListTags].Substring(1); // закрывающий тег: "</ и открывающий тег со второго символа"
                    int indexTagOpen = body.IndexOf(tagOpen,firstIndexTagOpen); // индекс следующего вхождения открывающего тега отсчетом от предудыщего
                    int indexTagClose = body.IndexOf(tagClose,firstIndexTagClose); // индекс следующего вхождения закрывающего тега отсчетом от предыдущего

                    firstIndexTagOpen = indexTagOpen + 1; // перестановка индекса предыдущего открывающего тега
                    firstIndexTagClose = indexTagClose + 1;// перестановка индекса предыдущего закрывающего тега
                    firstIndexListSequenceTag = currentIndexListSequenceTag + 1;
                    tagsContent[currentIndexListSequenceTag] = body.Substring(indexTagOpen + tagOpen.Length, indexTagClose - indexTagOpen - tagOpen.Length);
                    // 23.09
                    // иногда случается, что каким-то образом индекс закрывающего тега идет позади открывающего

                    DeletingTags(tagsContent[currentIndexListSequenceTag]);
                }
            }
        }
        private void ClosingTagSearch() // поиск закрывающих тегов
        {
            for (int iListTags=0;iListTags<listTags.Count();iListTags++)
            {
                if (listTags[iListTags][1] == '/') listClosingTags.Add(listTags[iListTags]); // просто добавляет в список то, что начинате с "/" в списке тегов
            }
        }
        private void ClosingTagRemove() // исключение из всех списков закрывающих тегов
        {
            for (int iListClosingTags=0;iListClosingTags<listClosingTags.Count();iListClosingTags++)
            {
                int iListTags = listTags.IndexOf(listClosingTags[iListClosingTags]); // переменную нужно "запомнить"

                listTags.RemoveAt(iListTags); // удаление из списка тегов
                listCountTags.RemoveAt(iListTags); // удаление из списка количества тегов
                //listTagsColor.RemoveAt(iListTags);
                listAverageCountEnclosedTags.RemoveAt(iListTags); // удаление из списка среднего количества вхождений тегов в тег         
            }

            // нужно ли вообще исключение закрывающих тегов из этих списков?
            // вроде работает, если закомментировать

            for (int iListSequenceTags = 0; iListSequenceTags < listCountEnclosedTags.Count(); iListSequenceTags++)
            {
                if (listSequenceTags[iListSequenceTags][1] == '/')
                {
                    listSequenceTags.RemoveAt(iListSequenceTags); // удаление из списка последовательности тегов
                    listCountEnclosedTags.RemoveAt(iListSequenceTags); // удаление из списка "дерева" тегов
                    listTreeTags.RemoveAt(iListSequenceTags); // удаление из списка индекса тега, включающего выбранный
                    listTreeInterruptTags.RemoveAt(iListSequenceTags);
                    iListSequenceTags--; // чтобы цикл работал верно, т.к. в первой строке он сокращается на 1 элемент
                }
            }
        }
        private void TagsContentLength() // подсчет количества символов и пробелов в строке тэга
        {
            for (int iTagsContentCount=0;iTagsContentCount<tagsContent.Length;iTagsContentCount++)
            {
                if (tagsContent[iTagsContentCount] == null) // некоторые строки ничего не содержат, т.к. содержали в себе теги-разделители, которые были заменены на пустоту
                {
                    listTagsContentLength.Add(0);
                    listTagsContentSpaceCount.Add(0); 
                }
                else
                {
                    listTagsContentLength.Add(tagsContent[iTagsContentCount].Length); // добавление количества символов в строке в список
                    listTagsContentSpaceCount.Add(tagsContent[iTagsContentCount].Split(' ').Length - 1); // добавление количества пробелов в строке в список
                }
            }
        }
        private void TagsContentLengthAverage() // подсчет среднего количества символов строк для каждого тега
        {
            for (int iListTags = 0; iListTags < listTags.Count(); iListTags++)
            {
                int sumStringSymbol = 0; // сумма символов в строке
                int iListCountTags = 0; // индекс в списке количества встречаемости тегов
                int lastITag = 0; // начальная позиция для поиска тега

                while (iListCountTags < listCountTags[iListTags])
                {
                    sumStringSymbol += listTagsContentLength[listSequenceTags.IndexOf(listTags[iListTags], lastITag)]; // прибавление значения из списка количества символов в теге,
                    // начиная с позиции последнего найденного такого же тега + 1
                    iListCountTags++;
                    lastITag = listSequenceTags.IndexOf(listTags[iListTags], lastITag) + 1; // назначение новой позиции для поиска
                }

                listTagsContentLengthAverage.Add(sumStringSymbol / listCountTags[iListTags]); // подсчет среднего количества символов в строке
            }
        }
        private void MassivesCalculations() // сомневаюсь, что это понадобится
        {
            includedTagsCount = new int[listSequenceTags.Count()];
            includedTagsPlusCount = new int[listSequenceTags.Count()];
            includedTagsSymbolCount = new int[listSequenceTags.Count()];
            includedTagsPlusSymbolCount = new int[listSequenceTags.Count()];

            for (int i=0;i<listSequenceTags.Count();i++)
            {
                includedTagsCount[i] = listTreeTags.Count(i.Equals);
                includedTagsPlusCount[i] = listTreeInterruptTags.Count(i.Equals);

                for (int iParagraphs=0;iParagraphs<listTreeTags.Count();iParagraphs++)
                {
                    if (listTreeTags[iParagraphs] == i)
                    {
                        if (tagsContent[iParagraphs]!=null)
                        {
                            includedTagsSymbolCount[i] += tagsContent[iParagraphs].Count();
                        }
                        else includedTagsSymbolCount[i] += 0;
                    }
                    if (listTreeInterruptTags[iParagraphs] == i)
                    {
                        if (tagsContent[iParagraphs] != null)
                        {
                            includedTagsPlusSymbolCount[i] += tagsContent[iParagraphs].Count();
                        }
                        else includedTagsPlusSymbolCount[i] += 0;
                    }
                }
            }
        }
        private void TagsColors()
        {           
            listTagsColor[listAverageCountEnclosedTags.IndexOf(listAverageCountEnclosedTags.Max())] = "red";
            listTagsColor[listAverageCountEnclosedTags.IndexOf(listAverageCountEnclosedTags.Min())] = "green";
            listTagsColor[listTagsContentLengthAverage.IndexOf(listTagsContentLengthAverage.Max())] = "red";
            listTagsColor[listCountTags.IndexOf(listCountTags.Min())] = "red";
            // если в книге нет этих тегов, будет ошибка
            listTagsColor[listTags.IndexOf("<p>")] = "green";
            listTagsColor[listTags.IndexOf("<v>")] = "green";
            listTagsColor[listTags.IndexOf("<title>")] = "red";
            listTagsColor[listTags.IndexOf("<subtitle>")] = "red";
            listTagsColor[listTags.IndexOf("<section>")] = "red";
            //listTagsColor[listTags.IndexOf("<epigraph>")] = "red";
        }


        //private void DeletingTags() // удаление тегов из получаемой функцией строки
        //{
        //    for (int iParapraphs=0;iParapraphs<paragraphs.Count();iParapraphs++)
        //    {
        //        while (paragraphs[iParapraphs].IndexOf('>') != -1) // цикл действует, пока есть хоть один символ ">"
        //        {
        //            paragraphs[iParapraphs] = paragraphs[iParapraphs].Remove(paragraphs[iParapraphs].IndexOf('<'), 
        //                paragraphs[iParapraphs].IndexOf('>') - paragraphs[iParapraphs].IndexOf('<') + 1); // удаление всего между "<" и ">"
        //        }
        //    }
            
        //}


        // список списков, в котором номер вложенного списка совпадает с номером тега из списка тегов, а внутри него - порядковые номера его в общей строке книги
        // список специальных классов
        //
        // для формирования иерархии создать дерево вида "верхний тег-средний тег-нижний тег-тег абзаца", 
        // где создание цитаты будет проходить проверку целостности, т.е. принадлежности одному смысловому отрезку,
        // сверху-вниз
        //
        // !!!
        // вычеркивать все уникальные теги?
        //
        // привязка данных о внутренних тегах к самому тегу
        // подсчет среднего количества симвоов внутри тега 0, 1 и 2 уровней (примерно)
        //
        // каждому тегу длина и его количество
        //
        // 0.1. перестроить все списки с тегами так, чтобы в них не было закрывающих тегов
        // 0.2. удалить все теги из списка строк
        //
        // 1. "тело" делится на список строк (как именно? брать только нулевые? или несколько еще?)
        // 2. выбирается случайный элемент списка
        // 3. записываются порядковые номера тегов, в которые входит эта строка
        // 4. выбирается следующая за ним строка
        // 5. сверяются порядковые номера тегов, в которые входит эта строка, с номерами "родительской строки"
        // 6. проверяется размер полученной цитаты
        // 7. повторяется, пока не будет достигнут нужный размер, или не будут обнаружены новые цифры, т.е. строка из нового тега
        // 7.1 если обнаружены новые цифры - сверка размерности строк нового размера с оставшимся; если размер позволяет - включение строк в цитату, 
        // если нет - добавление строк в начало и переход к п.4 с той разницой, что строки будут добавляться в начало
        // 7.2 если достигнут размер, проверка на различные смысловые "огрехи"
        //
        // сделать в дальнейшем проверку новой книги на "адекватность" тегов
        //
        // среднее количество символов в "нулевках" чтобы понять, что считать тегами заголовков?
        // а лучше вообще для всех тегов и количество символов списком и среднее
        //
        // вырезать по тегам, сделать функцию, принимающую тег, смотрящая его номер и составляющая список
        //
        //
        // класс инспектора будет содержать номера строк готовой цитаты
        // класс цитаты будет содеражать строку, состоящую из строк инспектора и описания места и произведения цитаты
        // класс цитаты формировать описание цитаты будет самостоятельно
    }
}
