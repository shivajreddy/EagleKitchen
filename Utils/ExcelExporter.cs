using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;

namespace EK24.Utils
{
    public class ExcelExporter
    {
        private string FilePath { get; set; }

        public ExcelExporter(string filePath)
        {
            FilePath = filePath;
            EagleKitchen.EagleKitchen.AppendLog("created an instance of ExcelExporter with filePath: " + filePath);
        }

        public void ExportToExcel(List<(string FamilyName, string TypeName)> data)
        {
            EagleKitchen.EagleKitchen.AppendLog("Starting 'ExportToExcel' method");

            // Here Application is Microsoft.Office.Interop.Excel.Application
            Application excelApp = new Application();

            if (excelApp == null)
            {
                //EagleKitchen.EagleKitchen.AppendLog($"{nameof(ExportToExcel)}");
                EagleKitchen.EagleKitchen.AppendLog("Failed to create excel app");
                throw new InvalidOperationException("Excel is not properly installed.");
            }

            EagleKitchen.EagleKitchen.AppendLog("'excelApp' created");

            // Create a new Workbook
            Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            EagleKitchen.EagleKitchen.AppendLog("workbook added Type.Missing");
            Worksheet worksheet = workbook.Sheets[1];
            EagleKitchen.EagleKitchen.AppendLog("workbook sheets[1]");
            worksheet = (Worksheet)workbook.ActiveSheet;
            EagleKitchen.EagleKitchen.AppendLog("worksheet is set to activesheet");
            worksheet.Name = "Cabinets";
            EagleKitchen.EagleKitchen.AppendLog("worksheet name is now 'Cabinets'");

            worksheet.Cells[1, 1] = "Family Name";
            EagleKitchen.EagleKitchen.AppendLog("'Cabinets' worksheet cells[1,1] is Family Name");
            worksheet.Cells[1, 2] = "Type Name";
            EagleKitchen.EagleKitchen.AppendLog("'Cabinets' worksheet cells[1,2] is Type Name");

            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = data[i].FamilyName;
                worksheet.Cells[i + 2, 2] = data[i].TypeName;
            }
            EagleKitchen.EagleKitchen.AppendLog("Data written to 'Cabinets' workbook");

            // Save the Workbook
            workbook.SaveAs(FilePath, XlFileFormat.xlWorkbookDefault);

            EagleKitchen.EagleKitchen.AppendLog("'Cabinets' workbook Saved");

            workbook.Close();
            excelApp.Quit();

            // Release the COM objects
            ReleaseObject(worksheet);
            ReleaseObject(workbook);
            ReleaseObject(excelApp);

        }


        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new InvalidOperationException("Unable to release the object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
