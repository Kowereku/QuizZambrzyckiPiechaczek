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

        private void LoadQuiz()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Quiz files (*.quiz)|*.quiz|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                Questions = AESCipher.DecryptFromFile<Quiz>(dialog.FileName).Questions;
                OnPropertyChanged(nameof(Questions));
            }
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
            int points = Questions.Count(q => q.Answers.All(a => a.IsCorrect == a.IsSelected));
            MessageBox.Show($"Twój wynik: {points} / {Questions.Count}");
        }
    }
}
