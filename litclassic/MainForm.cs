using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace litclassic
{
    public partial class MainForm : Form
    {
        public delegate void ChangeProgressValue();
        public ChangeProgressValue changeProgressValueDelegate;
        public delegate void ChangeProcessName();
        public ChangeProcessName changeProccessNameDelegate;
        public delegate void ChangePhaseName();
        public ChangePhaseName changePhaseNameDelegate;
        public delegate void ChangeProgressCurrentValue();
        public ChangeProgressCurrentValue changeProgressCurrentValueDelegate;
        public delegate void ChangeProgressMaximumValue();
        public ChangeProgressMaximumValue changeProgressMaximumValueDelegate;

        private string connection = "";
        private string bookPath = "";
        private string typeWrite = "";
        //private BackgroundWorker backgroundWork;
        private Book book;
        private int currentProgress;
        private int maximumProgress;
        private string currentProccess;
        private string currentPhaseName;
        private int listBox2ItemsCount;
        private List<string> listBooksPaths;
        private Words words;
        private string wordsFile = "";

        public MainForm()
        {
            InitializeComponent();

            changeProgressValueDelegate = new ChangeProgressValue(ChangeProgressValuesMethod);
            changeProccessNameDelegate = new ChangeProcessName(ChangeProccessNameMethod);
            changePhaseNameDelegate = new ChangePhaseName(ChangePhaseNameMethod);
            changeProgressCurrentValueDelegate = new ChangeProgressCurrentValue(ChangeProgressCurrentValueMethod);
            changeProgressMaximumValueDelegate = new ChangeProgressMaximumValue(ChangeProgressMaximumValueMethod);
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
        }


        public void SetProgressValues(int newCurrentProgressValue, int newMaximumProgressValue)
        {
            currentProgress = newCurrentProgressValue;
            maximumProgress = newMaximumProgressValue;
        }
        public void SetProgressCurrentValue(int newProgressCurrentValue)
        {
            currentProgress = newProgressCurrentValue;
        }
        public void SetProgressMaximumValue(int newProgressMaximumValue)
        {
            maximumProgress = newProgressMaximumValue;
        }
        public void ChangeProgressValuesMethod()
        {
            progressBar1.Value = currentProgress;
            progressBar1.Maximum = maximumProgress;
        }
        public void ChangeProgressCurrentValueMethod()
        {
            progressBar1.Value = currentProgress;
        }
        public void ChangeProgressMaximumValueMethod()
        {
            progressBar1.Maximum = maximumProgress;
            progressBar1.Value = 0;
        }
        public void SetProccessName(string newProccessName)
        {
            currentProccess = newProccessName;
        }
        public void ChangeProccessNameMethod()
        {
            listBox1.Items.Add(currentProccess);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        public void SetPhaseName(string newPhaseName)
        {
            currentPhaseName = newPhaseName;
        }
        public void ChangePhaseNameMethod()
        {
            listBox1.Items.Add(currentPhaseName);
        }



        // кнопка "Добавить книгу"
        private void buttonBookAd_Click(object sender, EventArgs e) 
        {
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                bookPath = openFileDialog1.FileName;
                //label2.Text = bookPath;

                listBox2.Items.Add(bookPath);
            }      
        }
        // кнопка "Внести данные в книгу"
        private void button1_Click_1(object sender, EventArgs e)
        {
            ComboBoxCheck();

            //foreach (object path in listBox2.Items)
            //for (int iListBox2 = 0; iListBox2 < listBox2.Items.Count; iListBox2++) 
            //{
            // присваивание глобальной переменной переменной пути книги из списка
            //bookPath = Convert.ToString(listBox2.Items[iListBox2]);
            //bookPath = Convert.ToString(path);
            listBox2ItemsCount = listBox2.Items.Count;
            // выделение в списке книги, которая в данный момент записывается
            //listBox2.SelectedIndex = listBox2.Items.IndexOf(bookPath);

            listBooksPaths = new List<string>();

            foreach (object item in listBox2.Items)
            {
                listBooksPaths.Add(Convert.ToString(item));
            }

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_AddBook);

                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("В фоновом режиме уже выполняется операция.");
            }      
        }


        private void ComboBoxCheck()
        {
            if (comboBox1.SelectedIndex == 1)
            {
                connection = "azure";
            }
            else if (comboBox1.SelectedIndex == 0)
            {
                connection = "local";
            }

            if (comboBox2.SelectedIndex == 0)
            {
                typeWrite = "full write";
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                typeWrite = "only lines";
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                typeWrite = "only particals";
            }
        }


        private void backgroundWorker1_AddBook(object sender, DoWorkEventArgs e)
        {
            if (bookPath != "")
            {
                //for (int iListBox2 = 0; iListBox2 < listBox2ItemsCount; iListBox2++)
                foreach (string bookPath in listBooksPaths)
                {
                    book = new Book();

                    book.SetBookPath(bookPath);
                    book.SetConnectionString(connection);
                    book.SetTypeWrite(typeWrite);
                    book.SetForm(this);
                    book.CreateBook();
                    book.AddBookToDB();
                }
            }

             backgroundWorker1.CancelAsync();
        }
        private void backgroundWorker1_AddWords(object sender, DoWorkEventArgs e)
        {
            if (wordsFile != "")
            {
                words = new Words();

                words.SetForm(this);
                words.CreateWords(wordsFile);
                words.AddWordsToDB();
            }

            backgroundWorker1.CancelAsync();
        }
        private void backgroundWorker2_DeleteParticals(object sender, DoWorkEventArgs e)
        {
            if (listBox3.Items.Count != 0)
            {
                SetProgressMaximumValue(listBox3.Items.Count);

                System.Timers.Timer currentTimer = new System.Timers.Timer(1000);
                //currentTimer.Elapsed += async (sender, e) => await HandleTimer();
                currentTimer.Elapsed += OnTimedEvent;
                //progressBar2.Maximum = listBox3.Items.Count;

                currentTimer.Start();

                //foreach (int ID in listBox3.Items)
                for (int iListBox3 = 0; iListBox3 < listBox3.Items.Count; iListBox3++) 
                {
                    var currentConnect = new BookDBConnect();

                    currentConnect.SetSQLConnectionToAzureDBLitClassicBooks();
                    currentConnect.DeleteParticals(Convert.ToInt32(listBox3.Items[iListBox3]));
                    currentProgress++;
                }

                currentTimer.Stop();

                //if (textBox1.Text.IndexOf(",") != -1)
                //{
                //    string[] idsString = textBox1.Text.Split(',');
                //    List<int> listIDs = new List<int>();

                //    progressBar2.Maximum = idsString.Length;

                //    foreach (string ID in idsString)
                //    {
                //        //listIDs.Add(Convert.ToInt32(ID));

                //        var currentConnect = new BookDBConnect();

                //        currentConnect.DeleteParticals(Convert.ToInt32(ID));

                //        progressBar2.Value++;
                //    }
                //}
                //else
                //{
                //    var currentConnect = new BookDBConnect();

                //    currentConnect.DeleteParticals(Convert.ToInt32(textBox1.Text));

                //    progressBar2.Value++;
                //}
            }

            backgroundWorker2.CancelAsync();
        }


        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //SetProgressCurrentValue(newProgressCurrentValue);
            Invoke(changeProgressCurrentValueDelegate);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


        // Обновление общей статистики
        private void button2_Click(object sender, EventArgs e)
        {
            BookDBConnect currentBookDBConnect = new BookDBConnect();

            currentBookDBConnect.SetSQLConnectionToAzureDBLitClassicBooks();
            currentBookDBConnect.UpdateTotalStatistics();
        }
        // удаление "частиц" по их номеру
        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DeleteParticals);

            backgroundWorker2.RunWorkerAsync();
        }


        private void label4_Click_1(object sender, EventArgs e)
        {

        }


        // кнопка "Добавить ID"
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                listBox3.Items.Add(Convert.ToInt32(textBox1.Text));
                textBox1.Text = "";
            }
        }
        // кнопка "Удалить выбранную книгу из списка"
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }
        // кнопка "Удалить все книги из списка"
        private void button6_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }
        // кнопка "Обновить общую статистику (быстро)"
        private void button7_Click(object sender, EventArgs e)
        {
            BookDBConnect currentBookDBConnect = new BookDBConnect();

            currentBookDBConnect.SetSQLConnectionToAzureDBLitClassicBooks();
            currentBookDBConnect.UpdateTotalStatisticsShort();
        }
        // кнопка "Добавить файл словаря"
        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                words = new Words();
                Encoding cp866 = Encoding.GetEncoding(866);

                using (var reader = new StreamReader(openFileDialog1.FileName, cp866))
                {
                    wordsFile = reader.ReadToEnd();
                }
            }
        }
        // кнопка "Внести слова в базу"
        private void button9_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_AddWords);

                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("В фоновом режиме уже выполняется операция.");
            }
        }
    }
}
