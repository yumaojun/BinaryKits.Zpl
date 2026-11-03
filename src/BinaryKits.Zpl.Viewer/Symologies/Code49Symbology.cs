using SkiaSharp;

using System;
using System.Collections.Generic;

namespace BinaryKits.Zpl.Viewer.Symologies
{
    /// <summary>
    /// Code 49 encoder.
    /// </summary>
    /// <remarks>
    /// Ported from the BWIPP reference implementation (https://github.com/bwipp/postscriptbarcode,
    /// MIT licensed). Produces a valid, scannable Code 49 symbol for ASCII input (0-127).
    /// </remarks>
    public static class Code49Symbology
    {
        // Codeword values (see BWIPP code49.charmap)
        private const int Shift1 = 43;   // code49.s1
        private const int Shift2 = 44;   // code49.s2
        private const int NumericShift = 48; // code49.ns

        // Number of rows -> data codeword capacity (BWIPP code49.metrics)
        private static readonly int[][] Metrics =
        {
            new[] { 2, 9 },
            new[] { 3, 16 },
            new[] { 4, 23 },
            new[] { 5, 30 },
            new[] { 6, 37 },
            new[] { 7, 42 },
            new[] { 8, 49 },
        };

        // Row parity for each of the first seven rows (BWIPP code49.parity)
        private static readonly string[] Parity =
        {
            "1001", "0101", "1100", "0011", "1010", "0110", "1111", "0000",
        };

        // Check character weights (BWIPP code49.weight*)
        private static readonly int[] BaseWeights =
        {
            1, 9, 31, 26, 2, 12, 17, 23, 37, 18, 22, 6, 27, 44, 15, 43, 39,
            11, 13, 5, 41, 33, 36, 8, 4, 32, 3, 19, 40, 25, 29, 10, 24, 30,
        };

        private static readonly int[] WeightX = BuildWeights(20, 0);
        private static readonly int[] WeightY = BuildWeights(16, 1);
        private static readonly int[] WeightZ = BuildWeights(38, 2);

        // ASCII code (0-127) -> either an int codeword or an int[] { shift, value } pair
        private static readonly object[] Charvals = BuildCharvals();

        private static int[] BuildWeights(int first, int offset)
        {
            int[] weights = new int[33];
            weights[0] = first;
            for (int i = 0; i < 32; i++)
            {
                weights[i + 1] = BaseWeights[offset + i];
            }

            return weights;
        }

        private static int BaseValue(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            if (c >= 'A' && c <= 'Z')
            {
                return c - 'A' + 10;
            }

            return c switch
            {
                '-' => 36,
                '.' => 37,
                ' ' => 38,
                '$' => 39,
                '/' => 40,
                '+' => 41,
                '%' => 42,
                _ => throw new ArgumentException($"Code 49 cannot encode character '{c}'."),
            };
        }

        private static object[] BuildCharvals()
        {
            object[] map = new object[128];
            for (int i = 0; i < 128; i++)
            {
                string combo = Code49SymbologyTables.Combos[i];
                char shift = combo[0];
                char value = combo[1];
                if (shift == '1')
                {
                    map[i] = new[] { Shift1, BaseValue(value) };
                }
                else if (shift == '2')
                {
                    map[i] = new[] { Shift2, BaseValue(value) };
                }
                else
                {
                    map[i] = BaseValue(value);
                }
            }

            return map;
        }

        /// <summary>
        /// Encodes the content as a Code 49 symbol.
        /// </summary>
        /// <param name="content">ASCII content to encode.</param>
        /// <param name="moduleWidth">Module (narrow bar) width in pixels.</param>
        /// <param name="rowHeight">Row height multiplier in modules.</param>
        public static SKBitmap Encode(string content, int moduleWidth, int rowHeight)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Code 49 content must not be empty.");
            }

            int[] msg = new int[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (c > 127)
                {
                    throw new ArgumentException("Code 49 only supports ASCII characters with values 0 to 127.");
                }

                msg[i] = c;
            }

            List<int> cws = EncodeCodewords(msg);

            int rows = 0;
            int dataCodewords = 0;
            foreach (int[] metric in Metrics)
            {
                if (cws.Count <= metric[1])
                {
                    rows = metric[0];
                    dataCodewords = metric[1];
                    break;
                }
            }

            if (rows == 0)
            {
                throw new ArgumentException("Code 49 content is too long.");
            }

            // Pad with the Numeric Shift value (BWIPP uses 48).
            while (cws.Count < dataCodewords)
            {
                cws.Add(NumericShift);
            }

            int[] ccs = BuildCodeCharacters(cws, rows, dataCodewords);
            bool[][] rowBits = BuildRowBits(ccs, rows);

            return RenderBitmap(rowBits, rows, moduleWidth, Math.Max(1, Math.Min(rowHeight, 50)));
        }

        private static List<int> EncodeCodewords(int[] msg)
        {
            List<int> cws = new List<int>();

            // Pre-compute numeric run lengths (right to left).
            int[] numericRuns = new int[msg.Length + 1];
            for (int i = msg.Length - 1; i >= 0; i--)
            {
                numericRuns[i] = msg[i] >= '0' && msg[i] <= '9' ? numericRuns[i + 1] + 1 : 0;
            }

            int mode;
            int index = 0;
            bool numeric = false;

            if (numericRuns[0] >= 5)
            {
                mode = 2;
                numeric = true;
            }
            else
            {
                object first = Charvals[msg[0]];
                if (first is int)
                {
                    mode = 0;
                }
                else
                {
                    int[] pair = (int[])first;
                    mode = pair[0] == Shift1 ? 4 : 5;
                    cws.Add(pair[1]);
                    index = 1;
                }
            }

            while (index < msg.Length)
            {
                if (!numeric)
                {
                    if (numericRuns[index] >= 5)
                    {
                        cws.Add(NumericShift);
                        numeric = true;
                    }
                    else
                    {
                        AppendCharval(msg[index], cws);
                        index++;
                    }
                }
                else
                {
                    if (numericRuns[index] < 5)
                    {
                        cws.Add(NumericShift);
                        numeric = false;
                    }
                    else
                    {
                        int run = numericRuns[index];
                        EncodeNumeric(msg, index, run, cws);
                        index += run;
                    }
                }
            }

            return cws;
        }

        private static void AppendCharval(int ascii, List<int> cws)
        {
            object value = Charvals[ascii];
            if (value is int single)
            {
                cws.Add(single);
            }
            else
            {
                int[] pair = (int[])value;
                cws.Add(pair[0]);
                cws.Add(pair[1]);
            }
        }

        private static void EncodeNumeric(int[] msg, int start, int length, List<int> cws)
        {
            string digits = new string(Array.ConvertAll(msg, c => (char)c), start, length);

            int remainder = length % 5;
            int groups = remainder != 2 ? length - remainder : length - remainder - 5;

            for (int i = 0; i < groups; i += 5)
            {
                Base48(digits.Substring(i, 5), 3, cws);
            }

            string tail = digits.Substring(groups);
            switch (tail.Length)
            {
                case 1:
                    cws.Add(BaseValue(tail[0]));
                    break;
                case 3:
                    Base48(tail, 2, cws);
                    break;
                case 4:
                    Base48("10" + tail, 3, cws);
                    break;
                case 7:
                    Base48("10" + tail.Substring(0, 4), 3, cws);
                    Base48(tail.Substring(4, 3), 2, cws);
                    break;
            }
        }

        private static void Base48(string decimalDigits, int count, List<int> cws)
        {
            int value = 0;
            foreach (char c in decimalDigits)
            {
                value = value * 10 + (c - '0');
            }

            int[] digits = new int[count];
            for (int i = count - 1; i >= 0; i--)
            {
                digits[i] = value % 48;
                value /= 48;
            }

            cws.AddRange(digits);
        }

        private static int[] BuildCodeCharacters(List<int> cws, int rows, int dataCodewords)
        {
            int[] ccs = new int[rows * 8];

            int source = 0;
            for (int row = 0; row <= rows - 2; row++)
            {
                int sum = 0;
                for (int k = 0; k < 7; k++)
                {
                    int codeword = cws[source + k];
                    ccs[row * 8 + k] = codeword;
                    sum += codeword;
                }

                ccs[row * 8 + 7] = sum % 49;
                source += 7;
            }

            if (source < dataCodewords)
            {
                for (int k = source; k < dataCodewords; k++)
                {
                    ccs[(rows - 1) * 8 + (k - source)] = cws[k];
                }
            }

            // Row count / mode character (BWIPP cr7). We always encode in the
            // automatically selected mode; its value is baked into this symbol.
            int mode = 0;
            int cr7 = (rows - 2) * 7 + mode;
            ccs[rows * 8 - 2] = cr7;

            int lastRow = (rows - 1) * 8;

            int wr1;
            if (rows >= 7)
            {
                wr1 = (cr7 * WeightZ[0] + CalcCheck(ccs, rows, WeightZ)) % 2401;
                ccs[lastRow] = wr1 / 49;
                ccs[lastRow + 1] = wr1 % 49;
            }
            else
            {
                wr1 = ccs[lastRow] * 49 + ccs[lastRow + 1];
            }

            int wr2 = (cr7 * WeightY[0] + CalcCheck(ccs, rows, WeightY) + wr1 * WeightY[4 * rows - 3]) % 2401;
            ccs[lastRow + 2] = wr2 / 49;
            ccs[lastRow + 3] = wr2 % 49;

            int wr3 = (cr7 * WeightX[0] + CalcCheck(ccs, rows, WeightX) + wr1 * WeightX[4 * rows - 3] + wr2 * WeightX[4 * rows - 2]) % 2401;
            ccs[lastRow + 4] = wr3 / 49;
            ccs[lastRow + 5] = wr3 % 49;

            int lastRowCheck = 0;
            for (int k = 0; k < 7; k++)
            {
                lastRowCheck += ccs[lastRow + k];
            }

            ccs[lastRow + 7] = lastRowCheck % 49;

            return ccs;
        }

        private static int CalcCheck(int[] ccs, int rows, int[] weights)
        {
            int score = 0;
            for (int i = 0; i <= 4 * rows - 5; i++)
            {
                int value = ccs[2 * i] * 49 + ccs[2 * i + 1];
                score += value * weights[i + 1];
            }

            return score;
        }

        private static bool[][] BuildRowBits(int[] ccs, int rows)
        {
            bool[][] rowBits = new bool[rows][];

            for (int i = 0; i < rows; i++)
            {
                string parity = i != rows - 1 ? Parity[i] : "0000";
                List<int> widths = new List<int> { 10, 1, 1 };

                for (int j = 0; j < 4; j++)
                {
                    int value = ccs[i * 8 + 2 * j] * 49 + ccs[i * 8 + 2 * j + 1];
                    string pattern = parity[j] == '0'
                        ? Code49SymbologyTables.PatternsEven[value]
                        : Code49SymbologyTables.PatternsOdd[value];
                    foreach (char c in pattern)
                    {
                        widths.Add(c - '0');
                    }
                }

                widths.Add(4);
                widths.Add(1);

                bool[] bits = new bool[81];
                int bitIndex = 0;
                for (int w = 0; w < widths.Count; w++)
                {
                    bool color = w % 2 == 1; // even elements are spaces, odd are bars
                    for (int k = 0; k < widths[w]; k++)
                    {
                        bits[bitIndex++] = color;
                    }
                }

                rowBits[i] = bits;
            }

            return rowBits;
        }

        private static SKBitmap RenderBitmap(bool[][] rowBits, int rows, int moduleWidth, int rowHeight)
        {
            const int width = 81;
            int symbolHeight = (rows + 1) + rows * rowHeight;

            using SKBitmap modules = new(width, symbolHeight);
            int y = 0;

            void DrawBand(bool[] bits, int height)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        modules.SetPixel(x, y + row, bits[x] ? SKColors.Black : SKColors.White);
                    }
                }

                y += height;
            }

            bool[] solidBar = new bool[width];
            bool[] separator = new bool[width];
            for (int x = 0; x < width; x++)
            {
                solidBar[x] = true;
                separator[x] = x >= 10 && x < 80; // 10 white + 70 black + 1 white
            }

            DrawBand(solidBar, 1);
            for (int i = 0; i <= rows - 2; i++)
            {
                DrawBand(rowBits[i], rowHeight);
                DrawBand(separator, 1);
            }

            DrawBand(rowBits[rows - 1], rowHeight);
            DrawBand(solidBar, 1);

            if (moduleWidth <= 1)
            {
                return modules.Copy();
            }

            SKSamplingOptions sampling = new(SKFilterMode.Nearest);
            return modules.Resize(new SKSizeI(width * moduleWidth, symbolHeight * moduleWidth), sampling);
        }
    }
}
