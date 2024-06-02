using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1.Model
{
    public class Student
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public Dictionary<int, List<string>> SubjectsPerSemester { get; set; }
        private List<Dictionary<string, int>> grades;

        public Student(string lastName, string firstName, DateTime dateOfBirth, int course, string group, Dictionary<int, List<string>> subjectsPerSemester)
        {
            LastName = lastName;
            FirstName = firstName;
            DateOfBirth = dateOfBirth;
            Course = course;
            Group = group;
            SubjectsPerSemester = subjectsPerSemester;
            grades = new List<Dictionary<string, int>>();

            foreach (var semester in SubjectsPerSemester.Keys)
            {
                var semesterGrades = new Dictionary<string, int>();
                foreach (var subject in SubjectsPerSemester[semester])
                {
                    semesterGrades[subject] = 0;
                }
                grades.Add(semesterGrades);
            }
        }

        public int this[int semester, string subject]
        {
            get
            {
                if (grades.Count >= semester && SubjectsPerSemester.ContainsKey(semester) && SubjectsPerSemester[semester].Contains(subject))
                {
                    return grades[semester - 1][subject];
                }
                else
                {
                    throw new ArgumentException("Invalid semester or subject");
                }
            }
            set
            {
                if (grades.Count >= semester && SubjectsPerSemester.ContainsKey(semester) && SubjectsPerSemester[semester].Contains(subject))
                {
                    grades[semester - 1][subject] = value;
                }
                else
                {
                    throw new ArgumentException("Invalid semester or subject");
                }
            }
        }

        public void AddGrade(int semester, string subject, int grade)
        {
            if (grades.Count >= semester && SubjectsPerSemester.ContainsKey(semester) && SubjectsPerSemester[semester].Contains(subject))
            {
                grades[semester - 1][subject] = grade;
            }
            else
            {
                throw new ArgumentException("Invalid semester or subject");
            }
        }

        public double CalculateAverageGrade()
        {
            double totalGrade = 0;
            int totalSubjects = 0;

            foreach (var semesterGrades in grades)
            {
                foreach (var grade in semesterGrades.Values)
                {
                    totalGrade += grade;
                    totalSubjects++;
                }
            }

            return totalGrade / totalSubjects;
        }

        public double CalculateAverageGradeForSubject(string subject)
        {
            double totalGrade = 0;
            int totalSubjects = 0;

            foreach (var semesterGrades in grades)
            {
                if (semesterGrades.ContainsKey(subject))
                {
                    totalGrade += semesterGrades[subject];
                    totalSubjects++;
                }
            }

            if (totalSubjects == 0)
            {
                throw new ArgumentException("No grades found for the specified subject.");
            }

            return totalGrade / totalSubjects;
        }

        public List<Tuple<int, string, int>> GetSubjectsInDebtWithGrades()
        {
            var subjectsInDebtWithGrades = new List<Tuple<int, string, int>>();

            for (int i = 0; i < grades.Count; i++)
            {
                foreach (var subjectGrade in grades[i])
                {
                    if (subjectGrade.Value < 50)
                    {
                        subjectsInDebtWithGrades.Add(new Tuple<int, string, int>(i + 1, subjectGrade.Key, subjectGrade.Value));
                    }
                }
            }

            return subjectsInDebtWithGrades;
        }


        public List<string> GetAllSubjects()
        {
            var allSubjects = new List<string>();

            foreach (var semesterSubjects in SubjectsPerSemester.Values)
            {
                allSubjects.AddRange(semesterSubjects);
            }

            return allSubjects.Distinct().ToList();
        }

        public Dictionary<string, int> GetAllGradesForSubject(string subject)
        {
            var allGradesForSubject = new Dictionary<string, int>();

            foreach (var semesterGrades in grades)
            {
                foreach (var grade in semesterGrades)
                {
                    if (grade.Key == subject)
                    {
                        allGradesForSubject.Add($"Semester {grades.IndexOf(semesterGrades) + 1}", grade.Value);
                    }
                }
            }

            return allGradesForSubject;
        }
    }
}
