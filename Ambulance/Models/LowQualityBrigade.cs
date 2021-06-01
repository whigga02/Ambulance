using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ambulance
{
    [Serializable]
    public class LowQualityBrigade : Brigade
    {
        //перевизначаємо автоматичні властивості
        public override Specializations Specialization { get; set; } = Specializations.Brigade;
        public override double AverageDrivingSpeed { get; set; } = 70;

        //перегружений конструктор
        //у випадку, коли використовується конструктор з параметром int id
        //викликається базовий конструктор абстрактного класу Brigade і передається туди id
        public LowQualityBrigade() : base() { }
        public LowQualityBrigade(int id) : base(id) { }

        //перевизначений асинхронний метод, в якому відбувається емуляція поїздки до пацієнта
        public async override Task GoToPatientAsync(Patient patient)
        {
            int travelTime = Convert.ToInt32(patient.Distance * 100 / AverageDrivingSpeed);
            int waitingTime = travelTime * 2 + patient.Illness.HealingTime;
            string message = $"Бригаду №{Id} було успішно відпрвлено до пацієнта: {patient} приблизний час вилікування і прибуття {waitingTime} секунд";
            new Thread(() => MessageBox.Show(message, "", MessageBoxButtons.OK)).Start();
            Patient = patient;
            await Task.Delay(travelTime * 1000);
        }

        //метод емуляції лікування хворого
        public async override Task HealAsync()
        {
            //час лікування
            int time = Patient.Illness.HealingTime * 2;

            //емуляція лікування
            await Task.Delay(time * 1000);
            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            string message = $"Бригада №{Id} успішно вилікувала пацієнта {Patient.FullName} від {Patient.Illness.Name} і тепер вже доступна";
            new Thread(() => MessageBox.Show(message, "", MessageBoxButtons.OK)).Start();

            //змінюємоо статус
            Status = Status.Free;
            //звільняємо пацієнта
            Patient = null;
            //добавляємо +1 до вилікуваних пацієнтів
            ++RecoveredPatients;
        }
    }
}
