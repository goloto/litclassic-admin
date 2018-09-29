using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litclassic
{
    class Comprehension
    {
        private int connection = 1;
        private List<Line> listLines = new List<Line>();
        private List<int> listRecommendedConnections = new List<int>();


        public List<int> Run(List<Line> newListLines)
        {
            listLines = newListLines;

            FillListsForNulls();
            SetConnectionsForColons();
            SetConnectionsForQuestions();
            SetConnectionsForDirectSpeech();
            SetConnectionsForEndMarks();

            return listRecommendedConnections;
        }


        private void FillListsForNulls()
        {
            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++) 
            {
                // "0" обозначет, что связи нет
                listRecommendedConnections.Add(0);
            }
        }
        // создание списка списков связи строки, оканчивающейся двоеточием
        private void SetConnectionsForColons() 
        {
            // Нужна ли действительно эта связь? Ведь она неминуемо означает, что после двоеточия начнется диалог, но диалог ведь уже 
            // (захватывая и строку перед ним) обозначается у меня связью?
            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++) 
            {
                // проверка на выход переменной за пределы списка
                if (iListLines < listLines.Count() - 1)
                {
                    // здесь бывало метод GetLine() возвращал null
                    if ((listLines[iListLines].GetLine().Replace("\n\r", "") != null) & (listLines[iListLines].GetLine().Replace("\n\r", "").Count() != 0)) 
                    {
                        if ((listLines[iListLines].GetLine().Replace("\n\r", "").Last() == ':') | (listLines[iListLines].GetLine().Replace("\n\r", "").Last() == ':'))
                        //if ((listLines[iListLines].GetLine()[listLines[iListLines].GetLine().Length - 3] == ':') | (listLines[iListLines].GetLine().Last() == ':')) 
                        {
                            listRecommendedConnections[iListLines] = connection;
                            listRecommendedConnections[iListLines + 1] = connection;

                            // увеличение переменной связи
                            connection++;
                        }
                    }
                }
            }
        }
        // создание списка списков связи строки, оканчивающейся знаком вопроса
        private void SetConnectionsForQuestions() 
        {
            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
            {
                // проверка на выход переменной за пределы списка
                if (iListLines < listLines.Count() - 1)
                {
                    // здесь бывало метод GetLine() возвращал null
                    if ((listLines[iListLines].GetLine().Replace("\n\r", "") != null) & (listLines[iListLines].GetLine().Replace("\n\r", "").Count() != 0))
                    {
                        string temp = listLines[iListLines].GetLine().Replace("\n\r", "");

                        if (/*(listLines[iListLines].GetLine()[listLines[iListLines].GetLine().Length - 3] == '?') | */(temp.Last() == '?'))
                        {
                            listRecommendedConnections[iListLines] = connection;
                            listRecommendedConnections[iListLines + 1] = connection;

                            // увеличение переменной связи
                            connection++;
                        }
                    }
                }
            }
        }
        // создание списка списков связи строк, содержащих диалог
        private void SetConnectionsForDirectSpeech() 
        {
            for (int iListLines = 1; iListLines < listLines.Count(); iListLines++)
            {
                string line = listLines[iListLines].GetLine();

                if ((line[0] == '—') | (line[1] == '—'))
                {
                    // добавление связи с предыдущей строкой
                    // мне кажется, это может повлечь лишние связи
                    //listRecommendedConnections[iListLines - 1] = connection;

                    // цикл длится, пока в текущая строка начинается с дефиса
                    while ((listLines[iListLines].GetLine()[0] == '—') | (listLines[iListLines].GetLine()[1] == '—'))
                    {
                        listRecommendedConnections[iListLines] = connection;

                        iListLines++;
                    }

                    // увеличение переменной связи
                    connection++;
                }
            }
        }
        // создание списка списков связок строк, не имеющих в своем конце знак препинания
        private void SetConnectionsForEndMarks() 
        {
            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
            {
                // проверка на выход переменной за пределы списка
                if (iListLines < listLines.Count() - 1)
                {
                    // здесь бывало метод GetLine() возвращал null
                    if ((listLines[iListLines].GetLine().Replace("\n\r", "") != null) & (listLines[iListLines].GetLine().Replace("\n\r", "").Count() != 0))
                    {
                        //if (/*((listLines[iListLines].GetLine()[listLines[iListLines].GetLine().Length - 3] != '.') | */(listLines[iListLines].GetLine().Replace("\r\n", "").Last() != '.')
                        //    & /*((listLines[iListLines].GetLine()[listLines[iListLines].GetLine().Length - 3] != '?') | */(listLines[iListLines].GetLine().Replace("\r\n", "").Last() != '?')
                        //    & /*((listLines[iListLines].GetLine()[listLines[iListLines].GetLine().Length - 3] != '!') | */(listLines[iListLines].GetLine().Replace("\r\n", "").Last() != '!'))
                        string temp = listLines[iListLines].GetLine().Replace("\n\r", "");
                        //char n = temp.Last();
                        //bool f = false;
                        //if (n == '.') f = true;

                        if ((temp.Last() != '.') & (temp.Last() != '?') & (temp.Last() != '!'))
                        //if ((listLines[iListLines].GetLine().Replace("\n", "").Last() == ',') | (listLines[iListLines].GetLine().Replace("\n", "").Last() == '—')) 
                        {
                            listRecommendedConnections[iListLines] = connection;
                            listRecommendedConnections[iListLines + 1] = connection;

                            // увеличение переменной связи
                            connection++;
                        }
                    }
                }
            }

        }
    }
}
