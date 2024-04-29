﻿using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using four.Utils;

namespace four.EagleKitchen
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ShowDockCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            EagleKitchenDockUtils.ShowDockablePanel(commandData.Application);
            return Result.Succeeded;
        }

        public static void CreatePushButtonAndAddToPanel(RibbonPanel ribbonPanel)
        {
            var declaringType = MethodBase.GetCurrentMethod()?.DeclaringType;
            if (declaringType == null) return;
            var pushButtonName = declaringType?.Name;
            const string pushButtonTextName = "Show EagleKitchen";
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyLocation = assembly.Location;
            const string iconName = "icon.png";
            const string fullClassName = "four.EagleKitchen.ShowDockCommand";
            const string toolTipInfo = "Show the EagleKitchen Dock";

            var pushButtonData = new PushButtonData(
                name: pushButtonName,
                text: pushButtonTextName,
                assemblyName: assemblyLocation,
                className: fullClassName
            )
            {
                ToolTip = toolTipInfo,
                Image = ImageUtilities.LoadImage(assembly, iconName),
                LargeImage = ImageUtilities.LoadImage(assembly, iconName),
                ToolTipImage = ImageUtilities.LoadImage(assembly, iconName)
            };
            ribbonPanel.AddItem(pushButtonData);
        }
    }


}
