using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Helpers;
using BinaryKits.Zpl.Viewer.Symologies;

using SkiaSharp;

using System;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    /// <summary>
    /// Drawer for Code 49 barcode elements.
    /// </summary>
    public class Barcode49ElementDrawer : BarcodeDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplBarcode49;
        }

        ///<inheritdoc/>
        public override SKPoint Draw(ZplElementBase element, DrawerOptions options, SKPoint currentPosition, InternationalFont internationalFont, int printDensityDpmm)
        {
            if (element is ZplBarcode49 barcode)
            {
                float x = barcode.PositionX;
                float y = barcode.PositionY;

                if (barcode.UseDefaultPosition)
                {
                    x = currentPosition.X;
                    y = currentPosition.Y;
                }

                string content = barcode.Content;
                if (barcode.HexadecimalIndicator is char hexIndicator)
                {
                    content = content.ReplaceHexEscapes(hexIndicator, internationalFont);
                }

                using SKBitmap image = Code49Symbology.Encode(content, barcode.ModuleWidth, barcode.Height);
                byte[] png = image.Encode(SKEncodedImageFormat.Png, 100).ToArray();

                this.DrawBarcode(png, x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation);

                bool printInterpretationLine = barcode.PrintInterpretationLine != "N";
                if (printInterpretationLine)
                {
                    float labelFontSize = FontScale.GetBitmappedFontSize("A", Math.Min(barcode.ModuleWidth, 10), printDensityDpmm).Value;
                    SKTypeface labelTypeFace = options.FontManager.FontLoader("A");
                    SKFont labelFont = new(labelTypeFace, labelFontSize);
                    bool aboveCode = barcode.PrintInterpretationLine == "A";
                    this.DrawInterpretationLine(content, labelFont, x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation, aboveCode, options);
                }

                return this.CalculateNextDefaultPosition(x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation, currentPosition);
            }

            return currentPosition;
        }
    }
}
