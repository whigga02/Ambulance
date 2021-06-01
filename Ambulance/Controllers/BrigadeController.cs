using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Model;

namespace Ambulance.Controllers
{
    //BrigadeController - клас, створений для того щоб контролювати бригадами,
    //піднімати в званні, створювати нову базову бригаду, розформовувати її
    public class BrigadeController
    {
        //інкапсульована змінна
        private readonly string BRIGADES_FILEPATH = "brigades.xml";
        //екземпляр файлового менеджера, створений для того щоб серіалізувати файли, обновляти і тд
        //BindingList<Brigade> - це об'єкт, який буде серіалізуватись
        //якщо файла з вище згаданою назвою немає, то він його створить
        private readonly FileManager<BindingList<Brigade>> _fileManager;

        public BindingList<Brigade> Brigades { get; set; } //список бригад якими ми будем контролювати
        //делегат, який викликається, коли кількість бригад змінюється(для того, щоб FormMenu дізнавався про зміни і обновляв статистику)
        public Action<int, int> OnChangedCount;
        //делегат, який викликається, коли в бригади змінюється статус
        public Action OnChangedBrigadeStatus;

        public BrigadeController()
        {
            OnChangedCount = new Action<int, int>((int a, int b) => { });
            OnChangedBrigadeStatus = new Action(()=> { });

            //якщо файл новий
            bool isNewFile = !File.Exists(BRIGADES_FILEPATH);

            _fileManager = new FileManager<BindingList<Brigade>>(BRIGADES_FILEPATH);
            //загружаєм BindingList<Brigade> з файлу
            Brigades = _fileManager.LoadObjectFromFile();
            //обнуляє статус бригад після завантаження файлу 
            var nonFreeBrigades = Brigades.Where(b => b.Status != Status.Free).ToArray();
            foreach (var brigade in nonFreeBrigades)
            {
                brigade.Patient = null;
                brigade.Status = Status.Free;
            }

            //якщо файл новий то заповняєм його базовими бригадами
            if (isNewFile)
                Add(BaseObjectsInitializer.GetDefaultBrigades().ToArray());
        }

        //логіка найму нової бригади
        public void HireBrigade()
        {
            int id;
            if (Brigades.Count > 0)
            {
                int lastNumber = Brigades.Count - 1;
                int lastId = Brigades[lastNumber].Id;
                id = ++lastId;
            }
            //якщо бригад нема, то ID нової бригади = 1
            else id = 1;
            //створюється нова бригада
            Brigade brigade = new LowQualityBrigade(id);
            //одразу ж добавляємо її, метод який добавить в список бригаду і обновить файл за допомогою файлового менеджеру
            Add(brigade);
        }

        //метод який добавляє нову бригаду params
        //Brigade[] дозволя закинути в якості параметра будь-яку кількість об'єктів бригади
        //метод Add використовує абстрактний об'єкт Brigade, не знаючи фактичного типу
        //сюди в якості параметру ми можемо підсунути або HighQualityBrigade або LowQualityBrigade і преобразувати його в Brigade
        private void Add(params Brigade[] brigades)
        {
            //добавляєм в список і обновляєм файл
            foreach (var brigade in brigades)
                Brigades.Add(brigade);
            _fileManager.UpdateFile(Brigades);
            //виклик делегату, який сповістить всіх "підписників" про зміни
            OnChangedCount.Invoke(HighQualityBrigadeCount, AllBrigadesCount);
        }

        //видалення бригади
        public void Remove(Brigade brigade)
        {
            //видаляєм з списку і обновляєм файл
            Brigades.Remove(brigade);
            _fileManager.UpdateFile(Brigades);
            //виклик делегату, який сповістить всіх "підписників" про зміни
            OnChangedCount.Invoke(HighQualityBrigadeCount, AllBrigadesCount);
        }
        
        //метод, який викликає форму вибору бригади для пацієнта
        public bool ChooseBrigadeForPatient(Patient patient)
        {
            //тут ми шукаємо всі бригади потрібної спеціалізації: для різних пріорітетів - різні бригади
            Specializations specialization;
            if (patient.Illness.Priority == Priorities.High)
                specialization = Specializations.HighQualityBrigade;
            else
                specialization = Specializations.Brigade;
            //тут за допомогою метода GetBrigades ми вибираємо бригади тількі підходящої спеціалізації
            BindingList<Brigade> brigades = GetBrigades(specialization);

            //якщо немає вільних бригад
            if (brigades.Count <= 0)
            {
                MessageBox.Show($"Наразі вільних бригад потрібної спеціалізації немає", "Увага", MessageBoxButtons.OK);
                return false;
            }
            //якщо бригади потрібної спеціалізації є, то викликається форма вибору бригади
            FormChooseBrigade formChoiceBrigade = new FormChooseBrigade(brigades);
            if (formChoiceBrigade.ShowDialog() == DialogResult.OK)//якщо ми вибрали і нажали ОК
            {
                //в форми FormChooseBrigade є метод GetSelectedBrigade який вертає нам вибрану бригаду
                Brigade brigade = formChoiceBrigade.GetSelectedBrigade();
                //тут запускається метод TakePatient в новому потоці, по суті в ньому ми відправляємо бригаду
                Task.Run(() => TakePatient(brigade, patient));
                return true;
            }
            else
                return false;
        }

        //асинхронний метод, в якому ми відправляємо бригаду взяти пацієнта
        public async void TakePatient(Brigade brigade, Patient patient)
        {
            //змінюємо статус бригади і викликаємо делегат
            brigade.Status = Status.OnTheWay;
            OnChangedBrigadeStatus.Invoke();

            //запускається асинхронний метод бригади GoToPatientAsync
            //емуляція того, що бригада їде до пацієнта, і залежно від того, яка це бригада, або лікує на місці, або відвозить до лікарні 
            await brigade.GoToPatientAsync(patient);

            //тоді коли бригада приїхала, змінюємо їй статус і викликаємо делегат
            brigade.Status = Status.HealsThePatient;
            OnChangedBrigadeStatus.Invoke();

            //HealAsync - метод, який починає лікувати пацієнта
            //лікуємо і оновлюємо файл, також викликаємо делагат
            await brigade.HealAsync();
            _fileManager.UpdateFile(Brigades);
            OnChangedBrigadeStatus.Invoke();
        }

        //логіка підвищення бригади
        public void RankUp(LowQualityBrigade brigade)
        {
            //створюємо новий об'єкт HighQualityBrigade на основі LowQualityBrigade
            Brigade highQualityBrigade = new HighQualityBrigade()
            {
                Id = brigade.Id,
                Status = brigade.Status,
                Patient = brigade.Patient,
                RecoveredPatients = brigade.RecoveredPatients
            };
            //вставляємо його назад у список під тим індексом, під яким він був
            int index = Brigades.IndexOf(brigade);
            Brigades.Remove(brigade);
            Brigades.Insert(index, highQualityBrigade);

            //оновлюємо файл і викликаємо делегат
            _fileManager.UpdateFile(Brigades);
            OnChangedCount.Invoke(HighQualityBrigadeCount, AllBrigadesCount);
        }

        //в цьому методі ми можемо взяти бригаду потрібної спеціалізації
        public BindingList<Brigade> GetBrigades(Specializations specialization)
        {
            return new BindingList<Brigade>(Brigades.Where(b =>
            b.Specialization == specialization &&
            b.Status == Status.Free).ToList());
        }

        //автоматичні властивості для інформації, скільки бригад є
        //використовуються делегатом для оповіщення своїх підписників про кількість бригад різних спеціалізації
        private int HighQualityBrigadeCount { get => Brigades.Where(b => b.Specialization == Specializations.HighQualityBrigade).Count(); }
        private int AllBrigadesCount { get => Brigades.Where(b => b.Specialization == Specializations.Brigade).Count(); }
        public (string, string) GetStatistics()
        {
            return (HighQualityBrigadeCount.ToString(), AllBrigadesCount.ToString());
        }
    }
}
