using System;
using System.Collections.Generic;
using System.Linq;
using www_sj.Models;

namespace www_sj.Extensions
{
    static class Extensions
    {
        public static Question NextQuestion(this IEnumerable<Question> _this)
        {
            var questions = _this?.ToArray();
            if (questions == null || questions.All(q => q.Asked))
            {
                // game over
                return null;
            }
            var rnd = new Random();
            var qst = rnd.Next(questions.Length);
            Question question;
            do
            {
                question = questions[qst];
                qst++;
                if (qst >= questions.Length)
                {
                    qst = 0;
                }
            } while (question.Asked);
            return question;
        }
    }
}
