using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarPDemo.Helper
{
    class FileHelper
    {
        public static void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }
    }
}
