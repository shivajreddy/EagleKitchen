using EK24.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EK24.EagleKitchen;

// This is the UIData Object. (almost like the View Model)
public static class UiData
{
    public static string GoToViewName { get; set; }


    public static bool HasLeftFillerStrip { get; set; }
    public static bool HasRightFillerStrip { get; set; }
    public static string LeftFillerStripValue { get; set; }
    public static string RightFillerStripValue { get; set; }

    public static string chosenFamilyValue { get; set; }
    public static string chosenTypeValue { get; set; }


    public static string[] BrandNames { get; set; }
    public static Dictionary<string, string[]> StylesByBrand { get; set; }
    public static Dictionary<(string Brand, string Style), string[]> FinishesByStyle { get; set; }

    // Method to initialize the data by parsin json file
    public static void InitializeFromJson()
    {
        string jsonFilePath = Path.Combine("Resources", "vendor_styles.json");
        string json = File.ReadAllText(jsonFilePath);

        var root = JsonConvert.DeserializeObject<Root>(json);

        // Initialize BrandNames
        BrandNames = root.Brands.Select(b => b.BrandName).ToArray();

        // Initialize StylesByBrand
        StylesByBrand = root.Brands.ToDictionary(
            b => b.BrandName,
            b => b.Styles.Select(s => s.StyleName).ToArray()
        );

        // Initialize FinishesByStyle
        FinishesByStyle = new Dictionary<(string Brand, string Style), string[]>();

        foreach (var brand in root.Brands)
        {
            foreach (var style in brand.Styles)
            {
                var finishes = style.SpeciesVariants.SelectMany(sv => sv.Finishes).ToArray();
                FinishesByStyle[(brand.BrandName, style.StyleName)] = finishes;
            }
        }
    }


}