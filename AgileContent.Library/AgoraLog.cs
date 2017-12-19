using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //Line Ex: "312 | 200 | HIT | "GET /robots.txt HTTP/1.1" | 100.2"
            var lineSplited = line.Split('|');
            this.Provider = @"""MINHA CDN""";
            this.ResponseSize = lineSplited[0];
            this.StatusCode = lineSplited[1];
            this.CacheStatus = lineSplited[2].Equals("INVALIDATE") ? "REFRESH_HIT" : lineSplited[2];
            this.HttpMethod = lineSplited[3].Replace("\"", string.Empty).Split(' ')[0];
            this.UriPath = lineSplited[3].Replace("\"", string.Empty).Split(' ')[1];
            this.TimeTaken = lineSplited[4].Split('.')[0];
            return this;
        }

    }
}
