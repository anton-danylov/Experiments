using System;
using System.Collections.Generic;

namespace StringComparer
{
    public class SimpleStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var partsX = x.Split('.');
            var partsY = y.Split('.');

            bool isIncorrectFormatX = partsX == null || partsX.Length != 2;
            bool isIncorrectFormatY = partsY == null || partsY.Length != 2;

            if (isIncorrectFormatX && isIncorrectFormatY) // don't copmpare strings of incorrect format
                return 0;
            else if (isIncorrectFormatX)
                return -1;
            else if (isIncorrectFormatY)
                return 1;


            isIncorrectFormatX = !int.TryParse(partsX[0].Trim(), out int numberX);
            isIncorrectFormatY = !int.TryParse(partsY[0].Trim(), out int numberY);

            if (isIncorrectFormatX && isIncorrectFormatY)
                return 0;
            else if (isIncorrectFormatX)
                return -1;
            else if (isIncorrectFormatY)
                return 1;


            int stringCompareResult = String.Compare(partsX[1], partsY[1], StringComparison.Ordinal); // Ordinal compare treats 'Z' < 'a, just according to the index in symbol table

            return stringCompareResult != 0 ? stringCompareResult : numberX.CompareTo(numberY);
        }
    }
}
