﻿namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats // Сделал статистику классом, а не структурой для работы инкрементирующего метода
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        public LetterStats(string letter, int count)
        {
            Letter = letter;
            Count = count;
        }
    }
}
