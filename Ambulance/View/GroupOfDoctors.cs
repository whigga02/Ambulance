using Ambulance.Controllers;
using System;
using System.Windows.Forms;

namespace Ambulance
{
    //вікно контролю бригад
    public partial class GroupOfDoctors : Form
    {
        //інкапсульоване поле
        private readonly BrigadeController _brigadeController;
        
        public GroupOfDoctors(BrigadeController brigadeController)
        {
            _brigadeController = brigadeController;
            //ініціалізуємо вікно потрібними властивостями
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //підписуєм на делегат SelectionChanged метод GridSelectionChanged
            //коли буде якесь виділення в формі, то буде викликаний метод GridSelectionChanged
            BrigadesGridView.SelectionChanged += GridSelectionChanged;

            //визначаємо джерело даних для BrigadesGridView, звідти будуть братись моделі для колонок і данних
            BrigadesGridView.DataSource = _brigadeController.Brigades;
            //коли буде змінюватись статус бригади, ми будемо оновляти саму табличку
            _brigadeController.OnChangedBrigadeStatus += UpdateGrid;
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            //оновлюється
            BrigadesGridView.DataSource = null;
            BrigadesGridView.DataSource = _brigadeController.Brigades;

            if (BrigadesGridView.Columns["Id"] == null)
                return;

            //визначається розмір форм
            BrigadesGridView.Columns["Id"].Width = 40;
            BrigadesGridView.Columns["Status"].Width = 75;
            BrigadesGridView.Columns["Specialization"].Width = 140;
            BrigadesGridView.Columns["Patient"].Width = 172;
            BrigadesGridView.Columns["RecoveredPatients"].Width = 130;
            BrigadesGridView.Columns["AverageDrivingSpeed"].Visible = false;

            //якщо бригад немає, то пропонуємо користувачеві найняти нову бригаду
            if (_brigadeController.Brigades.Count <= 0)
            {
                ButtonRemove.Visible = false;
                ButtonRankUp.Visible = false;
                if (MessageBox.Show($"Бригад немає, бажаєте створити нову бригаду?", "Увага", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _brigadeController.HireBrigade();
                    MessageBox.Show($"Нову бригаду створено", "Увага!", MessageBoxButtons.OK);
                }
            }
        }

        //метод, який викликається, коли виділення таблички змінюється
        //якщо не вибрана жодна з бригад, то кнопки "Видалити бригаду" і "Підвищити бригаду" станують недоступними
        private void GridSelectionChanged(object sender, EventArgs e)
        {
            if (BrigadesGridView.SelectedCells.Count > 0)
            {
                ButtonRemove.Visible = true;
                ButtonRankUp.Visible = true;
            }
            if (_brigadeController.Brigades.Count <= 0)
            {
                ButtonRemove.Visible = false;
                ButtonRankUp.Visible = false;
            }
        }

        //метод видалення бригади
        private void RemoveBrigade_Click(object sender, EventArgs e)
        {
            var selectedBrigade = TryToGetSelectedBrigade();//метод,  який обирає об'єкт виділеної/вибраної бригади

            if (MessageBox.Show($"Ви дійсно хочете розформувати бригаду №{selectedBrigade.Id}", "Видалення бригади", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //якщо так, то розформовує бригаду
                _brigadeController.Remove(selectedBrigade);
                MessageBox.Show($"Бригаду №{ selectedBrigade.Id} розформовано", "Видалення бригади", MessageBoxButtons.OK);
            }
        }

        //метод, який звертається до контроллера і створоює нову бригаду
        private void ButtonCreateBrigade_Click(object sender, EventArgs e)
        {
            _brigadeController.HireBrigade();
            MessageBox.Show($"Нову бригаду створено", "Увага!", MessageBoxButtons.OK);
        }

        //метод, який підвищує бригаду
        private void ButtonRankUp_Click(object sender, EventArgs e)
        {
            //беремо виділену бригаду
            var selectedBrigade = TryToGetSelectedBrigade();

            if (selectedBrigade.Specialization == Specializations.Brigade //якщо спеціалізація бригади звичайна
                && selectedBrigade.Status == Status.Free)// і якщо бригада зараз вільна
            {
                _brigadeController.RankUp((LowQualityBrigade)selectedBrigade);// то підвищує її
                MessageBox.Show($"Бригада була підвищена", "", MessageBoxButtons.OK);
            }
            else if(selectedBrigade.Status != Status.Free) //якщо бригада зараз не вільна, то викидаємо помилку
            {
                MessageBox.Show($"Бригада зараз в дорозі", "Помилка!", MessageBoxButtons.OK);
            }
            else if(selectedBrigade.Specialization != Specializations.Brigade)//якщо бригада і так максимального рангу, то викидаємо помилку
            {
                MessageBox.Show($"Бригада максимального рангу", "Помилка!", MessageBoxButtons.OK);
            }
        }

        //інкапсульований метод, який повертає об'єкт виділеної бригади
        private Brigade TryToGetSelectedBrigade()
        {
            var row = BrigadesGridView.SelectedRows[0];
            if (row.DataBoundItem is Brigade brigade)
                return brigade;
            else throw new InvalidCastException($"{row.DataBoundItem.GetType()} is'nt a Brigade");
        }
    }
}
