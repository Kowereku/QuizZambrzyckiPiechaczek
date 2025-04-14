using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab4quiz.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public ObservableObject CurrentViewModel { get; set; }
        public ICommand ShowEditorCommand { get; }
        public ICommand ShowSolverCommand { get; }

        public MainViewModel()
        {
            ShowEditorCommand = new RelayCommand(() => CurrentViewModel = new QuizEditorViewModel());
            ShowSolverCommand = new RelayCommand(() => CurrentViewModel = new QuizSolverViewModel());
        }
    }
}
