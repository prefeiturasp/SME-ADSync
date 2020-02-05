using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace SME.ADSync.Core.Extesions
{
    public static class Log
    {
        private static Dictionary<string, string> ArquivosPorContexto = new Dictionary<string, string>();

        public static string Caminho
        {
            get
            {
                var caminhoBase = $"{AppDomain.CurrentDomain.BaseDirectory}\\Log\\{DateTime.Today.ToString("yyyyMMdd")}";

                if (!Directory.Exists(caminhoBase))
                    Directory.CreateDirectory(caminhoBase);

                return caminhoBase;
            }
        }

        public static void GravarArquivo(string json, string prefixoArquivo = "")
        {
            var arquivo = $"{Caminho}\\{prefixoArquivo}{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";

            File.WriteAllText(arquivo, json);
        }

        public static void GravarLinha(string contextoLog, object seraUmJson, string prefixoArquivo)
        {
            string arquivo = string.Empty;

            if (!ArquivosPorContexto.TryGetValue(contextoLog, out arquivo) && !string.IsNullOrWhiteSpace(contextoLog))
            {
                arquivo = $"{Caminho}\\{prefixoArquivo}{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
                ArquivosPorContexto.Add(contextoLog, arquivo);
            }

            if (!string.IsNullOrWhiteSpace(arquivo))
                File.AppendAllLines(arquivo, new string[] { Newtonsoft.Json.JsonConvert.SerializeObject(seraUmJson) });
        }
    }
}
