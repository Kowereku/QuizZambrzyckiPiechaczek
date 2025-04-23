using lab4quiz.Models;
using lab4quiz.ViewModel.Base;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab4quiz.ViewModel
{
    public class QuizEditorViewModel : ViewModelBase
    {
        public string QuizTitle { get; set; }
        public string NewQuestionText { get; set; }
        public ObservableCollection<Answer> NewAnswers { get; set; } = new()
    {
        new Answer(), new Answer(), new Answer(), new Answer()
    };

        public ObservableCollection<Question> Questions { get; set; } = new();

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }

        public QuizEditorViewModel()
        {
            AddQuestionCommand = new RelayCommand(AddQuestion);
            SaveQuizCommand = new RelayCommand(SaveQuiz);
        }

        private void AddQuestion()
        {
            Questions.Add(new Question
            {
                Text = NewQuestionText,
                Answers = new ObservableCollection<Answer>(NewAnswers.Select(a => new Answer
                {
                    Title = a.Title,
                    IsCorrect = a.IsCorrect
                }))
            });

            NewQuestionText = string.Empty;
            NewAnswers = new ObservableCollection<Answer> { new(), new(), new(), new() };
            OnPropertyChanged(nameof(NewAnswers));
            OnPropertyChanged(nameof(NewQuestionText));
        }

        private void SaveQuiz()
        {
            var quiz = new Quiz { Title = QuizTitle, Questions = Questions };
            AESCipher.EncryptToFile(quiz, $"{QuizTitle}.quiz");
        }
    }
}
