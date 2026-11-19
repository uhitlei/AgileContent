using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace AgileContent.Library
{
    public class ConvertAgoraClass
    {

        /// <summary>
        /// Lê o log no formato MINHA CDN (URL ou arquivo local) e grava a versão no formato Agora em <paramref name="agoraLogPath"/>.
        /// </summary>
        public void ConvertLog(string oldLog, string agoraLogPath)
        {
            if (String.IsNullOrEmpty(oldLog) || String.IsNullOrEmpty(agoraLogPath))
                throw new ArgumentNullException(String.IsNullOrEmpty(oldLog) ? nameof(oldLog) : nameof(agoraLogPath), "oldLog ou AgoraLogPath não pode ser null");

            //Download (ou leitura local) do conteudo do arquivo.
            var lines = LerConteudo(oldLog);

            if (String.IsNullOrEmpty(lines))
                throw new InvalidOperationException("Falha no download do arquivo ou arquivo vazio");

            GravarArquivoLog(ConvertContent(lines, DateTime.Now), agoraLogPath);
        }

        /// <summary>
        /// Converte o conteúdo de um log MINHA CDN para o formato Agora. Linhas em branco são ignoradas;
        /// uma linha malformada gera <see cref="FormatException"/> indicando o número da linha.
        /// </summary>
        public string ConvertContent(string content, DateTime date)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var agoraLogList = new List<AgoraLog>();
            var lines = content.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(lines[i]))
                    continue;

                try
                {
                    agoraLogList.Add(new AgoraLog().ConvertLine(lines[i]));
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("Linha {0}: {1}", i + 1, e.Message), e);
                }
            }

            var strConteudo = new StringBuilder();

            strConteudo.AppendLine(String.Format("#Version: 1.0"));
            strConteudo.AppendLine(String.Format("#Date: {0}", date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
            strConteudo.AppendLine(String.Format("#Fields:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "provider", "http-method", "status-code", "uri-path", "time-taken", "response-size", "cache-status"));
            agoraLogList.ForEach(x => strConteudo.AppendLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", x.Provider, x.HttpMethod, x.StatusCode, x.UriPath, x.TimeTaken, x.ResponseSize, x.CacheStatus)));

            return strConteudo.ToString();
        }

        private static string LerConteudo(string oldLog)
        {
            if (File.Exists(oldLog))
                return File.ReadAllText(oldLog);

            using (var downloadClient = new WebClient())
            {
                return downloadClient.DownloadString(oldLog);
            }
        }

        private void GravarArquivoLog(string conteudo, string path)
        {
            var diretorio = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            using (var lSrLog = new StreamWriter(path, false, Encoding.UTF8))
            {
                lSrLog.Write(conteudo);
            }
        }
    }
}
