using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Models;

namespace BinaryKits.Zpl.Viewer.CommandAnalyzers
{
    public class Code49BarcodeZplCommandAnalyzer : ZplCommandAnalyzerBase
    {
        public Code49BarcodeZplCommandAnalyzer() : base("^B4") { }

        ///<inheritdoc/>
        public override ZplElementBase Analyze(string zplCommand, VirtualPrinter virtualPrinter, IPrinterStorage printerStorage)
        {
            string[] zplDataParts = this.SplitCommand(zplCommand);

            int tmpint;
            int height = virtualPrinter.BarcodeInfo.Height;
            string mode = "A";
            string printInterpretationLine = "N";

            FieldOrientation fieldOrientation = this.ConvertFieldOrientation(zplDataParts[0], virtualPrinter);

            if (zplDataParts.Length > 1 && int.TryParse(zplDataParts[1], out tmpint))
            {
                height = tmpint;
            }

            if (zplDataParts.Length > 2 && !string.IsNullOrEmpty(zplDataParts[2]))
            {
                printInterpretationLine = zplDataParts[2].ToUpperInvariant();
            }

            if (zplDataParts.Length > 3 && !string.IsNullOrEmpty(zplDataParts[3]))
            {
                mode = zplDataParts[3].ToUpperInvariant();
            }

            //The field data are processed in the FieldDataZplCommandAnalyzer
            virtualPrinter.SetNextElementFieldData(new Code49BarcodeFieldData
            {
                FieldOrientation = fieldOrientation,
                Height = height,
                Mode = mode,
                PrintInterpretationLine = printInterpretationLine
            });

            return null;
        }
    }
}
