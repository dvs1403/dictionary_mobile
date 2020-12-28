using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.Models
{
    public class Word
    {
        public string FirstWord { get; set; }
        public string SecondWord { get; set; }
        public bool IsLearn { get; set; }

        public Word(string first, string second)
        {
            FirstWord = first;
            SecondWord = second;
            IsLearn = false;
        }
    }
}
