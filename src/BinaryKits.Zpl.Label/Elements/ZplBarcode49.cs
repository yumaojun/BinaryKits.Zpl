using System.Collections.Generic;

namespace BinaryKits.Zpl.Label.Elements
{
    /// <summary>
    /// Code 49 Barcode
    /// </summary>
    /// <remarks>
    /// Code 49 is a two-dimensional, multi-row (2-8), continuous, variable-length symbology.
    /// Its module width comes from ^BY but it has a fixed print ratio, so only the module
    /// width is emitted.
    /// </remarks>
    public class ZplBarcode49 : ZplFieldDataElementBase
    {
        /// <summary>
        /// Height multiplier of the individual rows (1 to height of label).
        /// This value multiplied by the module equals the row height in dots.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Module width (in dots), emitted via ^BY.
        /// </summary>
        public int ModuleWidth { get; private set; }

        /// <summary>
        /// Starting mode: "A" (Automatic, default) or "0" through "5".
        /// </summary>
        public string Mode { get; private set; }

        /// <summary>
        /// Print interpretation line: "N" (none, default), "A" (above code) or "B" (below code).
        /// </summary>
        public string PrintInterpretationLine { get; private set; }

        /// <summary>
        /// Code 49 Barcode
        /// </summary>
        /// <param name="content"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="height">Height multiplier of the individual rows</param>
        /// <param name="moduleWidth">Module width in dots</param>
        /// <param name="mode">Starting mode ("A" automatic or "0"-"5")</param>
        /// <param name="printInterpretationLine">"N" none, "A" above, "B" below</param>
        /// <param name="fieldOrientation"></param>
        /// <param name="hexadecimalIndicator"></param>
        /// <param name="bottomToTop"></param>
        /// <param name="useDefaultPosition"></param>
        public ZplBarcode49(
            string content,
            int positionX,
            int positionY,
            int height = 20,
            int moduleWidth = 2,
            string mode = "A",
            string printInterpretationLine = "N",
            FieldOrientation fieldOrientation = FieldOrientation.Normal,
            char? hexadecimalIndicator = null,
            bool bottomToTop = false,
            bool useDefaultPosition = false)
            : base(content, positionX, positionY, fieldOrientation, hexadecimalIndicator, bottomToTop, useDefaultPosition)
        {
            Height = height;
            ModuleWidth = moduleWidth;
            Mode = mode;
            PrintInterpretationLine = printInterpretationLine;
        }

        ///<inheritdoc/>
        public override IEnumerable<string> Render(ZplRenderOptions context)
        {
            //^FO100,100
            //^BY2
            //^B4N,20,N,A
            //^FD12345ABCDE^FS
            var result = new List<string>();
            result.AddRange(RenderPosition(context));
            result.Add($"^BY{ModuleWidth}");
            result.Add($"^B4{RenderFieldOrientation()},{Height},{PrintInterpretationLine},{Mode}");
            result.Add(RenderFieldDataSection());

            return result;
        }
    }
}
