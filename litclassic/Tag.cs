using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litclassic
{
    class Tag
    {
        private string name; // имя тега без скобок
        private string fullName; // полное имя тега (включая команды и параметры) без скобок
        private List<string> listCommandTypes = new List<string>();
        private List<string> listCommands = new List<string>();
        private int orderNumber; // число встреч тега
        private string content; // содержание тега
        private List<Tag> listTags = new List<Tag>(); // теги, содержащиеся в данном теге


        public void CreateTag(string inputString)
        {
            CreateTagValues(inputString);
            SearchAnotherTags();
        }


        public string GetContent()
        {
            return content;
        }
        public string GetName() // запрос на получение имени тега без скобок 
        {
            return name;
        }
        public string GetFullName()
        {
            return fullName;
        }
        public List<string> GetListCommandTypes()
        {
            return listCommandTypes;
        }
        public List<string> GetListCommands()
        {
            return listCommands;
        }
        public List<Tag> GetListTags()
        {
            return listTags;
        }
        public int GetOrderNumber()
        {
            return orderNumber;
        }


        public void SetContent(string newContent)
        {
            content = newContent;
        }
        public void SetOrderNumber(int newNumberOfMeetings)
        {
            orderNumber = newNumberOfMeetings;
        }


        // -- не используется --
        private void FindTags(string inputString)
        {
            content = inputString;

            while ((content.IndexOf('<') != -1) & (content.IndexOf('>') != -1))
            {
                Tag newTag = new Tag();

                newTag.FillTagValues(content);
                listTags.Add(newTag);

                if ((newTag.GetFullName()[0] == '!')|(newTag.GetFullName().Last() == '/'))
                {
                    content = content.Replace("<" + newTag.GetFullName() + ">", "");
                }
                else
                {
                    string newString = CarveStringBetweenTags("<" + newTag.GetFullName() + ">", "</" + newTag.GetName() + ">", content);
                    content = content.Replace("<" + newTag.GetFullName() + ">" + newString + "</" + newTag.GetName() + ">", "");

                    //newTag.CreateTag(newString);
                    newTag.FindTags(newString);
                }
            }
        }
        private void FillTagValues(string inputString)
        {
            //заполнение тэга
            content = inputString;

            if ((content.IndexOf('<') != -1) & (content.IndexOf('>') != -1))
            {
                name = CarveStringBetweenStrings("<", ">", content);
                fullName = name;

                // проверка, содержит ли имя команду
                if (name.IndexOf(' ') != -1)
                {
                    name = CarveStringBetweenStrings("<", " ", content);
                    int commandsCount = 0;

                    // подсчет количества команд в теге
                    for (int iFullName = 0; iFullName < fullName.Length - 1; iFullName++)
                    {
                        if ((fullName[iFullName] == '=') & (fullName[iFullName + 1] == '\"')) commandsCount++;
                    }

                    // если в теге больше 1 команды
                    if (commandsCount > 1)
                    {
                        string tempString = fullName;

                        for (int iCommandsCount = 0; iCommandsCount < commandsCount; iCommandsCount++)
                        {
                            listCommands.Add(CarveStringBetweenStrings("=\"", "\"", tempString));
                            listCommandTypes.Add(CarveStringBetweenStrings(" ", "\"", tempString));

                            tempString = tempString.Substring(tempString.IndexOf(listCommands.Last()));
                        }
                    }
                    // если в теге команда лишь одна
                    else if (commandsCount == 1)
                    {
                        listCommands.Add(CarveStringBetweenStrings("=\"", "\"", fullName));
                        listCommandTypes.Add(CarveStringBetweenStrings(" ", "\"", fullName));
                    }
                }
            }
        }
        // -- не используется --


        private void CreateTagValues(string coreTag) // установление имени и выявление, вложена ли в него команда 
        {
            //заполнение тэга
            content = coreTag;

            if ((content.IndexOf('<') != -1) & (content.IndexOf('>') != -1))
            {
                name = CarveStringBetweenStrings("<", ">", content);
                fullName = name;

                // проверка, содержит ли имя команду
                if (name.IndexOf(' ') != -1)
                {
                    name = CarveStringBetweenStrings("<", " ", content);
                    int commandsCount = 0;

                    // подсчет количества команд в теге
                    for (int iFullName = 0; iFullName < fullName.Length - 1; iFullName++)
                    {
                        if ((fullName[iFullName] == '=') & (fullName[iFullName + 1] == '\"')) commandsCount++;
                    }

                    // если в теге больше 1 команды
                    if (commandsCount > 1)
                    {
                        string tempString = fullName;

                        for (int iCommandsCount = 0; iCommandsCount < commandsCount; iCommandsCount++)
                        {
                            listCommands.Add(CarveStringBetweenStrings("=\"", "\"", tempString));

                            if (listCommandTypes.Count() > 0)
                            {
                                listCommandTypes.Add(CarveStringBetweenStrings("\" ", "=\"", tempString));
                            }
                            else
                            {
                                listCommandTypes.Add(CarveStringBetweenStrings(" ", "=\"", tempString));
                            }

                            tempString = tempString.Substring(tempString.IndexOf(listCommands.Last()));
                        }
                    }
                    // если в теге команда лишь одна
                    else if (commandsCount == 1)
                    {
                        listCommands.Add(CarveStringBetweenStrings("=\"", "\"", fullName));
                        listCommandTypes.Add(CarveStringBetweenStrings(" ", "=\"", fullName));
                    }
                }
            }

            // далее идет удаление из content первых встреченных только что установленных открывающего и закрывающего тегов

            // заменано полное название тега на короткое, а также убрана закрывающая кавычка ради того, чтобы закрывающий тег 
            // находился верно, когда по пути к нему будет стоять открывающий тег, имеющий команду
            string newTagNameOpen = '<' + name/*fullName + '>'*/;
            string newTagNameClose = "</" + name + '>';
            int indexNewTagNameOpen = content.IndexOf(newTagNameOpen);

            // условие добавлено 28.12 после обнаружения вылетов
            //if (indexNewTagNameOpen != -1)
            //{
                int indexNewTagNameClose = content.IndexOf(newTagNameClose, indexNewTagNameOpen);

                // если существует такой же открывающий тег между открывающим и закрывающим текущим тегом
                if ((content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1) != -1) & (content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1) < indexNewTagNameClose))
                {
                    int tempIndexNewTagNameOpen = content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1);

                    // подсчет количества открывающих тегов перед текущим закрывающим
                    while ((tempIndexNewTagNameOpen != -1) && (content.IndexOf(newTagNameOpen, tempIndexNewTagNameOpen) < indexNewTagNameClose))
                    {
                        indexNewTagNameClose = content.IndexOf(newTagNameClose, indexNewTagNameClose + 1);
                        tempIndexNewTagNameOpen = content.IndexOf(newTagNameOpen, tempIndexNewTagNameOpen + 1);
                    }
                }

                int contentNewTagLength = indexNewTagNameClose - indexNewTagNameOpen - fullName.Length - 2;
                content = content.Substring(indexNewTagNameOpen + fullName.Length + 2, contentNewTagLength);
            //}
        }
        private void SearchAnotherTags() // 
        {
            // "И" или "ИЛИ"?
            while ((content.IndexOf('<') != -1) & (content.IndexOf('>') != -1))
            {
                Tag newTag = new Tag();
                // имя нового тега
                string newTagName = content.Substring(content.IndexOf('<') + 1, content.IndexOf('>') - content.IndexOf('<') - 1);
                // имя без внутренней команды
                string newTagShortName = newTagName;

                // проверка, содержит ли имя команду
                if (newTagName.IndexOf(' ') != -1)
                {
                    newTagShortName = newTagName.Substring(0, newTagName.IndexOf(' '));
                }

                // проверка, содержит ли тег в конце /
                if (newTagName.Last() == '/')
                {
                    content = content.Replace(newTagName, newTagName.Replace("/", "/></" + newTagShortName));
                }

                // заменано полное название тега на короткое, а также убрана закрывающая кавычка ради того, чтобы закрывающий тег 
                // находился верно, когда по пути к нему будет стоять открывающий тег, имеющий команду
                string newTagNameOpen = '<' + newTagShortName/*newTagName + '>'*/;
                string newTagNameClose = "</" + newTagShortName + '>';
                int indexNewTagNameOpen = content.IndexOf(newTagNameOpen);
                int indexNewTagNameClose = content.IndexOf(newTagNameClose, indexNewTagNameOpen);

                // если существует такой же открывающий тег между открывающим и закрывающим текущим тегом
                if ((content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1) != -1) & (content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1) < indexNewTagNameClose))
                {
                    int tempIndexNewTagNameOpen = content.IndexOf(newTagNameOpen, indexNewTagNameOpen + 1);

                    // подсчёт количества открывающих тегов перед текущим закрывающим
                    while ((tempIndexNewTagNameOpen != -1) && (content.IndexOf(newTagNameOpen, tempIndexNewTagNameOpen) < indexNewTagNameClose))
                    {
                        indexNewTagNameClose = content.IndexOf(newTagNameClose, indexNewTagNameClose + 1);
                        tempIndexNewTagNameOpen = content.IndexOf(newTagNameOpen, tempIndexNewTagNameOpen + 1);
                    }
                }

                // было +4
                int contentNewTagLength = indexNewTagNameClose + newTagShortName.Length + 3 - indexNewTagNameOpen;
                // строка, содержащая новый тег
                string contentNewTag = content.Substring(indexNewTagNameOpen, contentNewTagLength);
                // убирает ВСЕ вхождения, а нужно лишь первое
                content = ReplaceFirst(contentNewTag, content);

                newTag.CreateTag(contentNewTag);
                // добавление тега в список тегов текущего тега
                listTags.Add(newTag);
            }
        }



        private string CarveStringBetweenStrings(string startString, string finishString, string inputString)
        {
            string outputString = "";
            // номер в inputString первого вхождения startString
            int startStringPosition = inputString.IndexOf(startString) + startString.Length;
            // временная строка, которая содержит всё из inputString, начиная с startPositionString
            string tempString = inputString.Substring(startStringPosition);
            outputString = tempString.Substring(0, tempString.IndexOf(finishString));

            return outputString;
        }
        private string CarveStringBetweenStrings(string startString, string finishString, string inputString, int startPosition)
        {
            string outputString = "";
            // удаление ненужной части inputString
            inputString = inputString.Substring(startPosition);
            // номер в inputString первого вхождения startString
            int startStringPosition = inputString.IndexOf(startString) + startString.Length;
            // временная строка, которая содержит всё из inputString, начиная с startPositionString
            string tempString = inputString.Substring(startStringPosition);
            outputString = tempString.Substring(0, tempString.IndexOf(finishString));

            return outputString;
        }
        private string CarveStringBetweenTags(string startTag, string finishTag, string inputString)
        {
            string outputString = "";

            int indexNewTagNameOpen = inputString.IndexOf(startTag);
            int indexNewTagNameClose = inputString.IndexOf(finishTag, indexNewTagNameOpen);

            // если существует такой же открывающий тег между открывающим и закрывающим текущим тегом
            if ((inputString.IndexOf(startTag, indexNewTagNameOpen + 1) != -1) & (inputString.IndexOf(startTag, indexNewTagNameOpen + 1) < indexNewTagNameClose))
            {
                int tempIndexNewTagNameOpen = inputString.IndexOf(startTag, indexNewTagNameOpen + 1);

                // подсчет количества открывающих тегов перед текущим закрывающим
                while ((tempIndexNewTagNameOpen != -1) && (inputString.IndexOf(startTag, tempIndexNewTagNameOpen) < indexNewTagNameClose))
                {
                    indexNewTagNameClose = inputString.IndexOf(finishTag, indexNewTagNameClose + 1);
                    tempIndexNewTagNameOpen = inputString.IndexOf(startTag, tempIndexNewTagNameOpen + 1);
                }
            }

            //int contentNewTagLength = indexNewTagNameClose + nameN.Length + 4 - indexNewTagNameOpen;
            // раньше вместо finishTag.Length стояло name.Length + 4
            int contentNewTagLength = indexNewTagNameClose + finishTag.Length - indexNewTagNameOpen;
            // строка, содержащая новый тег
            outputString = inputString.Substring(indexNewTagNameOpen, contentNewTagLength);
            outputString = outputString.Replace(startTag, "");
            outputString = outputString.Replace(finishTag, "");

            return outputString;
        }
        private string ReplaceFirst(string replacingString, string inputString)
        {
            string result = inputString.Substring(0, inputString.IndexOf(replacingString)) + inputString.Substring(inputString.IndexOf(replacingString) + replacingString.Length);

            return result;
        }
    }
}
