using System;

namespace Ambulance
{
    //модель хвороби
    [Serializable]
    public class Illness
    {
        /// <summary>
        /// Час в секундах
        /// </summary>
        public int HealingTime { get; set; } //потрібний час для того, щоб вилікувати хворобу
        public string Name { get; set; } //назва хвороби
        public Priorities Priority { get; set; } //приорітет хвороби
        
        public Illness() { } //конструктори
        public Illness(string name, Priorities priority, int healingTime)
        {
            Name = name;
            Priority = priority;
            HealingTime = healingTime;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
