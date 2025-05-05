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

namespace lab4quiz.ViewModel
{
    public class QuizSolverViewModel : ViewModelBase
    {
        public List<Question> AllQuestions { get; set; } = new List<Question>();
        private int _currentQuestionIndex = 0;
        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged();
            }
        }

        public string TimerDisplay { get; set; } = "00:00";
        private Timer _timer;
        private int _secondsElapsed = 0;

        public ICommand NextQuestionCommand { get; }
        public ICommand LoadQuizCommand { get; }

        public bool IsLastQuestion => _currentQuestionIndex == AllQuestions.Count - 1;

        public QuizSolverViewModel()
        {
            NextQuestionCommand = new RelayCommand(NextQuestion);
            LoadQuizCommand = new RelayCommand(LoadQuiz);
        }

        private void LoadQuiz()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Quiz files (*.quiz)|*.quiz|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var quiz = AESCipher.DecryptFromFile<Quiz>(dialog.FileName);
                AllQuestions = quiz.Questions.ToList();

                _currentQuestionIndex = 0;
                CurrentQuestion = AllQuestions[_currentQuestionIndex];

                StartTimer();
                OnPropertyChanged(nameof(AllQuestions));
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(IsLastQuestion));
            }
        }

        private void StartTimer()
        {
            _secondsElapsed = 0;
            _timer = new Timer(UpdateTimer, null, 0, 1000);
        }

        private void UpdateTimer(object state)
        {
            _secondsElapsed++;
            TimerDisplay = TimeSpan.FromSeconds(_secondsElapsed).ToString(@"mm\:ss");
            OnPropertyChanged(nameof(TimerDisplay));
        }

        private void NextQuestion()
        {
            if (IsLastQuestion)
            {
                EndQuiz();
                return;
            }

            _currentQuestionIndex++;
            CurrentQuestion = AllQuestions[_currentQuestionIndex];
            OnPropertyChanged(nameof(IsLastQuestion));
        }

        private void EndQuiz()
        {
            _timer?.Dispose();
            int score = AllQuestions.Count(q => q.Answers.All(a => a.IsSelected == a.IsCorrect));
            string result = $"Wynik: {score} / {AllQuestions.Count}\n\nPoprawne odpowiedzi:\n\n";

            foreach (var q in AllQuestions)
            {
                result += $"{q.Text}\n";
                foreach (var a in q.Answers)
                {
                    if (a.IsCorrect)
                        result += $"✓ {a.Text}\n";
                }
                result += "\n";
            }

            MessageBox.Show(result, "Wynik", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
