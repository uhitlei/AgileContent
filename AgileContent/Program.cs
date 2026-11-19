using System;
using System.Linq;

using AgileContent.Library;

namespace AgileContent
{
    class Program
    {
        const string LogPadrao = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt";
        const string SaidaPadrao = "./output/minhaCdn1.csv";

        // Uso: AgileContent.Console.exe [log MINHA CDN (URL ou arquivo)] [arquivo de saída Agora]
        static int Main(string[] args)
        {
            var solutionClass = new SolutionClass();
            int[] array = {147, 483, 647, 100000000 , 100000001 };
            array.ToList().ForEach(x => Console.WriteLine(solutionClass.Solution(x)));

            var origem = args.Length > 0 ? args[0] : LogPadrao;
            var destino = args.Length > 1 ? args[1] : SaidaPadrao;

            var codigo = 0;
            try
            {
                new ConvertAgoraClass().ConvertLog(origem, destino);
                Console.Write(System.IO.File.ReadAllText(destino));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Erro ao converter o log: " + e.Message);
                codigo = 1;
            }

            if (!Console.IsInputRedirected)
                Console.ReadKey();

            return codigo;
        }

    }
}
