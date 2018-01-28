using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StringComparer
{
    public class UnsafeStringComparer2 : IComparer<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetNumber(char* pStart, char* pEnd)
        {
            int num = 0;
            int sign = 1;

            if (*pStart == '-')
            {
                sign = -1;
                pStart++;
            }

            while (pStart < pEnd)
            {
                num = num * 10 + *pStart - '0';
                pStart++;
            }

            return num * sign;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe char* GetStringStart(char* p)
        {
            while (*p++ != '.') ;

            return p;
        }

        public int Compare(string x, string y)
        {
            int lenX = x.Length;
            int lenY = y.Length;

            unsafe
            {
                fixed (char* pX = x)
                fixed (char* pY = y)
                {
                    char* pStrX = GetStringStart(pX);
                    char* pStrY = GetStringStart(pY);

                    char* pDotX = pStrX - 1;
                    char* pDotY = pStrY - 1;


                    char* pEndX = pX + lenX;
                    char* pEndY = pY + lenY;


                    while (pStrX < pEndX && pStrY < pEndY)
                    {
                        if (*pStrX != *pStrY)
                            return *pStrX - *pStrY;

                        pStrX++;
                        pStrY++;
                    }

                    bool isReachedEndX = pStrX >= pEndX;
                    bool isReachedEndY = pStrY >= pEndY;

                    if (!isReachedEndX && isReachedEndY)
                        return +1;
                    else if (!isReachedEndY && isReachedEndX)
                        return -1;

                    return GetNumber(pX, pDotX) - GetNumber(pY, pDotY);
                }
            }
        }
    }
}
