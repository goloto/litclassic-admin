using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litclassic
{
    class Title
    {
        // порядок:
        // находится цепочка заголовков с одинаковыми первыми частями
        // создаётся Title, titleString которого присваивают значение этой одинаковой части, он добавляется в список его "материнского" Title
        // изначальный список лишается первых элементов и передаётся в созданный Title
        // далее все идет сначала рекурсивно

        private string titleString;
        private List<Title> listTitles = new List<Title>();
        // то, что передаётся в рекурсию
        private List<List<string>> listPartsOfTitles = new List<List<string>>();
        private List<List<string>> listFuturePartsOfTitles = new List<List<string>>();
        // то, что содержит "материнский" заголовок
        private List<string> listTitleStrings = new List<string>();


        public void CreateTitle()
        {
            // индекс той части заголовка, с которой сравниваются остальные
            //int compareTitleIndex = 0;
            // переменная, которая принимает истинное значение, когда нужно выходит из цикла while
            //bool endWhile = false;

            if ((listPartsOfTitles.Count() != 0) && (listPartsOfTitles[0].Count() != 0)) 
            {
                string compareTitle = listPartsOfTitles[0][0];
                //int previousCompareIndex = compareTitleIndex;
                Title firstTitle = new Title();
                List<string> tempPartsOfTitle = new List<string>();

                // цикл на добавление в список всех частей одного заголовка, не включая первый
                for (int iPartOfTitle = 1; iPartOfTitle < listPartsOfTitles[0].Count(); iPartOfTitle++)
                {
                    tempPartsOfTitle.Add(listPartsOfTitles[0][iPartOfTitle]);
                }

                listFuturePartsOfTitles.Add(tempPartsOfTitle);
                firstTitle.SetTitleString(compareTitle);
                listTitles.Add(firstTitle);
                listTitles.Last().SetListPartsOfTitles(listFuturePartsOfTitles);

                // видимо, неправильно формируется listFuture...
                for (int iListPartsOfTitles = 1; iListPartsOfTitles < listPartsOfTitles.Count(); iListPartsOfTitles++)
                {
                    // если часть заголовка не совпадает
                    if (listPartsOfTitles[iListPartsOfTitles][0] != compareTitle)
                    {
                        // перед "обнулением" listFuturePartsOfTitles необходимо записать его в последний тег списка тегов
                        listTitles.Last().SetListPartsOfTitles(listFuturePartsOfTitles);

                        listFuturePartsOfTitles = new List<List<string>>();
                        List<string> anotherTempPartsOfTitle = new List<string>();

                        // цикл на добавление в список всех частей одного заголовка, не включая первый
                        for (int iPartOfTitle = 1; iPartOfTitle < listPartsOfTitles[iListPartsOfTitles].Count(); iPartOfTitle++)
                        {
                            anotherTempPartsOfTitle.Add(listPartsOfTitles[iListPartsOfTitles][iPartOfTitle]);
                        }

                        listFuturePartsOfTitles.Add(anotherTempPartsOfTitle);

                        compareTitle = listPartsOfTitles[iListPartsOfTitles][0];
                        Title newTitle = new Title();

                        newTitle.SetTitleString(compareTitle);
                        newTitle.SetListPartsOfTitles(listFuturePartsOfTitles);
                        // добавление нового тега в список тегов
                        listTitles.Add(newTitle);
                    }
                    // если часть заголовка совпадает
                    else
                    {
                        List<string> anotherTempPartsOfTitle = new List<string>();

                        // цикл на добавление в список всех частей одного заголовка, не включая первый
                        for (int iPartOfTitle = 1; iPartOfTitle < listPartsOfTitles[iListPartsOfTitles].Count(); iPartOfTitle++)
                        {
                            anotherTempPartsOfTitle.Add(listPartsOfTitles[iListPartsOfTitles][iPartOfTitle]);
                        }

                        listFuturePartsOfTitles.Add(anotherTempPartsOfTitle);

                        listTitles.Last().SetListPartsOfTitles(listFuturePartsOfTitles);
                    }
                }

                foreach (Title title in listTitles)
                {
                    title.CreateTitle();
                }
            }     
        }


        public void SetTitleString(string newTitleString)
        {
            titleString = newTitleString + "; ";
        }
        public void SetListPartsOfTitles(List<List<string>> newListPartsOfTitles)
        {
            listPartsOfTitles = newListPartsOfTitles;
        }


        public string GetTitleString()
        {
            return titleString;
        }
        public List<Title> GetListTitles()
        {
            return listTitles;
        }
    }
}
