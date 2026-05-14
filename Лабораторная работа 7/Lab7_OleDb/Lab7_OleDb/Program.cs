using System;
using System.Data.OleDb;

namespace Lab7_OleDb
{
    class Program
    {
        // ВАЖНО: Укажите ваш полный путь к файлу .mdb или .accdb
        static string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\rpk20\OneDrive\Документы\Database2.accdb";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа №7 ===");
            Console.WriteLine("Поиск по базе данных TextInfo\n");

            Console.Write("Введите текст для поиска (по всем полям): ");
            string searchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Ошибка: поисковый запрос не может быть пустым.");
                return;
            }

            SearchDatabase(searchTerm);
        }

        static void SearchDatabase(string searchTerm)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("\n[+] Подключение к базе данных установлено.\n");

                    // Поиск по всем полям таблицы TextInfo
                    string sql = @"
                        SELECT * FROM TextInfo 
                        WHERE 
                            InputText LIKE ? 
                            OR WordCount LIKE ? 
                            OR ShortestWord LIKE ? 
                            OR ShortestWordIndex LIKE ? 
                            OR LetterACounts LIKE ?";

                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        string likePattern = "%" + searchTerm + "%";

                        cmd.Parameters.AddWithValue("p1", likePattern);
                        cmd.Parameters.AddWithValue("p2", likePattern);
                        cmd.Parameters.AddWithValue("p3", likePattern);
                        cmd.Parameters.AddWithValue("p4", likePattern);
                        cmd.Parameters.AddWithValue("p5", likePattern);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("[-] Ничего не найдено.");
                                return;
                            }

                            Console.WriteLine("[+] Результаты поиска:\n");
                            Console.WriteLine(new string('=', 60));

                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["ID"]}");
                                Console.WriteLine($"Исходный текст: {reader["InputText"]}");
                                Console.WriteLine($"Количество слов: {reader["WordCount"]}");
                                Console.WriteLine($"Самое короткое слово: {reader["ShortestWord"]}");
                                Console.WriteLine($"Индекс короткого слова: {reader["ShortestWordIndex"]}");
                                Console.WriteLine($"Буква 'А' в словах: {reader["LetterACounts"]}");
                                Console.WriteLine(new string('-', 60));
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                Console.WriteLine("Ошибка базы данных: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}