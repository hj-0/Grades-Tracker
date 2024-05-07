using System;
using System.Collections.Generic;
using System.Text;

namespace GradesTracker
{
    class Evaluation
    {      
        public string Description { get; set; }
        public double Weight { get; set; }
        public int OutOf { get; set; }
        public double? EarnedMarks { get; set; }
    }
     class Course
    {
        public string Code { get; set; }

        public List<Evaluation> Evaluations = new List<Evaluation>();
        
    }
    
}
