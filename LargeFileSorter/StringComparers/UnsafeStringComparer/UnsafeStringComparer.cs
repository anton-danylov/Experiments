using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StringComparer
{
    public class UnsafeStringComparer : IComparer<string>
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

            unsafe
            {
                fixed (char* pX = x)
                fixed (char* pY = y)
                {
                    if (*pX == '-')
                    {
                        signX = -1;
                        iX++;
                    }

                    if (*pY == '-')
                    {
                        signY = -1;
                        iY++;
                    }


                    while (iX < lenX && iY < lenY)
                    {
                        char cx = *(pX + iX);
                        char cy = *(pY + iY);

                        if (dotPosX > 0 && dotPosY > 0)
                        {
                            if (cx != cy)
                                return cx - cy;

                            iX++;
                            iY++;
                        }
                        else
                        {
                            if (dotPosX < 0)
                            {
                                if (IsDigit(cx))
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

                                if (IsDigit(cy))
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
                }
                bool isReachedEndX = iX >= lenX;
                bool isReachedEndY = iY >= lenY;

                if (!isReachedEndX && isReachedEndY)
                    return +1;
                else if (!isReachedEndY && isReachedEndX)
                    return -1;

                numX *= signX;
                numY *= signY;

                return numX - numY;
            }
        }
    }
}
