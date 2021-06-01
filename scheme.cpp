 public async override Task GoToPatientAsync(Patient patient)
        {
            //визначається час подорожі в одну сторону
            int travelTime = Convert.ToInt32(patient.Distance * 100 / AverageDrivingSpeed);

            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            string message = $"Бригаду №{Id} було успішно відпрвлено до пацієнта: {patient} приблизний час прибуття {travelTime * 2} секунд";
            new writeMessageBox.Show(message, "", MessageBoxButtons.OK);
            
            Patient = patient;

            //емуляція подорожі до хворого
            await Task.Delay(travelTime * 1000);


            //емуляція подорожі назад
            await Task.Delay(travelTime * 1000);

            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            message = $"Бригада №{Id} доставила пацієнта: {Patient} до лікарні, вилікує приблизно через {Patient.Illness.HealingTime} секунд";
            writeMessageBox.Show(message, "", MessageBoxButtons.OK)).Start();
        }
 public async override Task HealAsync()
        {
            //час лікування
            int time = Patient.Illness.HealingTime;

            //емуляція лікування
            await Task.Delay(time * 1000);
            //запускаємо MessageBox в інший потік, щоб іконка не ступорила нам програму
            string message = $"Бригада №{Id} успішно вилікувала пацієнта {Patient.FullName} від {Patient.Illness.Name} і тепер вже доступна";
            writeMessageBox.Show(message, "", MessageBoxButtons.OK).Start();

            //змінюємо статус
            Status = Status.Free;
            //звільняємо пацієнта
            Patient = null;
            //добавляємо +1 до вилікуваних пацієнтів
            ++RecoveredPatients;
        }
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
        public static Patient Generate()
        {
        public static int MinTime  = 15;
        public static int MaxTime  = 30;
            //беремо індекс останнього елементу з усіх імен
            int endIndexOfFirstNames = Enum.GetNames(typeof(FirstNames)).Length - 1;
            //беремо індекс останнього елементу з усіх прізвищ
            int endIndexOfLastNames = Enum.GetNames(typeof(FirstNames)).Length - 1;

            var firstNameGenerator = new Random();
            var lastNameGenerator = new Random();

            //генеруємо ім'я та прізвище
            string firstName = Enum.GetName(typeof(FirstNames), firstNameGenerator.Next(0, endIndexOfFirstNames));
            string lastName = Enum.GetName(typeof(LastNames), lastNameGenerator.Next(0, endIndexOfLastNames));

            write return new Patient()
            {
                FullName = $"{lastName} {firstName}",
                Illness = GenerateIllness(), //генеруєм пацієнту нову хворобу
                Distance = new Random().Next(5, 50), //генеруєм дистанціє до пацієнта
                new Random().Next(MinTime, MaxTime);
            };
        }