using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

namespace four.EagleKitchen
{
    public enum CabinetConfiguration
    {
        All1Door,
        Only1Door,
        Only1Door1Drawer,
        All2Doors,
        Only2Doors,
        Only2Door1Drawer,
        Only2Door2Drawers,
    }

    public enum CabinetStyle
    {
        Style1,
        Style2
    }


    // This is to segregate EagleKitchen Dock logic.
    // The 'MainRequestHandler' class is where actual logic of revit-operations are coded,
    // but since it will become too large, we use the following utility class to write the code of
    // revit-operations here, and then call from 'MainRequestHandler'
    public static class EagleKitchen
    {

        // TODO: make a wrapper-class for CabinetFamilyName-Type-Instance
        public static ISet<ElementId> ActiveSelectedElementIds { get; set; }

        // Data for DockUI to update, that can be used for writing fn's below
        public static CabinetConfiguration ChosenCabinetConfiguration { get; set; }
        public static CabinetStyle ChosenCabinetStyle { get; set; }

        // Events functions. These execute when that revit event/behaviour happens
        public static void OnSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            // https://www.revitapidocs.com/2024/7d51c9f8-1bea-32ec-0e54-5921242e57c3.htm
            // You can use the `sender` and typecast to 'UIApplication'. and from that you can any of the following
            //      UIApplication app = commandData.Application;
            //      UIDocument uiDoc = app.ActiveUIDocument;
            //      Document doc = app.ActiveUIDocument.Document;
            // OR you can use the methods available on the <particular>EventArgs variable

            UIApplication app = sender as UIApplication;
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;
            //var doc = e.GetDocument();    // could've used this directly if all we want is Document

            Selection currentSelection = uiDoc.Selection;

            if (e.GetSelectedElements().Count == 0) return;
            ActiveSelectedElementIds = e.GetSelectedElements();

            EagleKitchenDockUtils.EagleKitchenUi.UIActiveSelections.Text = ActiveSelectedElementIds.ToString();
        }

        public static void OnDocumentChanged(object sender, EventArgs e)
        {

            var application = sender as Application;

            EagleKitchenDockUtils.EagleKitchenUi.EagleConsoleTextBlock.Text = "hi";


            // for now log something to the dev tab
            EagleKitchenDockUtils.EagleKitchenUi.EagleConsoleTextBlock.Text += application.Username;
            EagleKitchenDockUtils.EagleKitchenUi.EagleConsoleTextBlock.Text += e.ToString();

        }


        // Commands basically, which will get called through the EventHandler
        public static void Delete(UIApplication app)
        {
            var doc = app.ActiveUIDocument.Document;

            using (var trans = new Transaction(doc, "EK: Delete Items"))
            {
                trans.Start();
                doc.Delete(ActiveSelectedElementIds);
                trans.Commit();
            }

        }

        public static void SetView(UIApplication app)
        {

            Document doc = app.ActiveUIDocument.Document;

            FilteredElementCollector coll = new FilteredElementCollector(doc);

            var allViews = coll.OfClass(typeof(View));

            //string targetViewName = "EK 3D";
            View targetView = null;

            foreach (View view in allViews)
            {
                if (view.Name == UiData.GoToViewName)
                {
                    targetView = view;
                    break;
                }
            }

            if (targetView != null)
            {
                app.ActiveUIDocument.ActiveView = targetView;
            }

            ViewSheet targetViewSheet = null;


        }

        public static void DevTest(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            TaskDialog.Show("Dev", "Testing success");
        }

        public static void SelectElements(UIApplication app)
        {
            Cursor.Current = Cursors.WaitCursor;

            var uiDoc = app.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Filter for all the instances of 'ChosenCabinetConfiguration'
            FilteredElementCollector coll = new FilteredElementCollector(doc);
            IList<Element> familyInstances =
                coll.OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();

            ICollection<ElementId> filteredElementIds;

            switch (ChosenCabinetConfiguration)
            {
                case CabinetConfiguration.All1Door:
                    filteredElementIds = familyInstances
                        .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor-V2" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor-V3" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor+OneDrawer-V2" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor+OneDrawer-V2"
                                        )
                        .Select(x => x.Id)
                        .ToList();

                    uiDoc.Selection.SetElementIds(filteredElementIds);
                    break;
                case CabinetConfiguration.Only1Door:
                    filteredElementIds = familyInstances
                        .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor-V2" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor-V3"
                                        )
                        .Select(x => x.Id)
                        .ToList();
                    uiDoc.Selection.SetElementIds(filteredElementIds);
                    break;
                case CabinetConfiguration.Only1Door1Drawer:
                    filteredElementIds = familyInstances
                        .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor+OneDrawer-V2" ||
                                           (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_OneDoor-V3"
                                        )
                        .Select(x => x.Id)
                        .ToList();
                    uiDoc.Selection.SetElementIds(filteredElementIds);
                    break;
                default:
                    break;
            } 

            Cursor.Current = Cursors.Default;
        }

        public static void SetStyle(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Cursor.Current = Cursors.WaitCursor;

            Thread.Sleep(10000);

            // Get the current selection
            ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

            if (selectedIds.Count == 0) return;

            // Start a transaction to modify document
            using (Transaction trans = new Transaction(doc, "EK: Update Styles"))
            {
                trans.Start();
                foreach (ElementId id in selectedIds)
                {
                    Element elem = doc.GetElement(id);

                    // Check if the element has the property 'Mark'
                    //Parameter param = elem.LookupParameter("Mark");
                    Parameter param = elem.LookupParameter("Casework_Style");
                    if (param != null && param.IsReadOnly == false)
                    {
                        // Set a new value for the 'Mark' parameter
                        switch (ChosenCabinetStyle)
                        {
                            case CabinetStyle.Style1:
                                param.Set(1);
                                break;
                            case CabinetStyle.Style2:
                                param.Set(2);
                                break;

                        }
                    }
                }

                trans.Commit();
            }

            Cursor.Current = Cursors.Default;
        }

    }
}
