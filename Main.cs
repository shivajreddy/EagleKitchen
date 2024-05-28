using Autodesk.Revit.UI;

using four.RequestHandlingUtils;
using four.EagleKitchen;


namespace four
{


    public class Main : IExternalApplication
    {
        // static Request-handler variables
        public static MainRequestHandler AppsRequestHandler { get; set; }
        public static ExternalEvent MyExternalEvent { get; set; }

        private const string PluginTabName = "EagleKitchen";
        private const string PluginPanelName = "EagleKitchen";
        private static RibbonPanel _ribbonPanel;
        public Result OnStartup(UIControlledApplication application)
        {
            // Create Tab, Panel on RevitUI
            application.CreateRibbonTab(PluginTabName);
            _ribbonPanel = application.CreateRibbonPanel(PluginTabName, PluginPanelName);

            // Set up the EventHandler
            AppsRequestHandler = new MainRequestHandler();
            MyExternalEvent = ExternalEvent.Create(AppsRequestHandler);

            // Register Docks, Add PushButtons to Panels
            EagleKitchenDockUtils.RegisterDockablePanel(application);
            ShowDockCommand.CreatePushButtonAndAddToPanel(_ribbonPanel);

            // Subscribe to Events
            // List of Revit Events you can subscribe to: https://www.revitapidocs.com/2024/418cd49d-9c2f-700f-3db2-fcbe8929c5e5.htm
            // For each event, look at that particular EventArgs that revit will pass as 2nd arg to the given function.
            // the first argument is the sender: Here sender is the 'UIApplication' not 'UIControlledApplication' nor 'Application',
            // but however we can type cast to 'Application' type, if we need that type.
            application.SelectionChanged += EagleKitchen.EagleKitchen.OnSelectionChange;


            application.ViewActivated += EagleKitchen.EagleKitchen.OnViewActivated;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


    }

}
