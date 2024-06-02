using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Task1.Model;

namespace Task1.ViewModel
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly Student student;
        private int selectedSemester;
        private string selectedSubject;
        private int selectedSubjectGrade;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> GradesStr { get; set; }
        public ObservableCollection<int> Semesters { get; set; }
        private ObservableCollection<string> subjects;
        public ObservableCollection<string> Subjects
        {
            get { return subjects; }
            set
            {
                subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        public int SelectedSemester
        {
            get { return selectedSemester; }
            set
            {
                selectedSemester = value;
                OnPropertyChanged(nameof(SelectedSemester));
                UpdateSubjects();
            }
        }

        public string SelectedSubject
        {
            get { return selectedSubject; }
            set
            {
                selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
                UpdateSelectedSubjectGrade();
            }
        }

        public int SelectedSubjectGrade
        {
            get { return selectedSubjectGrade; }
            set
            {
                selectedSubjectGrade = value;
                OnPropertyChanged(nameof(SelectedSubjectGrade));
            }
        }

        public string LastName => student.LastName;
        public string FirstName => student.FirstName;
        public DateTime DateOfBirth => student.DateOfBirth;
        public int Course => student.Course;
        public string Group => student.Group;

        public ICommand CalculateAverageGradeCommand { get; private set; }
        public ICommand CalculateAverageGradeForSubjectCommand { get; private set; }
        public ICommand GetSubjectsInDebtCommand { get; private set; }

        public StudentViewModel()
        {
            var subjectsPerSemester = new Dictionary<int, List<string>>
            {
                {1, new List<string>{"Математический анализ", "История", "Английский", "Физкультура"}},
                {2, new List<string>{"Английский", "Дискретная математика", "Философия","Физкультура","История"}},
                {3, new List<string>{"ТФКП", "Проектирование баз данных", "МИСПИС", "ОКБ","Физкультура"}},
                {4, new List<string>{"АИСД", "ВЕБ", "Теория вероятности", "ОВП","Физкультура"}}
            };
            student = new Student("Головлев", "Гордей", new DateTime(2003, 6, 23), 4, "5", subjectsPerSemester);
            student.AddGrade(1, "Математический анализ", 50);
            student.AddGrade(1, "История", 75);
            student.AddGrade(1, "Английский", 80);
            student.AddGrade(1, "Физкультура", 100);
            student.AddGrade(2, "Английский", 90);
            student.AddGrade(2, "История", 51);
            student.AddGrade(2, "Дискретная математика", 90);
            student.AddGrade(2, "Физкультура", 25);
            student.AddGrade(2, "Философия", 30);
            student.AddGrade(3, "ТФКП", 60);
            student.AddGrade(3, "Проектирование баз данных", 30);
            student.AddGrade(3, "МИСПИС", 100);
            student.AddGrade(3, "ОКБ", 25);
            student.AddGrade(3, "Физкультура", 65);
            student.AddGrade(4, "АИСД", 90);
            student.AddGrade(4, "ВЕБ", 35);
            student.AddGrade(4, "Теория вероятности", 0);
            student.AddGrade(4, "Физкультура", 75);
            student.AddGrade(4, "ОВП", 87);

            GradesStr = new ObservableCollection<string>();
            Semesters = new ObservableCollection<int>(student.SubjectsPerSemester.Keys);
            SelectedSemester = 1;
            UpdateSubjects();

            // Инициализация команд
            CalculateAverageGradeCommand = new RelayCommand(param => CalculateAverageGrade());
            CalculateAverageGradeForSubjectCommand = new RelayCommand(param => CalculateAverageGradeForSubject());
            GetSubjectsInDebtCommand = new RelayCommand(param => GetSubjectsInDebt());
        }

        private void UpdateSubjects()
        {
            if (Semesters.Contains(SelectedSemester))
            {
                Subjects = new ObservableCollection<string>(student.SubjectsPerSemester[SelectedSemester]);
                SelectedSubject = Subjects.FirstOrDefault();
            }
        }

        private void CalculateAverageGrade()
        {
            ClearGrades();
            double averageGrade = Math.Round(student.CalculateAverageGrade(), 2);
            GradesStr.Add($"Средний балл: {averageGrade}");
        }

        private void CalculateAverageGradeForSubject()
        {
            ClearGrades();
            double averageSubjectGrade = Math.Round(student.CalculateAverageGradeForSubject(SelectedSubject), 2);
            GradesStr.Add($"Средний балл по предмету {SelectedSubject}: {averageSubjectGrade}");
        }

        private void GetSubjectsInDebt()
        {
            ClearGrades();
            var subjectsInDebt = student.GetSubjectsInDebtWithGrades();
            if (subjectsInDebt.Count > 0)
            {
                GradesStr.Add("Предметы в долге:");
                foreach (var subject in subjectsInDebt)
                {
                    var debtInfo = $"{subject.Item2} (Семестр: {subject.Item1}, Баллы: {subject.Item3})";
                    GradesStr.Add(debtInfo);
                }
            }
            else
            {
                GradesStr.Add("Нет предметов в долге.");
            }
        }

        private void ClearGrades()
        {
            GradesStr.Clear();
        }

        private void UpdateSelectedSubjectGrade()
        {
            if (SelectedSubject != null)
            {
                SelectedSubjectGrade = student[SelectedSemester, SelectedSubject];
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;

            public RelayCommand(Action<object> execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }
    }
}
