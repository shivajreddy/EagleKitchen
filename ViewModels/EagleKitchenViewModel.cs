using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using EK24.RequestHandlingUtils;
using EK24.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using SelectionChangedEventArgs = Autodesk.Revit.UI.Events.SelectionChangedEventArgs;
using View = Autodesk.Revit.DB.View;


namespace EK24.EagleKitchenView;

public enum CabinetConfiguration
{
    AllLowers,
    AllUppers,
    AllCabinets,
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
    Style1Henning,
    Style2TouraineBirch,
    Style3StillWater,
    Style4FillMore,
    Style5Langdon,
    Style6Everett,
}


// This is to segregate EagleKitchen Dock logic.
// The 'MainRequestHandler' class is where actual logic of revit-operations are coded,
// but since it will become too large, we use the following utility class to write the code of
// revit-operations here, and then call from 'MainRequestHandler'
public static class EagleKitchenViewModel
{
    // TODO: make a wrapper-class for CabinetFamilyName-Type-Instance
    public static ISet<ElementId> ActiveSelectedElementIds { get; set; }

    // Data for DockUI to update, that can be used for writing fn's below
    public static CabinetConfiguration ChosenCabinetConfiguration { get; set; }
    public static CabinetStyle ChosenCabinetStyle { get; set; }

    public static string UpdateCurrentViewToAnotherViewName { get; set; }


    // Method to append text to the console-TextBox
    public static void AppendLog(string log)
    {
        //EagleKitchenDockUtils.EagleKitchenMainUi.ConsoleTextBox.AppendText(log + "\n");
        //EagleKitchenDockUtils.EagleKitchenMainUi.ConsoleTextBox.ScrollToEnd(); // Scroll to the end to show the latest log
        EagleKitchenDockUtils.EagleKitchenMainUi.ConsoleTextBox.AppendText(log + "\n");
        EagleKitchenDockUtils.EagleKitchenMainUi.ConsoleTextBox.ScrollToEnd(); // Scroll to the end to show the latest log
    }

    public static void ExportQuantitiesToExcel(UIApplication app)
    {
        Document doc = app.ActiveUIDocument.Document;

        // Collect all casework and millwork family instances
        var collector = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(BuiltInCategory.OST_Casework);
        var data = new List<(string FamilyName, string TypeName)>();

        foreach (FamilyInstance instance in collector)
        {
            var familyName = instance.Symbol.Family.Name;
            var typeName = instance.Name;
            data.Add((familyName, typeName));
        }

        // Export to Excel
        //var filePath = @"C:\Users\sreddy\Desktop\test\LindenIII_Purchase_List.xlsx";
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "LindenIII_Purchase_List.xlsx");

        AppendLog("Excel_Export savedto: " + filePath);

        var excelExporter = new ExcelExporter(filePath);
        excelExporter.ExportToExcel(data);
    }

    // This is a test function to check if the plugin is working
    public static void DevTest(UIApplication app)
    {
        TaskDialog.Show("test", "test101");
    }

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

        Selection currentSelection = uiDoc.Selection;
        UpdateDynamicUi(currentSelection, doc);
    }

    public static void OnViewActivated(object sender, EventArgs e)
    {
        // Reset-UI
        EagleKitchenDockUtils.EagleKitchenMainUi.ListOfEK24Views.Children.Clear();
        EagleKitchenDockUtils.EagleKitchenMainUi.ListOfEK24Sheets.Children.Clear();

        // Grab the Document
        var app = sender as UIApplication;
        var uiDoc = app.ActiveUIDocument;
        var doc = app.ActiveUIDocument.Document;

        var collector = new FilteredElementCollector(doc);
        var allViews = collector.OfClass(typeof(View)).ToElements();


        foreach (var view in allViews)
        {
            // Check if the view has the parameter and if it matches the target value
            Parameter param = view.LookupParameter("EK24_view_sheet");

            // Yes/No parameters are stored as integers
            // Check if the integer value corresponds to the boolean parameterValue (1 for true, 0 for false)
            if (param is not { StorageType: StorageType.Integer } || param.AsInteger() != 1) continue;

            var button = new Button
            {
                Content = "",
                Tag = view.Name,
                Height = 30,
                IsEnabled = true,
                Style = EagleKitchenDockUtils.EagleKitchenMainUi.FindResource("GoToViewButtonStyle") as System.Windows.Style

            };
            button.Click += Update_Current_View;

            // Update-UI :: add buttons
            // This is a view sheet -> so a sheet in the project browser
            if (view.GetType() == typeof(ViewSheet))
            {
                var viewSheet = view as ViewSheet;
                button.Content = $"{viewSheet.SheetNumber} : {viewSheet.Name}";
                EagleKitchenDockUtils.EagleKitchenMainUi.ListOfEK24Sheets.Children.Add(button);
            }
            else
            {
                button.Content = $"{view.Name}";
                EagleKitchenDockUtils.EagleKitchenMainUi.ListOfEK24Views.Children.Add(button);
            }
        }
    }

    public static void UpdateDynamicUi(Selection currentSelection, Document doc)
    {
        // :: Enable the UI ::
        // Enable the dropdown boxes
        UpdateFamilyTypeDropDownUi(currentSelection, doc);
        UpdateTypeDropDownUi(currentSelection, doc);

        //Enable the Common Settings
        UpdateCommonSettingsUi(currentSelection, doc);

        // TODO: below two functions to be implemented
        // Enable the Single Door Settings
        UpdateSingleDoorSettingsUi(currentSelection, doc);
        // Enable the Upper Cabinet Settings
        UpdateUpperDoorSettingsUi(currentSelection, doc);

        // Enable the Style tab, Finish tab, Handles tab under 'Selections' tab
        UpdateStyleFinishHandlesUi(currentSelection, doc);
    }

    private static void Update_Current_View(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        var view_name = button.Tag as string;


        Main.AppsRequestHandler.RequestType = RequestType.UpdateView;
        //EagleKitchen.UpdateCurrentViewToAnotherViewName = view_name;
        // Now moving all static UI related data to UiData.cs file
        UiDataService.GoToViewName = view_name;

        EagleKitchenViewModel.UpdateCurrentViewToAnotherViewName = view_name;

        Main.MyExternalEvent.Raise();
    }

    public static void UpdateCommonSettingsUi(Selection currentSelection, Document doc)
    {
        //EagleKitchenDockUtils.EagleKitchenUi.InstanceParamIsButt.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasLeftFillerStrip.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasLeftFillerStrip.IsChecked = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.Text = "";
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripSetButton.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasRightFillerStrip.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasRightFillerStrip.IsChecked = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.Text = "";
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripSetButton.IsEnabled = false;

        var selectedIds = currentSelection.GetElementIds();

        // If not all Objects are not CaseWork category then Exit.
        if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

        // If not all chosen are not FamilyInstances then Exit.
        if (!AllElementsAreFamilyInstances(selectedIds, doc)) return;

        // if they don't have filler strip params, do not enable these buttons
        if (!AllFamilyInstancesHaveTargetParam(selectedIds, doc, "Left_FillerStrip_Width")) return;
        if (!AllFamilyInstancesHaveTargetParam(selectedIds, doc, "Right_FillerStrip_Width")) return;

        // Enable the Checkboxes
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasLeftFillerStrip.IsEnabled = true;
        //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasRightFillerStrip.IsEnabled = true;

        // show current values for filler strips
        // if multiple items are selected show <varies>
        if (selectedIds.Count > 1)
        {
            // show varies if the values of the selected items are different
            //var allLeftParamValues = new List<string>();
            var allLeftParamValues = new HashSet<string>();
            var allRightParamValues = new List<string>();
            foreach (var elementId in selectedIds)
            {
                var inst = doc.GetElement(elementId) as FamilyInstance;
                var leftVal = inst.LookupParameter("Left_FillerStrip_Width")?.AsValueString();
                var rightVal = inst.LookupParameter("Right_FillerStrip_Width")?.AsValueString();
                if (!allLeftParamValues.Contains(leftVal)) break;
                if (!allRightParamValues.Contains(rightVal)) break;
                allLeftParamValues.Add(leftVal);
                allRightParamValues.Add(rightVal);
            }
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.Text = "<varies>";
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.Text = "<varies>";
        }
        else
        {
            var firstId = selectedIds.First();
            var firstElement = doc.GetElement(firstId) as FamilyInstance;

            // Get the current values of the instance params
            var leftFillerStripParam = firstElement.LookupParameter("Left_FillerStrip_Width");
            var rightFillerStripParam = firstElement.LookupParameter("Right_FillerStrip_Width");

            var hasLeftFillerStripParam = firstElement.LookupParameter("Left_FillerStrip");
            var hasRightFillerStripParam = firstElement.LookupParameter("Right_FillerStrip");

            //if (hasLeftFillerStripParam != null)
            //{
            //    var x = hasLeftFillerStripParam.AsInteger();
            //    EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasLeftFillerStrip.IsChecked = hasLeftFillerStripParam.AsInteger() == 1;
            //}
            //if (hasRightFillerStripParam != null)
            //{
            //    EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamHasRightFillerStrip.IsChecked = hasRightFillerStripParam.AsInteger() == 1;
            //}


            //var leftParamAsValueString = leftFillerStripParam.AsValueString();
            //var rightParamAsValueString = rightFillerStripParam.AsValueString();

            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamLeftFillerStripValue.Text = ExtractMeasurement(leftParamAsValueString);
            //EagleKitchenDockUtils.EagleKitchenMainUi.InstanceParamRightFillerStripValue.Text = ExtractMeasurement(rightParamAsValueString);
        }

    }

    static string ExtractMeasurement(string input)
    {
        // Find the position of the hyphen
        int hyphenIndex = input.IndexOf('-');
        if (hyphenIndex == -1)
        {
            return string.Empty; // Return empty if no hyphen is found
        }

        // Extract the substring after the hyphen and trim whitespace and quotes
        string result = input.Substring(hyphenIndex + 1).Trim(' ', '\"');

        return result;
    }

    // TODO: implement these two functions
    public static void UpdateSingleDoorSettingsUi(Selection currentSelection, Document doc)
    {
        //EagleKitchenDockUtils.EagleKitchenMainUi.SingleDoorSettings.IsEnabled = false;
    }

    public static void UpdateUpperDoorSettingsUi(Selection currentSelection, Document doc)
    {
        //EagleKitchenDockUtils.EagleKitchenMainUi.UpperCabinetSettings.IsEnabled = false;
    }

    public class EagleCabinetFamilyInstance
    {
        public string FamilyName { get; set; }
        public string TypeName { get; set; }
        public override string ToString()
        {
            return $"{FamilyName} : {TypeName}";
        }
    }

    public static void UpdateFamilyTypeDropDownUi(Selection currentSelection, Document doc)
    {
        //EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeOptions.IsEnabled = false;
        //EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeOptions.ItemsSource = null;
        EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.IsEnabled = false;
        EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeSettingsUpdate.IsEnabled = false;

        var selectedIds = currentSelection.GetElementIds();
        // If not all Objects are not CaseWork category then Exit.
        if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

        // Get only the cabinet families from the selected 'case-work' elements
        var caseworkItems = new List<EagleCabinetFamilyInstance>();

        // Collect all family symbols in the document
        var collector = new FilteredElementCollector(doc);
        var familySymbols = collector.OfClass(typeof(FamilySymbol))
            .OfCategory(BuiltInCategory.OST_Casework)
            .ToElements();
        foreach (FamilySymbol symbol in familySymbols)
        {
            // only add the family type if it is Eagle Cabinet family
            if (!symbol.Family.Name.StartsWith("Base_Cabinet") &&
                !symbol.Family.Name.StartsWith("Wall_Cabinet")) continue;

            var item = new EagleCabinetFamilyInstance
            {
                TypeName = symbol.Name,
                FamilyName = symbol.Family.Name
            };
            caseworkItems.Add(item);
        }

        // Enable the UI
        //EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeOptions.ItemsSource = caseworkItems;
        //EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeOptions.IsEnabled = true;
        //EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.IsEnabled = true;
    }

    public static void UpdateTypeDropDownUi(Selection currentSelection, Document doc)
    {

        EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.IsEnabled = false;
        EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.ItemsSource = null;

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
        EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.ItemsSource = typeNames;
        EagleKitchenDockUtils.EagleKitchenMainUi.TypeOptions.IsEnabled = true;
        // show the current active size
        //EagleKitchenDockUtils.EagleKitchenUi.TypeOptions.setle
        EagleKitchenDockUtils.EagleKitchenMainUi.FamilyTypeSettingsUpdate.IsEnabled = true;
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

    private static bool AllFamilyInstancesHaveTargetParam(ICollection<ElementId> ids, Document doc, string paramName)
    {
        foreach (var id in ids)
        {
            var familyInstance = doc.GetElement(id) as FamilyInstance;
            if (familyInstance == null) continue;

            var param = familyInstance.LookupParameter(paramName);
            if (param == null)
            {
                return false;
            }
        }
        return true;
    }

    public static void UpdateStyleFinishHandlesUi(Selection currentSelection, Document doc)
    {
        EagleKitchenDockUtils.EagleKitchenMainUi.StyleInstanceParam.IsEnabled = false;
        EagleKitchenDockUtils.EagleKitchenMainUi.MaterialInstanceParam.IsEnabled = false;
        EagleKitchenDockUtils.EagleKitchenMainUi.HandleInstanceParam.IsEnabled = false;

        var selectedIds = currentSelection.GetElementIds();
        // If not all Objects are not CaseWork category then Exit.
        if (!AllElementsAreCaseworkMillWork(selectedIds, doc)) return;

        // make sure all family instances have the instance param 'Casework_Style' in them
        // if they don't do not enable these buttons
        if (!AllElementsAreFamilyInstances(selectedIds, doc)) return;
        if (!AllFamilyInstancesHaveTargetParam(selectedIds, doc, "Casework_Style")) return;

        EagleKitchenDockUtils.EagleKitchenMainUi.StyleInstanceParam.IsEnabled = true;
        EagleKitchenDockUtils.EagleKitchenMainUi.MaterialInstanceParam.IsEnabled = true;
        EagleKitchenDockUtils.EagleKitchenMainUi.HandleInstanceParam.IsEnabled = true;
    }

    public static bool AllElementsAreCaseworkMillWork(ICollection<ElementId> allElementIds, Document doc)
    {
        return doc != null && allElementIds != null && allElementIds.Count != 0 && allElementIds.All(elementId => doc.GetElement(elementId).Category.Name == "Casework");
    }

    public static bool AllElementsAreSameFamily(ICollection<ElementId> allElementIds, Document doc)
    {

        var targetFamilyName = (doc.GetElement(allElementIds.First()) as FamilyInstance)?.Symbol.FamilyName;
        foreach (var elementId in allElementIds)
        {
            var inst = doc.GetElement(elementId) as FamilyInstance;
            if (inst.Symbol.Family.Name != targetFamilyName) return false;
        }
        return true;
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
            if (view.Name == UiDataService.GoToViewName)
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

    public static void SelectElements(UIApplication app)
    {
        Cursor.Current = Cursors.WaitCursor;

        var uiDoc = app.ActiveUIDocument;
        var doc = uiDoc.Document;

        // Create an ElementOwnerView filter with id of active view
        ElementOwnerViewFilter elementOwnerViewFilter = new ElementOwnerViewFilter(doc.ActiveView.Id);

        // Filter for all the instances of 'ChosenCabinetConfiguration'
        FilteredElementCollector coll = new FilteredElementCollector(doc, doc.ActiveView.Id);
        IList<Element> familyInstances = coll.OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();

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
            case CabinetConfiguration.AllCabinets:
                filteredElementIds = familyInstances
                    .Where(x => x is FamilyInstance fi &&
                            (fi.Symbol.Family.Name.Contains("Wall_Cabinet") ||
                             fi.Symbol.Family.Name.Contains("Base_Cabinet")))
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
            case CabinetConfiguration.All2Doors:
                filteredElementIds = familyInstances
                    .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_TwoDoor" ||
                                       (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_TwoDoors+OneDrawer-V3"
                                    )
                    .Select(x => x.Id)
                    .ToList();
                uiDoc.Selection.SetElementIds(filteredElementIds);
                break;
            case CabinetConfiguration.Only2Doors:
                filteredElementIds = familyInstances
                    .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_TwoDoor")
                    .Select(x => x.Id)
                    .ToList();
                uiDoc.Selection.SetElementIds(filteredElementIds);
                break;
            case CabinetConfiguration.Only2Door1Drawer:
                filteredElementIds = familyInstances
                    .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_TwoDoors+OneDrawer-V3")
                    .Select(x => x.Id)
                    .ToList();
                uiDoc.Selection.SetElementIds(filteredElementIds);
                break;
            case CabinetConfiguration.Only2Door2Drawers:
                filteredElementIds = familyInstances
                    .Where(x => (x as FamilyInstance)?.Symbol.Family.Name == "Base_Cabinet_TwoDoors+TwoDrawer")
                    .Select(x => x.Id)
                    .ToList();
                uiDoc.Selection.SetElementIds(filteredElementIds);
                break;
        }

        Cursor.Current = Cursors.Default;
    }

    public static void UpdateCabinetFamilyAndType(UIApplication app) { }

    public static void UpdateCabinetType(UIApplication app)
    {
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
        using (Transaction trans = new Transaction(doc, "EK: Update Cabinet Type"))
        {
            trans.Start();
            var val = UiDataService.HasLeftFillerStrip;

            var instance = doc.GetElement(selectedIds.First()) as FamilyInstance;

            // change the type of the familyinstance
            string newTypeName = UiDataService.chosenTypeValue;

            FamilySymbol newFamilySymbol = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfType<FamilySymbol>()
                .FirstOrDefault(s => s.FamilyName == instance.Symbol.Family.Name && s.Name == newTypeName);

            // Ensure the new family symbol is loaded into the project and activated
            if (!newFamilySymbol.IsActive)
            {
                newFamilySymbol.Activate();
                doc.Regenerate();
            }

            // Set the new symbol to the family instance
            instance.Symbol = newFamilySymbol;


            trans.Commit();
        }
        Cursor.Current = Cursors.Default;

    }

    public static void UpdateHasLeftFillerStrip(UIApplication app)
    {
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
        using (Transaction trans = new Transaction(doc, "EK: Modify Left Filler Strip"))
        {
            trans.Start();
            var val = UiDataService.HasLeftFillerStrip;

            var instance = doc.GetElement(selectedIds.First()) as FamilyInstance;
            var param = instance.LookupParameter("Left_FillerStrip");
            var x = param.AsInteger();
            param.Set(param.AsInteger());

            trans.Commit();
        }
        Cursor.Current = Cursors.Default;

    }

    public static void UpdateHasRightFillerStrip(UIApplication app)
    {
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
        using (Transaction trans = new Transaction(doc, "EK: Modify Left Filler Strip"))
        {
            trans.Start();
            var val = UiDataService.HasLeftFillerStrip;

            var instance = doc.GetElement(selectedIds.First()) as FamilyInstance;
            var param = instance.LookupParameter("Right_FillerStrip");
            param.Set(param.AsInteger());

            trans.Commit();
        }
        Cursor.Current = Cursors.Default;



    }

    public static void UpdateLeftFillerStripValue(UIApplication app)
    {
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
        using (Transaction trans = new Transaction(doc, "EK: Modify Left Filler Strip"))
        {
            trans.Start();
            var val = UiDataService.LeftFillerStripValue;
            var finalVal = double.Parse(val) / 12;

            var instance = doc.GetElement(selectedIds.First()) as FamilyInstance;
            var param = instance.LookupParameter("Left_FillerStrip_Width");
            param.Set(finalVal);

            trans.Commit();
        }
        Cursor.Current = Cursors.Default;
    }

    public static void UpdateRightFillerStripValue(UIApplication app)
    {
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
        using (Transaction trans = new Transaction(doc, "EK: Modify Right Filler Strip"))
        {
            trans.Start();
            var val = UiDataService.RightFillerStripValue;
            trans.Commit();
        }
        Cursor.Current = Cursors.Default;
    }

    public static void SetStyle(UIApplication app)
    {
        // by this point, we know all selected elements are FamilyInstances
        // and they have the param 'Casework_Style' in them
        UIDocument uiDoc = app.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Get the current selection
        ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

        if (selectedIds.Count == 0) return;

        // Start a transaction to modify document
        Cursor.Current = Cursors.WaitCursor;
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
                        case CabinetStyle.Style1Henning:
                            param.Set(1);
                            break;
                        case CabinetStyle.Style2TouraineBirch:
                            param.Set(2);
                            break;
                        case CabinetStyle.Style3StillWater:
                            param.Set(3);
                            break;
                        case CabinetStyle.Style4FillMore:
                            param.Set(4);
                            break;
                        case CabinetStyle.Style5Langdon:
                            param.Set(5);
                            break;
                        case CabinetStyle.Style6Everett:
                            param.Set(6);
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


