using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using four.RequestHandlingUtils;
using Button = System.Windows.Controls.Button;


namespace four.EagleKitchen
{
    public partial class EagleKitchenUi : IDockablePaneProvider
    {

        // Static variables
        public static ISet<ElementId> ActiveSelectedElementIds;
        public static string EagleConsole = "empty console";

        public BitmapImage HenniBitmapImage { get; private set; }

        // Constructor for the XAML page
        public EagleKitchenUi()
        {
            InitializeComponent();
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


        //private void ClickOpenView(object sender, RoutedEventArgs e)
        //{
        //    Main.AppsRequestHandler.RequestType = RequestType.UpdateView;
        //    Main.MyExternalEvent.Raise();
        //}

        private void ClickDevTest(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.DevTest;
            Main.MyExternalEvent.Raise();
        }





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
    }
}
