using lab4quiz.Models;
using lab4quiz.ViewModel.Base;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.Win32;
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
        public Question SelectedQuestion { get; set; }

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }
        public ICommand LoadQuizCommand { get; }
        public ICommand RemoveQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }

        public QuizEditorViewModel()
        {
            AddQuestionCommand = new RelayCommand(AddQuestion);
            SaveQuizCommand = new RelayCommand(SaveQuiz);
            LoadQuizCommand = new RelayCommand(LoadQuiz);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion, () => SelectedQuestion != null);
            EditQuestionCommand = new RelayCommand(EditQuestion, () => SelectedQuestion != null);
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

        private void LoadQuiz()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Quiz files (*.quiz)|*.quiz|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var quiz = AESCipher.DecryptFromFile<Quiz>(dialog.FileName);
                QuizTitle = quiz.Title;
                Questions = quiz.Questions;
                OnPropertyChanged(nameof(QuizTitle));
                OnPropertyChanged(nameof(Questions));
            }
        }

        private void RemoveQuestion()
        {
            if (SelectedQuestion != null)
            {
                Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                OnPropertyChanged(nameof(SelectedQuestion));
            }
        }

        private void EditQuestion()
        {
            if (SelectedQuestion != null)
            {
                NewQuestionText = SelectedQuestion.Text;
                NewAnswers = new ObservableCollection<Answer>(
                    SelectedQuestion.Answers.Select(a => new Answer
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    })
                );

                Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;

                OnPropertyChanged(nameof(NewQuestionText));
                OnPropertyChanged(nameof(NewAnswers));
                OnPropertyChanged(nameof(SelectedQuestion));
            }
        }
    }
}
