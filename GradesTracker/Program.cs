using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

 /*
  * Program: Grade Tracking System Application		
  * Purpose: stores and calculates all evaluations for each courses in a list			
  * Coder: Haris		
  * Date: June 7th, 2021			
  */

namespace GradesTracker
{
    class Program
    {
        public const string JSON_FILE = "grades.json";
        public const string SCHEMA_FILE = "grades-schema.json";
        static void Main(string[] args)
        {        
            string json_schema;
            if (TrackerFunctions.ReadFile(SCHEMA_FILE, out json_schema))
            {
                string json_data;
                if (!TrackerFunctions.ReadFile(JSON_FILE, out _))
                {
                    Console.Write("Grades data file 'grade.json' not found. Create new file? (y/n): ");
                    string input = Console.ReadKey().KeyChar.ToString().ToLower();

                    switch (input)
                    {
                        case "y":
                            File.WriteAllText(JSON_FILE, "[]");
                            Console.WriteLine("\n\n'grades.json' has been created.\n");
                            break;
                        case "n":
                            Console.WriteLine("\nExiting...");
                            return;
                    }
                }

                bool flagA = true;
                do
                {
                    TrackerFunctions.ReadFile(JSON_FILE, out json_data);
                    JArray allCourses = JsonConvert.DeserializeObject<JArray>(json_data);
                    TrackerFunctions.displayCourses(allCourses);
                    

                    string commandInput = Console.ReadKey().KeyChar.ToString().ToUpper();
                                 
                    switch (commandInput)
                    {
                        case "X":
                            Console.WriteLine("\nQuiting...");
                            flagA = false;
                            break;
                        case "A":
                            TrackerFunctions.addCourseCode(allCourses, json_schema);
                            break;
                    }

                    int courseNo;
                    bool isCourseNoValid = int.TryParse(commandInput, out courseNo);

                    if(isCourseNoValid)
                    {
                        bool flagB = true;
                        do
                        {
                            TrackerFunctions.displayEvaluations(allCourses, courseNo);

                            commandInput = Console.ReadKey().KeyChar.ToString().ToUpper();
                            

                            switch (commandInput)
                            {
                                case "X":
                                    flagB = false;
                                    break;
                                case "A":
                                    if(TrackerFunctions.addEvaluation(allCourses, json_schema, courseNo)) { TrackerFunctions.ReadFile(JSON_FILE, out _); }
                                    break;
                                case "D":
                                    TrackerFunctions.deleteCourse(allCourses, courseNo);
                                    break;
                            }

                            int evalNo;
                            bool isEvalNoValid = int.TryParse(commandInput, out evalNo);

                            if (isEvalNoValid)
                            {
                                bool flagC = true;
                                do
                                {
                                    TrackerFunctions.displaySingleCourseEvaluation(allCourses, courseNo, evalNo);

                                    commandInput = Console.ReadKey().KeyChar.ToString().ToUpper();

                                    switch (commandInput)
                                    {
                                        case "X":
                                            flagC = false;
                                            break;
                                        case "E":
                                            if (TrackerFunctions.editEvaluation(allCourses, json_schema, courseNo, evalNo)) { TrackerFunctions.ReadFile(JSON_FILE, out _); } 
                                            break;
                                        case "D":
                                            if (TrackerFunctions.deleteEvaluation(allCourses, json_schema, courseNo, evalNo)) { TrackerFunctions.ReadFile(JSON_FILE, out _); }
                                            break;
                                    }
                                }
                                while (flagC == true);
                            }
                        }
                        while (flagB == true);
                    }
                }
                while (flagA == true);
            }
            else
            {
                Console.Write("\nCouldn't find schema file. Exiting..");
                return;
            }          
        }
    }
}
