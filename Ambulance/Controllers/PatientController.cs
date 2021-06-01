using System;
using System.ComponentModel;

namespace Ambulance.Controllers
{
    //Контроллер для пацієнтів
    public class PatientController
    {
        public Action<int> OnChangedCount; //делегат, який викликається при зміні кількості пацієнтів
        public BindingList<Patient> Patients { get; set; } //список пацієнтів
        public PatientController()
        {
            OnChangedCount = new Action<int>((int a) => { });
            
            Patients = new BindingList<Patient>()
            {
                //генеруються два рандомних об'єкта пацієнтів в початковий список
                PatientsGenerator.Generate(),
                PatientsGenerator.Generate()
            };
        }
        //метод, який дозволяє створити нового рандомного пацієнта і одразу додати його до списку всіх пацієнтів
        public void NewRandomPatient()
        {
            Patients.Add(PatientsGenerator.Generate());
            //звук
            Console.Beep();
            //виклик делегату
            OnChangedCount.Invoke(Patients.Count);
        }
    }
}
