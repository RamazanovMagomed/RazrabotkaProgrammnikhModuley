using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueueExample
{
    public partial class Form1 : Form
    {
        private Queue<string> queue = new Queue<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string item = txtInput.Text;
            if (!string.IsNullOrEmpty(item))
            {
                queue.Enqueue(item);
                txtInput.Clear();
                MessageBox.Show($"Элемент '{item}' добавлен в очередь.");
            }
            else
            {
                MessageBox.Show("Введите элемент для добавления.");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (queue.Count > 0)
            {
                string removedItem = queue.Dequeue();
                MessageBox.Show($"Элемент '{removedItem}' удален из очереди.");
            }
            else
            {
                MessageBox.Show("Очередь пуста.");
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            foreach (var item in queue)
            {
                listBox.Items.Add(item);
            }
        }
    }
}