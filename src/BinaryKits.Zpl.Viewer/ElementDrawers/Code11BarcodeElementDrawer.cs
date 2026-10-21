using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Helpers;

using SkiaSharp;

using System;

using Barcode = BarcodeStandard.Barcode;
using BarcodeType = BarcodeStandard.Type;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    /// <summary>
    /// Drawer for Code 11 Barcode elements
    /// </summary>
    public class Code11BarcodeElementDrawer : BarcodeDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplBarcodeCode11;
        }

        ///<inheritdoc/>
        public override SKPoint Draw(ZplElementBase element, DrawerOptions options, SKPoint currentPosition, InternationalFont internationalFont, int printDensityDpmm)
        {
            if (element is ZplBarcodeCode11 barcode)
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

                // Note: BarcodeLib always appends the Code 11 check digit(s) (C, and K
                // when the data length is >= 10), so the ZPL ^B1 "check digit" flag cannot
                // be honored here. The wide-to-narrow ratio is fixed at 2:1 by BarcodeLib.
                Barcode generator = new Barcode
                {
                    BarWidth = barcode.ModuleWidth,
                    Height = barcode.Height,
                    IncludeLabel = false
                };

                using SKImage image = generator.Encode(BarcodeType.Code11, content, SKColors.Black, SKColors.White);
                byte[] png = image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
                this.DrawBarcode(png, x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation);

                if (barcode.PrintInterpretationLine)
                {
                    float labelFontSize = FontScale.GetBitmappedFontSize("A", Math.Min(barcode.ModuleWidth, 10), printDensityDpmm).Value;
                    SKTypeface labelTypeFace = options.FontManager.FontLoader("A");
                    SKFont labelFont = new(labelTypeFace, labelFontSize);
                    this.DrawInterpretationLine(content, labelFont, x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation, barcode.PrintInterpretationLineAboveCode, options);
                }

                return this.CalculateNextDefaultPosition(x, y, image.Width, image.Height, barcode.FieldOrigin != null, barcode.FieldOrientation, currentPosition);
            }

            return currentPosition;
        }
    }
}
