using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Models;

namespace BinaryKits.Zpl.Viewer.CommandAnalyzers
{
    public class Code11BarcodeZplCommandAnalyzer : ZplCommandAnalyzerBase
    {
        public Code11BarcodeZplCommandAnalyzer() : base("^B1") { }

        ///<inheritdoc/>
        public override ZplElementBase Analyze(string zplCommand, VirtualPrinter virtualPrinter, IPrinterStorage printerStorage)
        {
            string[] zplDataParts = this.SplitCommand(zplCommand);

            int tmpint;
            bool checkDigit = false;
            int height = virtualPrinter.BarcodeInfo.Height;
            bool printInterpretationLine = true;
            bool printInterpretationLineAboveCode = false;

            FieldOrientation fieldOrientation = this.ConvertFieldOrientation(zplDataParts[0], virtualPrinter);
            if (zplDataParts.Length > 1)
            {
                checkDigit = this.ConvertBoolean(zplDataParts[1]);
            }

            if (zplDataParts.Length > 2 && int.TryParse(zplDataParts[2], out tmpint))
            {
                height = tmpint;
            }

            if (zplDataParts.Length > 3)
            {
                printInterpretationLine = this.ConvertBoolean(zplDataParts[3], "Y");
            }

            if (zplDataParts.Length > 4)
            {
                printInterpretationLineAboveCode = this.ConvertBoolean(zplDataParts[4]);
            }

            //The field data are processing in the FieldDataZplCommandAnalyzer
            virtualPrinter.SetNextElementFieldData(new Code11BarcodeFieldData
            {
                FieldOrientation = fieldOrientation,
                Height = height,
                PrintInterpretationLine = printInterpretationLine,
                PrintInterpretationLineAboveCode = printInterpretationLineAboveCode,
                CheckDigit = checkDigit
            });

            return null;
        }
    }
}
