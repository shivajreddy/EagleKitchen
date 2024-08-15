using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using EK24.VendorViewModels;
using EK24.JsonModels;

namespace EK24.EagleKitchenView;


// UTILITY class.
// This is the UIData Object. (almost like the View Model)
public static class UiDataService
{
    public static string GoToViewName { get; set; }

    public static bool HasLeftFillerStrip { get; set; }
    public static bool HasRightFillerStrip { get; set; }
    public static string LeftFillerStripValue { get; set; }
    public static string RightFillerStripValue { get; set; }

    public static string chosenFamilyValue { get; set; }
    public static string chosenTypeValue { get; set; }


    // Json related data
    // This will hold all the Brands with their Styles from ek24_brands_styles.json
    public static List<BrandStylesModel> BrandsWithStyles { get; private set; }

    // This will hold all the Brands with their Shapes and Types from ek24_brands_shapes_types.json
    public static List<BrandShapesTypesModel> BrandsWithShapesAndTypes { get; private set; }

    // This will hold the list of VendorStyleFinishViewModel which contains all the finishes
    public static List<VendorStyleFinishViewModel> VendorFinishes { get; private set; }


    // Declare the variable that holds all the Brands that we got from ek24_brands_styles.json
    // Declare the variable that has all brands, and each brand holding all the styles
    // Declare the variable that holds all the Brands that we got from ek24_brands_shapes_types.json
    // Declare the variable that has all brands, and each brand holding all the shapes
    // Declare a variable for that has all brand, and each brand holding all the types in it.


    // Constructor
    public static void StartTheDataService()
    {
        InitializeBrandShapesTypes();
        InitializeBrandsStylesFinishes();
        InitializeVendorFinishes();
    }

    public static void InitializeVendorFinishes()
    {
        string jsonFilePath = Path.Combine("Resources", "json", "ek24_brands_styles.json");
        string json = File.ReadAllText(jsonFilePath);

        var root = JsonConvert.DeserializeObject<JsonBrandsStylesModel>(json);

        VendorFinishes = new List<VendorStyleFinishViewModel>();

        // Populate the VendorFinishes list
        foreach (var brand in root.Brands)
        {
            foreach (var style in brand.Styles)
            {
                foreach (var speciesVariant in style.SpeciesVariants)
                {
                    foreach (var finish in speciesVariant.Finishes)
                    {
                        var vendorFinish = new VendorStyleFinishViewModel
                        {
                            BrandName = brand.BrandName,
                            StyleName = style.StyleName,
                            SpeciesName = speciesVariant.Species,
                            FinishName = finish,
                            StyleFinishName = $"{style.StyleName}-{finish}",
                            ImagePath = $"/ek24;component/Resources/styles/ek_styles/{style.StyleName}_{speciesVariant.Species}_{finish}.png"
                        };

                        VendorFinishes.Add(vendorFinish);
                    }
                }
            }
        }

        // Store the result in the BrandsWithStyles property
        BrandsWithStyles = root.Brands;
    }

    // Initializes the data for BrandsWithShapesAndTypes
    public static void InitializeBrandShapesTypes()
    {
        string jsonFilePath = Path.Combine("Resources", "json", "ek24_brands_shapes_types.json");
        string json = File.ReadAllText(jsonFilePath);

        var root = JsonConvert.DeserializeObject<JsonBrandsShapesTypesModel>(json);

        // Store the result in the BrandsWithShapesAndTypes property
        BrandsWithShapesAndTypes = root.Brands;

        // Populate the types for each shape in each brand
        foreach (var brand in root.Brands)
        {
            foreach (var shape in brand.Shapes)
            {
                foreach (var type in shape.Types)
                {
                    // This could be a placeholder to show where you might add additional logic
                    // related to handling types, if necessary.
                }
            }
        }
    }

    // Initializes the data for BrandsWithStyles and other related properties
    public static void InitializeBrandsStylesFinishes()
    {
        string jsonFilePath = Path.Combine("Resources", "json", "ek24_brands_styles.json");
        string json = File.ReadAllText(jsonFilePath);

        var root = JsonConvert.DeserializeObject<JsonBrandsStylesModel>(json);

        // Store the result in the BrandsWithStyles property
        BrandsWithStyles = root.Brands;
    }


}