using System;
using System.Linq;

namespace AgileContent.Library
{
    public class SolutionClass
    {
        public const int MaxValue = 100000000;

        /// <summary>
        /// Retorna o maior número formado pelos dígitos de <paramref name="N"/>, ou -1 se ele passar de <see cref="MaxValue"/>.
        /// </summary>
        public int Solution(int N)
        {
            if (N < 0)
                throw new ArgumentOutOfRangeException(nameof(N), "N deve ser maior ou igual a zero");

            string retorno = string.Empty;
            var list = N.ToString().ToCharArray().OrderByDescending(x => x);
            list.ToList().ForEach(x => retorno += x);
            var ret = Convert.ToInt64(retorno);
            return (int)(ret > MaxValue ? -1 : ret);
        }

    }
}
