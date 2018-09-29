using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace litclassic
{
    public partial class FormForDiagnostics : Form
    {
        Book newBook = new Book();
        List<String> listParagraph = new List<String>(); // список абзацей
        List<int> listNumTitle = new List<int>(); // список номеров последнего абзаца в главе, включая название сл.главы
        List<int> listNumSubtitle = new List<int>(); // список номеров последнего подабзаца в главе
        public FormForDiagnostics()
        {
            InitializeComponent();

            BookSerializer bookSerializer = new BookSerializer();

            //ListBooks listBooks = new ListBooks();

            //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ListBooks));
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//ListBooks.xml";
            //System.IO.FileStream file = System.IO.File.Create(path);
            //writer.Serialize(file, listBooks);
            //file.Close();

            // создание книги

            //if (listBox3.SelectedIndex!=-1)
            //{
            //    listBox2.Select(listBox3.SelectedIndex,1);
            //    listBox6.Select();
            //    listBox8.Select();
            //    listBox9.Select();
            //    listBox11.Select();
            //}

            
        }

        //public void ShowDiagnostics(Outsourcing currentOutSourcing)
        //{
        //    for (int iListBox3 = 0; iListBox3 < currentOutSourcing.GetListTags().Count(); iListBox3++)
        //    {
        //        listBox3.Items.Add(currentOutSourcing.GetListTags()[iListBox3]);
        //    }
        //    for (int iListBox4 = 0; iListBox4 < currentOutSourcing.GetListSequenceTags().Count(); iListBox4++)
        //    {
        //        listBox4.Items.Add(currentOutSourcing.GetListSequenceTags()[iListBox4]);
        //    }
        //    for (int iListBox2 = 0; iListBox2 < currentOutSourcing.GetListCountEnclosedTags().Count(); iListBox2++)
        //    {
        //        listBox2.Items.Add(currentOutSourcing.GetListCountEnclosedTags()[iListBox2]);
        //    }
        //    for (int iListBox1 = 0; iListBox1 < currentOutSourcing.GetListCountTags().Count(); iListBox1++)
        //    {
        //        listBox1.Items.Add(currentOutSourcing.GetListCountTags()[iListBox1]);
        //    }

        //    //label1.Text = newBook.GetDescription();

        //    for (int iListBox5 = 0; iListBox5 < currentOutSourcing.GetListAverageCountEnclosedTags().Count(); iListBox5++)
        //    {
        //        listBox5.Items.Add(currentOutSourcing.GetListAverageCountEnclosedTags()[iListBox5]);
        //    }
        //    for (int iListBox6 = 0; iListBox6 < currentOutSourcing.GetListParagraphCount().Count(); iListBox6++)
        //    {
        //        listBox6.Items.Add(currentOutSourcing.GetListParagraphCount()[iListBox6]);
        //    }
        //    for (int iListBox7 = 0; iListBox7 < currentOutSourcing.GetListParagraphCountAverage().Count(); iListBox7++)
        //    {
        //        listBox7.Items.Add(currentOutSourcing.GetListParagraphCountAverage()[iListBox7]);
        //    }
        //    for (int iListBox8 = 0; iListBox8 < currentOutSourcing.GetListTreeTags().Count(); iListBox8++)
        //    {
        //        listBox8.Items.Add(currentOutSourcing.GetListTreeTags()[iListBox8]);
        //    }
        //    for (int iListBox9 = 0; iListBox9 < currentOutSourcing.GetListTreeInterruptTags().Count(); iListBox9++)
        //    {
        //        listBox9.Items.Add(currentOutSourcing.GetListTreeInterruptTags()[iListBox9]);
        //    }
        //    for (int iListBox10 = 0; iListBox10 < currentOutSourcing.GetListTagsColor().Count(); iListBox10++)
        //    {
        //        listBox10.Items.Add(currentOutSourcing.GetListTagsColor()[iListBox10]);
        //    }
        //    for (int iListBox11 = 0; iListBox11 < currentOutSourcing.GetArrayColorTags().Count(); iListBox11++)
        //    {
        //        listBox11.Items.Add(currentOutSourcing.GetArrayColorTags()[iListBox11]);
        //    }

        //    label21.Text = Convert.ToString(currentOutSourcing.GetListCountTags().Count());
        //    label22.Text = Convert.ToString(currentOutSourcing.GetListTagsColor().Count());
        //    label23.Text = Convert.ToString(currentOutSourcing.GetListTags().Count());
        //    label24.Text = Convert.ToString(currentOutSourcing.GetListAverageCountEnclosedTags().Count());
        //    label25.Text = Convert.ToString(currentOutSourcing.GetListParagraphCountAverage().Count());
        //    label26.Text = Convert.ToString(currentOutSourcing.GetListSequenceTags().Count());
        //    label27.Text = Convert.ToString(currentOutSourcing.GetListCountEnclosedTags().Count());
        //    label28.Text = Convert.ToString(currentOutSourcing.GetListParagraphCount().Count());
        //    label29.Text = Convert.ToString(currentOutSourcing.GetListTreeTags().Count());
        //    label30.Text = Convert.ToString(currentOutSourcing.GetListTreeInterruptTags().Count());
        //    label31.Text = Convert.ToString(currentOutSourcing.GetArrayColorTags().Count());
        //}

        private string TakeQuote() // создание цитаты
        {
            // ПАМЯТКА
            // - Продолжать цитату, если в конце ее двоеточие;
            // - В стихах выбор НЕ между главами, а подглавами?
            // - Параграф - диалог и оканчивается вопросом - добавить еще параграф
            // - Если при добавлении параграфа выход за предел - отменить добавление и прибавлять в начало

            // 1. Выбор между <title> и <subtitle> в пользу большего количества
            // 2. Формирование цитаты путем зациклинного добавления новых абзацей до тех пор, пока длина символов не войдет в заданный диапазон
            // 3. Сверка смысловая
            // 4. Сверка с не заходом за рамки и <title> и <subtitle>
            //
            // на каждом этапе необходимо как-то фиксировать числами этап формирования цитаты, чтобы эту цитату на основе записанных чисел можно было получить вновь
            //
            // на входе в методе получать каждый раз цитату, а затем проверять?
            //
            // прозу цитировать лишь в пределах <title>, в поэзии же каждый <subtitle> выносить как заголовок

            string quote = "";            
            Random rnd = new Random();
            if (listNumTitle.Count > listNumSubtitle.Count)
            {
                int rndTitle = rnd.Next(0, listNumTitle.Count - 1); // номер случайной главы; "-1" из-за того, что общее число элементов начинает считаться не с нуля, а с единицы
                int rndParagraph=0;
                try
                {
                    rndParagraph = rnd.Next(listNumTitle[rndTitle]+1, listNumTitle[rndTitle + 1]-1); // номер случайного абзаца внутри случайной главы;
                    // "+1" в начале для переходя от абзаца с названием главы; "+1" в конце - следующая глава;
                }
                catch
                {
                    TakeQuote();
                }
               
                while (quote.Length < 1900) // условие выхода: количество символов 
                {                    
                    try
                    {
                        string quoteLengthCheck=quote + listParagraph[rndParagraph] + Environment.NewLine; // временная строка с новым абзацем
                        if ((quoteLengthCheck.Length < 2500) && (rndParagraph < listNumTitle[rndTitle + 1]-1)) quote += listParagraph[rndParagraph] + Environment.NewLine;
                        // проверка на количество символов и вхождения в выбранную главу; "-1" в конце для не входа в цитату названия главы
                        rndParagraph++;
                    }
                    catch
                    {
                        break;
                    }
                    
                    if (quote!="") // проверка на наличие двоеточия в конце, если внутри цитаты уже есть строки
                    {
                        if (quote.Last() == ':')
                        {
                            quote += listParagraph[rndParagraph] + Environment.NewLine;
                            rndParagraph++;
                        }
                    }
                }
            }
            else
            {

            }
            //if (quoteFinal == quote) TakeQuote();
            //quoteFinal = quote;
            //return quoteFinal;
            return quote;
        }

        private void button1_Click(object sender, EventArgs e) // Цитата
        {
            label1.Text = TakeQuote();
            label3.Text = Convert.ToString(TakeQuote().Length);

            //label1.Text = listParagraph[rndParagraph] +Environment.NewLine+ listParagraph[rndParagraph+1] + Environment.NewLine + listParagraph[rndParagraph+2] 
            //    + Environment.NewLine + listParagraph[rndParagraph+3];

        }
        private void button2_Click(object sender, EventArgs e) // Абзац
        {
            Random rnd = new Random();
            int r = rnd.Next(0, listParagraph.Count);

            label4.Text = Convert.ToString(listParagraph.Count);
            label1.Text = listParagraph[r];
            label3.Text = Convert.ToString(listParagraph[r].Length);
        }
        private void label9_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e) // массивы чисел
        {
            //label3.Text = Convert.ToString(newBook.GetIncludedTagsCount(Convert.ToInt32(textBox1.Text)));
            //label5.Text = Convert.ToString(newBook.GetIncludedTagsPlusCount(Convert.ToInt32(textBox1.Text)));
        }
        private void button4_Click(object sender, EventArgs e) // массивы чисел символов
        {
            //label17.Text = Convert.ToString(newBook.GetIncludedTagsSymbolCount(Convert.ToInt32(textBox2.Text)));
            //label15.Text = Convert.ToString(newBook.GetIncludedTagsPlusSymbolCount(Convert.ToInt32(textBox2.Text)));
        }
        private void button5_Click(object sender, EventArgs e) // загрузка книг
        {

        }
        private void button6_Click(object sender, EventArgs e) // добавление книги
        {
            openFileDialog1.ShowDialog();
            //openFileDialog1.OpenFile();
            string path = openFileDialog1.FileName;
            //newBook.SetBook(path);

            
        }
        void listBox11_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //int index = this.listBox11.IndexFromPoint(e.Location);
            //if (index != System.Windows.Forms.ListBox.NoMatches)
            //{
            //    MessageBox.Show(index.ToString());
            //}
            Form Paragraph = new Form();

            //MessageBox.Show(newBook.GetParagraphs()[listBox11.SelectedIndex]);
        }

        private void listBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
