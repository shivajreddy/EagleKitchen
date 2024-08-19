using Autodesk.Revit.UI;
using EK24_old.EagleKitchenView;
using System;


namespace EK24_old;

// This utility class defines the DockablePanel and which internally Creates an instance of the XAML UI,
// and uses that to
// We expose the XAML-UI Page externally by making it static, so that we can modify the UI from outside.

// All the core functionality of needed by the XAML-UI is defined in EagleKitchen
// This is for just setting up the dock, everything that happens inside the dock
// is written inside EagleKitchenUtils class
public static class EagleKitchenDockUtils
{
    public static string PanelId = "5AC2AC54-638F-4E89-9640-AC33D22B7E0B";
    private static readonly DockablePaneId DockId = new DockablePaneId(new Guid(PanelId));
    private const string DockName = "Eagle Kitchen_old";
    //public static EagleKitchenUi EagleKitchenUi;
    public static EagleKitchenMainUi EagleKitchenMainUi;


    public static void RegisterDockablePanel(UIControlledApplication uiControlledApplication)
    {
        // no need to call this method specifically
        // its now moved to the static classe's constructor
        //UiData.InitializeFromJson();

        //EagleKitchenUi = new EagleKitchenUi();
        //uiControlledApplication.RegisterDockablePane(DockId, DockName, EagleKitchenUi);

        // i want to basically make sure this service class first gets created because 
        // uiControlledApplication.RegisterDockablePane(DockId, DockName, EagleKitchenMainUi); depends on
        // the data the service creates

        UiDataService.StartTheDataService();

        EagleKitchenMainUi = new EagleKitchenMainUi();
        uiControlledApplication.RegisterDockablePane(DockId, DockName, EagleKitchenMainUi);
    }

    public static void ShowDockablePanel(UIApplication app)
    {
        var dock = app.GetDockablePane(DockId);
        // Don't show if already is showing
        if (dock == null || dock.IsShown()) return;
        dock.Show();
    }
}
