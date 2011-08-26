using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Hasci.TestApp
{
    class Savepath
    {
        const string saveDir = "\\TestResults\\Foto";
        public static string GetPath(string namePattern)
        {
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);
            string timestamp = DateTime.Today.ToString("yyMMdd");
            string filePattern = namePattern + "_" + timestamp + "_";
            int curCounter = Directory.GetFiles(saveDir, filePattern + "*.jpg").Length;
            if (curCounter < 100)
                return Path.Combine(saveDir,filePattern + curCounter.ToString("D2") + ".jpg");
            else
                return Path.Combine(saveDir,filePattern + curCounter.ToString("D") + ".jpg");
        }
    }
}
