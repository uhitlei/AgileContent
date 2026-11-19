using AgileContent.Library;
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AgileContent.Test
{
    [TestFixture]
    public class UnitTest
    {
        [Test]
        public void TestSolution_147()
        {
            Assert.AreEqual(741, new AgileContent.Library.SolutionClass().Solution(147));
        }

        [Test]
        public void TestSolution_483()
        {
            Assert.AreEqual(843, new AgileContent.Library.SolutionClass().Solution(483));
        }

        [Test]
        public void TestSolution_647()
        {
            Assert.AreEqual(764, new AgileContent.Library.SolutionClass().Solution(647));
        }

        [Test]
        public void TestSolution_100000001()
        {
            Assert.AreEqual(-1, new AgileContent.Library.SolutionClass().Solution(100000001));
        }

        [Test]
        public void TestSolution_100000000_NoLimite()
        {
            Assert.AreEqual(100000000, new SolutionClass().Solution(100000000));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(1020, 2100)]
        [TestCase(98765432, 98765432)]
        [TestCase(int.MaxValue, -1)]
        public void TestSolution_Casos(int n, int esperado)
        {
            Assert.AreEqual(esperado, new SolutionClass().Solution(n));
        }

        [Test]
        public void TestSolution_Negativo()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SolutionClass().Solution(-147));
        }

        [Test]
        public void TestSolution_Array()
        {
            var solutionClass = new SolutionClass();
            int[] array = { 147, 483, 647, 100000000 };
            array.ToList().ForEach(x => Assert.IsTrue(x <= solutionClass.Solution(x)));

        }

        // Depende de rede e do arquivo público original do desafio; rode manualmente.
        [Test, Explicit, Category("Rede")]
        public void TestConvert_CndToAgora()
        {
            var cndLog = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt";
            var AgoraLogPath = Path.Combine(Path.GetTempPath(), "minhaCdn1.csv");
            new AgileContent.Library.ConvertAgoraClass().ConvertLog(cndLog, AgoraLogPath);

            var conteudo = File.ReadAllText(AgoraLogPath);

            Assert.IsNotEmpty(conteudo);
            StringAssert.StartsWith("#Version: 1.0", conteudo);
        }
    }
}
