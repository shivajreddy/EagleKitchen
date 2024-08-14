using System.Collections.Generic;
using System.IO;
using System.Reflection;


// not using this util now, cuz now we are subscribing to View Activated event, and we are getting the views from the event args.
// and the function 'OnViewActivated' inside EagleKitchen.cs file
namespace EK24.EagleKitchen.CoreModel
{
    public static class Utils
    {


        public static string EmbeddedData()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Import all the text files here. TODO: later create a class with all core-model database objects as fields 
            string resourceName = "EK24.EagleKitchen.CoreModel.Data.corvallis.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public static (List<string> views, List<string> sheets) ParseData()
        {
            string rawData = Utils.EmbeddedData();

            // Normalize line breaks and split the text
            string[] lines = rawData.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

            List<string> views = new List<string>();
            List<string> sheets = new List<string>();

            bool isViewSection = false;
            bool isSheetSection = false;

            foreach (var line in lines)
            {
                // Check if the line is a section header
                if (line.StartsWith("Views-Designers-Can-See"))
                {
                    isViewSection = true;
                    isSheetSection = false;
                    continue; // Skip the header line
                }
                else if (line.StartsWith("Sheets-Designers-Can-See"))
                {
                    isSheetSection = true;
                    isViewSection = false;
                    continue; // Skip the header line
                }

                // Add to the appropriate list based on the current section
                if (isViewSection && !string.IsNullOrWhiteSpace(line))
                {
                    views.Add(line.Trim());
                }
                else if (isSheetSection && !string.IsNullOrWhiteSpace(line))
                {
                    sheets.Add(line.Trim());
                }
            }

            return (views, sheets);
        }



    }

}
