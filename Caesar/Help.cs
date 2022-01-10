using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caesar
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            label1.Text = "Программа предназначена для расшифровки и шифровки текста на русском языке.\n\n Для шифровки текста, наберите его с помощью клавиатуры или нажмите на вкладку 'Файл' и выберете нужный документ, затем введите свой ключ шифрования, а после жмите кнопку 'зашифровать'.\n Для расшифровки введите зашифрованный текст с клавиатуры или из файла. Нажмите на кнопку 'расшифровать'.";

            label3.Text = "Шифр Цезаря — это вид шифра подстановки, в котором каждый символ в открытом тексте заменяется символом, находящимся на некотором постоянном числе позиций левее или правее него в алфавите. Например, в шифре со сдвигом вправо на 3, А была бы заменена на Г, Б станет Д, и так далее.";
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Help_Load(object sender, EventArgs e)
        {

        }

    
    }
}
