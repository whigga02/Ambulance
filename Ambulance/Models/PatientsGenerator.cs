using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ambulance
{

    //генератор рандомних пацієнтів
    public static class PatientsGenerator
    {
        //часовий діапазон в якому буде появлятись новий пацієнт
        public static int MinValue { get; set; } = 15;
        public static int MaxValue { get; set; } = 30;
        //всі можливі хвороби
        private static readonly BindingList<Illness> _diseases = BaseObjectsInitializer.GetDefaultDiseases();

        //статичний метод, який генерує нового пацієнта
        public static Patient Generate()
        {
            //беремо індекс останнього елементу з усіх імен
            int endIndexOfFirstNames = Enum.GetNames(typeof(FirstNames)).Length - 1;
            //беремо індекс останнього елементу з усіх прізвищ
            int endIndexOfLastNames = Enum.GetNames(typeof(FirstNames)).Length - 1;

            var firstNameGenerator = new Random();
            var lastNameGenerator = new Random();

            //генеруємо ім'я та прізвище
            string firstName = Enum.GetName(typeof(FirstNames), firstNameGenerator.Next(0, endIndexOfFirstNames));
            string lastName = Enum.GetName(typeof(LastNames), lastNameGenerator.Next(0, endIndexOfLastNames));

            return new Patient()
            {
                FullName = $"{lastName} {firstName}",
                Illness = GenerateIllness(), //генеруєм пацієнту нову хворобу
                Distance = new Random().Next(5, 50), //генеруєм дистанціє до пацієнта
            };
        }

        //метод, який генерує хворобу
        private static Illness GenerateIllness()
        {
            Random random = new Random();
            int randomValue = random.Next(0, _diseases.Count);
            return _diseases[randomValue];
        }

        //генерує час з діапазону, визначає, коли появиться новий хворий
        public static int GetRandomTime()
        {
            return new Random().Next(MinValue, MaxValue);
        }
    }
}
