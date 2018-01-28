using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StringComparer
{
    public class OptimizedStringComparer : IComparer<string>
    {
        private bool ProcessString(string str, out int number, out int stringStartPosition)
        {
            int num = 0;
            int sign = +1;
            int dotPos = -1;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (i == 0 && c == '-')
                {
                    sign = -1;
                    continue;
                }

                if (Char.IsDigit(c))
                {
                    int digit = c - '0';
                    num = num * 10 + digit;
                }
                else if (c == '.')
                {
                    dotPos = i;
                    break;
                }
            }


            number = num * sign;
            stringStartPosition = dotPos;

            return true;
        }

        public int Compare(string x, string y)
        {
            ProcessString(x, out int numX, out int startPosX);
            ProcessString(y, out int numY, out int startPosY);

            bool isNotEndedX = true, isNotEndedY = true;
            while (isNotEndedX && isNotEndedY)
            {
                char csx = x[startPosX];
                char csy = y[startPosY];

                if (csx < csy)
                    return -1;
                else if (csx > csy)
                    return +1;

                isNotEndedX = ++startPosX < x.Length;
                isNotEndedY = ++startPosY < y.Length;
            }

            if (isNotEndedX && !isNotEndedY)
                return +1;
            else if (!isNotEndedX && isNotEndedY)
                return -1;

            if (numX < numY)
                return -1;
            else if (numX > numY)
                return +1;


            return 0;
        }
    }
}
