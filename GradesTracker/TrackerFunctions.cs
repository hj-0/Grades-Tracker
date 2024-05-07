using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace GradesTracker
{
    class TrackerFunctions
    {
        public static void displayCourses(JArray courses)
        {

            try
            {
                Console.WriteLine("\n\t\t\t~ GRADES TRACKING SYSTEM ~\n");
                Console.WriteLine("+-----------------------------------------------------------------+");
                Console.WriteLine("|                          Grades Summary                         |");
                Console.WriteLine("+-----------------------------------------------------------------+");

                string json_data = JsonConvert.SerializeObject(courses);

                if (json_data != "[]")
                {
                    int counter = 1;
                    string line = "";
                    double weight = 0.0;
                    int outOf = 0;
                    double earnedMarks = 0.0;
                    Console.WriteLine();
                    line += "#.";
                    line += string.Format("{0,15}", "Course");
                    line += string.Format("{0,20}", "Marks Earned");
                    line += string.Format("{0,15}", "Out of");
                    line += string.Format("{0,15}", "Percent");
                    Console.WriteLine(line);
                    Console.WriteLine();
                    line = "";

                    foreach (JObject courseList in courses)
                    {
                        Console.Write($"{counter++}.");
                        line += string.Format("{0,15}", courseList.GetValue("Code"));

                        if (courseList.GetValue("Evaluations").ToString() != "[]")
                        {
                            foreach (JObject eval in courseList.GetValue("Evaluations"))
                            {
                                double tempEarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());
                                int tempOutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                double tempWeight = Double.Parse(eval.GetValue("Weight").ToString());

                                double percent = Math.Round((double)tempEarnedMarks / tempOutOf * 100, 1);
                                double courseMarks = Math.Round((double)tempWeight * percent / 100.0, 1);
                                earnedMarks += (double)courseMarks;
                                outOf += (int)tempWeight;
                            }

                        }
                        weight = Math.Round((double)earnedMarks / outOf * 100, 1);

                        line += string.Format("{0,20:0.0}", Math.Round(earnedMarks, 1));
                        line += string.Format("{0,15:0.0}", outOf);

                        if (double.IsNaN(weight)) { weight = 0.0; }
                        line += string.Format("{0,15:0.0}", weight);

                        Console.WriteLine(line);
                        line = "";
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("\nThere are currently no saved courses.\n");
                }
                coursesCommands();
            }
            catch (JsonException msg)
            {
                Console.WriteLine("\nError has occured.");
                Console.WriteLine($"{msg.Message}");

            }
        }

        public static void addCourseCode(JArray course, string json_schema)
        {
            bool flag = true;

            do
            {
                Course courseCode = new Course();
                string input;
                Console.Write("\nEnter a course code: ");
                input = Console.ReadLine();
                courseCode.Code = input;

                IList<string> messages;
                if (ValidateData(courseCode, json_schema, out messages))
                {
                    Console.WriteLine($"{courseCode.Code} course has been added\n");
                    course.Add(JToken.FromObject(courseCode));
                    File.WriteAllText(Program.JSON_FILE, course.ToString());
                    flag = false;
                }
                else
                {
                    Console.WriteLine("ERROR: Invalid Course Code.");
                    foreach (string msg in messages)
                    {
                        Console.WriteLine($"{msg}");
                    }

                }
            }
            while (flag == true);
        }

        public static void displayEvaluations(JArray course, int courseID)
        {
            try
            {
                int counter = 1;
                foreach (JObject evalList in course)
                {
                    if (counter++ == courseID)
                    {
                        Console.WriteLine("\n\t\t\t~ GRADES TRACKING SYSTEM ~\n");
                        Console.WriteLine("+------------------------------------------------------------------------------+");
                        Console.WriteLine($"|                          {evalList.GetValue("Code")} Evaluations                               |");
                        Console.WriteLine("+------------------------------------------------------------------------------+\n");

                        if (evalList.GetValue("Evaluations").ToString() == "[]")
                        {
                            Console.WriteLine("There are currently no evaluations for " + evalList.GetValue("Code") + ".\n");
                            break;
                        }
                        else
                        {
                            string line = "";
                            double percent = 0.0;
                            double courseMarks = 0.0;
                            int courseCounter = 1;
                            Console.Write("#.");
                            line += string.Format("{0,13}", "Evaluation");
                            line += string.Format("{0,15}", "Marks Earned");
                            line += string.Format("{0,10}", "Out of");
                            line += string.Format("{0,10}", "Percent");
                            line += string.Format("{0,15}", "Course Marks");
                            line += string.Format("{0,15}", "Weight/100");
                            Console.WriteLine(line);
                            Console.WriteLine();
                            line = "";

                            foreach (JObject eval in evalList.GetValue("Evaluations"))
                            {
                                line += string.Format("{0,13:0.0}", eval.GetValue("Description"));
                                if (Double.Parse(eval.GetValue("EarnedMarks").ToString()) == 0.0)
                                {
                                    line += string.Format("{0,13:0.0}", "");

                                }
                                else
                                {
                                    line += string.Format("{0,13:0.0}", eval.GetValue("EarnedMarks"));
                                }
                                line += string.Format("{0,10:0.0}", eval.GetValue("OutOf"));


                                double tempEarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());
                                int tempOutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                double tempWeight = Double.Parse(eval.GetValue("Weight").ToString());

                                if (eval.GetValue("EarnedMarks").ToString() != "")
                                {
                                    percent = Math.Round(100 * (double)tempEarnedMarks / tempOutOf, 1);
                                    courseMarks = Math.Round((double)tempWeight * percent / 100.0, 1);
                                }

                                line += string.Format("{0,10:0.0}", percent);
                                line += string.Format("{0,15:0.0}", courseMarks);
                                line += string.Format("{0,15:0.0}", tempWeight);
                                Console.Write($"{courseCounter++}.");
                                Console.Write(line);
                                line = "";
                                Console.WriteLine();
                            }
                        }
                    }
                }
                evaluationsCommands();
            }
            catch (JsonException msg)
            {
                Console.WriteLine("\nError has occured.");
                Console.WriteLine($"{msg.Message}");

            }

        }

        public static bool addEvaluation(JArray course, string json_schema, int courseID)
        {

            Evaluation newEvaluation = new Evaluation();

            string input;
            int counter = 1;
            bool flag = true;

            do
            {
                foreach (JObject list in course)
                {
                    if (counter++ == courseID)
                    {
                        Course courseCode = new Course();
                        courseCode.Code = list.GetValue("Code").ToString();

                        if (list.GetValue("Evaluations").ToString() != "[]")
                        {
                            foreach (JObject eval in list.GetValue("Evaluations"))
                            {
                                Evaluation evaluation = new Evaluation();
                                evaluation.Description = eval.GetValue("Description").ToString();
                                evaluation.Weight = Double.Parse(eval.GetValue("Weight").ToString());
                                evaluation.OutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                evaluation.EarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());

                                courseCode.Evaluations.Add(evaluation);
                            }

                        }

                        Console.Write("\nEnter a Description: ");
                        input = Console.ReadLine();
                        newEvaluation.Description = input;

                        Console.Write("Enter the 'out of' mark: ");
                        input = Console.ReadLine();
                        int outOfTemp;

                        if (input != "")
                        {
                            bool isInteger = int.TryParse(input, out outOfTemp);

                            if (isInteger)
                            {
                                newEvaluation.OutOf = outOfTemp;
                            }
                        }

                        Console.Write("Enter the % weight: ");
                        input = Console.ReadLine();
                        double weightTemp;

                        if (input != "")
                        {
                            bool isDouble = Double.TryParse(input, out weightTemp);

                            if (isDouble)
                            {
                                newEvaluation.Weight = weightTemp;
                            }
                        }

                        Console.Write("Enter marks earned or press ENTER to skip: ");
                        input = Console.ReadLine();
                        double earnedMarksTemp;

                        if (input != "")
                        {
                            bool isDouble = Double.TryParse(input, out earnedMarksTemp);

                            if (isDouble)
                            {
                                newEvaluation.EarnedMarks = earnedMarksTemp;
                            }
                        }
                        else
                        {
                            newEvaluation.EarnedMarks = 0.0;
                        }

                        courseCode.Evaluations.Add(newEvaluation);
                        course.RemoveAt(courseID - 1);
                        course.Insert(courseID - 1, JToken.FromObject(courseCode));

                        IList<string> messages;
                        if (ValidateData(courseCode, json_schema, out messages))
                        {
                            File.WriteAllText(Program.JSON_FILE, course.ToString());
                            Console.WriteLine("Evaluation added!\n");
                            flag = false;
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Invalid Evaluation.");
                            foreach (string msg in messages)
                            {
                                Console.WriteLine($"{msg}");
                            }
                        }
                        courseCode.Evaluations.Remove(newEvaluation);
                        course.RemoveAt(courseID - 1);
                        course.Insert(courseID - 1, JToken.FromObject(courseCode));
                    }
                }
            }
            while (flag == true);
            return false;
        }

        public static void deleteCourse(JArray course, int courseID)
        {
            string input;

            Console.Write("\nDo you want to delete this course? (Y/N) ");
            input = Console.ReadLine();

            if (input.ToUpper() == "Y")
            {
                course.RemoveAt(courseID - 1);
                File.WriteAllText(Program.JSON_FILE, course.ToString());
                Console.WriteLine("Course Deleted!\n");
            }
            else { return; }

        }

        public static void displaySingleCourseEvaluation(JArray course, int courseID, int evalID)
        {
            try
            {
                int courseCounter = 1;

                foreach (JObject evalList in course)
                {
                    if (courseCounter++ == courseID)
                    {
                        foreach (JObject evalTemp in evalList.GetValue("Evaluations"))
                        {
                            Console.WriteLine("\n\t\t~ GRADES TRACKING SYSTEM ~\n");
                            Console.WriteLine("+------------------------------------------------------------------+");
                            Console.WriteLine($"                  {evalList.GetValue("Code")} {evalTemp.GetValue("Description")}");
                            Console.WriteLine("+------------------------------------------------------------------+\n");
                            break;
                        }

                        if (evalList.GetValue("Evaluations").ToString() == "[]")
                        {
                            Console.WriteLine("There are currently no evaluations for " + evalList.GetValue("Code") + ".\n");
                            break;
                        }
                        else
                        {

                            string line = "";
                            double percent = 0.0;
                            double courseMarks = 0.0;
                            line += string.Format("{0,13}", "Marks Earned");
                            line += string.Format("{0,10}", "Out of");
                            line += string.Format("{0,10}", "Percent");
                            line += string.Format("{0,15}", "Course Marks");
                            line += string.Format("{0,13}", "Weight/100");
                            Console.WriteLine(line);
                            Console.WriteLine();
                            line = "";

                            int evalCounter = 1;
                            foreach (JObject eval in evalList.GetValue("Evaluations"))
                            {
                                if (evalCounter++ == evalID)
                                {
                                    if (Double.Parse(eval.GetValue("EarnedMarks").ToString()) == 0.0)
                                    {
                                        line += string.Format("{0,13:0.0}", "");

                                    }
                                    else
                                    {
                                        line += string.Format("{0,13:0.0}", eval.GetValue("EarnedMarks"));
                                    }                                  
                                    line += string.Format("{0,10:0.0}", eval.GetValue("OutOf"));

                                    double tempEarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());
                                    int tempOutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                    double tempWeight = Double.Parse(eval.GetValue("Weight").ToString());

                                    if (eval.GetValue("EarnedMarks").ToString() != "")
                                    {
                                        percent = Math.Round((double)tempEarnedMarks / tempOutOf * 100, 1);
                                        courseMarks = Math.Round((double)tempWeight * percent / 100.0, 1);
                                        line += string.Format("{0,10:0.0}", percent);
                                        line += string.Format("{0,15:0.0}", courseMarks);
                                    }

                                    line += string.Format("{0,13:0.0}", tempWeight);
                                    Console.Write(line);
                                    line = "";
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                }
                singleCourseCommands();
            }
            catch (JsonException msg)
            {
                Console.WriteLine("\nError has occured.");
                Console.WriteLine($"{msg.Message}");

            }

        }

        public static bool editEvaluation(JArray course, string json_schema, int courseID, int evalID)
        {

            Evaluation editEvaluation = new Evaluation();

            string input;
            int courseCount = 1;
            bool flag = true;

            do
            {
                foreach (JObject list in course)
                {
                    if (courseCount++ == courseID)
                    {
                        Course courseCode = new Course();
                        courseCode.Code = list.GetValue("Code").ToString();

                        if (list.GetValue("Evaluations").ToString() != "[]")
                        {
                            int evalCount = 1;
                            foreach (JObject eval in list.GetValue("Evaluations"))
                            {
                                if (evalCount++ == evalID)
                                {
                                    editEvaluation.Description = eval.GetValue("Description").ToString();
                                    editEvaluation.Weight = Double.Parse(eval.GetValue("Weight").ToString());
                                    editEvaluation.OutOf = int.Parse(eval.GetValue("OutOf").ToString());

                                    bool validFlag = true;
                                    do
                                    {
                                        Console.Write($"\nEnter marks out of {eval.GetValue("OutOf")}, press ENTER to leave unassigned: ");
                                        input = Console.ReadLine();
                                        double earnedMarksTemp;

                                        if (input != "")
                                        {
                                            bool isDouble = Double.TryParse(input, out earnedMarksTemp);

                                            if (isDouble)
                                            {
                                                editEvaluation.EarnedMarks = earnedMarksTemp;
                                                validFlag = false;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Please enter a valid value and try again.");
                                            }
                                        }
                                        else
                                        {
                                            editEvaluation.EarnedMarks = 0.0;
                                        }
                                    }
                                    while (validFlag == true);

                                    courseCode.Evaluations.Add(editEvaluation);
                                    course.RemoveAt(courseID - 1);
                                    course.Insert(courseID - 1, JToken.FromObject(courseCode));

                                    IList<string> messages;
                                    if (ValidateData(courseCode, json_schema, out messages))
                                    {
                                        File.WriteAllText(Program.JSON_FILE, course.ToString());
                                        Console.WriteLine("Evaluation edited!\n");
                                        flag = false;
                                        return true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR: Invalid Evaluation.");
                                        foreach (string msg in messages)
                                        {
                                            Console.WriteLine($"{msg}");
                                        }
                                    }
                                }
                                else
                                {
                                    Evaluation evaluation = new Evaluation();
                                    evaluation.Description = eval.GetValue("Description").ToString();
                                    evaluation.Weight = Double.Parse(eval.GetValue("Weight").ToString());
                                    evaluation.OutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                    evaluation.EarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());

                                    courseCode.Evaluations.Add(evaluation);
                                }
                            }
                        }
                        courseCode.Evaluations.Remove(editEvaluation);
                        course.RemoveAt(courseID - 1);
                        course.Insert(courseID - 1, JToken.FromObject(courseCode));
                    }
                }
            }
            while (flag == true);
            return false;
        }
        public static bool deleteEvaluation(JArray course, string json_schema, int courseID, int evalID) 
        {
            string input;
            int courseCount = 1;
            bool flag = true;
            do
            {
                foreach(JObject list in course)
                {
                    if (courseCount++ == courseID)
                    {
                        Course courseCode = new Course();
                        courseCode.Code = list.GetValue("Code").ToString();

                        if (list.GetValue("Evaluations").ToString() != "[]")
                        {
                            int evalCount = 1;
                            foreach (JObject eval in list.GetValue("Evaluations"))
                            {
                                if (evalCount++ != evalID)
                                {
                                    Evaluation evaluation = new Evaluation();
                                    evaluation.Description = eval.GetValue("Description").ToString();
                                    evaluation.Weight = Double.Parse(eval.GetValue("Weight").ToString());
                                    evaluation.OutOf = int.Parse(eval.GetValue("OutOf").ToString());
                                    evaluation.EarnedMarks = Double.Parse(eval.GetValue("EarnedMarks").ToString());

                                    courseCode.Evaluations.Add(evaluation);
                                }
                            }

                        }

                        Console.Write("\nDelete you want to delete this evaluation? (Y/N) ");
                        input = Console.ReadLine();

                        if (input.ToUpper() == "Y")
                        {
                            course.RemoveAt(courseID - 1);
                            course.Insert(courseID - 1, JToken.FromObject(courseCode));

                            IList<string> messages;
                            if (ValidateData(courseCode, json_schema, out messages))
                            {
                                File.WriteAllText(Program.JSON_FILE, course.ToString());
                                Console.WriteLine("Evaluation deleted!\n");
                                flag = false;
                                return true;

                            }
                            else
                            {
                                Console.WriteLine("ERROR: Invalid Evaluation.");
                                foreach (string msg in messages)
                                {
                                    Console.WriteLine($"{msg}");
                                }
                            }
                        }
                        else { return false; }                      
                    }
                }
            }
            while (flag == true);
            return false;
        }

        private static void coursesCommands()
        {

            Console.WriteLine($"\n-------------------------------------------------------------------");
            Console.WriteLine("Press # from the above list to view/edit/delete a specific course.");
            Console.WriteLine("Press A to add a new course.");
            Console.WriteLine("Press X to quit.");
            Console.WriteLine($"-------------------------------------------------------------------");
            Console.Write("Enter a command: ");

        }

        private static void evaluationsCommands()
        {
            Console.WriteLine($"\n--------------------------------------------------------------------------------");
            Console.WriteLine("Press D to delete this course.");
            Console.WriteLine("Press A to add an evaluation.");
            Console.WriteLine("Press # from the above list to view/edit/delete a specific evaluation.");
            Console.WriteLine("Press X to return to the main menu.");
            Console.WriteLine($"--------------------------------------------------------------------------------");
            Console.Write("Enter a command: ");
        }

        private static void singleCourseCommands()
        {
            Console.WriteLine($"\n--------------------------------------------------------------------------------");
            Console.WriteLine("Press D to delete this evaluation.");
            Console.WriteLine("Press E to add edit this evaluation.");
            Console.WriteLine("Press X to return to the previous menu.");
            Console.WriteLine($"--------------------------------------------------------------------------------");
            Console.Write("Enter a command: ");
        }
        private static bool ValidateData<T>(T data, string json_schema, out IList<string> messages)
        {
            string json_data = JsonConvert.SerializeObject(data);
            JSchema schema = JSchema.Parse(json_schema);
            JObject courses = JObject.Parse(json_data);
            return courses.IsValid(schema, out messages);
        }

        public static bool ReadFile(string path, out string json)
        {
            try
            {
                json = File.ReadAllText(path);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }
    }
}
