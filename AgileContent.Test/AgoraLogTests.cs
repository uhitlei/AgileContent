using System;
using AgileContent.Library;
using NUnit.Framework;

namespace AgileContent.Test
{
    [TestFixture]
    public class AgoraLogTests
    {
        [Test]
        public void ConvertLine_LinhaValida_PreencheTodosOsCampos()
        {
            var log = new AgoraLog().ConvertLine("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2");

            Assert.AreEqual("\"MINHA CDN\"", log.Provider);
            Assert.AreEqual("GET", log.HttpMethod);
            Assert.AreEqual("200", log.StatusCode);
            Assert.AreEqual("/robots.txt", log.UriPath);
            Assert.AreEqual("100", log.TimeTaken);
            Assert.AreEqual("312", log.ResponseSize);
            Assert.AreEqual("HIT", log.CacheStatus);
        }

        [Test]
        public void ConvertLine_ComEspacosEntreOsCampos_IgnoraOsEspacos()
        {
            var log = new AgoraLog().ConvertLine("312 | 200 | HIT | \"GET /robots.txt HTTP/1.1\" | 100.2");

            Assert.AreEqual("GET", log.HttpMethod);
            Assert.AreEqual("200", log.StatusCode);
            Assert.AreEqual("/robots.txt", log.UriPath);
            Assert.AreEqual("100", log.TimeTaken);
            Assert.AreEqual("312", log.ResponseSize);
            Assert.AreEqual("HIT", log.CacheStatus);
        }

        [Test]
        public void ConvertLine_Invalidate_ViraRefreshHit()
        {
            var log = new AgoraLog().ConvertLine("312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1");

            Assert.AreEqual("REFRESH_HIT", log.CacheStatus);
        }

        [Test]
        public void ConvertLine_InvalidateComEspacos_ViraRefreshHit()
        {
            var log = new AgoraLog().ConvertLine("312 | 200 | INVALIDATE | \"GET /robots.txt HTTP/1.1\" | 245.1");

            Assert.AreEqual("REFRESH_HIT", log.CacheStatus);
        }

        [TestCase("MISS")]
        [TestCase("HIT")]
        public void ConvertLine_OutrosStatusDeCache_SaoMantidos(string status)
        {
            var log = new AgoraLog().ConvertLine("101|200|" + status + "|\"POST /myImages HTTP/1.1\"|319.4");

            Assert.AreEqual(status, log.CacheStatus);
            Assert.AreEqual("POST", log.HttpMethod);
        }

        [Test]
        public void ConvertLine_FinalDeLinhaWindows_RemoveCarriageReturn()
        {
            var log = new AgoraLog().ConvertLine("199|404|MISS|\"GET /not-found HTTP/1.1\"|142\r");

            Assert.AreEqual("142", log.TimeTaken);
        }

        [TestCase("100.2", "100")]
        [TestCase("142.9", "142")]
        [TestCase("7", "7")]
        [TestCase("0.5", "0")]
        public void ConvertLine_TempoDeResposta_DescartaACasaDecimal(string tempo, string esperado)
        {
            var log = new AgoraLog().ConvertLine("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|" + tempo);

            Assert.AreEqual(esperado, log.TimeTaken);
        }

        [Test]
        public void ConvertLine_RequisicaoSemVersaoHttp_ExtraiMetodoECaminho()
        {
            var log = new AgoraLog().ConvertLine("10|200|HIT|\"GET /index.html\"|1.0");

            Assert.AreEqual("GET", log.HttpMethod);
            Assert.AreEqual("/index.html", log.UriPath);
        }

        [TestCase("")]
        [TestCase("312|200|HIT|100.2")]
        [TestCase("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2|extra")]
        [TestCase("texto qualquer sem separador")]
        public void ConvertLine_QuantidadeDeCamposErrada_LancaFormatException(string linha)
        {
            Assert.Throws<FormatException>(() => new AgoraLog().ConvertLine(linha));
        }

        [TestCase("312|200|HIT|\"GET\"|100.2")]
        [TestCase("312|200|HIT|\"\"|100.2")]
        public void ConvertLine_RequisicaoSemCaminho_LancaFormatException(string linha)
        {
            Assert.Throws<FormatException>(() => new AgoraLog().ConvertLine(linha));
        }

        [TestCase("abc|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2")]
        [TestCase("312|OK|HIT|\"GET /robots.txt HTTP/1.1\"|100.2")]
        [TestCase("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|rapido")]
        [TestCase("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|")]
        [TestCase("-1|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2")]
        public void ConvertLine_CampoNumericoInvalido_LancaFormatException(string linha)
        {
            Assert.Throws<FormatException>(() => new AgoraLog().ConvertLine(linha));
        }

        [Test]
        public void ConvertLine_Nula_LancaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AgoraLog().ConvertLine(null));
        }
    }
}
