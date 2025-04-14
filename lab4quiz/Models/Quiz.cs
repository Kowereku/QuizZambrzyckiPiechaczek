using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4quiz.Models
{
    public class Quiz
    {
        string Name { get; set; }
        public ObservableCollection<Question> Questions = new (); // different constructor
    }
}
