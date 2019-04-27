using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoreDemo
{
    public class PhotosFolder
    {
        public static string Current
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                path = Path.Combine(path, "PhotoStoreDemo");
                var di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di.Create();
                    string location = Assembly.GetExecutingAssembly().Location;
                    int index = location.LastIndexOf("\\");
                    string photosPath = $"{location.Substring(0, index)}\\Photos";
                    var photoDir = new DirectoryInfo(photosPath);
                    var files = photoDir.GetFiles();
                    foreach (var file in files)
                    {
                        string destinationPath = Path.Combine(path, file.Name);
                        file.CopyTo(destinationPath, true);
                    }
                }
                return path;
            }
        }   
    }
}
