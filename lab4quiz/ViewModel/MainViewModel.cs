using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using lab4quiz.Views;
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
        public ICommand OpenGeneratorViewCommand { get; }
        public ICommand OpenSolverViewCommand { get; }

        public MainViewModel()
        {
            OpenGeneratorViewCommand = new RelayCommand(OpenGenerator);
            OpenSolverViewCommand = new RelayCommand(OpenSolver);
        }

        private void OpenGenerator()
        {
            var view = new QuizEditorView { DataContext = new QuizEditorViewModel() };
            new Window { Content = view, Title = "Edytor Quizu", Width = 500, Height = 600 }.Show();
        }

        private void OpenSolver()
        {
            var view = new QuizSolverView { DataContext = new QuizSolverViewModel() };
            new Window { Content = view, Title = "Rozwiązywanie Quizu", Width = 500, Height = 600 }.Show();
        }
    }
}
