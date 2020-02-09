using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_questions_to_array
{
    public static class shuffle
    {

        private static Random rnd = new Random();

        public static void do_shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;

                var k = rnd.Next(n + 1);

                T value = list[k];

                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
