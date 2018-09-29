using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litclassic
{
    class Coloring
    {
        private Outsourcing currentOutsourcing;
        private string[] colorTags;

        public void Initialize(Outsourcing outsourcing) // весь процесс "инспектора" будет здесь
        {
            currentOutsourcing = outsourcing;
            colorTags = new string[currentOutsourcing.GetListTreeInterruptTags().Count()];

            ColoringForCountTags();
            ColoringForTagsTree();
            ColoringForTreeTagsAverageTags();
            ColoringForParagraphCountAverage();
            ColoringForParagraphCount();
            ColoringForIncludedTagsPlus();
            ColoringParagraphWithoutSpace();
        }

        public string[] GetColorTags() // получение массива с цветами тегов
        {
            return colorTags;
        }


        // сначала нужно выполнить все окрашивания в зеленый, затем в красный
        // окрашивания в зеленый
        private void ColoringForCountTags() // окрашивание в зеленый тегов из списка общего количества конкретных тегов
        {
            int maxNumberOfMeetings = 0;

            for (int iListTags = 0; iListTags < currentOutsourcing.GetListTags().Count(); iListTags++)
            {
                if (currentOutsourcing.GetListTags()[iListTags].GetNumberOfMeetings() > maxNumberOfMeetings)
                    maxNumberOfMeetings = currentOutsourcing.GetListTags()[iListTags].GetNumberOfMeetings();
            }
            for (int iListTags = 0; iListTags < currentOutsourcing.GetListTags().Count(); iListTags++)  
                // цикл сделан для возможности, что будет несколько максимальных значений
            {
                if (currentOutsourcing.GetListTags()[iListTags].GetNumberOfMeetings() ==
                    maxNumberOfMeetings) ColoringTagGreen(currentOutsourcing.GetListTags()[iListTags].GetName());
            }

            EndingColoring();
        }
        private void ColoringForTagsTree() // окрашивание в зеленый тегов из списка дерева тегов, когда в них не включены никакие теги
        {
            for (int iListTagsTree = 0; iListTagsTree < currentOutsourcing.GetListCountEnclosedTags().Count(); iListTagsTree++)
            {
                if (currentOutsourcing.GetListCountEnclosedTags()[iListTagsTree] == 0) colorTags[iListTagsTree]="green";
            }

            EndingColoring();
        }
        private void ColoringForTreeTagsAverageTags() // окрашивание в зеленый тегов из списка среднего количества подтегов конкретного тега
        {
            for (int iListTreeTagsAverage = 0; iListTreeTagsAverage < currentOutsourcing.GetListAverageCountEnclosedTags().Count(); iListTreeTagsAverage++) 
                // цикл сделан для возможности, что будет несколько минимальных значений
            {
                if (currentOutsourcing.GetListAverageCountEnclosedTags()[iListTreeTagsAverage] == 
                    currentOutsourcing.GetListAverageCountEnclosedTags().Min()) ColoringTagGreen(currentOutsourcing.GetListTags()[iListTreeTagsAverage].GetName());
            }

            EndingColoring();
        }

        // окрашивания в красный
        private void ColoringForParagraphCountAverage() // окрашивание в красный тегов из списка среднего количества символов в конкретном теге
        {
            for (int iListParagraphCountAverage = 0; iListParagraphCountAverage < currentOutsourcing.GetListParagraphCountAverage().Count(); iListParagraphCountAverage++)
            // цикл сделан для возможности, что будет несколько минимальных значений
            {
                if (currentOutsourcing.GetListParagraphCountAverage()[iListParagraphCountAverage] ==
                    currentOutsourcing.GetListParagraphCountAverage().Min()) ColoringTagRed(currentOutsourcing.GetListTags()[iListParagraphCountAverage].GetName());
            }

            EndingColoring();
        }
        private void ColoringForParagraphCount() // окрашивание в красный тегов из списка последовательности количества символов в конкретном теге
        {
            for (int iListParagraphCount = 0; iListParagraphCount < currentOutsourcing.GetListParagraphCount().Count(); iListParagraphCount++) 
            {
                if (currentOutsourcing.GetListParagraphCount()[iListParagraphCount] < 10)
                    colorTags[iListParagraphCount]="red";
            }

            EndingColoring();
        }
        private void ColoringForIncludedTagsPlus() // окрашивание в красный тегов из улучшенного списка включенных тегов в конкретный тег
        {
            for (int iListTreeInterruptTags = 1; iListTreeInterruptTags < currentOutsourcing.GetListTreeInterruptTags().Count() - 1; iListTreeInterruptTags++) 
            {
                if (currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags] != currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags - 1])
                {
                    if (currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags] != currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags + 1])
                    colorTags[iListTreeInterruptTags] = "red";
                }                 
            }

            EndingColoring();
        }
        private void ColoringParagraphWithoutSpace() // окрашивание в красный тегов, в строчке которых нет пробела
        {
            for (int iListSequenceTags = 0; iListSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iListSequenceTags++)
            {
                // для сокращения вычислений поиск идет только среди тегов, уже окрашенных в зеленый
                if ((colorTags[iListSequenceTags] == "green") && (currentOutsourcing.GetTagsContent()[iListSequenceTags].IndexOf(' ') == -1)) colorTags[iListSequenceTags] = "red";
            }

            EndingColoring();
        }

        // также нужен список, который формировал бы "группы" тегов, которые могут быть связаны друг с другом по смыслу (например, стихотворные строки)
        // нужно заменить теги стихов на теги параграфов, но как-то сохранить то, что это - именно неделимые стихи,
        // т.к. начала коротких стихотворных строк входят в конфликт с определением их цвета
        // видимо, нужно оставлять и теги <poem> и <stanza>

        private void ColoringTagGreen(string tag) // окрашивание в зеленый всех встречающихся примеров конкретного тега в массиве тегов
        {
            for (int iSequenceTags = 0; iSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iSequenceTags++) 
            {
                if (currentOutsourcing.GetListSequenceTags()[iSequenceTags] == tag) colorTags[iSequenceTags] = "green";
            }
        }
        private void ColoringTagRed(string tag) // окрашивание в красный всех встречающихся примеров конкретного тега в массиве тегов
        {
            for (int iSequenceTags = 0; iSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iSequenceTags++)
            {
                if (currentOutsourcing.GetListSequenceTags()[iSequenceTags] == tag) colorTags[iSequenceTags] = "red";
            }
        }
        private void EndingColoring() // проверка каждого coloring-метода в конце на наличие одного из двух цветов в каждом элементе массива с тегами
        {
            for (int iColorTags = 0; iColorTags < colorTags.Count(); iColorTags++)
            {
                if ((colorTags[iColorTags] != "green") && (colorTags[iColorTags] != "red")) colorTags[iColorTags] = "red";
            }
        }


        private void LookForFirstParagraph()
        {
            string candidateForFirstParagraph;
            bool firstFound = false;
            while (firstFound==false)
            {
                int indexParagraph = 0;
                if (currentOutsourcing.GetListCountEnclosedTags()[indexParagraph]==0)
                {

                }
                else
                {
                    //if ()
                }
                candidateForFirstParagraph = currentOutsourcing.GetTagsContent()[indexParagraph];
                indexParagraph++;
            }
            
        }

       
        // присвоение цветов тегам будет идти слева-направо, согласно спискам в запущенной программе
        // в конце нужно сделать такой алгоритм, который бы присваивал тегу красный цвет, если впереди или сзади номер в списке incl.tags+ не соответсвует проверяемому тегу
        // в итоге, должны получиться лишь красные и зеленые теги
        // в цитату будут браться все зеленые, но с условием, что из списка incl.tags+ теги с одинаковыми значениями будут браться всей "пачкой"
        //
        //
        // нужно придумывать общий вид, модель того, как "инспектор" будет брать данные из класса книги,
        // будет их обрабатывать, а затем куда-то складывать
        // + все это обязательно должно быть масштабируемым и изменяемым (ну или не все, а только обработка)
    }
}
