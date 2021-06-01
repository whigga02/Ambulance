 public async override Task GoToPatientAsync(Patient patient)
        {
            //����������� ��� ������� � ���� �������
            int travelTime = Convert.ToInt32(patient.Distance * 100 / AverageDrivingSpeed);

            //��������� MessageBox � ����� ����, ��� ������ �� ��������� ��� ��������
            string message = $"������� �{Id} ���� ������ ��������� �� ��������: {patient} ���������� ��� �������� {travelTime * 2} ������";
            new writeMessageBox.Show(message, "", MessageBoxButtons.OK);
            
            Patient = patient;

            //�������� ������� �� �������
            await Task.Delay(travelTime * 1000);


            //�������� ������� �����
            await Task.Delay(travelTime * 1000);

            //��������� MessageBox � ����� ����, ��� ������ �� ��������� ��� ��������
            message = $"������� �{Id} ��������� ��������: {Patient} �� �����, ����� ��������� ����� {Patient.Illness.HealingTime} ������";
            writeMessageBox.Show(message, "", MessageBoxButtons.OK)).Start();
        }
 public async override Task HealAsync()
        {
            //��� ��������
            int time = Patient.Illness.HealingTime;

            //�������� ��������
            await Task.Delay(time * 1000);
            //��������� MessageBox � ����� ����, ��� ������ �� ��������� ��� ��������
            string message = $"������� �{Id} ������ ��������� �������� {Patient.FullName} �� {Patient.Illness.Name} � ����� ��� ��������";
            writeMessageBox.Show(message, "", MessageBoxButtons.OK).Start();

            //������� ������
            Status = Status.Free;
            //��������� ��������
            Patient = null;
            //���������� +1 �� ���������� ��������
            ++RecoveredPatients;
        }
 public bool ChooseBrigadeForPatient(Patient patient)
        {
            //��� �� ������ �� ������� ������� ������������: ��� ����� �������� - ��� �������
            Specializations specialization;
            if (patient.Illness.Priority == Priorities.High)
                specialization = Specializations.HighQualityBrigade;
            else
                specialization = Specializations.Brigade;
            //��� �� ��������� ������ GetBrigades �� �������� ������� ���� �������� ������������
            BindingList<Brigade> brigades = GetBrigades(specialization);

            //���� ���� ������ ������
            if (brigades.Count <= 0)
            {
                MessageBox.Show($"����� ������ ������ ������� ������������ ����", "�����", MessageBoxButtons.OK);
                return false;
            }
            //���� ������� ������� ������������ �, �� ����������� ����� ������ �������
            FormChooseBrigade formChoiceBrigade = new FormChooseBrigade(brigades);
            if (formChoiceBrigade.ShowDialog() == DialogResult.OK)//���� �� ������� � ������ ��
            {
                //� ����� FormChooseBrigade � ����� GetSelectedBrigade ���� ����� ��� ������� �������
                Brigade brigade = formChoiceBrigade.GetSelectedBrigade();
                //��� ����������� ����� TakePatient � ������ ������, �� ��� � ����� �� ����������� �������
                Task.Run(() => TakePatient(brigade, patient));
                return true;
            }
            else
                return false;
        }
        public static Patient Generate()
        {
        public static int MinTime  = 15;
        public static int MaxTime  = 30;
            //������ ������ ���������� �������� � ��� ����
            int endIndexOfFirstNames = Enum.GetNames(typeof(FirstNames)).Length - 1;
            //������ ������ ���������� �������� � ��� ������
            int endIndexOfLastNames = Enum.GetNames(typeof(FirstNames)).Length - 1;

            var firstNameGenerator = new Random();
            var lastNameGenerator = new Random();

            //�������� ��'� �� �������
            string firstName = Enum.GetName(typeof(FirstNames), firstNameGenerator.Next(0, endIndexOfFirstNames));
            string lastName = Enum.GetName(typeof(LastNames), lastNameGenerator.Next(0, endIndexOfLastNames));

            write return new Patient()
            {
                FullName = $"{lastName} {firstName}",
                Illness = GenerateIllness(), //������� �������� ���� �������
                Distance = new Random().Next(5, 50), //������� ��������� �� ��������
                new Random().Next(MinTime, MaxTime);
            };
        }