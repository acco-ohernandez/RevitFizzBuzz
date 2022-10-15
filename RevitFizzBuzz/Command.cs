#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RevitFizzBuzz
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            double offset = 0.03;
            double offsetCalc = offset * doc.ActiveView.Scale;

            XYZ curPoint = new XYZ(0, 0, 0);
            XYZ offsetPoint = new XYZ(0, offsetCalc, 0);

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(TextNoteType));

            // This transaction lock the Revit model while it's being edited.
            string transactionNote = "Create Text Note from command class. ";
            Transaction t = new Transaction(doc, transactionNote);
            t.Start();

            int range = 100;
            for (int i = 1; i <= range; i++)
            {
                double divisibleBy3 = i % 3;
                double divisibleBy5 = i % 5;
                string result = "";
                if (divisibleBy3 == 0 && divisibleBy5 == 0)
                {
                    result = i + ": Divisible by 3 and 5: FIZZ-BUZZ";
                }
                else if (divisibleBy3 == 0)
                {
                    result = i + ": Divisible by 3: FIZZ";

                }
                else if (divisibleBy5 == 0)
                {
                    result = i + ": Divisible by 5: BUZZ";

                }
                else
                {
                    result = i + ":============================== Not divisable by 3 nor 5";
                }
                TextNote curNote = TextNote.Create(doc, doc.ActiveView.Id, curPoint, result, collector.FirstElementId());
                curPoint = curPoint.Subtract(offsetPoint);
            }


            t.Commit();
            t.Dispose();


            return Result.Succeeded;
        }
    }
}
