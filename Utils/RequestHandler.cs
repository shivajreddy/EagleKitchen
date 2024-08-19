using Autodesk.Revit.UI;
using EK24_old.EagleKitchenView;

namespace EK24_old.RequestHandlingUtils;


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
                EagleKitchenViewModel.SetView(app);
                break;

            // Configuration settings
            case RequestType.UpdateCabinetFamilyAndType:
                EagleKitchenViewModel.UpdateCabinetFamilyAndType(app);
                break;
            case RequestType.UpdateCabinetType:
                EagleKitchenViewModel.UpdateCabinetType(app);
                break;

            // Common settings
            case RequestType.UpdateHasLeftFillerStrip:
                EagleKitchenViewModel.UpdateHasLeftFillerStrip(app);
                break;
            case RequestType.UpdateHasRightFillerStrip:
                EagleKitchenViewModel.UpdateHasRightFillerStrip(app);
                break;
            case RequestType.UpdateLeftFillerStripValue:
                EagleKitchenViewModel.UpdateLeftFillerStripValue(app);
                break;
            case RequestType.UpdateRightFillerStripValue:
                EagleKitchenViewModel.UpdateRightFillerStripValue(app);
                break;

            case RequestType.MakeSelections:
                EagleKitchenViewModel.SelectElements(app);
                break;
            case RequestType.MakeCustomizations:
                EagleKitchenViewModel.SetStyle(app);
                break;

            // Export data
            case RequestType.PrintDrawings:
                EagleKitchenViewModel.PrintDocument(app);
                break;
            case RequestType.ExportQuantitiesToExcel:
                EagleKitchenViewModel.ExportQuantitiesToExcel(app);
                break;

            case RequestType.DevTest:
                EagleKitchenViewModel.DevTest(app);
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
