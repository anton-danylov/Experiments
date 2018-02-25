using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSamples
{
    /// <summary>
    /// GenomicRangeQuery problem solutions in C#
    /// https://app.codility.com/programmers/lessons/5-prefix_sums/genomic_range_query/
    /// </summary>
    public class GenomicRangeQuery
    {
        public class FirstSolutionWithStruct
        {
            struct NucleotideSum
            {
                public uint A { get; set; }
                public uint C { get; set; }
                public uint G { get; set; }
                public uint T { get; set; }

                public override string ToString() => $"{A}, {C}, {G}, {T}";

                public NucleotideSum Process(char nucleotide)
                {
                    NucleotideSum next = this;
                    switch (nucleotide)
                    {
                        case 'A': next.A++; break;
                        case 'C': next.C++; break;
                        case 'G': next.G++; break;
                        case 'T': next.T++; break;
                    }

                    return next;
                }

                public static int GetImpactFactor(char nucleotide)
                {
                    switch (nucleotide)
                    {
                        case 'A': return 1;
                        case 'C': return 2;
                        case 'G': return 3;
                        case 'T': return 4;
                    }

                    return 0;
                }

                public int GetMinimalImpactFactor(NucleotideSum previousSum)
                {
                    if (A - previousSum.A != 0)
                        return GetImpactFactor('A');
                    if (C - previousSum.C != 0)
                        return GetImpactFactor('C');
                    if (G - previousSum.G != 0)
                        return GetImpactFactor('G');
                    if (T - previousSum.T != 0)
                        return GetImpactFactor('T');

                    return 0;
                }
            }

            public int[] Solution(string S, int[] P, int[] Q)
            {
                NucleotideSum[] prefix = new NucleotideSum[S.Length + 1];

                prefix[1] = prefix[1].Process(S[0]);
                for (int i = 2; i < S.Length + 1; i++)
                {
                    prefix[i] = prefix[i - 1].Process(S[i - 1]);
                }

                int[] result = new int[P.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    int q = Q[i] + 1;
                    int p = P[i];
                    result[i] = prefix[q].GetMinimalImpactFactor(prefix[p]);
                }

                return result;
            }
        }

        /// <summary>
        /// This does not violate OCP as previous soulution: if new nucletoids are added it would be enough to provide new
        /// impact table
        /// </summary>
        public class BetterSolution
        {
            readonly Dictionary<char, int> _impactTable = new Dictionary<char, int>()
            {
                {'A', 1},
                {'C', 2},
                {'G', 3},
                {'T', 4},
            };

        
            public int[] Solution(string S, int[] P, int[] Q)
            {
                Dictionary<char, int[]> prefix = new Dictionary<char, int[]>();

                foreach (var key in _impactTable.Keys)
                {
                    prefix[key] = new int[S.Length];
                }

                prefix[S[0]][0] = 1;
                for (int i = 1; i < S.Length; i++)
                {
                    foreach (var kvp in prefix)
                    {
                        var array = kvp.Value;
                        array[i] = array[i - 1];
                    }

                    prefix[S[i]][i] += 1;
                }

                int[] result = new int[P.Length];
                for (int i = 0; i < P.Length; i++)
                {
                    int p = P[i] - 1;
                    int q = Q[i];

                    int minImpact = int.MaxValue;
                    foreach (var kvp in prefix)
                    {
                        var array = kvp.Value;
                        int currImpact = array[q] - ((p >= 0) ? array[p] : 0);

                        if (currImpact != 0)
                        {
                            currImpact = _impactTable[kvp.Key];
                            if (currImpact < minImpact)
                            {
                                minImpact = currImpact;
                            }
                        }
                    }
                    result[i] = minImpact;
                }

                return result;
            }
        }
    }
}
