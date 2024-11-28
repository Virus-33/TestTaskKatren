namespace TestTask
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

        /// <summary>
        /// Является буква/пара гласной или согласной
        /// </summary>
        public CharType Type;

        public LetterStats(string letter, int count, CharType ct)
        {
            Letter = letter;
            Count = count;
            Type = ct;
        }
    }
}
