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
        private bool[] colorTags;

        // весь процесс "инспектора" будет здесь
        public void Initialize(Outsourcing outsourcing) 
        {
            currentOutsourcing = outsourcing;
            colorTags = new bool[currentOutsourcing.GetListTreeInterruptTags().Count()];

            ColoringForCountTags();
            ColoringForTagsTree();
            ColoringForTreeTagsAverageTags();
            ColoringForParagraphCountAverage();
            ColoringForParagraphCount();
            ColoringForIncludedTagsPlus();
            ColoringParagraphWithoutSpace();
        }

        // получение массива с цветами тегов
        public bool[] GetColorTags() 
        {
            return colorTags;
        }


        // сначала нужно выполнить все окрашивания в зеленый, затем в красный


        // --- окрашивания в зеленый ---
        // окрашивание в зеленый тегов из списка общего количества конкретных тегов
        private void ColoringForCountTags() 
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
        // окрашивание в зеленый тегов из списка дерева тегов, когда в них не включены никакие теги
        private void ColoringForTagsTree() 
        {
            for (int iListTagsTree = 0; iListTagsTree < currentOutsourcing.GetListCountEnclosedTags().Count(); iListTagsTree++)
            {
                if (currentOutsourcing.GetListCountEnclosedTags()[iListTagsTree] == 0) colorTags[iListTagsTree] = true;
            }

            EndingColoring();
        }
        // окрашивание в зеленый тегов из списка среднего количества подтегов конкретного тега
        private void ColoringForTreeTagsAverageTags() 
        {
            for (int iListTreeTagsAverage = 0; iListTreeTagsAverage < currentOutsourcing.GetListAverageCountEnclosedTags().Count(); iListTreeTagsAverage++) 
                // цикл сделан для возможности, что будет несколько минимальных значений
            {
                if (currentOutsourcing.GetListAverageCountEnclosedTags()[iListTreeTagsAverage] == 
                    currentOutsourcing.GetListAverageCountEnclosedTags().Min()) ColoringTagGreen(currentOutsourcing.GetListTags()[iListTreeTagsAverage].GetName());
            }

            EndingColoring();
        }

        // --- окрашивания в красный ---
        // окрашивание в красный тегов из списка среднего количества символов в конкретном теге
        private void ColoringForParagraphCountAverage() 
        {
            for (int iListParagraphCountAverage = 0; iListParagraphCountAverage < currentOutsourcing.GetListParagraphCountAverage().Count(); iListParagraphCountAverage++)
            // цикл сделан для возможности, что будет несколько минимальных значений
            {
                if (currentOutsourcing.GetListParagraphCountAverage()[iListParagraphCountAverage] ==
                    currentOutsourcing.GetListParagraphCountAverage().Min()) ColoringTagRed(currentOutsourcing.GetListTags()[iListParagraphCountAverage].GetName());
            }

            EndingColoring();
        }
        // окрашивание в красный тегов из списка последовательности количества символов в конкретном теге
        private void ColoringForParagraphCount() 
        {
            for (int iListParagraphCount = 0; iListParagraphCount < currentOutsourcing.GetListParagraphCount().Count(); iListParagraphCount++) 
            {
                if (currentOutsourcing.GetListParagraphCount()[iListParagraphCount] < 10)
                    colorTags[iListParagraphCount] = false;
            }

            EndingColoring();
        }
        // окрашивание в красный тегов из улучшенного списка включенных тегов в конкретный тег
        private void ColoringForIncludedTagsPlus() 
        {
            for (int iListTreeInterruptTags = 1; iListTreeInterruptTags < currentOutsourcing.GetListTreeInterruptTags().Count() - 1; iListTreeInterruptTags++) 
            {
                if (currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags] != currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags - 1])
                {
                    if (currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags] != currentOutsourcing.GetListTreeInterruptTags()[iListTreeInterruptTags + 1])
                    colorTags[iListTreeInterruptTags] = false;
                }                 
            }

            EndingColoring();
        }
        // окрашивание в красный тегов, в строчке которых нет пробела
        private void ColoringParagraphWithoutSpace() 
        {
            for (int iListSequenceTags = 0; iListSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iListSequenceTags++)
            {
                // для сокращения вычислений поиск идет только среди тегов, уже окрашенных в зеленый
                if ((colorTags[iListSequenceTags] == true) && (currentOutsourcing.GetTagsContent()[iListSequenceTags].IndexOf(' ') == -1)) colorTags[iListSequenceTags] = false;
            }

            EndingColoring();
        }
        // окрашивание в зеленый всех встречающихся примеров конкретного тега в массиве тегов
        private void ColoringTagGreen(string tag) 
        {
            for (int iSequenceTags = 0; iSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iSequenceTags++) 
            {
                if (currentOutsourcing.GetListSequenceTags()[iSequenceTags] == tag) colorTags[iSequenceTags] = true;
            }
        }
        // окрашивание в красный всех встречающихся примеров конкретного тега в массиве тегов
        private void ColoringTagRed(string tag) 
        {
            for (int iSequenceTags = 0; iSequenceTags < currentOutsourcing.GetListSequenceTags().Count(); iSequenceTags++)
            {
                if (currentOutsourcing.GetListSequenceTags()[iSequenceTags] == tag) colorTags[iSequenceTags] = false;
            }
        }
        // проверка каждого coloring-метода в конце на наличие одного из двух цветов в каждом элементе массива с тегами
        private void EndingColoring() 
        {
            for (int iColorTags = 0; iColorTags < colorTags.Count(); iColorTags++)
            {
                if ((colorTags[iColorTags] != true) && (colorTags[iColorTags] != false)) colorTags[iColorTags] = false;
            }
        }
    }
}
