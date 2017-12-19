using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgileContent.Library
{
    public class ConvertAgoraClass
    {

        public void ConvertLog(string oldLog, string agoraLogPath)
        {
            try
            {
                if(String.IsNullOrEmpty(oldLog) || String.IsNullOrEmpty(agoraLogPath))
                    throw new NullReferenceException("oldLog ou AgoraLogPath não pode ser null");


                var downloadClient = new WebClient();
                var fileName = oldLog.Split('/')[oldLog.Split('/').Length - 1];

                //Download do conteudo do arquivo.
                var lines = downloadClient.DownloadString(oldLog);

                if (String.IsNullOrEmpty(lines))
                    throw new NullReferenceException("Falha No Download Do arquivo");
                

                var agoraLogList = new List<AgoraLog>();

                lines.Split('\n').ToList().ForEach((x) =>
                {
                    if (!String.IsNullOrEmpty(x))
                        agoraLogList.Add(new AgoraLog().ConvertLine(x));
                } );
                var strConteudo = new StringBuilder();

                strConteudo.AppendLine(String.Format("#Version: 1.0"));
                strConteudo.AppendLine(String.Format("#Date: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
                strConteudo.AppendLine(String.Format("#Fields:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "provider", "http-method", "status-code", "uri-path", "time-taken", "response-size", "cache-status"));
                agoraLogList.ForEach(x => strConteudo.AppendLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", x.Provider, x.HttpMethod, x.StatusCode, x.UriPath, x.TimeTaken, x.ResponseSize, x.CacheStatus)));

                GravarArquivoLog(strConteudo.ToString(), agoraLogPath);

            }
            catch (Exception e)
            {
                
                throw e;
            }

           

        }

        private void GravarArquivoLog(string conteudo, string path)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                StreamWriter lSrLog = new StreamWriter(path, false, Encoding.UTF8);
                lSrLog.WriteLine(conteudo);
                lSrLog.Flush();
                lSrLog.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }
    }
}
