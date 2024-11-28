using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // Создал экземпляр наследника класса Stream, т.к. он абстрактен
            try
            {
                _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException) {
                throw new Exception("Невозможно открыть указанный файл.\nЗакройте его в других программах и проверьте указанный путь на наличие ошибок.");
            }
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get  // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            { 
                return _localStream != null && _localStream.Position == _localStream.Length;
            }
            private set
            {

            }
        }

        /// <summary>
        /// Функция закрытия файла после прочтения
        /// </summary>
        public void Dispose()
        {
            _localStream.Close();
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (!IsEof)
            {
                return (char) _localStream.ReadByte();
            }
            throw new Exception("Достигнут конец файла");
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }
    }
}
