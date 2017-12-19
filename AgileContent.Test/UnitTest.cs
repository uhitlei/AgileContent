using AgileContent.Library;
using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileContent.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestSolution_147()
        {
            Assert.AreEqual(741, new AgileContent.Library.SolutionClass().Solution(147));
        }

        [TestMethod]
        public void TestSolution_483()
        {
            Assert.AreEqual(843, new AgileContent.Library.SolutionClass().Solution(483));
        }

        [TestMethod]
        public void TestSolution_647()
        {
            Assert.AreEqual(764, new AgileContent.Library.SolutionClass().Solution(647));
        }

        [TestMethod]
        public void TestSolution_100000001()
        {
            Assert.AreEqual(-1, new AgileContent.Library.SolutionClass().Solution(100000001));
        }


        [TestMethod]
        public void TestSolution_Array()
        {
            var solutionClass = new SolutionClass();
            int[] array = { 147, 483, 647, 100000000 };
            array.ToList().ForEach(x => Assert.IsTrue(x <= solutionClass.Solution(x)));

        }

        [TestMethod]
        public void TestConvert_CndToAgora()
        {
            var cndLog = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt";
            var AgoraLogPath = "./output/minhaCdn1.csv";
            new AgileContent.Library.ConvertAgoraClass().ConvertLog(cndLog, AgoraLogPath);

            var conteudo = new StringBuilder();

            System.IO.File.ReadAllLines(AgoraLogPath).ToList().ForEach(x => conteudo.AppendLine(x));
            
            Assert.AreNotEqual(String.Empty, conteudo);
        }
    }
}
