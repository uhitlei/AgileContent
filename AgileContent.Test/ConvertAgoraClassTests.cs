using System;
using System.IO;
using AgileContent.Library;
using NUnit.Framework;

namespace AgileContent.Test
{
    [TestFixture]
    public class ConvertAgoraClassTests
    {
        const string LogMinhaCdn =
            "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\n" +
            "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4\n" +
            "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9\n" +
            "312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1\n";

        static readonly DateTime Data = new DateTime(2017, 12, 15, 23, 1, 6);

        string pastaTemporaria;

        [SetUp]
        public void CriarPastaTemporaria()
        {
            pastaTemporaria = Path.Combine(Path.GetTempPath(), "logconverter-" + Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void ApagarPastaTemporaria()
        {
            if (Directory.Exists(pastaTemporaria))
                Directory.Delete(pastaTemporaria, true);
        }

        static string[] Linhas(string conteudo)
        {
            return conteudo.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
        }

        [Test]
        public void ConvertContent_LogCompleto_GeraCabecalhoELinhasNoFormatoAgora()
        {
            var linhas = Linhas(new ConvertAgoraClass().ConvertContent(LogMinhaCdn, Data));

            CollectionAssert.AreEqual(new[]
            {
                "#Version: 1.0",
                "#Date: 15/12/2017 23:01:06",
                "#Fields:provider\thttp-method\tstatus-code\turi-path\ttime-taken\tresponse-size\tcache-status",
                "\"MINHA CDN\"\tGET\t200\t/robots.txt\t100\t312\tHIT",
                "\"MINHA CDN\"\tPOST\t200\t/myImages\t319\t101\tMISS",
                "\"MINHA CDN\"\tGET\t404\t/not-found\t142\t199\tMISS",
                "\"MINHA CDN\"\tGET\t200\t/robots.txt\t245\t312\tREFRESH_HIT",
            }, linhas);
        }

        [Test]
        public void ConvertContent_DataDaTarde_UsaFormato24Horas()
        {
            var linhas = Linhas(new ConvertAgoraClass().ConvertContent(LogMinhaCdn, new DateTime(2017, 12, 15, 13, 5, 0)));

            Assert.AreEqual("#Date: 15/12/2017 13:05:00", linhas[1]);
        }

        [Test]
        public void ConvertContent_FinaisDeLinhaWindowsELinhasEmBranco_SaoIgnorados()
        {
            var conteudo = "\r\n312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\r\n\r\n   \r\n101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4\r\n";

            var linhas = Linhas(new ConvertAgoraClass().ConvertContent(conteudo, Data));

            Assert.AreEqual(5, linhas.Length);
            Assert.AreEqual("\"MINHA CDN\"\tGET\t200\t/robots.txt\t100\t312\tHIT", linhas[3]);
            Assert.AreEqual("\"MINHA CDN\"\tPOST\t200\t/myImages\t319\t101\tMISS", linhas[4]);
        }

        [Test]
        public void ConvertContent_SemLinhas_GeraApenasCabecalho()
        {
            var linhas = Linhas(new ConvertAgoraClass().ConvertContent("\n\n", Data));

            Assert.AreEqual(3, linhas.Length);
            StringAssert.StartsWith("#Fields:", linhas[2]);
        }

        [Test]
        public void ConvertContent_LinhaMalformada_InformaONumeroDaLinha()
        {
            var conteudo = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\nlinha quebrada\n";

            var erro = Assert.Throws<FormatException>(() => new ConvertAgoraClass().ConvertContent(conteudo, Data));

            StringAssert.StartsWith("Linha 2:", erro.Message);
        }

        [Test]
        public void ConvertContent_Nulo_LancaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ConvertAgoraClass().ConvertContent(null, Data));
        }

        [Test]
        public void ConvertLog_ArquivoLocal_GravaSaidaCriandoAPasta()
        {
            Directory.CreateDirectory(pastaTemporaria);
            var entrada = Path.Combine(pastaTemporaria, "input-01.txt");
            var saida = Path.Combine(pastaTemporaria, "output", "agora.txt");
            File.WriteAllText(entrada, LogMinhaCdn);

            new ConvertAgoraClass().ConvertLog(entrada, saida);

            var linhas = File.ReadAllLines(saida);
            Assert.AreEqual(7, linhas.Length);
            Assert.AreEqual("#Version: 1.0", linhas[0].TrimStart('﻿'));
            Assert.AreEqual("\"MINHA CDN\"\tGET\t200\t/robots.txt\t245\t312\tREFRESH_HIT", linhas[6]);
        }

        [Test]
        public void ConvertLog_SaidaSemPasta_GravaNoDiretorioAtual()
        {
            Directory.CreateDirectory(pastaTemporaria);
            var entrada = Path.Combine(pastaTemporaria, "input-01.txt");
            File.WriteAllText(entrada, LogMinhaCdn);
            var diretorioAnterior = Environment.CurrentDirectory;

            try
            {
                Environment.CurrentDirectory = pastaTemporaria;
                new ConvertAgoraClass().ConvertLog(entrada, "agora.txt");
            }
            finally
            {
                Environment.CurrentDirectory = diretorioAnterior;
            }

            Assert.IsTrue(File.Exists(Path.Combine(pastaTemporaria, "agora.txt")));
        }

        [Test]
        public void ConvertLog_ArquivoVazio_LancaInvalidOperationException()
        {
            Directory.CreateDirectory(pastaTemporaria);
            var entrada = Path.Combine(pastaTemporaria, "vazio.txt");
            File.WriteAllText(entrada, string.Empty);

            Assert.Throws<InvalidOperationException>(() => new ConvertAgoraClass().ConvertLog(entrada, Path.Combine(pastaTemporaria, "saida.txt")));
        }

        [TestCase(null, "saida.txt")]
        [TestCase("", "saida.txt")]
        [TestCase("entrada.txt", null)]
        [TestCase("entrada.txt", "")]
        public void ConvertLog_ParametroVazio_LancaArgumentNullException(string entrada, string saida)
        {
            Assert.Throws<ArgumentNullException>(() => new ConvertAgoraClass().ConvertLog(entrada, saida));
        }
    }
}
