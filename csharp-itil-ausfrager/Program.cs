using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace csharp_questions_to_array
{

    class Program
    {

        // Config
        private static bool RANDOMIZE_ANSWERS = true; // Randomize answers so that you can't just memorize the right letter
        private static bool RANDOMIZE_QUESTIONS = true; // Randomize questions so that you can't just memorize the questions in order

        // Needed variables
        private static string umbrellaterm = "Übungen"; // the term that includes all other questions for that field
        private static string[] lines; // All the lines from our .txt file
        private static List<(string umbrella_term, List<single_question> questions)> questions_list = new List<(string umbrella_term, List<single_question> questions)>();

        private static int parse_questions(string umbrella_term, int start_index, string tosearch = "Übung")
        {
            var list = new List<single_question>();

            for (int i = start_index; i < lines.Count();)
            {
                var line = lines[i];

                if (line.Contains($"{umbrellaterm} "))
                {
                    if (list.Count > 0)
                    {
                        Console.WriteLine($"[INFO]: Parsed, questions found: {list.Count()}");

                        questions_list.Add((umbrella_term, list));
                    }

                    return i;
                }
                else if (line.Contains(tosearch))
                {
                    var question_num = int.Parse(line.Substring(tosearch.Length + 1));

                    string question = "", temp = "";

                    // parse the question
                    int x = 1;

                    for (; ; x++)
                    {
                        temp = lines[i + x];

                        if (temp.Length > 3)
                        {
                            if (temp[1] == ')') // answer
                            {
                                break;
                            }

                            question += question.Length == 0 ? temp : "\n" + temp;
                        }
                    }

                    i += x;

                    // done, parse the answers
                    x = i;

                    List<string> answers = new List<string>();

                    string[] answers_temp = new string[4]; // just to order them correctly

                    string answer_temp = "";

                    int answer_count = 0, answer_index = 0, correct_answer_index = -1;

                    for (; ; x++)
                    {
                        temp = lines[x];

                        if (temp.Length > 3 && temp[1] == ')') // answer
                        {
                            if (answer_index < 0 || answer_index > 3)
                            {
                                Console.WriteLine($"[ERROR]: Failure while parsing questions at line index {i}: Invalid answer index...");
                                Console.ReadKey();

                                return -2;
                            }

                            if (answer_temp.Length > 0)
                            {
                                while (answer_temp[0] == ' ') answer_temp = answer_temp.Substring(1);

                                if (answer_temp.Length > 0)
                                {
                                    answers_temp[answer_index] = answer_temp;
                                    answer_count++;
                                }
                            }

                            answer_index = temp[0] - 'a';
                            answer_temp = temp.Substring(3);
                        }
                        else
                        {
                            if (temp.Length == 0 || temp.Contains("Richtig:"))
                            {
                                if (answer_temp.Length > 0)
                                {
                                    while (answer_temp[0] == ' ') answer_temp = answer_temp.Substring(1);

                                    if (answer_temp.Length > 0)
                                    {
                                        answers_temp[answer_index] = answer_temp;
                                        answer_count++;
                                    }

                                    answer_temp = "";
                                }

                                break;
                            }

                            answer_temp += "\n" + temp;
                        }
                    }

                    if (answer_count != 4)
                    {
                        Console.WriteLine($"[ERROR]: Failure while parsing questions at line index {i}: Didn't find 4 answers...");
                        Console.ReadKey();

                        return -2;
                    }

                    // now, add our answers to our list
                    for (int y = 0; y < 4; y++) answers.Add(answers_temp[y]);

                    i = x;

                    // now, find the correct answer
                    for (; ; )
                    {
                        line = lines[i];

                        if (line.Contains(tosearch))
                        {
                            // already found the next question but didn't get a correct answer
                            Console.WriteLine($"[ERROR]: Failure while parsing questions at line index {i}: Didn't find the correct answer...");
                            Console.ReadKey();

                            return -2;
                        }
                        else if (line.Contains("Richtig: "))
                        {
                            correct_answer_index = line[9] - 'a';

                            if (correct_answer_index < 0 || correct_answer_index > 3)
                            {
                                // invalid correct answer
                                Console.WriteLine($"[ERROR]: Failure while parsing questions at line index {i}: Correct answer is invalid...");
                                Console.ReadKey();

                                return -2;
                            }

                            break;
                        }

                        i++;
                    }

                    list.Add(new single_question(question_num, question, correct_answer_index, answers));
                }

                i++;
            }

            if (list.Count > 0)
            {
                Console.WriteLine($"[INFO]: Parsed, questions found: {list.Count()}");

                questions_list.Add((umbrella_term, list));
            }

            return -1;
        }

        private static bool read_bool_input()
        {
            for (; ; )
            {
                var answer = Console.ReadLine();

                if (answer.Length > 1 || answer.Length <= 0)
                {
                    Console.WriteLine("[ERROR]: Invalid input. Try again [Y/N]");

                    continue;
                }

                if (answer[0] == 'Y' || answer[0] == 'y')
                {
                    return true;
                }
                else if (answer[0] == 'N' || answer[0] == 'n')
                {
                    return false;
                }

                Console.WriteLine("[ERROR]: Invalid input. Try again [Y/N]");
            }
        }

        private static int read_question_input()
        {
            Console.WriteLine("");
            Console.WriteLine("- Antwort? (a-d)");

            for (; ; )
            {
                var answer = Console.ReadLine();

                if (answer.Length > 1 || answer.Length <= 0)
                {
                    Console.WriteLine("Invalid input. Try again [a-d]");

                    continue;
                }

                if (answer[0] >= 'a' && answer[0] <= 'd') return answer[0] - 'a';

                Console.WriteLine("Invalid input. Try again [a-d]");
            }
        }

        static void Main(string[] args)
        {
            string input_file = "itil_fragen.txt";

            try
            {
                lines = File.ReadAllLines(input_file, Encoding.Default);
            }
            catch
            {
                Console.WriteLine($"[ERROR]: Input file not found or is empty! ({input_file})");
                Console.ReadKey();

                return;
            }

            if (lines.Count() == 0)
            {
                Console.WriteLine($"[ERROR]: Input file not found or is empty! ({input_file})");
                Console.ReadKey();

                return;
            }

            Console.OutputEncoding = Encoding.Default;
            Console.WriteLine($"[INFO]: Input file: {input_file}");
            Console.WriteLine($"[INFO]: Number of lines: {lines.Count()}");
            Console.WriteLine("-------------------");
            Console.WriteLine($"[QUESTION]: Do you want to change the standard settings? (Randomize answers: {RANDOMIZE_ANSWERS}, randomize questions: {RANDOMIZE_QUESTIONS})? [Y/N]");

            if (read_bool_input())
            {
                Console.WriteLine("[QUESTION]: Enable randomizing of answers? [Y/N]");

                RANDOMIZE_ANSWERS = read_bool_input();

                Console.WriteLine($"[INFO]: Randomize answers: {RANDOMIZE_ANSWERS}");
                Console.WriteLine("[QUESTION]: Enable randomizing of questions? [Y/N]");

                RANDOMIZE_QUESTIONS = read_bool_input();

                Console.WriteLine($"[INFO]: Randomize questions: {RANDOMIZE_ANSWERS}");
            }

            Console.WriteLine("-------------------");
            Console.WriteLine("[INFO]: Parsing questions...");

            for (int i = 0; i < lines.Count();)
            {
                var line = lines[i];

                if (line.Contains($"{umbrellaterm} "))
                {
                    var exercise = line.Substring(umbrellaterm.Length + 1);

                    Console.WriteLine($"[INFO]: Exercise found: {exercise}, parsing...");

                    var res = parse_questions(exercise, i + 1);

                    if (res == -2) return;
                    if ((i = res) == -1) break;
                }
            }

            Console.WriteLine("-------------------");
            Console.WriteLine("[QUESTION]: Ready to start the test? [Y/N]");

            for (; ; )
            {
                var ans = read_bool_input();

                if (ans) break;

                Console.WriteLine("[INFO]: Well, I have time. Waiting for you :). Ready now? [Y/N]");
            }

            if (RANDOMIZE_ANSWERS || RANDOMIZE_QUESTIONS)
            {
                //bool done = false;

                for (int i = 0; i < questions_list.Count; i++)
                {
                    var list = questions_list[i];

                    if (RANDOMIZE_QUESTIONS) list.questions.do_shuffle();

                    if (RANDOMIZE_ANSWERS)
                    {
                        for (int x = 0; x < list.questions.Count(); x++)
                        {
                            var question = list.questions[x];
                            var original_right_answer = question.answers[question.correct_answer_index];

                            //if (!done) Console.WriteLine($"{question.correct_answer_index}: {question.answers[question.correct_answer_index]}");

                            question.answers.do_shuffle();

                            int new_correct_answer_index = -1;

                            for (int y = 0; y < question.answers.Count(); y++)
                            {
                                var str = question.answers[y];

                                if (str.CompareTo(original_right_answer) == 0)
                                {
                                    new_correct_answer_index = y;

                                    break;
                                }
                            }

                            if (new_correct_answer_index == -1)
                            {
                                Console.WriteLine("[ERROR]: Error occured while shuffling answers...");

                                return;
                            }

                            question.correct_answer_index = new_correct_answer_index;

                            /*if (!done)
                            {
                                Console.WriteLine($"{question.correct_answer_index}: {question.answers[question.correct_answer_index]}");
                                Console.ReadKey();

                                done = true;
                            }*/
                        }
                    }
                }
            }

            //Console.ReadKey();
            Console.Clear();
            Console.WriteLine("[INFO]: Starting test.");

            Thread.Sleep(3000);

            Console.Clear();

            for (int i = 0; i < questions_list.Count(); i++)
            {
                var list = questions_list[i];
                var total_questions = list.questions.Count();

                int right_answers = 0;

                //Console.WriteLine($"[THEMA]: {list.umbrella_term}");

                var old_ = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(list.umbrella_term);
                Console.ForegroundColor = old_;

                string temp = "";

                for (int w = 0; w < list.umbrella_term.Length; w++) temp += "-";

                Console.WriteLine(temp);

                for (int x = 0; x < list.questions.Count(); x++)
                {
                    var question = list.questions[x];

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Übung {question.question_num}");
                    Console.ForegroundColor = old_;
                    Console.WriteLine("");
                    Console.WriteLine(question.question);
                    Console.WriteLine("");

                    for (int y = 0; y < question.answers.Count(); y++)
                    {
                        var answer_ = question.answers[y];

                        Console.WriteLine($"{(char)('a' + y)}) {answer_}");
                    }

                    var answer = read_question_input();

                    if (answer == question.correct_answer_index)
                    {
                        var old = Console.ForegroundColor;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Richtig.");
                        Console.ForegroundColor = old;

                        right_answers++;
                    }
                    else
                    {
                        var old = Console.ForegroundColor;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Falsch. Die richtige Antwort ist:");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{(char)('a' + question.correct_answer_index)}) - {question.answers[question.correct_answer_index]}");
                        Console.ForegroundColor = old;
                    }

                    Console.WriteLine("Warte auf Input...");
                    Console.ReadKey();
                    //Console.Clear();
                    Console.WriteLine("");
                }

                Console.WriteLine("Thema fertig.");
                Console.WriteLine($"{right_answers} von {total_questions} Antworten richtig ({((float)right_answers / (float)total_questions).ToString("P2")})");
                Console.WriteLine("Warte auf Input...");
                Console.ReadKey();
                //Console.Clear();
                Console.WriteLine("");
            }

            Console.ReadKey();
        }
    }
}
