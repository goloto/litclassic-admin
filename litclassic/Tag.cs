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
        private bool tagOpening; // определение, открывающий ли тег
        private bool tagClosing; // определение, закрывающий ли тег
        private int numberOfMeetings; // число встреч тега
        private string content; // содержание тега


        // get
        public string GetName() // запрос на получение имени тега без скобок 
        {
            return name;
        }
        public bool TagOpening() // запрос на получение определения, открывающий ли тег 
        {
            return tagOpening;
        }
        public bool TagClosing() // запрос на получение попределения, закрывающий ли тег 
        {
            return tagClosing;
        }
        public int GetNumberOfMeetings() // запрос на получение числа встреч тега 
        {
            return numberOfMeetings;
        }


        // set
        public void SetNumberOfMeetings(int newNumber) // задание нового числа встреч тега 
        {
            numberOfMeetings = newNumber;
        }
        public void SetName(string newName) // задание нового имени тега, а также определение, закрывающий или открывающий он 
        {
            name = newName;

            if (name[1]=='/')
            {
                tagClosing = true;
                tagOpening = false;
            }
            else
            {
                tagClosing = false;
                tagOpening = true;
            }
        }
    }
}
