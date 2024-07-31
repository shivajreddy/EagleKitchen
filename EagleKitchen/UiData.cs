namespace four.EagleKitchen;

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
}