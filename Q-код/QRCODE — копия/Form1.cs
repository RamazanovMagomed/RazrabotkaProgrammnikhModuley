using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Черный ящик для работы с QR-кодами
        private QRCodeBlackBox _qrCodeBox;

        public Form1()
        {
            InitializeComponent();
            InitializeQRCodeBox();
        }

        private void InitializeQRCodeBox()
        {
            try
            {
                _qrCodeBox = new QRCodeBlackBox();

                // Настройка диалоговых окон через черный ящик
                saveFileDialog1.Filter = _qrCodeBox.GetSupportedImageFormats();
                openFileDialog1.Filter = _qrCodeBox.GetSupportedImageFormats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "ЗАКОДИРОВАТЬ"
        private void button1_Click(object sender, EventArgs e)
        {
            if (_qrCodeBox == null)
            {
                MessageBox.Show("Система QR-кодов не инициализирована", "Ошибка");
                return;
            }

            // Используем черный ящик - не нужно знать как он работает внутри
            var result = _qrCodeBox.GenerateFromText(textBox1.Text);

            if (result.Success)
            {
                pictureBox1.Image = result.Image;
                statusLabel.Text = result.Message;
            }
            else
            {
                MessageBox.Show(result.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Кнопка "СОХРАНИТЬ" (исправлено с САХРОНИТЬ)
        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Нет изображения для сохранения", "Информация");
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Используем черный ящик для сохранения
                var result = _qrCodeBox.SaveImageToFile(
                    pictureBox1.Image,
                    saveFileDialog1.FileName
                );

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(result.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Кнопка "РАСПОЗНАТЬ"
        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Загрузите изображение с QR-кодом", "Информация");
                return;
            }

            // Используем черный ящик для распознавания
            var result = _qrCodeBox.DecodeFromImage(pictureBox1.Image);

            if (result.Success)
            {
                textBox2.Text = result.DecodedText;
                statusLabel.Text = result.Message;
            }
            else
            {
                MessageBox.Show(result.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Кнопка "ЗАГРУЗИТЬ"
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.ImageLocation = openFileDialog1.FileName;

                    // Автоматическое распознавание при загрузке (опционально)
                    // var result = _qrCodeBox.DecodeFromFile(openFileDialog1.FileName);
                    // if (result.Success) textBox2.Text = result.DecodedText;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Опционально: автоматическая генерация при вводе
            // Для длинных текстов можно добавить задержку
        }

        // Добавьте этот метод в конструктор формы или Load событие
        private void Form1_Load(object sender, EventArgs e)
        {
            // Можно добавить статусную строку
            statusLabel = new Label
            {
                Text = "Готово",
                Dock = DockStyle.Bottom,
                BorderStyle = BorderStyle.Fixed3D
            };
            this.Controls.Add(statusLabel);
        }

        // Добавьте в класс формы
        private Label statusLabel;

        // Очистка ресурсов
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _qrCodeBox?.Dispose();
        }
    }
}