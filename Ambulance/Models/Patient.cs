using System;

namespace Ambulance
{
    //модель пацієнта
    [Serializable]
    public class Patient
    {
        public string CallTime { get; set; } //час виклику
        public string FullName { get; set; } //повне імя
        public double Distance { get; set; } //дистанція до пацієнта
        public Illness Illness { get; set; } //хвороба
        public Priorities Priority { get => Illness.Priority; } //приорітет пацієнта, беремо з приорітету хвороби

        public Patient() //дата виклику буде братись з часу ініціалізації конструктора
        {
            CallTime = DateTime.Now.ToString("dd/MM H:mm:ss");
        }

        public override string ToString()
        {
            return $"{FullName}";
        }
    }
}
