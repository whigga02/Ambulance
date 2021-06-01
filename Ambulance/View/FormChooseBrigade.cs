using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ambulance
{
    public partial class FormChooseBrigade : Form
    {
        //форма вибору бригади
        //приймає список доступних бригад
        public FormChooseBrigade(BindingList<Brigade> brigades)
        {
            InitializeComponent();

            //при зміні виділення викликає метод GridSelectionChanged, який перевіряє, чи виділена зараз якась бригада
            BrigadesGridView.SelectionChanged += GridSelectionChanged;

            //визначає видимість і розміри колонок
            BrigadesGridView.DataSource = brigades;
            BrigadesGridView.Columns["Patient"].Visible = false;
            BrigadesGridView.Columns["AverageDrivingSpeed"].Visible = false;
            BrigadesGridView.Columns["RecoveredPatients"].Width = 150;
            BrigadesGridView.Columns["Specialization"].Width = 150;
        }

        //метод, який дозволяє взяти об`єкт виділеної бригади
        public Brigade GetSelectedBrigade()
        {
            var row = BrigadesGridView.SelectedRows[0];
            if (row.DataBoundItem is Brigade brigade)
                return brigade;
            else
                throw new InvalidCastException($"{row.DataBoundItem.GetType()} is'nt a Brigade");
        }

        private void GridSelectionChanged(object sender, EventArgs e)
        {
            if (BrigadesGridView.SelectedCells.Count > 0)
                ButtonChoice.Visible = true;
            else
                ButtonChoice.Visible = false;
        }
    }
}
