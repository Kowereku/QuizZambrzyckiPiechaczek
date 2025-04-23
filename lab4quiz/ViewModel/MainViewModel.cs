using lab4quiz.Views;
using lab4quiz.ViewModel.Base;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace lab4quiz.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowEditorViewCommand { get; }
        public ICommand ShowSolverViewCommand { get; }

        private readonly QuizEditorViewModel _generatorViewModel = new();
        private readonly QuizSolverViewModel _solverViewModel = new();

        public MainViewModel()
        {
            ShowEditorViewCommand = new RelayCommand(() => CurrentViewModel = _generatorViewModel);
            ShowSolverViewCommand = new RelayCommand(() => CurrentViewModel = _solverViewModel);

            CurrentViewModel = _generatorViewModel;
        }
    }
}
