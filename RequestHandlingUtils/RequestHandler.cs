using Autodesk.Revit.UI;

namespace EK24.RequestHandlingUtils
{

    // Since we don't have multiple Execute functions, because IExternalEventHandler interface
    // expects one `Execute` method, we define the all types of events as RequestType enumeration.
    // And this is also a ppty on the `MainRequestHandler` class.
    // our plugin i.e., 'Main' class creates a MainRequestHandler instance, and we update the 
    // RequestType ppty on that MainRequestHandler instance, before calling the Execute method.
    public enum RequestType
    {
        UpdateView,

        // configuration settings
        UpdateCabinetFamilyAndType,
        UpdateCabinetType,

        // Common Settings
        UpdateHasLeftFillerStrip,
        UpdateHasRightFillerStrip,
        UpdateLeftFillerStripValue,
        UpdateRightFillerStripValue,

        MakeSelections,
        MakeCustomizations,

        PrintDrawings,
        ExportQuantitiesToExcel,

        DevTest,
        None,
    }


    public class MainRequestHandler : IExternalEventHandler
    {

        // Ui should set this RequestType ppty before calling the Execute method
        public RequestType RequestType { get; set; }

        public void Execute(UIApplication app)
        {
            // Execute the fn based on the type of request
            switch (RequestType)
            {
                case RequestType.UpdateView:
                    EagleKitchen.EagleKitchen.SetView(app);
                    break;

                // Configuration settings
                case RequestType.UpdateCabinetFamilyAndType:
                    EagleKitchen.EagleKitchen.UpdateCabinetFamilyAndType(app);
                    break;
                case RequestType.UpdateCabinetType:
                    EagleKitchen.EagleKitchen.UpdateCabinetType(app);
                    break;

                // Common settings
                case RequestType.UpdateHasLeftFillerStrip:
                    EagleKitchen.EagleKitchen.UpdateHasLeftFillerStrip(app);
                    break;
                case RequestType.UpdateHasRightFillerStrip:
                    EagleKitchen.EagleKitchen.UpdateHasRightFillerStrip(app);
                    break;
                case RequestType.UpdateLeftFillerStripValue:
                    EagleKitchen.EagleKitchen.UpdateLeftFillerStripValue(app);
                    break;
                case RequestType.UpdateRightFillerStripValue:
                    EagleKitchen.EagleKitchen.UpdateRightFillerStripValue(app);
                    break;

                case RequestType.MakeSelections:
                    EagleKitchen.EagleKitchen.SelectElements(app);
                    break;
                case RequestType.MakeCustomizations:
                    EagleKitchen.EagleKitchen.SetStyle(app);
                    break;

                // Export data
                case RequestType.PrintDrawings:
                    EagleKitchen.EagleKitchen.PrintDocument(app);
                    break;
                case RequestType.ExportQuantitiesToExcel:
                    EagleKitchen.EagleKitchen.ExportQuantitiesToExcel(app);
                    break;

                case RequestType.DevTest:
                    EagleKitchen.EagleKitchen.DevTest(app);
                    break;
                case RequestType.None:
                    break;
            }
        }

        // required method by the interface IExternalEventHandler
        public string GetName()
        {
            return "My Request Handler";
        }
    }
}
