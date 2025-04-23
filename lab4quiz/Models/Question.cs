using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lab4quiz.Models
{
    public class Question
    {
        public string Text { get; set; }
        [JsonInclude]
        public ObservableCollection<Answer> Answers = new ();
    }
}
