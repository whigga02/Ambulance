using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ambulance
{   
    //атрибути, які дозволять серіалізувати обєкт в файл
    [XmlInclude(typeof(HighQualityBrigade))]
    [XmlInclude(typeof(LowQualityBrigade))]
    [XmlInclude(typeof(Patient))]
    [Serializable]
    //абстрактна модель бригади
    public abstract class Brigade
    {
        public int Id { get; set; }
        public abstract Specializations Specialization { get; set; }
        public Status Status { get; set; }

        public Patient Patient { get; set; }
        //кількість вилікуваних пацієнтів
        public int RecoveredPatients { get; set; } = 0;

        //середня швидкість, абстрактна автоматична властивість, в конкретних класах бригад вона буде перевизначатись
        public abstract double AverageDrivingSpeed { get; set; }

        //абстрактні методи без тіла, будуть перевизначатись в конкретних об'єктах бригад
        public abstract Task HealAsync();
        public abstract Task GoToPatientAsync(Patient patient);

        public Brigade() {}
        //перегружений конструктор
        public Brigade(int id)
        {
            Id = id;
        }
    }
}
