using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WindowsFormsApp1.Model
{
    //файловий менеджер, який працює з файлом, у якості параметра T задається об'єкт файлу
    public class FileManager<T> where T : new()
    {
        public readonly string _filePath;

        //інкапсульовані поля, доступні тільки в межах цього класу
        private readonly XmlSerializer _serializer; //серіалізатор
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;

        public FileManager(string path)
        {
            //перевіряємо вхідні дані
           
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("File path is empty or null", nameof(path));
           
            _filePath = path;
            _serializer = new XmlSerializer(typeof(T));
        }

        //метод, який оновлює об'єкт у файлі
        public virtual void UpdateFile(T fileObject)
        {
            try
            {
                _streamWriter = new StreamWriter(_filePath);
                _serializer.Serialize(_streamWriter, fileObject);
                _streamWriter.Dispose();
            }
            catch (SerializationException ex)
            {
                throw new FileLoadException(ex.Message, _filePath);
            }
        }

        //метод, який загружає об'єкт з файлу, якщо його не існує, то створює його
        public virtual T LoadObjectFromFile()
        {
            if (!File.Exists(_filePath))
            {
                T anyObject = new T();
                UpdateFile(anyObject);
                return anyObject;
            }
            try
            {
                _streamReader = new StreamReader(_filePath);
                T xmlObject = (T)_serializer.Deserialize(_streamReader);
                _streamReader.Dispose();
                return xmlObject;
            }
            catch (SerializationException ex)
            {
                throw new FileLoadException(ex.Message, _filePath);
            }
        }
    }
}
