using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileContent.Library
{
    public class SolutionClass
    {
        public const int MaxValue = 100000000;

        public int Solution(int N)
        {
            string retorno = string.Empty;
            var list = N.ToString().ToCharArray().OrderByDescending(x => x);
            list.ToList().ForEach(x => retorno += x);
            var ret = Convert.ToInt64(retorno);
            return (int)(ret > MaxValue ? -1 : ret);
        }

    }
}
