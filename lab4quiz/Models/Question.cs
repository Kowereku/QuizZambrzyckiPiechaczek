using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4quiz.Models
{
    public class Question
    {
        public string Text { get; set; }
        public ObservableCollection<Answer> Answers = new ();
    }
}
