using Ambulance.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ambulance
{
    public partial class PatientsList : Form
    {
        //інкапсульовані поля позначені модифікатором private
        private readonly BrigadeController _brigadeController;
        private readonly PatientController _patientController;

        //CancellationToken створені для того, щоб зупинити дію в іншому потоці
        private readonly CancellationTokenSource cancelTokenSource;
        private readonly CancellationToken token;

        public PatientsList(BrigadeController brigadeController, PatientController patientController)
        {
            //визначаємо CancellationTokenSource
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            _brigadeController = brigadeController;
            _patientController = patientController;

            //якщо форму буде закрито, то ми зупиняємо потік нових клієнтів
            //можна забрати, тоді нові клієнти будуть появлятись тоді, коли форма PatientsList закрита
            FormClosed += StopReceive;
            InitializeComponent();

            //визначаємо джерело даних для PatientsGridView
            PatientsGridView.DataSource = _patientController.Patients;
            //підписуємся на подію SelectionChanged, при зміні виділення буде викликатись метод PatientsGridView_SelectionChanged
            PatientsGridView.SelectionChanged += PatientsGridView_SelectionChanged;

            //встановлюємо ширину для колонок
            PatientsGridView.Columns["CallTime"].Width = 100;
            PatientsGridView.Columns["FullName"].Width = 145;
            PatientsGridView.Columns["Distance"].Width = 60;
            PatientsGridView.Columns["Priority"].Width = 65;

            //асинхронно (щоб не блокувало інші процеси) запускаємо метод, який стартує генерацію нових пацієнтів
            Task.Run(() => StartAsync());
        }

        public async Task StartAsync()
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    return;

                await Task.Delay(PatientsGenerator.GetRandomTime() * 1000);
                _patientController.NewRandomPatient();
            }
        }

        private void PatientsGridView_SelectionChanged(object sender, EventArgs e)
        {
            //якщо не вибрано жодного пацієнта, то кнопка, яка дозволяє задати бригаду для пацієнта, зникає
            if (_patientController.Patients.Count <= 0)
                ButtonSetGroup.Visible = false;
            else
                ButtonSetGroup.Visible = true;
        }

        //кнопка, яка задає бригаду для пацієнта
        private void ButtonSetGroup_Click(object sender, EventArgs e)
        {
            //обираємо виділену колонку
            var row = PatientsGridView.SelectedRows[0];
            //беремо з неї об'єкт пацієнта
            if(row.DataBoundItem is Patient patient)
            {
                //викликаємо з контроллера метод вибору бригади для пацієнта
                if (_brigadeController.ChooseBrigadeForPatient(patient) == true)
                {
                    //якщо вибір бригади пройшов успішно, відбувається видалення пацієнта зі списку
                    _patientController.Patients.Remove(patient);
                    //якщо більше немає пацієнтів, то відключаємо кнопку "Вибрати групу(для пацієнта)"
                    if (_patientController.Patients.Count <= 0)
                        ButtonSetGroup.Visible = false;
                }
            }
        }
        //метод, що зупиняє потік зі StartAsync(); і створення нових пацієнтів зупиняється
        public void StopReceive(object sender, EventArgs e) => cancelTokenSource.Cancel();
    }
}
