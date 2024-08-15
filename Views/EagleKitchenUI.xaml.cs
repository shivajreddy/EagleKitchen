using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using EK24.RequestHandlingUtils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;


namespace EK24.EagleKitchenView;

//public partial class EagleKitchenUi : IDockablePaneProvider
public partial class EagleKitchenMainUi : IDockablePaneProvider
{
    // Static variables
    public static ISet<ElementId> ActiveSelectedElementIds;
    public static string EagleConsole = "empty console";

    // Constructor for the XAML page
    public EagleKitchenMainUi()
    {
        InitializeComponent();

        PopulateComboBoxes();

        DataContext = this;
    }


    // Must implement to use Revit Dock
    public void SetupDockablePane(DockablePaneProviderData data)
    {
        data.FrameworkElement = this;
        data.InitialState = new DockablePaneState
        {
            DockPosition = DockPosition.Tabbed,
            TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
        };
    }


    #region SELECTIONS 
    // :: SELECTIONS ::
    private void ClickSelectionAllLowers(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.AllLowers;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionAllUppers(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.AllUppers;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionAllCabinets(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.AllCabinets;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionAll1Door(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.All1Door;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionOnly1Door(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.Only1Door;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionOnly1Door1Drawer(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.Only1Door1Drawer;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionAll2Doors(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.All2Doors;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionOnly2Doors(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.Only2Doors;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionOnly2Doors1Drawer(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.Only2Door1Drawer;
        Main.MyExternalEvent.Raise();
    }
    private void ClickSelectionOnly2Doors2Drawers(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
        EagleKitchenViewModel.ChosenCabinetConfiguration = CabinetConfiguration.Only2Door2Drawers;
        Main.MyExternalEvent.Raise();
    }
    #endregion

    #region SELECTIONS - other stuff
    // :: Selections :: Instance Param Updates ::
    private void ClickCustomizeStyle(object sender, RoutedEventArgs e)
    {
        //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        Main.AppsRequestHandler.RequestType = RequestType.MakeCustomizations;

        var button = sender as Button;
        switch (button.Tag)
        {
            case "Style1":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style1Henning;
                break;
            case "Style2":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style2TouraineBirch;
                break;
            case "Style3":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style3StillWater;
                break;
            case "Style4":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style4FillMore;
                break;
            case "Style5":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style5Langdon;
                break;
            case "Style6":
                EagleKitchenViewModel.ChosenCabinetStyle = CabinetStyle.Style6Everett;
                break;
        }
        Main.MyExternalEvent.Raise();
    }

    // :: PRINTING :: //
    private void ClickPrintDrawingSet(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.PrintDrawings;
        Main.MyExternalEvent.Raise();
    }
    // :: EXPORT QUANTITIES :: //
    private void ExportQuantities_Click(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.ExportQuantitiesToExcel;
        Main.MyExternalEvent.Raise();
    }

    // Update Left Filler Strip Value based on the checkbox value
    private void InstanceParamHasLeftFillerStrip_StateChanged(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as System.Windows.Controls.CheckBox;
        if (checkbox == null) return;

        if (checkbox.IsChecked == true)
        {
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.IsEnabled = true;
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripSetButton.IsEnabled = true;

            // Set the Event so that Event Handler knows to use the appropriate Event
            UiDataService.HasLeftFillerStrip = true;
            Main.AppsRequestHandler.RequestType = RequestType.UpdateHasLeftFillerStrip;
            Main.MyExternalEvent.Raise();
        }
        else
        {
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.IsEnabled = false;
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripSetButton.IsEnabled = false;

            UiDataService.HasLeftFillerStrip = false;
            Main.AppsRequestHandler.RequestType = RequestType.UpdateHasLeftFillerStrip;
            Main.MyExternalEvent.Raise();
        }
    }

    // Update Right Filler Strip Value based on the checkbox value
    private void InstanceParamHasRightFillerStrip_StateChanged(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as System.Windows.Controls.CheckBox;
        if (checkbox == null) return;

        if (checkbox.IsChecked == true)
        {
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.IsEnabled = true;
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripSetButton.IsEnabled = true;

            // Set the Event so that Event Handler knows to use the appropriate Event
            Main.AppsRequestHandler.RequestType = RequestType.UpdateHasLeftFillerStrip;
            Main.MyExternalEvent.Raise();

            UiDataService.HasRightFillerStrip = true;
            Main.AppsRequestHandler.RequestType = RequestType.UpdateHasRightFillerStrip;
            Main.MyExternalEvent.Raise();
        }
        else
        {
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.IsEnabled = false;
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripSetButton.IsEnabled = false;

            UiDataService.HasRightFillerStrip = false;
            Main.AppsRequestHandler.RequestType = RequestType.UpdateHasRightFillerStrip;
            Main.MyExternalEvent.Raise();
        }
    }

    private void ButtonClick_SetLeftFillerStripWidth(object sender, RoutedEventArgs e)
    {
        //var valueInString = EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.Text;
        //UiDataService.LeftFillerStripValue = valueInString;

        // Set the Event so that Event Handler knows to use the appropriate Event
        Main.AppsRequestHandler.RequestType = RequestType.UpdateLeftFillerStripValue;
        Main.MyExternalEvent.Raise();
    }
    private void ButtonClick_SetRightFillerStripWidth(object sender, RoutedEventArgs e)
    {
        //var valueInString = EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.Text;
        //UiDataService.RightFillerStripValue = valueInString;

        // Set the Event so that Event Handler knows to use the appropriate Event
        Main.AppsRequestHandler.RequestType = RequestType.UpdateRightFillerStripValue;
        Main.MyExternalEvent.Raise();
    }

    private void FamilyTypeSettingsUpdate_Click(object sender, RoutedEventArgs e)
    {
        // chosen type
        var chosenType = EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.SelectedItem;

        UiDataService.chosenTypeValue = chosenType as string;

        // Set the Event so that Event Handler knows to use the appropriate Event
        Main.AppsRequestHandler.RequestType = RequestType.UpdateCabinetType;
        Main.MyExternalEvent.Raise();
    }
    #endregion

    private void ClickDevTest(object sender, RoutedEventArgs e)
    {
        Main.AppsRequestHandler.RequestType = RequestType.DevTest;
        Main.MyExternalEvent.Raise();
    }

    // TAB 2: Events
    /*
    private void FilterItems(ListBox listBox)
    {
        string searchText = SearchTermTextBox.Text.ToLower();
        var filteredItems = new List<string>();
        if (NewVendor2.SelectedIndex == 1)
        {
            filteredItems = vendor_1_types.Where(item => item.ToLower().Contains(searchText)).ToList();
        }
        if (NewVendor2.SelectedIndex == 2)
        {
            filteredItems = vendor_2_types.Where(item => item.ToLower().Contains(searchText)).ToList();
        }

        if (filteredItems.Any())
        {
            listBox.ItemsSource = filteredItems;
            listBox.Visibility = System.Windows.Visibility.Visible;
        }
        else
        {
            listBox.Visibility = System.Windows.Visibility.Hidden;
        }
    }
    */

    private void PopulateComboBoxes()
    {

        // Default item
        string defaultItem = "Choose Input";

        // Populate NewVendor ComboBox with brand name
        NewVendor.Items.Clear();
        NewVendor.Items.Add(defaultItem);
        foreach (var brand in UiDataService.BrandsWithShapesAndTypes)
        {
            NewVendor.Items.Add(brand.BrandName.ToString());
        }
        NewVendor.SelectedIndex = 0; // Set default item as selected

        // Populate NewVendorV2 ComboBox with brand name
        NewVendorV2.Items.Clear();
        NewVendorV2.Items.Add(defaultItem);
        foreach (var brand in UiDataService.BrandsWithShapesAndTypes)
        {
            NewVendorV2.Items.Add(brand.BrandName.ToString());
        }
        NewVendorV2.SelectedIndex = 0; // Set default item as selected


        // Initialize NewShape with just the default item
        NewShape.Items.Clear();
        NewShape.Items.Add(defaultItem);
        NewShape.SelectedIndex = 0; // Set default item as selected

        // Initialize NewType with just the default item
        NewType.Items.Clear();
        NewType.Items.Add(defaultItem);
        NewType.SelectedIndex = 0; // Set default item as selected

        //InstancePptyFinish.ItemsSource = UiDataService.VendorFinishes;
    }

    private void NewVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string defaultItem = "Choose Input";

        // Clear and reset the NewShape and NewType ComboBoxes
        NewShape.Items.Clear();
        NewShape.Items.Add(defaultItem);
        NewShape.SelectedIndex = 0;

        NewType.Items.Clear();
        NewType.Items.Add(defaultItem);
        NewType.SelectedIndex = 0;

        // Get the selected brand
        string selectedBrand = NewVendor.SelectedItem as string;
        if (!string.IsNullOrEmpty(selectedBrand))
        {
            // Find the brand with the name 'selectedBrand'
            var targetBrand = UiDataService.BrandsWithShapesAndTypes
                .FirstOrDefault(brand => brand.BrandName == selectedBrand);

            if (targetBrand != null)
            {
                // Now 'targetBrand' contains the brand object with the selected name
                // You can now work with targetBrand's Shapes and Types
                foreach (var shape in targetBrand.Shapes)
                {
                    NewShape.Items.Add(shape.ShapeName.ToString());
                }
                NewShape.SelectedIndex = 0;
            }
        }
    }


    private void NewShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string defaultItem = "Choose Input";

        // Clear the existing items in the NewType ComboBox
        NewType.Items.Clear();
        NewType.Items.Add(defaultItem);
        NewType.SelectedIndex = 0;

        // Get the selected brand and style
        string selectedBrand = NewVendor.SelectedItem as string;
        string selectedShape = NewShape.SelectedItem as string;

        // Populate NewType (finishes) based on the selected brand and style
        if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != defaultItem &&
            !string.IsNullOrEmpty(selectedShape) && selectedShape != defaultItem)
        {
            // Find the brand with the name 'selectedBrand'
            var targetBrand = UiDataService.BrandsWithShapesAndTypes
                .FirstOrDefault(brand => brand.BrandName == selectedBrand);

            if (targetBrand != null)
            {
                // Now 'targetBrand' contains the brand object with the selected name
                // You can now work with targetBrand's Shapes and Types
                foreach (var shape in targetBrand.Shapes)
                {
                    foreach (var type in shape.Types)
                    {
                        NewType.Items.Add(type.ToString());
                    }
                }
            }
        }
        //NewShape.SelectedIndex = 0;
    }

    private void NewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Check if all ComboBoxes have a selected value other than the default "Choose a value"
        bool allSelected = NewVendor.SelectedIndex > 0 && NewShape.SelectedIndex > 0 && NewType.SelectedIndex > 0;
        NewCreateButton.IsEnabled = allSelected;
    }


    //private List<string> allTypesOfTheBrand = new List<string>();

    private void NewVendorV2_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected brand
        string selectedBrand = NewVendorV2.SelectedItem as string;

        //allTypesOfTheBrand.Clear();

        // Get the selected brand
        if (!string.IsNullOrEmpty(selectedBrand))
        {
            // Find the brand with the name 'selectedBrand'
            var targetBrand = UiDataService.BrandsWithShapesAndTypes
                .FirstOrDefault(brand => brand.BrandName == selectedBrand);

            if (targetBrand != null)
            {
                //NewTypeV2.Items.Clear();
                foreach (var shape in targetBrand.Shapes)
                {
                    foreach (var type in shape.Types)
                    {
                        NewTypeV2.Items.Add(type);
                        //allTypesOfTheBrand.Add(type);
                    }
                }
            }
            NewTypeV2.SelectedIndex = 0;
        }
        // Update the ComboBox with the full list of types for the selected brand
    }

    private List<string> AllTypesOfTheBrand = new List<string>();

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchQuery = SearchTermTextBox.Text.ToLower();

        //var allTypesOfAGivenBrand = NewTypeV2.Items.Cast<string>().ToList();

        //string[] filteredTypes = allTypesOfAGivenBrand.Where(type => type.ToLower().Contains(searchQuery)).ToArray();

        // Filter the types based on the search query
        //var filteredTypes = allTypesOfTheBrand
        //    .Where(type => type.ToLower().Contains(searchQuery))
        //    .ToList();

        //// Opens the dropdown if there are matchesw
        //NewTypeV2.IsDropDownOpen = filteredTypes.Count > 0;
        //SearchTermTextBox.Focus();
        string searchText = SearchTermTextBox.Text.ToLower();
        var filteredItems = new List<string>();
        if (NewVendorV2.SelectedIndex == 1)
        {
            //filteredItems = vendor_1_types.Where(item => item.ToLower().Contains(searchText)).ToList();
            var filteredTypes = NewTypeV2.Items.Cast<string>().ToArray()
                .Where(type => type.ToLower().Contains(searchText))
                .ToList();
        }

    }


    private void NamesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Enable the button only if an item is selected
        NewCreateButtonV2.IsEnabled = NewTypeV2.SelectedItem != null;
        SearchTermTextBox.Clear();
    }

    private void InstancePptyFinish_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void VendorStyleFinish_VendorName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected brand
        //string selectedBrand = VendorStyleFinish_VendorName.SelectedItem as string;

        // Get the selected brand
        //if (!string.IsNullOrEmpty(selectedBrand))
        //{
        //    // Find the brand with the name 'selectedBrand'
        //    var targetBrand = UiDataService.BrandsWithShapesAndTypes
        //        .FirstOrDefault(brand => brand.BrandName == selectedBrand);

        //    if (targetBrand != null)
        //    {
        //    }
        //    //var availableStyleFinishes = GetAllStyleFinishesOfAGivenBrand(selectedBrand).ToArray();

        //    // now set the update ui to show only availabe items
        //}


    }
}
