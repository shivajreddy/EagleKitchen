using Autodesk.Revit.UI;
using System;


namespace EK24.EagleKitchen
{
    // This utility class defines the DockablePanel and which internally Creates an instance of the XAML UI,
    // and uses that to
    // We expose the XAML-UI Page externally by making it static, so that we can modify the UI from outside.

    // All the core functionality of needed by the XAML-UI is defined in EagleKitchen
    // This is for just setting up the dock, everything that happens inside the dock
    // is written inside EagleKitchenUtils class
    public static class EagleKitchenDockUtils
    {
        public static string PanelId = "409EC347-50BC-48D2-BBEB-48B40E799927";
        private static readonly DockablePaneId DockId = new DockablePaneId(new Guid(PanelId));
        private const string DockName = "Eagle Kitchen";
        public static EagleKitchenUi EagleKitchenUi;

        public static void RegisterDockablePanel(UIControlledApplication uiControlledApplication)
        {
            // no need to call this method specifically
            // its now moved to the static classe's constructor
            //UiData.InitializeFromJson();

            EagleKitchenUi = new EagleKitchenUi();
            uiControlledApplication.RegisterDockablePane(DockId, DockName, EagleKitchenUi);
        }

        public static void ShowDockablePanel(UIApplication app)
        {
            var dock = app.GetDockablePane(DockId);
            // Don't show if already is showing
            if (dock == null || dock.IsShown()) return;
            dock.Show();
        }
    }
}
