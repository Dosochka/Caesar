using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;


namespace Caesar
{

    public partial class Main : Form
    {
        public Main()
        {
            // Скрытие некоторых чатей интерфейса формы
            InitializeComponent();

        }
       
        private string Data(string text, int key) // Функция Data, которая хранит в себе основные данные
        {
            string alfabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"; // Русский алфавит в 32 символа (Возможен алфавит из 31 символа, т.к в криптографии е = ё)
            alfabet = alfabet.ToLower();

            int aNum = alfabet.Length;  // Длинна переменной alfabet типа string
            string result = ""; // Результат шифрования/дешифровки

            // Контроль над ключем: 0 < key < 32, т.е его модуль
            if (key > 32 && key < 0)
            {
                key = key % 32;
            }

            for (int i = 0; i < text.Length; i++) // Цикл, выполняющийся по длине текста
            {
                var c = text[i]; // Берём строку посимвольно
                var index = alfabet.IndexOf(c);
                if (index < 0) result += c.ToString(); // Добавление пробелов после шифрования/дешифровки
                else
                {
                    var codeIndex = (aNum + index + key) % aNum; // Замена символа на шифрующийся/дешифрующийся         
                    result += alfabet[codeIndex]; // Добавление в переменную result
                }
            }
                return result; // Возвращаем результат
        }

        private string[] WordSearch(string text, string[] word, int equate)
        {
            string input = text;
            string[] str = input.Split(new Char[] { ' ', ',', '.', ':', '!', '?', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int maxlen = 0, index = 0;

            // Поиск самого случайного слова из equate символов 
            for (int k = 0; k < str.Length; k++)
            {
                if (str[k].Length == equate)
                {
                    maxlen = str[k].Length;
                    index = k;
                }
            }

            word[0] = str[index]; // Первоначальное слово в нулевом элементе массива
            for (int i = 1; i < 32; i++)
            {
                word[i] = Data(str[index], -i); // Подбор всех ключей
            }

            return word;
        }

        private string Descript(string text)
        {
            string dictionary = Properties.Resources.RUS; // Массив-словарь
            int key = 0, key2 = 0, key3 = 0, result = 0, maxlen = 0, index = 0;// Переменные    
            string input = text; // Вводимый текст
            string[] str = input.Split(new Char[] { ' ', ',', '.', ':', '!', '?', ';' }, StringSplitOptions.RemoveEmptyEntries); // Избавление от знаков препинания

            // Массивы для шифрования выбранных слов (несколько массивов для понимания того, что дешифровка проходит несколько раз в зависимости от результата
            string[] word = new string[32];
            string[] word2 = new string[32];
            string[] word3 = new string[32];
           
            for (int i = 0; i < str.Length; i++) // Поиск самого длинного слова (word)
            {
                if (str[i].Length > maxlen)
                {
                    maxlen = str[i].Length;
                    index = i;
                }
            }   

            // Шифрование слов в массиве str
            word[0] = str[index];
            for (int i = 1; i < 32; i++)
            {
                word[i] = Data(str[index], -i);
            }

            // Попытка поиска ключа
            for (int i = 0; i < 32; i++)
            {
                if (dictionary.Contains(word[i]))
                {
                    key = i;
                    break;
                }
            }
            result = key;

            // Если ключ не найден
            if (key == 0 || key == 1)
            {
                word2 = WordSearch(text, word2, 5);
                for (int i = 0; i < 32; i++)
                {
                    if (dictionary.Contains(word2[i]))
                    {
                        key2 = i;
                        break;
                    }
                }
                result = key2;

                // Если и второй поиск оказался неудачным
                if (key2 == 0 || key2 == 1)
                {
                    word3 = WordSearch(text, word3, 6);
                    for (int i = 0; i < 32; i++)
                    {
                        if (dictionary.Contains(word3[i]))
                        {
                            key3 = i;
                            break;
                        }
                    }
                    result = key3;
                }
            }
            textBox2.Text = (result).ToString();
            return result.ToString(); // Возвращаем ключ
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num;
            bool isNum = int.TryParse(textBox2.Text, out num);
            textBox3.Text = "";

            string str = textBox1.Text;
            int[] m = new int[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    m[i] = 1;
                }
                else {
                    m[i] = 0;
                }
            }

            if (textBox2.TextLength != 0 && isNum) // Проверка на ввод ключа
            {
                string text = textBox1.Text.ToLower();
                int key = Convert.ToInt32(textBox2.Text);
                string res = Data(text, key);

                for(int i = 0; i < textBox1.TextLength; i++)
                {
                    if (m[i] == 1)
                    {                  
                        textBox3.Text += res[i].ToString().ToUpper();
                    } else {
                        textBox3.Text += res[i];
                    }
                    
                }
            }
            else {
                MessageBox.Show("Введите числовой ключ");
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
          
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = saveFileDialog1.FileName ; // Получаем выбранный файл

            System.IO.File.WriteAllText(filename + ".txt", textBox3.Text);// Сохраняем текст в файл
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = openFileDialog1.FileName; // Получаем выбранный файл

            string fileText = System.IO.File.ReadAllText(filename); // Читаем файл в строку
            textBox1.Text = fileText;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "About") // Меню "О программе"
                { 
                    return;
                }
            }
            About got = new About();
            got.ShowDialog();
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "Help") // Меню "Помощь"
                {
                    f.Activate();
                    return;
                }
            }
            Help help = new Help();
            help.ShowDialog();
        }      

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e) // Кнопка для свапа текста между блоками
        {
            string text = textBox3.Text;
            textBox3.Text = textBox1.Text;
            textBox1.Text = text;
        }


        void PrintPageHandler(object sender, PrintPageEventArgs e) // Обработчик события печати
        {
            e.Graphics.DrawString(result, new Font("Arial", 14), Brushes.Black, 0, 0);
        }

        // текст для печати
        private string result = "";

        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Задаем текст для печати
            if (textBox3.Text.Length != 0)
            {
                result = textBox3.Text;

                // Объект для печати
                PrintDocument printDocument = new PrintDocument();

                // Обработчик события печати
                printDocument.PrintPage += PrintPageHandler;

                // Диалог настройки печати
                PrintDialog printDialog = new PrintDialog();

                // Установка объекта печати для его настройки
                printDialog.Document = printDocument;

                // Если в диалоге было нажато ОК
                if (printDialog.ShowDialog() == DialogResult.OK)
                    printDialog.Document.Print(); // печатаем
            } else {
                MessageBox.Show("Поле с выводом пусто!");
            }                       
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Сохранение регистра букв, после дешифровки
            if (textBox1.Text != "")
            {
                textBox3.Text = "";
                string text = textBox1.Text.ToLower(), key;
                key = Descript(text);
                string res = Data(text, -Convert.ToInt32(key));
            
                string str = textBox1.Text;
                int[] m = new int[str.Length];

                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsUpper(str[i]))
                    {
                        m[i] = 1;
                    }
                    else {
                        m[i] = 0;
                    }
                }

                for (int i = 0; i < textBox1.TextLength; i++)
                {
                    if (m[i] == 1)
                    {
                        textBox3.Text += res[i].ToString().ToUpper();
                    }
                    else {
                        textBox3.Text += res[i];
                    }
                }

            }
            else {
                MessageBox.Show("Введите исходный текст");
            }


        }
    }
}
