using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using four.RequestHandlingUtils;
using four.Utils;
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

            CoreData.Text = CoreModel.Utils.EmbeddedData();

            HenniBitmapImage = ImageUtilities.GetCabinetStyleImage("henning.png");


            LoadDynamicButtons();

            EagleConsoleTextBlock.Text = EagleConsole;

        }

        private void LoadDynamicButtons()
        {

            var (views, sheets) = CoreModel.Utils.ParseData();

            foreach (var view in views)
            {
                var button = new Button
                {
                    Content = view,
                };
                button.Click += Update_View; // Assign click event
                UIViews.Children.Add(button); // Add to Views StackPanel
            }
        }

        private static void Update_View(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            Button button = sender as Button;
            //MessageBox.Show($"Button clicked: {button.Content}");

            UiData.GoToViewName = button.Content.ToString();
            Main.AppsRequestHandler.RequestType = RequestType.UpdateView;
            Main.MyExternalEvent.Raise();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

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

        private void ClickDeleteButton(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.Delete;
            Main.MyExternalEvent.Raise();
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

        // :: Customizations ::
        private void ClickCustomizeStyleHenning(object sender, RoutedEventArgs e)
        {

            //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            Main.AppsRequestHandler.RequestType = RequestType.MakeCustomizations;
            EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style1;
            Main.MyExternalEvent.Raise();
            //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

        }
        private void ClickCustomizeStyleMarquez(object sender, RoutedEventArgs e)
        {
            Main.AppsRequestHandler.RequestType = RequestType.MakeCustomizations;
            EagleKitchen.ChosenCabinetStyle = CabinetStyle.Style2;
            Main.MyExternalEvent.Raise();
        }


        private void GoToPage2(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void GoToPage3(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
