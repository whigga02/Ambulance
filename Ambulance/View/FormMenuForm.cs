using Ambulance.Controllers;
using System;
using System.Windows.Forms;

namespace Ambulance
{
    //базова форма меню
    public partial class FormMenuForm : Form
    {
        //інкапсульовані поля
        //контроллери
        private readonly BrigadeController _brigadeController;
        private readonly PatientController _patientController;

        public FormMenuForm()
        {
            //ініціалізація контроллерів
            _brigadeController = new BrigadeController();
            _patientController = new PatientController();

            InitializeComponent();

            //підписуємось на делегати, які будуть викликатись при зміні кількості бригад чи пацієнтів
            //методи BrigadeStatisticChange і PatientsStatisticChange будуть оновляти лічильник
            _brigadeController.OnChangedCount += BrigadeStatisticChange;
            _patientController.OnChangedCount += PatientsStatisticChange;

            (HighQBrigadesCured.Text, BrigadesCount.Text) = _brigadeController.GetStatistics();
            NumberOfCalls.Text = _patientController.Patients.Count.ToString();

        }
        //оновляє лічильник з новою інформацією про кількість бригад
        private void BrigadeStatisticChange(int highQualityBrigadeCount, int brigadesCount)
        {
            HighQBrigadesCured.Text = highQualityBrigadeCount.ToString();
            BrigadesCount.Text = brigadesCount.ToString();
        }
        //оновляє лічильник з новою інформацією про кількість бригад
        private void PatientsStatisticChange(int patientsCount)
        {
            NumberOfCalls.Text = patientsCount.ToString();
        }

        //визивається нова форма GroupOfDoctors
        private void ButtonOpenGOD_Click(object sender, EventArgs e)
        {
            if(Application.OpenForms["GroupOfDoctors"]  == null)
                new GroupOfDoctors(_brigadeController).Show();
        }
        //визивається нова форма FormPatientList
        private void ButtonOpenPL_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["PatientsList"] == null)
                new PatientsList(_brigadeController, _patientController).Show();
        }
    }
}
