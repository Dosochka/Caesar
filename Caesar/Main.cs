using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Caesar
{
    /// <summary>
    /// Главная форма приложения CaesarChip — шифрование и расшифровка шифра Цезаря для русского текста.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>Размер алфавита (русский: 32 буквы с Ё).</summary>
        private const int AlphabetSize = 32;

        /// <summary>Словарь русских слов для подбора ключа (загружается один раз).</summary>
        private static HashSet<string> _dictionary;

        /// <summary>Разделители слов при разборе текста.</summary>
        private static readonly char[] WordSeparators = { ' ', ',', '.', ':', '!', '?', ';' };

        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Возвращает множество слов из встроенного словаря (по одному слову на строку).
        /// Поиск по словарю выполняется как точное совпадение слова, а не подстрока.
        /// </summary>
        private static HashSet<string> GetDictionary()
        {
            if (_dictionary != null)
                return _dictionary;

            string raw = Properties.Resources.RUS;
            if (string.IsNullOrEmpty(raw))
            {
                _dictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                return _dictionary;
            }

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string line in raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string word = line.Trim();
                if (word.Length > 0)
                    set.Add(word);
            }
            _dictionary = set;
            return _dictionary;
        }

        /// <summary>
        /// Шифрует или расшифровывает текст шифром Цезаря с заданным ключом.
        /// </summary>
        /// <param name="text">Исходный текст (только буквы алфавита сдвигаются).</param>
        /// <param name="key">Ключ сдвига (приводится к диапазону 0..31).</param>
        /// <returns>Результат преобразования.</returns>
        private static string CaesarShift(string text, int key)
        {
            const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

            // Нормализация ключа в диапазон 0..31 (включая отрицательные ключи)
            key = ((key % AlphabetSize) + AlphabetSize) % AlphabetSize;

            var result = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int index = alphabet.IndexOf(c);
                if (index < 0)
                {
                    result.Append(c); // Не буква — оставляем как есть (пробел, цифры, знаки)
                }
                else
                {
                    int codeIndex = (index + key) % AlphabetSize;
                    result.Append(alphabet[codeIndex]);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Ищет в тексте слово заданной длины и заполняет массив вариантами этого слова при ключах 0..31.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <param name="word">Массив для 32 вариантов слова (должен быть выделен вызывающим).</param>
        /// <param name="requiredLength">Требуемая длина слова в символах.</param>
        /// <returns>true, если найдено хотя бы одно слово нужной длины; иначе false.</returns>
        private static bool TryFindWordOfLength(string text, string[] word, int requiredLength)
        {
            if (string.IsNullOrWhiteSpace(text) || word == null || word.Length < AlphabetSize)
                return false;

            string[] parts = text.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            int index = -1;
            for (int k = 0; k < parts.Length; k++)
            {
                if (parts[k].Length == requiredLength)
                {
                    index = k;
                    // Оставляем последнее подходящее слово (как в оригинале)
                }
            }

            if (index < 0)
                return false;

            word[0] = parts[index];
            for (int i = 1; i < AlphabetSize; i++)
                word[i] = CaesarShift(parts[index], -i);

            return true;
        }

        /// <summary>
        /// Подбирает ключ шифра Цезаря по словарю: по самому длинному слову, при неудаче — по словам длины 5 и 6.
        /// </summary>
        /// <param name="text">Зашифрованный текст.</param>
        /// <returns>Найденный ключ в виде строки (0..31) или "0" при неудаче.</returns>
        private string GetKeyByDictionary(string text)
        {
            var dictionary = GetDictionary();
            string[] str = text.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (str.Length == 0)
            {
                textBox2.Text = "0";
                return "0";
            }

            // Самое длинное слово
            int maxlen = 0, index = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Length > maxlen)
                {
                    maxlen = str[i].Length;
                    index = i;
                }
            }

            var word = new string[AlphabetSize];
            word[0] = str[index];
            for (int i = 1; i < AlphabetSize; i++)
                word[i] = CaesarShift(str[index], -i);

            bool keyFound = false;
            int result = 0;

            for (int i = 0; i < AlphabetSize; i++)
            {
                if (dictionary.Contains(word[i]))
                {
                    result = i;
                    keyFound = true;
                    break;
                }
            }

            if (!keyFound)
            {
                var word2 = new string[AlphabetSize];
                if (TryFindWordOfLength(text, word2, 5))
                {
                    for (int i = 0; i < AlphabetSize; i++)
                    {
                        if (dictionary.Contains(word2[i]))
                        {
                            result = i;
                            keyFound = true;
                            break;
                        }
                    }
                }
            }

            if (!keyFound)
            {
                var word3 = new string[AlphabetSize];
                if (TryFindWordOfLength(text, word3, 6))
                {
                    for (int i = 0; i < AlphabetSize; i++)
                    {
                        if (dictionary.Contains(word3[i]))
                        {
                            result = i;
                            keyFound = true;
                            break;
                        }
                    }
                }
            }

            textBox2.Text = result.ToString();
            return result.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация формы (при необходимости)
        }

        /// <summary>Шифрование текста по введённому ключу.</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            bool isNum = int.TryParse(textBox2.Text, out int num);
            textBox3.Text = "";

            string str = textBox1.Text;
            int[] m = new int[str.Length];

            // Запоминаем регистр букв для восстановления после шифрования
            for (int i = 0; i < str.Length; i++)
                m[i] = char.IsUpper(str[i]) ? 1 : 0;

            if (textBox2.TextLength != 0 && isNum)
            {
                string text = textBox1.Text.ToLower();
                int key = Convert.ToInt32(textBox2.Text);
                string res = CaesarShift(text, key);

                var sb = new StringBuilder(res.Length);
                for (int i = 0; i < textBox1.TextLength; i++)
                {
                    if (m[i] == 1)
                        sb.Append(char.ToUpper(res[i]));
                    else
                        sb.Append(res[i]);
                }
                textBox3.Text = sb.ToString();
            }
            else
            {
                MessageBox.Show("Введите числовой ключ");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        /// <summary>Сохранить результат в файл (расширение .txt добавляется только если пользователь не указал своё).</summary>
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = saveFileDialog1.FileName;
            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
                filename += ".txt";

            File.WriteAllText(filename, textBox3.Text, Encoding.UTF8);
        }

        /// <summary>Открыть текст из файла в поле ввода.</summary>
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = openFileDialog1.FileName;
            string fileText = File.ReadAllText(filename, Encoding.UTF8);
            textBox1.Text = fileText;
        }

        /// <summary>Показать окно «О программе» (одно экземпляр).</summary>
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

        /// <summary>Показать окно «Справка» (если уже открыто — активировать).</summary>
        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "Help")
                {
                    f.Activate();
                    return;
                }
            }
            new Help().ShowDialog();
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e) { }

        /// <summary>Обмен текстом между полем ввода и полем вывода.</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            string text = textBox3.Text;
            textBox3.Text = textBox1.Text;
            textBox1.Text = text;
        }

        /// <summary>Текст для печати (используется в обработчике страницы).</summary>
        private string _textToPrint = "";

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(_textToPrint, new Font("Arial", 14), Brushes.Black, 0, 0);
        }

        /// <summary>Печать содержимого поля вывода.</summary>
        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Поле с выводом пусто!");
                return;
            }

            _textToPrint = textBox3.Text;
            var printDocument = new PrintDocument();
            printDocument.PrintPage += PrintPageHandler;
            var printDialog = new PrintDialog { Document = printDocument };

            if (printDialog.ShowDialog() == DialogResult.OK)
                printDialog.Document.Print();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) { }

        /// <summary>Закрыть приложение.</summary>
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>Расшифровка текста: подбор ключа по словарю и восстановление текста с сохранением регистра.</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите исходный текст");
                return;
            }

            textBox3.Text = "";
            string text = textBox1.Text.ToLower();
            string keyStr = GetKeyByDictionary(text);
            int key = int.Parse(keyStr);
            string res = CaesarShift(text, -key);

            string str = textBox1.Text;
            int[] m = new int[str.Length];
            for (int i = 0; i < str.Length; i++)
                m[i] = char.IsUpper(str[i]) ? 1 : 0;

            var sb = new StringBuilder(res.Length);
            for (int i = 0; i < textBox1.TextLength; i++)
            {
                if (m[i] == 1)
                    sb.Append(char.ToUpper(res[i]));
                else
                    sb.Append(res[i]);
            }
            textBox3.Text = sb.ToString();
        }
    }
}
