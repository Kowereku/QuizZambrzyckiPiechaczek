using lab4quiz.Models;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using lab4quiz.ViewModel.Base;
using Microsoft.Win32;
using System.IO;

namespace lab4quiz.ViewModel
{
    public class QuizSolverViewModel : ViewModelBase
    {
        public ObservableCollection<Question> Questions { get; set; }
        public ICommand StartQuizCommand { get; }
        public ICommand EndQuizCommand { get; }
        public ICommand LoadQuizCommand { get; }

        public string TimerDisplay { get; set; } = "00:00";
        private Timer _timer;
        private int _secondsElapsed = 0;

        public QuizSolverViewModel()
        {
            StartQuizCommand = new RelayCommand(StartQuiz);
            EndQuizCommand = new RelayCommand(EndQuiz);
            LoadQuizCommand = new RelayCommand(LoadQuiz);
        }

        private void StartQuiz()
        {
            _secondsElapsed = 0;
            _timer = new Timer(UpdateTimer, null, 0, 1000);
            OnPropertyChanged(nameof(Questions));
        }

        private void StartTimer()
        {
            _secondsElapsed = 0;
            _timer?.Dispose();
            _timer = new Timer(UpdateTimer, null, 0, 1000);
        }

        private void UpdateTimer(object state)
        {
            _secondsElapsed++;
            TimerDisplay = TimeSpan.FromSeconds(_secondsElapsed).ToString(@"mm\:ss");
            OnPropertyChanged(nameof(TimerDisplay));
        }

        private void EndQuiz()
        {
            _timer?.Dispose();

            int correctAnswers = 0;
            foreach (var question in Questions)
            {
                bool allCorrect = question.Answers.All(a => a.IsCorrect == a.IsSelected);
                if (allCorrect) correctAnswers++;
            }

            var msg = $"Twój wynik: {correctAnswers} / {Questions.Count}\nCzas: {TimerDisplay}";
            MessageBox.Show(msg, "Wynik", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadQuiz()
        {
            string basePath = AppContext.BaseDirectory;
            string projectPath = Path.GetFullPath(Path.Combine(basePath, "SavedQuizes"));
            var dialog = new OpenFileDialog
            {
                InitialDirectory = projectPath,
                Filter = "Quiz files (*.quiz)|*.quiz|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var quiz = AESCipher.DecryptFromFile<Quiz>(dialog.FileName);
                Questions = new ObservableCollection<Question>(
                    quiz.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        Answers = new ObservableCollection<Answer>(
                            q.Answers.Select(a => new Answer
                            {
                                Text = a.Text,
                                IsCorrect = a.IsCorrect,
                                IsSelected = false
                            })
                        )
                    })
                );


                foreach (var q in Questions)
                {
                    foreach (var a in q.Answers)
                        a.IsSelected = false;
                }

                OnPropertyChanged(nameof(Questions));

                StartTimer();
            }
        }

    }
}
