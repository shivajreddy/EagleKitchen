using Autodesk.Revit.UI;

namespace four.RequestHandlingUtils
{

    // Since we don't have multiple Execute functions, because IExternalEventHandler interface
    // expects one `Execute` method, we define the all types of events as RequestType enumeration.
    // And this is also a ppty on the `MainRequestHandler` class.
    // our plugin i.e., 'Main' class creates a MainRequestHandler instance, and we update the 
    // RequestType ppty on that MainRequestHandler instance, before calling the Execute method.
    public enum RequestType
    {
        Delete,
        UpdateView,
        MakeSelections,
        MakeCustomizations,
        PrintDrawings,

        DevTest,
        None,
    }


    public class MainRequestHandler: IExternalEventHandler
    {

        // Ui should set this RequestType ppty before calling the Execute method
        public RequestType RequestType { get; set; }

        public void Execute(UIApplication app)
        {
            // Execute the fn based on the type of request
            switch (RequestType)
            {
                case RequestType.None:
                    break;
                case RequestType.Delete:
                    // Call the that events method handler
                    EagleKitchen.EagleKitchen.Delete(app);
                    break;
                case RequestType.UpdateView:
                    EagleKitchen.EagleKitchen.SetView(app);
                    break;
                case RequestType.MakeSelections:
                    EagleKitchen.EagleKitchen.SelectElements(app);
                    break;
                case RequestType.DevTest:
                    EagleKitchen.EagleKitchen.DevTest(app);
                    break;
                case RequestType.MakeCustomizations:
                    EagleKitchen.EagleKitchen.SetStyle(app);
                    break;
                case RequestType.PrintDrawings:
                    EagleKitchen.EagleKitchen.PrintDocument(app);
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
