using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ambulance
{
    [Serializable]
    public class HighQualityBrigade : Brigade
    {
        //перевизначаємо автоматичні властивості
        public override Specializations Specialization { get; set; } = Specializations.HighQualityBrigade;
        public override double AverageDrivingSpeed { get; set; } = 100;

        //перегружений конструктор
        //в випадку, коли ми використовуємо конструктор з параметром int id
        //визиваємо базовий конструктор абстрактного класу Brigade і передаємо туди id
        public HighQualityBrigade() { }
        public HighQualityBrigade(int id) : base(id) { }

        //перевизначений асинхронний метод, в якому відбувається емуляція поїздки до пацієнта
        public async override Task GoToPatientAsync(Patient patient)
        {
            //визначається час подорожі в одну сторону
            int travelTime = Convert.ToInt32(patient.Distance * 100 / AverageDrivingSpeed);

            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            string message = $"Бригаду №{Id} було успішно відпрвлено до пацієнта: {patient} приблизний час прибуття {travelTime * 2} секунд";
            //new Thread(() => MessageBox.Show(message, "", MessageBoxButtons.OK)).Start();
            MessageBox.Show(message, "", MessageBoxButtons.OK);

            Patient = patient;

            //емуляція подорожі до хворого
            await Task.Delay(travelTime * 1000);


            //емуляція подорожі назад
            await Task.Delay(travelTime * 1000);

            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            message = $"Бригада №{Id} доставила пацієнта: {Patient} до лікарні, вилікує приблизно через {Patient.Illness.HealingTime} секунд";
            new Thread(() => MessageBox.Show(message, "", MessageBoxButtons.OK)).Start();
        }

        //метод емуляції лікування хворого
        public async override Task HealAsync()
        {
            //час лікування
            int time = Patient.Illness.HealingTime;

            //емуляція лікування
            await Task.Delay(time * 1000);
            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            string message = $"Бригада №{Id} успішно вилікувала пацієнта {Patient.FullName} від {Patient.Illness.Name} і тепер вже доступна";
            new Thread(() => MessageBox.Show(message, "", MessageBoxButtons.OK)).Start();

            //змінюємо статус
            Status = Status.Free;
            //звільняємо пацієнта
            Patient = null;
            //добавляємо +1 до вилікуваних пацієнтів
            ++RecoveredPatients;
        }
    }
}
