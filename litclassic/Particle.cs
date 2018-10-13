using System;
using System.Collections.Generic;
using System.Linq;

namespace litclassic
{
    class Particle
    {
        private List<Line> listLines = new List<Line>(); // полученный из-вне список всех строк
        private List<Line> lines; // список строк формируемых частиц
        private List<List<Line>> listLinesInRecommendConnection; // список списка строк формируемых частиц, собранных с учетом рекомендуемой связи
        private List<List<List<Line>>> listFutureParticles; // список списка списков, по которому затем сформируется итоговый список готовых частиц
        private int globalIndex; // индекс, используемый при формировании частиц
        private int indexOfLastLineWithTwoStrongConnection = 0;
        private List<string> listParticles = new List<string>();
        private List<string> listParticlesTitles = new List<string>();
        private List<Title> listTitles = new List<Title>();
        private List<int> listIndexLastParticleLines = new List<int>();
        private int sumStrongConnections;
        private int linesEntryPercent = 0;
        private List<int> listSumStrongConnections = new List<int>();
        private List<int> listThemeTypes = new List<int>();


        public void BuildParticles(List<Line> newListLines)
        {
            listLines = newListLines;
            listFutureParticles = new List<List<List<Line>>>();
            globalIndex = 0;

            for (; globalIndex < listLines.Count(); globalIndex++)
            {
                BuildListParticles();
                CheckParticles(listLinesInRecommendConnection/*, globalIndex*/);
            }

            FillListIndexLastParticleLines();
            CheckThemeTypesInListLines();
            FillListParticles();
            FillListParticlesTitles();
            CalculateLinesEntry();
            FillListSumStrongConnections();
        }


        private void BuildListParticles()
        {
            // условие для связи, длина фрагмента которой позволят его соединять с другими
            if (listLines[globalIndex].GetExplanationStrongConnection() == 0)
            {
                BuildZeroStrongConnectionParticle();
            }
            // условие для связи, длина фрагмента которой позволят его брать только одного и целиком
            else if (listLines[globalIndex].GetExplanationStrongConnection() == 1)
            {
                BuildOneStrongConnectionParticle();
            }
            // условие для связи, длина фрагмента которой позволят создавать частицы лишь внутри него
            else if (listLines[globalIndex].GetExplanationStrongConnection() == 2)
            {
                if (globalIndex != 0) BuildTwoStrongConnectionParticle(globalIndex - 1);
                else BuildTwoStrongConnectionParticle(globalIndex);
                //BuildTwoStrongConnectionPartical(globalIndex);
            }
        }
        private void BuildZeroStrongConnectionParticle(/*int index*/)
        {
            //int currentLength = 0;
            lines = new List<Line>();
            listLinesInRecommendConnection = new List<List<Line>>();
            int currentStrongConnection = listLines[globalIndex].GetStrongConnection();
            sumStrongConnections = 0;
            int i = globalIndex;

            // решение всех моих проблем:
            // сделано с целью того, чтобы препятствовать лишнему увеличению индекса в BuildListParticals после создания частицы
            //if (listFutureParticals.Count() > listFutureParticalsCount)
            //{
            //    listFutureParticalsCount = listFutureParticals.Count();
            //    globalIndex--;
            //}

            if ((globalIndex != 0) && (listLines[globalIndex - 1].GetStrongConnection() == currentStrongConnection))
            {
                globalIndex--;
            }

            // условие о сохранении одной строгой связи, о не выходе индекса за пределы массива и того, чтобы объяснение строгой связи было равно нулю
            while ((globalIndex < listLines.Count()) && (listLines[globalIndex].GetStrongConnection() == currentStrongConnection))
            {
                lines.Add(listLines[globalIndex]);

                sumStrongConnections += listLines[globalIndex].GetStrongConnection();
                globalIndex++;
            }

            listLinesInRecommendConnection.Add(lines);
        }
        private void BuildOneStrongConnectionParticle()
        {
            lines = new List<Line>();
            listLinesInRecommendConnection = new List<List<Line>>();
            int currentStrongConnection = listLines[globalIndex].GetStrongConnection();
            sumStrongConnections = 0;
            //int temp = 0;
            //int i = globalIndex;

            if ((globalIndex != 0) && (listLines[globalIndex - 1].GetStrongConnection() == currentStrongConnection)) 
            {
                globalIndex--;
            }

            // условие о сохранении одной строгой связи, о не выходе индекса за пределы массива и того, чтобы объяснение строгой связи было равно единице
            while ((globalIndex < listLines.Count()) && (listLines[globalIndex].GetStrongConnection() == currentStrongConnection))
            {
                lines.Add(listLines[globalIndex]);

                sumStrongConnections += listLines[globalIndex].GetStrongConnection();
                globalIndex++;
                //temp = 1;
            }

            // иначе индекс будет лишний раз увеличиваться в цикле в BuildListParticals
            //if (temp == 1) globalIndex--;

            //if (lines.Count() != 0) globalIndex = lines.Last().GetIndex();

            indexOfLastLineWithTwoStrongConnection = globalIndex;

            listLinesInRecommendConnection.Add(lines);
        }
        private void BuildTwoStrongConnectionParticle(int index)
        {
            //int currentLength = 0;
            lines = new List<Line>();
            listLinesInRecommendConnection = new List<List<Line>>();
            int currentStrongConnection = listLines[index].GetStrongConnection();
            sumStrongConnections = 0;

            if (listLines[index].GetExplanationRecommendConnection() == 0)
            {
                // добавление строки с нейтральной связью
                lines.Add(listLines[index]);

                //currentLength += listLines[index].GetLine().Length;
                sumStrongConnections += listLines[index].GetStrongConnection();
                indexOfLastLineWithTwoStrongConnection = index + 1;
            }
            else if (listLines[index].GetExplanationRecommendConnection() == 1)
            {
                // проверка на то, что следующая связь равна "2", и индекс не выходит за рамки списка
                if ((index + 1 < listLines.Count() - 1) && (listLines[index + 1].GetExplanationRecommendConnection() == 2) & (listLines[index + 1].GetStrongConnection() == currentStrongConnection))
                {
                    // добавление строки с связью "1"
                    lines.Add(listLines[index]);

                    //currentLength += listLines[index].GetLine().Length;
                    sumStrongConnections += listLines[/*globalndex*/index].GetStrongConnection();
                    // иначе цикл while сразу же завершится
                    index++;

                    // добавление строк со связью "2", пока эта связь не прекратится
                    while ((index < listLines.Count()) && (listLines[index].GetExplanationRecommendConnection() == 2) & (listLines[index].GetStrongConnection() == currentStrongConnection))
                    {
                        lines.Add(listLines[index]);

                        //currentLength += listLines[index].GetLine().Length;
                        sumStrongConnections += listLines[/*globalndex*/index].GetStrongConnection();
                        index++;
                    }

                    indexOfLastLineWithTwoStrongConnection = index - 1;
                    //index--;
                }
            }

            listLinesInRecommendConnection.Add(lines);
        }
        private void CheckParticles(List<List<Line>> inputListLists/*, int index*/)
        {
            if (inputListLists.Last().Count() != 0)
            {
                if (inputListLists.Last().Last().GetExplanationStrongConnection() == 0)
                {
                    CheckZeroStrongConnectionParticle(inputListLists);
                }
                else if (inputListLists.Last().Last().GetExplanationStrongConnection() == 1)
                {
                    CheckOneStrongConnectionParticle(inputListLists);
                }
                else if (inputListLists.Last().Last().GetExplanationStrongConnection() == 2)
                {
                    CheckTwoStrongConnectionParticle(inputListLists/*, index*/);
                }
            }
        }
        // в каждой проверке хранится своя копия
        private void CheckZeroStrongConnectionParticle(List<List<Line>> inputListLists)
        {
            // только 0 строгие связи
            // стихи добавлять, если с ними нет связи
            // разделять условия на необходимые для успешной компиляции и необходиые для создания частиц

            // стих должен проверятся на длину (минимум 300) и идти дальше, если следующая строка тоже стих и не имеет второй строго связи

            // индекс последнего элемента
            int currentIndex = inputListLists.Last().Last().GetIndex();

            if ((CountParticleLength(inputListLists) > 1900) & (CountParticleLength(inputListLists) < 3900)) 
            {
                if (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                {
                    // текущая строка не последняя в книге
                    if (currentIndex + 1 < listLines.Count())
                    {
                        // если строка не последняя в книге, то следует проверить, является ли следующая связанной с ней
                        if (listLines[currentIndex + 1].GetExplanationRecommendConnection() != 2)
                            listFutureParticles.Add(inputListLists);
                    }
                    // текущая строка последняя в книге, поэтому текущий список становится частицей без каких-либо условий
                    else listFutureParticles.Add(inputListLists);
                }
                //if ((globalIndex + 1 < listLines.Count())
                //    && (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                //    //& (inputListLists.Last().Last().GetExplanationRecommendConnection() != 2))
                //    & (listLines[globalIndex + 1].GetExplanationRecommendConnection() != 2))
                //{
                //    listFutureParticals.Add(inputListLists);
                //}
                else if ((currentIndex + 1 < listLines.Count()) && (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0)) 
                {
                    globalIndex = currentIndex + 1;

                    BuildZeroStrongConnectionParticle();

                    foreach (List<Line> listLines in listLinesInRecommendConnection)
                    {
                        inputListLists.Add(listLines);
                    }

                    CheckParticles(inputListLists);
                }
            }
            else if (CountParticleLength(inputListLists) < 1900)
            {
                // если верно, значит это стих
                if (sumStrongConnections < -10)
                {
                    // необходимое для создания частицы условие
                    // сделано для того, чтобы явно не окончившаяся строфа продолжилась и далее
                    if (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                    {
                        // если следующий фрагмент - стих, и имеет "0" строгую связь
                        if ((currentIndex + 1 < listLines.Count())
                            && (listLines[currentIndex + 1].GetStrongConnection() < 0)
                            & (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0))
                        {
                            globalIndex = currentIndex + 1;

                            BuildZeroStrongConnectionParticle();

                            foreach (List<Line> listLine in listLinesInRecommendConnection)
                            {
                                inputListLists.Add(listLine);
                            }

                            // есть ли разница, какой индекс ставить?
                            CheckParticles(inputListLists);
                        }
                        else if ((currentIndex + 1 < listLines.Count())
                            && (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0))
                        {
                            globalIndex = currentIndex + 1;

                            BuildZeroStrongConnectionParticle();

                            foreach (List<Line> listLine in listLinesInRecommendConnection)
                            {
                                inputListLists.Add(listLine);
                            }

                            CheckParticles(inputListLists);
                        }
                        else listFutureParticles.Add(inputListLists);
                    }
                    // не знаю, как будет с этим условием
                    else if ((currentIndex + 1 < listLines.Count())
                        && (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0))
                    {
                        globalIndex = currentIndex + 1;

                        BuildZeroStrongConnectionParticle();

                        foreach (List<Line> listLine in listLinesInRecommendConnection)
                        {
                            inputListLists.Add(listLine);
                        }

                        // есть ли разница, какой индекс ставить?
                        CheckParticles(inputListLists);
                    }
                }
                // не стих
                else
                {
                    // необходимое для создания частицы условие
                    if ((currentIndex + 1 < listLines.Count())
                    && (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0))
                    {
                        globalIndex = currentIndex + 1;

                        BuildZeroStrongConnectionParticle();

                        foreach (List<Line> listLine in listLinesInRecommendConnection)
                        {
                            inputListLists.Add(listLine);
                        }

                        CheckParticles(inputListLists);
                    }
                }
            }
        }
        private void CheckOneStrongConnectionParticle(List<List<Line>> inputListLists)
        {
            int currentIndex = inputListLists.Last().Last().GetIndex();

            if (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
            {
                // текущая строка не последняя в книге
                if (currentIndex + 1 < listLines.Count())
                {
                    // если строка не последняя в книге, то следует проверить, является ли следующая связанной с ней
                    if (listLines[currentIndex + 1].GetExplanationRecommendConnection() != 2)
                        listFutureParticles.Add(inputListLists);
                }
                // текущая строка последняя в книге, поэтому текущий список становится частицей без каких-либо условий
                else listFutureParticles.Add(inputListLists);
            }
            //if ((globalIndex + 1 < listLines.Count())
            //    && (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
            //    //& (inputListLists.Last().Last().GetExplanationRecommendConnection() != 2))
            //    & (listLines[globalIndex + 1].GetExplanationRecommendConnection() != 2))
            //{
            //    listFutureParticals.Add(inputListLists);
            //}
            else if ((currentIndex + 1 < listLines.Count()) && (listLines[currentIndex + 1].GetExplanationStrongConnection() == 0)) 
            {
                BuildZeroStrongConnectionParticle(/*index + 1*/);

                foreach (List<Line> listLines in listLinesInRecommendConnection)
                {
                    inputListLists.Add(listLines);
                }

                CheckParticles(inputListLists);
            }
        }
        private void CheckTwoStrongConnectionParticle(List<List<Line>> inputListLists/*, int index*/)
        {
            // только в рамках текущей строгой связи
            // все рекомендуемые, пока длина не будет выше 1900 и ниже 3900
            // стихи добавлять, если с ними нет связи
            // разделять условия на необходимые для успешной компиляции и необходиые для создания частиц

            int currentIndex = inputListLists.Last().Last().GetIndex();

            if ((CountParticleLength(inputListLists) > 1900) 
                & (CountParticleLength(inputListLists) < 3900))
            {
                if (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                {
                    // текущая строка не последняя в книге
                    if (currentIndex + 1 < listLines.Count())
                    {
                        // если строка не последняя в книге, то следует проверить, является ли следующая связанной с ней
                        if (listLines[currentIndex + 1].GetExplanationRecommendConnection() != 2)
                            listFutureParticles.Add(inputListLists);
                    }
                    // текущая строка последняя в книге, поэтому текущий список становится частицей без каких-либо условий
                    else listFutureParticles.Add(inputListLists);
                }
                //if ((indexOfLastLineWithTwoStrongConnection + 1 < listLines.Count())
                //    && (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                //    //& (inputListLists.Last().Last().GetExplanationRecommendConnection() != 2)) 
                //    & (listLines[indexOfLastLineWithTwoStrongConnection +1].GetExplanationRecommendConnection() != 2))
                //{
                //    listFutureParticals.Add(inputListLists);
                //}
                else if ((listLines[currentIndex + 1].GetExplanationStrongConnection() == 2) 
                    & (listLines[currentIndex + 1].GetStrongConnection() == listLines[currentIndex].GetStrongConnection()))
                {
                    BuildTwoStrongConnectionParticle(currentIndex + 1);

                    foreach (List<Line> listLines in listLinesInRecommendConnection)
                    {
                        inputListLists.Add(listLines);
                    }

                    CheckParticles(inputListLists);
                }

            }
            else if (CountParticleLength(inputListLists) < 1900)
            {
                // необходимое для успешной компиляции условие
                if (currentIndex + 1 < listLines.Count())
                {
                    // если верно, значит это стих
                    if (sumStrongConnections < -10)
                    {
                        // необходимое для создания частицы условие
                        if (inputListLists.Last().Last().GetExplanationRecommendConnection() != 1)
                        {
                            // если следующий фрагмент - стих, и имет "0" строгую связь
                            if ((listLines[currentIndex + 1].GetStrongConnection() < 0) 
                                & (listLines[currentIndex + 1].GetExplanationStrongConnection() == 2)
                                & (listLines[currentIndex + 1].GetStrongConnection() == listLines[currentIndex].GetStrongConnection()))
                            {
                                BuildTwoStrongConnectionParticle(currentIndex + 1);

                                foreach (List<Line> listLine in listLinesInRecommendConnection)
                                {
                                    inputListLists.Add(listLine);
                                }

                                // есть ли разница, какой индекс ставить?
                                CheckParticles(inputListLists);
                            }
                            else listFutureParticles.Add(inputListLists);
                        }
                        // не знаю, как будет с этим условием 
                        else if ((listLines[currentIndex + 1].GetExplanationStrongConnection() == 2)
                            & (listLines[currentIndex + 1].GetStrongConnection() == listLines[currentIndex].GetStrongConnection()))
                        {
                            BuildTwoStrongConnectionParticle(currentIndex + 1);

                            foreach (List<Line> listLines in listLinesInRecommendConnection)
                            {
                                inputListLists.Add(listLines);
                            }

                            CheckParticles(inputListLists);
                        }

                    }
                    // не стих
                    else
                    {
                        // необходимое для создания частицы условие
                        if ((listLines[currentIndex + 1].GetExplanationStrongConnection() == 2) 
                            & (listLines[currentIndex + 1].GetStrongConnection() == listLines[currentIndex].GetStrongConnection()))
                        {
                            BuildTwoStrongConnectionParticle(currentIndex + 1);

                            foreach (List<Line> listLines in listLinesInRecommendConnection)
                            {
                                inputListLists.Add(listLines);
                            }

                            CheckParticles(inputListLists);
                        }
                    }
                }
            }
        }
        private void CheckParticlesOld(List<List<Line>> inputListLists)
        {
            List<List<Line>> tempListListsLines = new List<List<Line>>();

            foreach (List<Line> list in inputListLists)
            {
                List<Line> tempListLines = new List<Line>();

                foreach (Line line in list)
                {
                    Line tempLine = line;

                    tempListLines.Add(tempLine);
                }

                tempListListsLines.Add(tempListLines);
            }

            if ((tempListListsLines.Last().Count() != 0) && (tempListListsLines.Last().Last().GetExplanationStrongConnection() != 1))
            {
                // если длина меньше 1200
                if (CountParticleLength(tempListListsLines) < 1200)
                {
                    // если верно, значит это стих
                    if (sumStrongConnections < -10)
                    {
                        if (/*(tempListListsLines.Last().Last().GetExplanationRecommendConnection() != 1) & */(tempListListsLines.Last().Last().GetExplanationStrongConnection() == 0))
                        {
                            // не выход индекса за пределы списка
                            if (globalIndex + 1 < listLines.Count())
                            {
                                // следующее объяснение строгой связи должно быть только нулевым
                                if (listLines[globalIndex + 1].GetExplanationStrongConnection() == 0)
                                {
                                    globalIndex++;
                                    BuildZeroStrongConnectionParticle();

                                    foreach (List<Line> newList in listLinesInRecommendConnection)
                                    {
                                        List<Line> tempListLines = new List<Line>();

                                        foreach (Line line in newList)
                                        {
                                            tempListLines.Add(line);
                                        }

                                        tempListListsLines.Add(tempListLines);
                                    }

                                    CheckParticles(tempListListsLines/*, globalIndex*/);
                                }
                            }
                        }
                        else listFutureParticles.Add(tempListListsLines);
                    }
                    // это не стих
                    else
                    {
                        // не выход индекса за пределы списка
                        if ((globalIndex + 1 < listLines.Count()) & (tempListListsLines.Last().Count() != 0))
                        {
                            if ((tempListListsLines.Last().Last().GetExplanationRecommendConnection() != 2)
                                & (tempListListsLines.Last().Last().GetExplanationStrongConnection() != 2))
                            {
                                // следующее объяснение строгой связи должно быть только нулевым
                                if (listLines[globalIndex + 1].GetExplanationStrongConnection() == 0)
                                {
                                    globalIndex++;
                                    BuildZeroStrongConnectionParticle();

                                    foreach (List<Line> newList in listLinesInRecommendConnection)
                                    {
                                        List<Line> tempListLines = new List<Line>();

                                        foreach (Line line in newList)
                                        {
                                            tempListLines.Add(line);
                                        }

                                        tempListListsLines.Add(tempListLines);
                                    }

                                    CheckParticles(tempListListsLines/*, globalIndex*/);
                                }
                            }
                        }
                    }
                }
                // если длина меньше 3600
                else if (CountParticleLength(tempListListsLines) < 3600)
                {
                    if ((tempListListsLines.Last().Count() != 0) &&
                        (tempListListsLines.Last().Last().GetExplanationRecommendConnection() != 1))
                        listFutureParticles.Add(tempListListsLines);
                }
            }
            // если фрагмент был с объяснением строгой связи, равным "1"
            else if ((tempListListsLines.Last().Count() != 0) && (tempListListsLines.Last().Last().GetExplanationStrongConnection() == 1))
            {
                listFutureParticles.Add(tempListListsLines);
            }
        }


        private int CountParticleLength(List<List<Line>> listLines)
        {
            int length = 0;

            foreach(List<Line> list in listLines)
            {
                foreach(Line line in list)
                {
                    length += line.GetLine().Length;
                }
            }

            return length;
        }
        private void FillListIndexLastParticleLines()
        {
            for (int i = 0; i < listFutureParticles.Count(); i++) 
            {
                if (listFutureParticles[i].Last().Count() == 0)
                {
                    listIndexLastParticleLines.Add(listFutureParticles[i][listFutureParticles[i].Count() - 2].Last().GetIndex());
                }
                else
                {
                    listIndexLastParticleLines.Add(listFutureParticles[i].Last().Last().GetIndex());
                }

            }
        }
        private void CheckThemeTypesInListLines()
        {
            // длинна списка будущих "частиц"
            int listFutureParticlesCount = listFutureParticles.Count();

            for (int iListFutureParticles = 0; iListFutureParticles < listFutureParticlesCount; iListFutureParticles++) 
            {
                // изменился ли тип темы
                bool themeChange = false;
                // тип темы
                int theme = listFutureParticles[iListFutureParticles][0][0].GetThemeType();

                foreach (List<Line> listLines in listFutureParticles[iListFutureParticles])
                {
                    foreach (Line line in listLines)
                    {
                        // изменилась тема или её тип = "99"
                        if ((line.GetThemeType() != theme) || (line.GetThemeType() == 99)) themeChange = true;
                    }
                }

                // если изменилась
                if (themeChange)
                {
                    // "частица" исключается из списка будущих "частиц"
                    listFutureParticles.RemoveAt(listFutureParticles.IndexOf(listFutureParticles[iListFutureParticles]));

                    // правятся индексы в списке
                    iListFutureParticles--;
                    listFutureParticlesCount--;
                }
                // если не изменялась
                else
                {
                    // тип темы добавляется в список
                    listThemeTypes.Add(theme);
                }
            }
        }
        private void FillListParticles()
        {
            foreach (List<List<Line>> listListLines in listFutureParticles)
            {
                string particleLine = "";
                int strongConnectionPreviousLine = 0;

                foreach (List<Line> listLines in listListLines)
                {
                    if (((listLines[0].GetStrongConnection() < 0) & (strongConnectionPreviousLine < 0)) 
                        & (listLines[0].GetStrongConnection() != strongConnectionPreviousLine)) particleLine += "...\n\r";

                    foreach (Line line in listLines)
                    {
                        particleLine += line.GetLine();
                        strongConnectionPreviousLine = line.GetStrongConnection();
                    }                  
                }

                listParticles.Add(particleLine);
            }
        }
        private void FillListParticlesTitles()
        {
            foreach (List<List<Line>> listListsLines in listFutureParticles)
            {
                // -- формирование списков частей заголовка строк --
                string particleTitle = "";
                List<string> listFinalTitles = new List<string>();
                // список списка отдельных частей заголовков
                List<List<string>> listPartsOfTitles = new List<List<string>>();
                // длинный полный список со всеми заголовками в упорядоченном порядке
                //List<string> listTitles = new List<string>();

                foreach (List<Line> listLines in listListsLines)
                {
                    foreach (Line line in listLines)
                    {
                        string[] title;
                        title = line.GetTitleOld().Split(';');
                        List<string> listStringTitles = new List<string>();

                        // убирает лишний последний элемент массива, который неминуемо возникает пустым, т.к. заголовок оканчивается на ";"
                        Array.Resize(ref title, title.Length - 1);

                        // переделка массива в список
                        foreach (string titleString in title)
                        {
                            listStringTitles.Add(titleString);
                        }

                        listPartsOfTitles.Add(listStringTitles);
                    }

                }
                // -- формирование списков частей заголовка строк --



                // -- формирование первой версии заголовка --
                //string compareTitle = listPartsOfTitles[0][0];
                //int previousCompareIndex = compareTitleIndex;
                Title firstTitle = new Title();
                //List<string> tempPartsOfTitle = new List<string>();

                //// цикл на добавление в список всех частей одного заголовка, не включая первый
                //for (int iPartOfTitle = 1; iPartOfTitle < listPartsOfTitles[0].Count(); iPartOfTitle++)
                //{
                //    tempPartsOfTitle.Add(listPartsOfTitles[0][iPartOfTitle]);
                //}

                //listFuturePartsOfTitles.Add(tempPartsOfTitle);
                listTitles.Add(firstTitle);
                listTitles.Last().SetListPartsOfTitles(listPartsOfTitles);
                //firstTitle.SetTitleString(compareTitle);
                firstTitle.CreateTitle();

                // создание строки заголовка
                particleTitle = BuildParticlesTitle(firstTitle);
                // -- формирование первой версии заголовка --



                // -- формирование окончательной версии заголовка --
                string[] largePartsOfTitle;
                largePartsOfTitle = particleTitle.Split(';');

                // убирает лишний последний элемент массива, который неминуемо возникает пустым, т.к. заголовок оканчивается на ";"
                Array.Resize(ref largePartsOfTitle, largePartsOfTitle.Length - 1);

                for (int iLargePartsOfTitle = 0; iLargePartsOfTitle < largePartsOfTitle.Count(); iLargePartsOfTitle++)
                {
                    string[] componentOfTitle;
                    componentOfTitle = largePartsOfTitle[iLargePartsOfTitle].Split(' ');

                    if (componentOfTitle.Count() > 2)
                    {
                        if ((componentOfTitle[2] == "раздел") | (componentOfTitle[2] == "стих") | (componentOfTitle[2] == "строфа")
                        | (componentOfTitle[2] == "абзац") | (componentOfTitle[2] == "эпиграф") | (componentOfTitle[2] == "аннотация") | (componentOfTitle[2] == "цитата"))
                        {
                            // номер первой однотипной части заголовка
                            string firstIndexComponent = componentOfTitle[1];
                            // номер последней однотипной части заголовка
                            string lastIndexComponent = componentOfTitle[1];
                            // однотипная часть заголовка, т.е. "стих", "раздел" и т.п.
                            string component = componentOfTitle[2];
                            // строка заголовков, которая затем будет заменяться в общем заголовке на объединённый
                            string partForReplace = firstIndexComponent + ' ' + component + "; ";

                            iLargePartsOfTitle++;


                            while (((iLargePartsOfTitle < largePartsOfTitle.Count()) && (largePartsOfTitle[iLargePartsOfTitle].Split(' ').Count() > 2)) 
                                && (largePartsOfTitle[iLargePartsOfTitle].Split(' ')[2] == component)) 
                            {
                                lastIndexComponent = largePartsOfTitle[iLargePartsOfTitle].Split(' ')[1];
                                partForReplace += largePartsOfTitle[iLargePartsOfTitle].Split(' ')[1] + ' ' + largePartsOfTitle[iLargePartsOfTitle].Split(' ')[2] + "; ";

                                iLargePartsOfTitle++;
                            }

                            // если однотипная часть заголовка лишь одна, т.е. объединять нечего
                            if (firstIndexComponent != lastIndexComponent)
                            {
                                particleTitle = ReplaceFirst(particleTitle, partForReplace, firstIndexComponent + '-' + lastIndexComponent + ' ' + component + "; ");
                            }

                            iLargePartsOfTitle--;
                        }
                    }                
                }

                //particalTitle = particalTitle.Replace("; ", ";\n");
                // -- формирование окончательной версии заголовка --



                // добавление строки заголовка в общий список заголовков
                listParticlesTitles.Add(particleTitle);
            }
        }
        private void CalculateLinesEntry()
        {
            List<bool> listLinesEntry = new List<bool>();

            for (int iListLines = 0; iListLines < listLines.Count(); iListLines++)
            {
                listLinesEntry.Add(false);
            }

            foreach (List<List<Line>> listListLine in listFutureParticles)
            {
                foreach (List<Line> listLine in listListLine)
                {
                    foreach (Line line in listLine)
                    {
                        listLinesEntry[line.GetIndex()] = true;
                    }
                }
            }

            foreach (bool entry in listLinesEntry)
            {
                if (entry == true) linesEntryPercent++; 
            }

            double doubleLinesEntryPercent = Convert.ToDouble(linesEntryPercent) / Convert.ToDouble(listLines.Count()) * 100;
            linesEntryPercent = Convert.ToInt32(doubleLinesEntryPercent);
        }
        private string BuildParticlesTitle(Title title)
        {
            string titleString = "";
            titleString += title.GetTitleString();

            //if (title.GetListTitles().Count() != 0)
            //{
                foreach (Title elementTitle in title.GetListTitles())
                {
                    titleString += BuildParticlesTitle(elementTitle);
                }
            //}

            return titleString;
        }
        // просто скопировал со StacksOverFlow
        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);

            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        private void FillListSumStrongConnections()
        {
            foreach (List<List<Line>> listListLines in listFutureParticles)
            {
                int currentSum = 0;

                foreach (List<Line> listLines in listListLines)
                {
                    foreach (Line line in listLines)
                    {
                        currentSum += line.GetStrongConnection();
                    }
                }

                listSumStrongConnections.Add(currentSum);
            }
        }


        public List<string> GetListParticles()
        {
            return listParticles;
        }
        public List<string> GetListParticlesTitles()
        {
            return listParticlesTitles;
        }
        public List<int> GetListThemeTypes()
        {
            return listThemeTypes;
        }
        public List<int> GetListIndexLastParticlesLines()
        {
            return listIndexLastParticleLines;
        }
        public int GetLinesEntryPercent()
        {
            return linesEntryPercent;
        }
        public List<int> GetListSumStrongConnections()
        {
            return listSumStrongConnections;
        }
    }
}
