using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AgileContent.Library;

namespace AgileContent
{
    class Program
    {
        static void Main(string[] args)
        {
            var solutionClass = new SolutionClass();
            int[] array = {147, 483, 647, 100000000 , 100000001 };
            array.ToList().ForEach(x => Console.WriteLine(solutionClass.Solution(x)));

            new ConvertAgoraClass().ConvertLog("https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt", "./output/minhaCdn1.csv");

            var conteudo = new StringBuilder();

            System.IO.File.ReadAllLines("./output/minhaCdn1.csv").ToList().ForEach(x => conteudo.AppendLine(x));

            Console.WriteLine(conteudo.ToString());

            Console.ReadKey();
        }



    }
}
