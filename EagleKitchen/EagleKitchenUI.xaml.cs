using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using EK24.RequestHandlingUtils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Button = System.Windows.Controls.Button;


namespace EK24.EagleKitchen
{
    public partial class EagleKitchenUi : IDockablePaneProvider
    {
        // Static variables
        public static ISet<ElementId> ActiveSelectedElementIds;
        public static string EagleConsole = "empty console";

        public BitmapImage HenniBitmapImage { get; private set; }


        /*
        private ObservableCollection<string> _items;
        private ObservableCollection<string> _filteredItems;
        private string _searchText;

        public ObservableCollection<string> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public ObservableCollection<string> FilteredItems
        {
            get => _filteredItems;
            set
            {
                _filteredItems = value;
                OnPropertyChanged(nameof(FilteredItems));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterItems();
            }
        }
        */

        // Constructor for the XAML page
        public EagleKitchenUi()
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
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.AllLowers;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionAllUppers(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.AllUppers;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionAllCabinets(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.AllCabinets;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionAll1Door(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.All1Door;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionOnly1Door(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.Only1Door;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionOnly1Door1Drawer(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.Only1Door1Drawer;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionAll2Doors(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.All2Doors;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionOnly2Doors(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.Only2Doors;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionOnly2Doors1Drawer(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.Only2Door1Drawer;
            Main.MyExternalEvent.Raise();
        }
        private void ClickSelectionOnly2Doors2Drawers(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeSelections;
            EagleKitchen.ChosenCabinetConfiguration = CabinetConfiguration.Only2Door2Drawers;
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
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style1Henning;
                    break;
                case "Style2":
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style2TouraineBirch;
                    break;
                case "Style3":
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style3StillWater;
                    break;
                case "Style4":
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style4FillMore;
                    break;
                case "Style5":
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style5Langdon;
                    break;
                case "Style6":
                    EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style6Everett;
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
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamLeftFillerStripValue.IsEnabled = true;
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamLeftFillerStripSetButton.IsEnabled = true;

                // Set the Event so that Event Handler knows to use the appropriate Event
                UiData.HasLeftFillerStrip = true;
                Main.AppsRequestHandler.RequestType = RequestType.UpdateHasLeftFillerStrip;
                Main.MyExternalEvent.Raise();
            }
            else
            {
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamLeftFillerStripValue.IsEnabled = false;
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamLeftFillerStripSetButton.IsEnabled = false;

                UiData.HasLeftFillerStrip = false;
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
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamRightFillerStripValue.IsEnabled = true;
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamRightFillerStripSetButton.IsEnabled = true;

                // Set the Event so that Event Handler knows to use the appropriate Event
                Main.AppsRequestHandler.RequestType = RequestType.UpdateHasLeftFillerStrip;
                Main.MyExternalEvent.Raise();

                UiData.HasRightFillerStrip = true;
                Main.AppsRequestHandler.RequestType = RequestType.UpdateHasRightFillerStrip;
                Main.MyExternalEvent.Raise();
            }
            else
            {
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamRightFillerStripValue.IsEnabled = false;
                EagleKitchenDockUtils.EagleKitchenUi.InstanceParamRightFillerStripSetButton.IsEnabled = false;

                UiData.HasRightFillerStrip = false;
                Main.AppsRequestHandler.RequestType = RequestType.UpdateHasRightFillerStrip;
                Main.MyExternalEvent.Raise();
            }
        }

        private void ButtonClick_SetLeftFillerStripWidth(object sender, RoutedEventArgs e)
        {
            var valueInString = EagleKitchenDockUtils.EagleKitchenUi.InstanceParamLeftFillerStripValue.Text;
            UiData.LeftFillerStripValue = valueInString;

            // Set the Event so that Event Handler knows to use the appropriate Event
            Main.AppsRequestHandler.RequestType = RequestType.UpdateLeftFillerStripValue;
            Main.MyExternalEvent.Raise();
        }
        private void ButtonClick_SetRightFillerStripWidth(object sender, RoutedEventArgs e)
        {
            var valueInString = EagleKitchenDockUtils.EagleKitchenUi.InstanceParamRightFillerStripValue.Text;
            UiData.RightFillerStripValue = valueInString;

            // Set the Event so that Event Handler knows to use the appropriate Event
            Main.AppsRequestHandler.RequestType = RequestType.UpdateRightFillerStripValue;
            Main.MyExternalEvent.Raise();
        }

        private void FamilyTypeSettingsUpdate_Click(object sender, RoutedEventArgs e)
        {
            // chosen type
            var chosenType = EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.SelectedItem;

            UiData.chosenTypeValue = chosenType as string;

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
            foreach (var brand in UiData.BrandNames)
            {
                NewVendor.Items.Add(brand);
            }
            NewVendor.SelectedIndex = 0; // Set default item as selected

            // Initialize NewShape with just the default item
            NewShape.Items.Clear();
            NewShape.Items.Add(defaultItem);
            NewShape.SelectedIndex = 0; // Set default item as selected

            // Initialize NewType with just the default item
            NewType.Items.Clear();
            NewType.Items.Add(defaultItem);
            NewType.SelectedIndex = 0; // Set default item as selected
        }

        private void NewVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string defaultItem = "Choose Input";

            // Clear and reset the NewShape and NewType ComboBoxes
            NewShape.Items.Clear();
            NewShape.Items.Add(defaultItem);
            NewShape.IsEnabled = false;
            NewShape.SelectedIndex = 0;

            NewType.Items.Clear();
            NewType.Items.Add(defaultItem);
            NewType.SelectedIndex = 0;

            // Get the selected brand
            string selectedBrand = NewVendor.SelectedItem as string;

            // Populate NewShape (styles) based on the selected brand
            if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != defaultItem)
            {
                if (UiData.StylesByBrand.ContainsKey(selectedBrand))
                {
                    foreach (var style in UiData.StylesByBrand[selectedBrand])
                    {
                        NewShape.Items.Add(style);
                    }
                }
                NewShape.SelectedIndex = 0; // Set default item as selected
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
            string selectedStyle = NewShape.SelectedItem as string;

            // Populate NewType (finishes) based on the selected brand and style
            if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != defaultItem &&
                !string.IsNullOrEmpty(selectedStyle) && selectedStyle != defaultItem)
            {
                var key = (selectedBrand, selectedStyle);
                if (UiData.FinishesByStyle.ContainsKey(key))
                {
                    foreach (var finish in UiData.FinishesByStyle[key])
                    {
                        NewType.Items.Add(finish);
                    }
                }
                NewType.SelectedIndex = 0; // Set default item as selected
            }
        }

        private void NewType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Check if all ComboBoxes have a selected value other than the default "Choose a value"
            bool allSelected = NewVendor.SelectedIndex > 0 && NewShape.SelectedIndex > 0 && NewType.SelectedIndex > 0;
            NewCreateButton.IsEnabled = allSelected;
        }





        /*
        private void NewVendor_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (NewVendor.SelectedIndex == 0)
            {
                NewShape.SelectedIndex = 0;
                NewShape.IsEnabled = false;

                NewType.SelectedIndex = 0;
                NewType.IsEnabled = false;

                NewCreateButton.IsEnabled = false;
            }
            else
            {
                NewShape.SelectedIndex = 0;
                NewShape.IsEnabled = true;
            }
        }
        private void NewShape_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (NewShape.SelectedIndex == 0)
            {
                NewType.SelectedIndex = 0;
                NewType.IsEnabled = false;

                NewCreateButton.IsEnabled = false;
            }
            else
            {
                NewType.SelectedIndex = 0;
                NewType.IsEnabled = true;
            }
        }
        private void NewType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Check if all ComboBoxes have a selected value other than the default "Choose a value"
            bool allSelected = NewVendor.SelectedIndex > 0 && NewShape.SelectedIndex > 0 && NewType.SelectedIndex > 0;
            NewCreateButton.IsEnabled = allSelected;
        }
        */

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
