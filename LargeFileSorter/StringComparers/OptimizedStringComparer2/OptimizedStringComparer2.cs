using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace StringComparer
{
    public class OptimizedStringComparer2 : IComparer<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsDigit(char c) => c >= '0' && c <= '9';

        public int Compare(string x, string y)
        {
            int lenX = x.Length;
            int lenY = y.Length;

            int iX = 0;
            int iY = 0;

            int signX = +1, signY = +1;
            int dotPosX = -1, dotPosY = -1;
            int numX = 0, numY = 0;

            while (iX < lenX && iY < lenY)
            {
                char cx = x[iX];
                char cy = y[iY];

                if (dotPosX > 0 && dotPosY > 0)
                {
                    if (cx < cy)
                        return -1;
                    else if (cx > cy)
                        return +1;

                    iX++;
                    iY++;
                }
                else
                {
                    if (dotPosX < 0)
                    {
                        if (iX == 0 && cx == '-')
                        {
                            signX = -1;
                        }
                        else if (IsDigit(cx))
                        {
                            numX = numX * 10 + (cx - '0');
                        }
                        else if (cx == '.')
                        {
                            dotPosX = iX;
                        }

                        iX++;
                    }

                    if (dotPosY < 0)
                    {
                        if (iY == 0 && cy == '-')
                        {
                            signY = -1;
                        }
                        else if (IsDigit(cy))
                        {
                            numY = numY * 10 + (cy - '0');
                        }
                        else if (cy == '.')
                        {
                            dotPosY = iY;
                        }

                        iY++;
                    }
                }
            }

            bool isNotEndedX = iX < lenX;
            bool isNotEndedY = iY < lenY;

            if (isNotEndedX && !isNotEndedY)
                return +1;
            else if (!isNotEndedX && isNotEndedY)
                return -1;

            numX *= signX;
            numY *= signY;


            if (numX < numY)
                return -1;
            else if (numX > numY)
                return +1;


            return 0;
        }
    }
}
