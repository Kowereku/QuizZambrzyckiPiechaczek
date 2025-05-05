using lab4quiz.Models;
using lab4quiz.ViewModel.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace lab4quiz.ViewModel
{
    public class QuizEditorViewModel : ViewModelBase
    {
        public string QuizTitle { get; set; }
        public string NewQuestionText { get; set; }
        private ObservableCollection<Answer> _newAnswers = new() { new Answer(), new Answer(), new Answer(), new Answer() };
        public ObservableCollection<Answer> NewAnswers
        {
            get => _newAnswers;
            set
            {
                _newAnswers = value;
                OnPropertyChanged(nameof(NewAnswers));
            }
        }
        public ObservableCollection<Question> Questions { get; set; } = new();
        public Question SelectedQuestion { get; set; }

        private bool isEditing = false;

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
            if (!ValidateInputs()) return;

            Questions.Add(new Question
            {
                Text = NewQuestionText,
                Answers = new ObservableCollection<Answer>(NewAnswers.Select(a => new Answer
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }))
            });

            NewQuestionText = string.Empty;
            NewAnswers = new ObservableCollection<Answer> { new(), new(), new(), new() };
            OnPropertyChanged(nameof(NewAnswers));
            OnPropertyChanged(nameof(NewQuestionText));

            isEditing = false;
        }

        private void SaveQuiz()
        {
            if (string.IsNullOrWhiteSpace(QuizTitle))
            {
                MessageBox.Show("Uzupełnij tytuł quizu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                var quiz = new Quiz { Title = QuizTitle, Questions = Questions };
                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedQuizes");
                Directory.CreateDirectory(folderPath);
                var fileName = $"{QuizTitle}.quiz";
                var currentDirectory = @"..\SavedQuizes\" + fileName;
                var fullPath = Path.Combine(folderPath, fileName);
                AESCipher.EncryptToFile(quiz, fullPath);
                
                MessageBox.Show($"Quiz zapisany jako {fileName}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                QuizTitle = quiz.Title;
                Questions = quiz.Questions;
                OnPropertyChanged(nameof(QuizTitle));
                OnPropertyChanged(nameof(Questions));
            }

            isEditing = false;
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
            if (isEditing)
            {
                MessageBox.Show("Najpierw zakończ edycję aktualnego pytania.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

                isEditing = true;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(NewQuestionText))
            {
                MessageBox.Show("Uzupełnij treść pytania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NewAnswers.Count != 4 || NewAnswers.Any(a => string.IsNullOrWhiteSpace(a.Text)))
            {
                MessageBox.Show("Uzupełnij wszystkie 4 odpowiedzi.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!NewAnswers.Any(a => a.IsCorrect))
            {
                MessageBox.Show("Zaznacz przynajmniej jedną poprawną odpowiedź.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
