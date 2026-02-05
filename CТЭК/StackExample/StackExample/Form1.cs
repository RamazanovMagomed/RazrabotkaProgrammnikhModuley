using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StackExample
{
    public partial class Form1 : Form
    {
        private Stack<string> stack = new Stack<string>();

        public Form1()
        {
            InitializeComponent(); // Это вызывает метод из Form1.Designer.cs
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string item = txtInput.Text;
            if (!string.IsNullOrEmpty(item))
            {
                stack.Push(item);
                txtInput.Clear();
                MessageBox.Show($"Элемент '{item}' добавлен в стек.");
            }
            else
            {
                MessageBox.Show("Введите элемент для добавления.");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (stack.Count > 0)
            {
                string removedItem = stack.Pop();
                MessageBox.Show($"Элемент '{removedItem}' удален из стека.");
            }
            else
            {
                MessageBox.Show("Стек пуст.");
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            foreach (var item in stack)
            {
                listBox.Items.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Пусто или код инициализации
        }
    }
}