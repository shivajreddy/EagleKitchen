using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Button = System.Windows.Controls.Button;
using View = Autodesk.Revit.DB.View;
using MessageBox = System.Windows.Forms.MessageBox;
using SelectionChangedEventArgs = Autodesk.Revit.UI.Events.SelectionChangedEventArgs;


namespace four.EagleKitchen
{
    public enum CabinetConfiguration
    {
        AllLowers,
        AllUppers,
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
        Style2,
        //Style2,
        //Style2,
        //Style2,
        //Style2,
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

            UpdateDynamicUi(currentSelection, doc);

            //UpdateTypeDropDown(currentSelection, doc);

            //if (e.GetSelectedElements().Count == 0) return;
            //ActiveSelectedElementIds = e.GetSelectedElements();
            //EagleKitchenDockUtils.EagleKitchenUi.UIActiveSelections.Text = ActiveSelectedElementIds.ToString();
        }

        public static void OnViewActivated(object sender, EventArgs e)
        {
            // Reset-UI
            EagleKitchenDockUtils.EagleKitchenUi.EK24Views.Text = "";
            EagleKitchenDockUtils.EagleKitchenUi.EK24Sheets.Text = "";

            // Grab the Document
            UIApplication app = sender as UIApplication;
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            //var views = collector.OfClass(typeof(View));
            ICollection<Element> views = collector.OfClass(typeof(View)).WhereElementIsNotElementType().ToElements();

            var sheets = collector.OfClass(typeof(ViewSheet));

            IList<View> targetViews = new List<View>(); 
            IList<ViewSheet> targetSheets = new List<ViewSheet>();

            foreach (View view in views)
            {
                // Check if the view has the parameter and if it matches the target value
                Parameter param = view.LookupParameter("EK24_view_sheet");
                var x = view.Name;

                if (param != null && param.StorageType == StorageType.Integer) // Yes/No parameters are stored as integers
                {
                    // Check if the integer value corresponds to the boolean parameterValue (1 for true, 0 for false)
                    if (param.AsInteger() == 1 ) targetViews.Add(view);
                }
            }

            foreach (ViewSheet viewSheet in sheets)
            {
                // Check if the view has the parameter and if it matches the target value
                Parameter param = viewSheet.LookupParameter("EK24_view_sheet");
                if (param != null && param.StorageType == StorageType.Integer) // Yes/No parameters are stored as integers
                {
                    // Check if the integer value corresponds to the boolean parameterValue (1 for true, 0 for false)
                    if (param.AsInteger() == 1 ) targetSheets.Add(viewSheet);
                }
            }


            // Update-UI
            // add buttons
            foreach (var view in targetViews) EagleKitchenDockUtils.EagleKitchenUi.EK24Views.Text += (view.Name + "\n");
            foreach (var sheet in targetSheets) EagleKitchenDockUtils.EagleKitchenUi.EK24Sheets.Text += (sheet.Name + "\n");

        }

        public static void CreateViewButton()
        {
            // Create a new button
            var newButton = new System.Windows.Controls.Button
            {
                Content = "Click Me",
            };

            // Add an event handler (optional)
            newButton.Click += NewButton_Click;
            EagleKitchenDockUtils.EagleKitchenUi.UIPages.Children.Add(newButton);
        }

        //private static void Update_View(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        //    Button button = sender as Button;
        //    //MessageBox.Show($"Button clicked: {button.Content}");

        //    UiData.GoToViewName = button.Content.ToString();
        //    Main.AppsRequestHandler.RequestType = RequestType.UpdateView;
        //    Main.MyExternalEvent.Raise();
        //    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

        //}



        private static void NewButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button clicked!");

        }

        public static void UpdateDynamicUi(Selection currentSelection, Document doc)
        {
            // Reset the UI-Text first
            //EagleKitchenDockUtils.EagleKitchenUi.TotalSelections.Text = "";

            //var selectedIds = currentSelection.GetElementIds();
            // If not all Objects are not CaseWork category then Exit.
            //if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;


            //string totalSelections = currentSelection.GetElementIds().Count.ToString();
            //EagleKitchenDockUtils.EagleKitchenUi.TotalSelections.Text = totalSelections;

            // Disable the UI
            EagleKitchenDockUtils.EagleKitchenUi.FamilyTypeOptions.IsEnabled = false;
            EagleKitchenDockUtils.EagleKitchenUi.FamilyTypeOptions.ItemsSource= null;
            EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.IsEnabled = false;
            EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.ItemsSource= null;
            EagleKitchenDockUtils.EagleKitchenUi.StyleInstanceParam.IsEnabled = false;

            foreach (var child in EagleKitchenDockUtils.EagleKitchenUi.StyleInstanceParam.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }


            // Enable the UI
            UpdateFamilyTypeDropDown(currentSelection, doc);
            UpdateTypeDropDown(currentSelection, doc);
            UpdateStyleUi(currentSelection, doc);
            //UpdateFinishUi(currentSelection, doc);
            //UpdateHandlesUi(currentSelection, doc);

            // show the family(or)type names of selection, only if the selected are cabinets,
            // if they are not, then disable the text-block
            // Filter our the cabinet families
        }

        public class CaseworkItem
        {
            public string FamilyName { get; set; }
            public string TypeName { get; set; }
            public override string ToString()
            {
                return $"{FamilyName} : {TypeName}";
            }
        }

        public static void UpdateFamilyTypeDropDown(Selection currentSelection, Document doc)
        {
            var selectedIds = currentSelection.GetElementIds();
            // If not all Objects are not CaseWork category then Exit.
            if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

            List<CaseworkItem> caseworkItems = new List<CaseworkItem>();
            //var caseworkItems = new ObservableCollection<CaseworkItem>();

            // Collect all family symbols in the document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> familySymbols = collector.OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Casework)
                .ToElements();
            foreach (FamilySymbol symbol in familySymbols)
            {
                // Each symbol is a family type in the Casework category
                CaseworkItem item = new CaseworkItem
                {
                    TypeName = symbol.Name,
                    FamilyName = symbol.Family.Name
                };
                caseworkItems.Add(item);
            }

            // Enable the UI
            EagleKitchenDockUtils.EagleKitchenUi.FamilyTypeOptions.ItemsSource = caseworkItems;
            EagleKitchenDockUtils.EagleKitchenUi.FamilyTypeOptions.IsEnabled = true;
        }

        public static void UpdateTypeDropDown(Selection currentSelection, Document doc)
        {
            // Make sure that all the selected elements are of the same family
            var selectedIds = currentSelection.GetElementIds();
            // If not all Objects are not CaseWork category then Exit.
            if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

            // Get all the type properties of this family
            if (!AllElementsAreSameFamily(selectedIds, doc)) return;

            var familyName = (doc.GetElement(selectedIds.First()) as FamilyInstance)?.Symbol.FamilyName;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> families = collector.OfClass(typeof(Family)).ToElements();

            Family targetFamily = null;
            foreach (Family family in families)
            {
                if (family.Name.Equals(familyName))
                {
                    targetFamily = family; // Return the family if the name matches
                }
            }

            //var validTypes = targetFamily.GetValidTypes();
            var validTypes = targetFamily.GetFamilySymbolIds();

            List<string> typeNames = new List<string>();
            foreach (ElementId typeId in validTypes)
            {
                ElementType type = doc.GetElement(typeId) as ElementType;
                if (type != null)
                {
                    typeNames.Add(type.Name);
                }
            }

            // Enable the UI
            EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.ItemsSource= typeNames;
            EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.IsEnabled = true;
        }

        private static bool AllElementsAreFamilyInstances(ICollection<ElementId> ids, Document doc)
        {
            foreach (var id in ids)
            {
                var element = doc.GetElement(id);
                if (!(element is FamilyInstance))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AllFamilyInstancesHaveCaseworkStyleParam(ICollection<ElementId> ids, Document doc)
        {
            foreach (var id in ids)
            {
                var familyInstance = doc.GetElement(id) as FamilyInstance;
                if (familyInstance == null) continue;

                var param = familyInstance.LookupParameter("Casework_Style");
                if (param == null)
                {
                    return false;
                }
            }
            return true;
        }


        public static void UpdateStyleUi(Selection currentSelection, Document doc)
        {
            var selectedIds = currentSelection.GetElementIds();
            // If not all Objects are not CaseWork category then Exit.
            if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

            // make sure all family instances have the instance param 'Casework_Style' in them
            // if they don't do not enable these buttons
            if (!AllElementsAreFamilyInstances(selectedIds, doc)) return;
            if (!AllFamilyInstancesHaveCaseworkStyleParam(selectedIds, doc)) return;

            EagleKitchenDockUtils.EagleKitchenUi.StyleInstanceParam.IsEnabled = true;

        }

        public static bool AllElementsAreCaseworkMillWork(ICollection<ElementId> allElementIds, Document doc)
        {
            return allElementIds != null && allElementIds.Count != 0 && allElementIds.All(elementId => doc.GetElement(elementId).Category.Name == "Casework");
        }

        public static bool AllElementsAreSameFamily(ICollection<ElementId> allemElementIds, Document doc)
        {

            var targetFamilyName = (doc.GetElement(allemElementIds.First()) as FamilyInstance)?.Symbol.FamilyName;
            foreach (var elementId in allemElementIds)
            {
                var inst = doc.GetElement(elementId) as FamilyInstance;
                if (inst.Symbol.Family.Name != targetFamilyName) return false;
            }
            return true;
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
                case CabinetConfiguration.AllLowers:
                    filteredElementIds = familyInstances
                        .Where(x => (x as FamilyInstance)?.Symbol.Family.Name.Contains("Base_Cabinet") ?? false)
                        .Select(x => x.Id)
                        .ToList();
                    uiDoc.Selection.SetElementIds(filteredElementIds);
                    break;
                case CabinetConfiguration.AllUppers:
                    filteredElementIds = familyInstances
                        .Where(x => (x as FamilyInstance)?.Symbol.Family.Name.Contains("Wall_Cabinet") ?? false)
                        .Select(x => x.Id)
                        .ToList();
                    uiDoc.Selection.SetElementIds(filteredElementIds);
                    break;
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
            // by this point, we know all selected elements are FamilyInstances
            // and they have the param 'Casework_Style' in them
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Cursor.Current = Cursors.WaitCursor;

            //Thread.Sleep(10000);

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

        public static PrintSetting GetPrintSettingByName(Document doc, string targetName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PrintSetting));

            foreach (PrintSetting ps in collector.ToElements())
            {
                if (ps.Name == targetName)
                {
                    return ps;
                }
            }
            return null; // Return null if no matching print setting is found
        }

        public static void PrintDocument(UIApplication app)
        {

            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Get the print manager from the document
            PrintManager printManager = doc.PrintManager;
            printManager.PrintRange = PrintRange.Select;

            // For now
            var printSettingName = "CABINETRY DRAWINGS";
            var viewSheetSetName = "EAGLE CABINETRY - LOT SPEC";

            // Apply the print setting by name
            PrintSetting printSetting = GetPrintSettingByName(doc, printSettingName);
            if (printSetting != null)
            {
                printManager.PrintSetup.CurrentPrintSetting = printSetting;
            }
            else
            {
                TaskDialog.Show("Error", "Print setting named '" + printSettingName + "' not found.");
                return;
            }

            // Get the ViewSheetSet by name (a set of views/sheets to print)
            ViewSheetSet viewSheetSet = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheetSet))
                .Cast<ViewSheetSet>()
                .FirstOrDefault(vss => vss.Name == viewSheetSetName);

            if (viewSheetSet == null)
            {
                TaskDialog.Show("Error", "ViewSheetSet named '" + viewSheetSetName + "' not found.");
                return;
            }

            // Set the viewsheet set to print
            printManager.ViewSheetSetting.CurrentViewSheetSet = viewSheetSet;
            printManager.CombinedFile = true;

            // Execute the print command
            printManager.SubmitPrint();

            // Make sure to save the current print setting to apply changes
            printManager.PrintSetup.Save();


        }


    }
}
