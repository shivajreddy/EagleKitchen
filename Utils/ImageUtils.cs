using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace EK24.Utils
{
    public static class ImageUtilities
    {
        public static BitmapImage LoadImage(Assembly assembly, string imageName)
        {
            var img = new BitmapImage();
            try
            {
                var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(imageName));
                var stream = assembly.GetManifestResourceStream(resourceName);
                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return img;
        }


        public static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static BitmapImage GetCabinetStyleImage(string style_name)
        {
            style_name = "henning.png";

            var image = ImageUtilities.LoadImage(Assembly.GetExecutingAssembly(), style_name);

            return image;
        }

    }
}
