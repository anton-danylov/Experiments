using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSamples
{
    /// <summary>
    /// NumberOfDiscIntersections problem on Codility
    /// https://app.codility.com/programmers/lessons/6-sorting/number_of_disc_intersections/
    /// </summary>
    public class DiscIntersectionCounter
    {
        struct Range
        {
            public long Start { get; set; }
            public long Length { get; set; }
        }

        public int SolutionN2(int[] A)
        {
            int intersectionsCount = 0;
            for (int i = 0; i < A.Length; i++)
            {
                long si = i - (long)A[i], ei = i + (long)A[i];
                for (int j = i + 1; j < A.Length; j++)
                {
                    long sj = j - (long)A[j], ej = j + (long)A[j];

                    bool isIntersect = ((si - sj) * (ei - sj) <= 0) || ((si - ej) * (ei - ej) <= 0)
                        || ((sj - si) * (ej - si) <= 0) || ((sj - ei) * (ej - ei) <= 0);

                    if (isIntersect)
                    {
                        intersectionsCount++;

                        if (intersectionsCount > 10000000)
                        {
                            return -1;
                        }
                    }
                }
            }

            return intersectionsCount;
        }

        public int Solution(int[] A)
        {
            List<Range> listRanges = new List<Range>();

            for (long i = 0; i < A.Length; i++)
            {
                Range range = new Range() { Start = i - A[i], Length = 2L * A[i] };
                listRanges.Add(range);
            }

            // Sort ranges by starting point
            listRanges.Sort((x, y) => (int)(x.Start - y.Start));

            int intersectionsCountFast = 0;
            for (int i = 0; i < listRanges.Count; i++)
            {
                Range range = listRanges[i];
                long maxReach = range.Start + range.Length;

                // Binary search the last range that falls into a current range reach
                int searchStart = i, searchEnd = listRanges.Count - 1;
                while (searchEnd - searchStart > 1)
                {
                    int middle = (searchEnd + searchStart) / 2;
                    if (listRanges[middle].Start <= maxReach)
                    {
                        searchStart = middle;
                    }
                    else
                    {
                        searchEnd = middle;
                    }
                }
                int currentLimit = (listRanges[searchEnd].Start <= maxReach) ? searchEnd : searchStart;

                intersectionsCountFast += currentLimit - i;

                if (intersectionsCountFast > 10000000)
                {
                    return -1;
                }
            }

            return intersectionsCountFast;
        }

    }
}

#region Range
//struct Range
//{
//    public long Start { get; set; }
//    public long End { get; set; }

//    public long Center { get; set; }

//    public bool IsIntersect(Range other) 
//        => ((Start - other.Start) * (End - other.Start) <= 0) 
//        || ((Start - other.End) * (End - other.End) <= 0)
//        || ((other.Start - Start) * (other.End - Start) <= 0)
//        || ((other.Start - End) * (other.End - End) <= 0);


//    public override string ToString()
//    {
//        return $"[{Start}, {End}]";
//    }
//}

#endregion
