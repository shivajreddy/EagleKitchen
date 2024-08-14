using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EK24.EagleKitchen;

public class Brand
{
    public string BrandName { get; set; }
    public List<Style> Styles { get; set; }
    public List<Shape> Shapes { get; set; }
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

public class Shape
{
    public string ShapeName { get; set; }
    public List<string> Types { get; set; }
}

public class Root
{
    public List<Brand> Brands { get; set; }
}





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

    public static string[] FamilyBrandNames { get; set; }
    public static Dictionary<string, string[]> FamilyShapesByBrand { get; set; }
    public static Dictionary<(string Brand, string Type), string[]> FamilyTypesByShape { get; set; }

    static UiData()
    {
        InitializeBrandShapesTypes();
        InitializeBrandsStylesFinishes();
    }

    public static void InitializeBrandShapesTypes()
    {
        string jsonFilePath = Path.Combine("Resources", "ek24_cabinet_shapes.json");
        string json = File.ReadAllText(jsonFilePath);

        var root = JsonConvert.DeserializeObject<Root>(json);

        // Initialize BrandNames
        FamilyBrandNames = root.Brands.Select(b => b.BrandName).ToArray();

        // Initialize FamilyShapesByBrand
        FamilyShapesByBrand = root.Brands.ToDictionary(
            b => b.BrandName,
            b => b.Shapes.Select(s => s.ShapeName).ToArray()
        );

        // Initialize FamilyTypesByShape
        FamilyTypesByShape = new Dictionary<(string Brand, string Type), string[]>();

        foreach (var brand in root.Brands)
        {
            foreach (var shape in brand.Shapes)
            {
                var types = shape.Types.ToArray();
                FamilyTypesByShape[(brand.BrandName, shape.ShapeName)] = types;
            }
        }
    }


    // Method to initialize the data by parsin json file
    public static void InitializeBrandsStylesFinishes()
    {
        string jsonFilePath = Path.Combine("Resources", "ek24_brands_styles_finishes.json");
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