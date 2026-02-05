using System;
using System.Drawing;
using System.IO;
using System.Text;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Класс-обертка для результата операций с QR-кодом
    /// </summary>
    public class QRCodeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Image Image { get; set; }
        public string DecodedText { get; set; }

        public QRCodeResult(bool success, string message = "", Image image = null, string text = "")
        {
            Success = success;
            Message = message;
            Image = image;
            DecodedText = text;
        }
    }

    /// <summary>
    /// ЧЕРНЫЙ ЯЩИК для работы с QR-кодами
    /// Вся сложная логика инкапсулирована внутри
    /// </summary>
    public class QRCodeBlackBox
    {
        // Внутренние настройки (скрыты от пользователя)
        private readonly Encoding _defaultEncoding = Encoding.UTF8;
        private readonly QRCodeEncoder.ENCODE_MODE _encodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
        private readonly QRCodeEncoder.ERROR_CORRECTION _errorCorrection = QRCodeEncoder.ERROR_CORRECTION.M;

        // Внутренние компоненты
        private QRCodeEncoder _encoder;
        private QRCodeDecoder _decoder;

        /// <summary>
        /// Конструктор черного ящика
        /// </summary>
        public QRCodeBlackBox()
        {
            InitializeComponents();
        }

        /// <summary>
        /// Инициализация внутренних компонентов
        /// </summary>
        private void InitializeComponents()
        {
            try
            {
                _encoder = new QRCodeEncoder
                {
                    QRCodeEncodeMode = _encodeMode,
                    QRCodeErrorCorrect = _errorCorrection,
                    QRCodeVersion = 7,
                    QRCodeScale = 4
                };

                _decoder = new QRCodeDecoder();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Ошибка инициализации QRCode компонентов: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// ПУБЛИЧНЫЙ МЕТОД: Генерация QR-кода из текста
        /// </summary>
        /// <param name="text">Текст для кодирования</param>
        /// <returns>Результат с изображением QR-кода</returns>
        public QRCodeResult GenerateFromText(string text)
        {
            try
            {
                // Валидация входных данных
                var validationResult = ValidateText(text);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Внутренняя генерация QR-кода
                Bitmap qrCodeBitmap = _encoder.Encode(text, _defaultEncoding);

                return new QRCodeResult(
                    success: true,
                    message: "QR-код успешно сгенерирован",
                    image: qrCodeBitmap
                );
            }
            catch (Exception ex)
            {
                return HandleError("Ошибка генерации QR-кода", ex);
            }
        }

        /// <summary>
        /// ПУБЛИЧНЫЙ МЕТОД: Декодирование QR-кода из изображения
        /// </summary>
        /// <param name="image">Изображение с QR-кодом</param>
        /// <returns>Результат с распознанным текстом</returns>
        public QRCodeResult DecodeFromImage(Image image)
        {
            try
            {
                // Валидация изображения
                var validationResult = ValidateImage(image);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Внутренняя логика распознавания
                Bitmap bitmap = new Bitmap(image);
                QRCodeBitmapImage qrImage = new QRCodeBitmapImage(bitmap);
                string decodedText = _decoder.Decode(qrImage, _defaultEncoding);

                return new QRCodeResult(
                    success: true,
                    message: "QR-код успешно распознан",
                    image: image,
                    text: decodedText
                );
            }
            catch (Exception ex)
            {
                return HandleError("Ошибка распознавания QR-кода", ex);
            }
        }

        /// <summary>
        /// ПУБЛИЧНЫЙ МЕТОД: Декодирование QR-кода из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Результат с распознанным текстом</returns>
        public QRCodeResult DecodeFromFile(string filePath)
        {
            try
            {
                // Валидация пути
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return new QRCodeResult(false, "Путь к файлу не указан");
                }

                if (!File.Exists(filePath))
                {
                    return new QRCodeResult(false, "Файл не найден: " + filePath);
                }

                // Загрузка изображения
                using (var image = Image.FromFile(filePath))
                {
                    return DecodeFromImage(image);
                }
            }
            catch (Exception ex)
            {
                return HandleError("Ошибка загрузки файла", ex);
            }
        }

        /// <summary>
        /// ПУБЛИЧНЫЙ МЕТОД: Сохранение изображения в файл
        /// </summary>
        /// <param name="image">Изображение для сохранения</param>
        /// <param name="filePath">Путь для сохранения</param>
        /// <param name="format">Формат изображения</param>
        /// <returns>Результат операции</returns>
        public QRCodeResult SaveImageToFile(Image image, string filePath,
            System.Drawing.Imaging.ImageFormat format = null)
        {
            try
            {
                // Валидация
                if (image == null)
                {
                    return new QRCodeResult(false, "Изображение не загружено");
                }

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return new QRCodeResult(false, "Путь для сохранения не указан");
                }

                // Определение формата
                var imageFormat = format ?? System.Drawing.Imaging.ImageFormat.Png;

                // Проверка расширения файла
                string extension = Path.GetExtension(filePath).ToLower();
                if (string.IsNullOrEmpty(extension))
                {
                    filePath += GetDefaultExtension(imageFormat);
                }

                // Сохранение
                image.Save(filePath, imageFormat);

                return new QRCodeResult(
                    success: true,
                    message: $"Изображение сохранено: {Path.GetFileName(filePath)}"
                );
            }
            catch (Exception ex)
            {
                return HandleError("Ошибка сохранения файла", ex);
            }
        }

        /// <summary>
        /// ПУБЛИЧНЫЙ МЕТОД: Получение поддерживаемых форматов
        /// </summary>
        public string GetSupportedImageFormats()
        {
            return "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|BMP (*.bmp)|*.bmp";
        }

        #region ПРИВАТНЫЕ МЕТОДЫ (внутренняя реализация)

        /// <summary>
        /// Валидация текста для кодирования
        /// </summary>
        private QRCodeResult ValidateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new QRCodeResult(false, "Текст для кодирования не может быть пустым");
            }

            if (text.Length > 1000)
            {
                return new QRCodeResult(false, "Текст слишком длинный (максимум 1000 символов)");
            }

            return new QRCodeResult(true);
        }

        /// <summary>
        /// Валидация изображения
        /// </summary>
        private QRCodeResult ValidateImage(Image image)
        {
            if (image == null)
            {
                return new QRCodeResult(false, "Изображение не загружено");
            }

            // Проверка минимального размера
            if (image.Width < 50 || image.Height < 50)
            {
                return new QRCodeResult(false, "Изображение слишком маленькое для распознавания");
            }

            return new QRCodeResult(true);
        }

        /// <summary>
        /// Обработка ошибок
        /// </summary>
        private QRCodeResult HandleError(string message, Exception ex)
        {
            // Здесь может быть сложная логика логирования ошибок
            string fullMessage = $"{message}: {ex.Message}";

            // Для отладки можно добавить детали
#if DEBUG
            fullMessage += $"\n\nДетали: {ex.StackTrace}";
#endif

            return new QRCodeResult(false, fullMessage);
        }

        /// <summary>
        /// Получение расширения по умолчанию для формата
        /// </summary>
        private string GetDefaultExtension(System.Drawing.Imaging.ImageFormat format)
        {
            if (format == System.Drawing.Imaging.ImageFormat.Png) return ".png";
            if (format == System.Drawing.Imaging.ImageFormat.Jpeg) return ".jpg";
            if (format == System.Drawing.Imaging.ImageFormat.Bmp) return ".bmp";
            return ".png";
        }

        #endregion

        #region Дополнительные публичные методы (опционально)

        /// <summary>
        /// Генерация QR-кода для URL
        /// </summary>
        public QRCodeResult GenerateForUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            return GenerateFromText(url);
        }

        /// <summary>
        /// Генерация QR-кода для контакта (vCard)
        /// </summary>
        public QRCodeResult GenerateForContact(string name, string phone, string email)
        {
            string vCard = $"BEGIN:VCARD\n" +
                          $"VERSION:3.0\n" +
                          $"FN:{name}\n" +
                          $"TEL:{phone}\n" +
                          $"EMAIL:{email}\n" +
                          $"END:VCARD";

            return GenerateFromText(vCard);
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Dispose()
        {
            // Очистка ресурсов, если нужно
        }

        #endregion
    }
}