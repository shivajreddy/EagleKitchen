using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace EK24.Utils
{
    public class Brand
    {
        public string BrandName { get; set; }
        public List<Style> Styles { get; set; }
    }

    public class Style
    {
        public string StyleName { get; set; }
        public List<string> Properties { get; set; }
        public List<SpeciesVariant> SpeciesVariants { get; set; }
    }

    public class SpeciesVariant
    {
        public string Species { get; set; }
        public List<string> Finishes { get; set; }
    }

    public class Root
    {
        public List<Brand> Brands { get; set; }
    }

    public class JsonToEnumConverter
    {

        static void hello()
        {
            string jsonFilePath = "path_to_your_json_file.json";
            string json = File.ReadAllText(jsonFilePath);

            Root root = JsonConvert.DeserializeObject<Root>(json);

            // Now you can query the data
            // Example: Get all brand names
            var brandNames = GetAllBrandNames(root);
            foreach (var brand in brandNames)
            {
                Debug.WriteLine(brand);
            }

            // Example: Get all styles for a specific brand
            var styles = GetStylesByBrand(root, "Aristokraft");
            foreach (var style in styles)
            {
                Debug.WriteLine(style);
            }

            // Example: Get all finishes for a specific style in a specific brand
            var finishes = GetFinishesByStyle(root, "Aristokraft", "Sinclair");
            foreach (var finish in finishes)
            {
                Debug.WriteLine(finish);
            }
        }

        static List<string> GetAllBrandNames(Root root)
        {
            return root.Brands.ConvertAll(brand => brand.BrandName);
        }

        static List<string> GetStylesByBrand(Root root, string brandName)
        {
            var brand = root.Brands.Find(b => b.BrandName == brandName);
            return brand?.Styles.ConvertAll(style => style.StyleName) ?? new List<string>();
        }

        static List<string> GetFinishesByStyle(Root root, string brandName, string styleName)
        {
            var brand = root.Brands.Find(b => b.BrandName == brandName);
            if (brand != null)
            {
                var style = brand.Styles.Find(s => s.StyleName == styleName);
                if (style != null)
                {
                    var finishes = new List<string>();
                    foreach (var variant in style.SpeciesVariants)
                    {
                        finishes.AddRange(variant.Finishes);
                    }
                    return finishes;
                }
            }
            return new List<string>();
        }
    }

}
