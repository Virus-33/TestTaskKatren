using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            string vowels = LoadVowels(args[2]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1, vowels);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2, vowels);



            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            Console.WriteLine("Статистика по буквам:");
            PrintStatistic(singleLetterStats);

            Console.WriteLine("Статистика по парам букв:");
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Для выхода из программы нажмите любую клавишу");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Возвращает строку, в которой содержится список поддерживаемых приложением гласных
        /// </summary>
        /// <param name="filepath">Путь к файлу с гласными</param>
        /// <returns>Сплошная строка гласных без разделителей и служебных символов</returns>
        private static string LoadVowels(string filepath)
        {
            string result = "";

            IReadOnlyStream input = GetInputStream(filepath);
            while (!input.IsEof)
            {
                try
                {
                    result += input.ReadNextChar().ToString();
                }
                catch {
                    break;
                }
            }

            input.Dispose();

            return result;
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream, string vowels)
        {
            List<LetterStats> letterStats = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                try
                {
                    string c = stream.ReadNextChar().ToString();
                    AddOrIncrement(letterStats, c, vowels);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            stream.Dispose();
            return letterStats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream, string vowels)
        {
            List<LetterStats> letterStats = new List<LetterStats>();

            string previous = "";

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                try
                {
                    // Игнорируем регистр. Пары букв это в любом случае строки, поэтому будем работать сразу с ними
                    string c = stream.ReadNextChar().ToString().ToLower();

                    if (c != previous) { previous = c; continue; }

                    string both = previous + c;

                    AddOrIncrement(letterStats, both, vowels);
                    
                    previous = c;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            stream.Dispose();
            return letterStats;
        }

        private static void AddOrIncrement(List<LetterStats> letterStats, string s, string vowels)
        {
            int LSid = letterStats.FindIndex(i => i.Letter == s);
            if (letterStats.Count < 1 || LSid == -1)
            {
                letterStats.Add(new LetterStats(s, 1, vowels.Contains(s.Substring(0, 1).ToLower()) ? CharType.Vowel : CharType.Consonants));
            }
            else
            {
                IncStatistic(letterStats[LSid]);
            }
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {

            IList<LetterStats> toBeDeleted = new List<LetterStats>();

            foreach (LetterStats Ls in letters)
            {
                if (Ls.Type == charType)
                { 
                    toBeDeleted.Add(Ls);
                }
            }

            foreach (LetterStats Ls in toBeDeleted)
            {
                if (letters.IndexOf(Ls) != -1)
                {
                    letters.Remove(Ls);
                }
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            List<LetterStats> sorted = letters.OrderBy(i => i.Letter).ToList();

            foreach (LetterStats ls in sorted)
            {
                Console.WriteLine(string.Format("{0} : {1}", ls.Letter, ls.Count));
            }

            Console.WriteLine();

        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
