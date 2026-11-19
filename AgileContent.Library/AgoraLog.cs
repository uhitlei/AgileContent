using System;
using System.Globalization;
using System.Linq;

namespace AgileContent.Library
{
    public class AgoraLog
    {
        public string Provider { get; set; }
        public string HttpMethod { get; set; }
        public string StatusCode { get; set; }
        public string UriPath { get; set; }
        public string TimeTaken { get; set; }
        public string ResponseSize { get; set; }
        public string CacheStatus { get; set; }

        public AgoraLog ConvertLine(string line)
        {
            //Line Ex: "312|200|HIT|"GET /robots.txt HTTP/1.1"|100.2" (espaços em volta do '|' são aceitos)
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            var lineSplited = line.Split('|').Select(x => x.Trim()).ToArray();
            if (lineSplited.Length != 5)
                throw new FormatException(String.Format("Esperados 5 campos separados por '|', encontrados {0}: {1}", lineSplited.Length, line.Trim()));

            var request = lineSplited[3].Replace("\"", string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (request.Length < 2)
                throw new FormatException(String.Format("Requisição sem método ou caminho: {0}", lineSplited[3]));

            int numero;
            if (!int.TryParse(lineSplited[0], NumberStyles.None, CultureInfo.InvariantCulture, out numero))
                throw new FormatException(String.Format("Tamanho da resposta inválido: {0}", lineSplited[0]));
            if (!int.TryParse(lineSplited[1], NumberStyles.None, CultureInfo.InvariantCulture, out numero))
                throw new FormatException(String.Format("Status code inválido: {0}", lineSplited[1]));

            decimal tempo;
            if (!decimal.TryParse(lineSplited[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tempo))
                throw new FormatException(String.Format("Tempo de resposta inválido: {0}", lineSplited[4]));

            this.Provider = @"""MINHA CDN""";
            this.ResponseSize = lineSplited[0];
            this.StatusCode = lineSplited[1];
            this.CacheStatus = lineSplited[2].Equals("INVALIDATE") ? "REFRESH_HIT" : lineSplited[2];
            this.HttpMethod = request[0];
            this.UriPath = request[1];
            this.TimeTaken = Math.Truncate(tempo).ToString(CultureInfo.InvariantCulture);
            return this;
        }

    }
}
