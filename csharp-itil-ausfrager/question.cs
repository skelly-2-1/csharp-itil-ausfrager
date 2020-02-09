using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_questions_to_array
{
    class single_question
    {

        public single_question(int question_num, string question, int correct_answer_index, List<string> answers)
        {
            this.question_num = question_num;
            this.question = question;
            this.correct_answer_index = correct_answer_index;
            this.answers = answers;
        }

        public int question_num;
        public string question;
        public int correct_answer_index;
        public List<string> answers;

    }
}
