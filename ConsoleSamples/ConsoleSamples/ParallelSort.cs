using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSamples
{
    public static class Sort
    {
        public static int Threshold = 150; // list length to use InsertionSort instead of SequentialQuickSort

        public static void InsertionSort<T>(IList<T> list, IComparer<T> comparer, int from, int to)
        {
            for (int i = from + 1; i < to; i++)
            {
                var a = list[i];
                int j = i - 1;

                while (j >= from && comparer.Compare(list[j], a) == -1) 
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = a;
            }
        }

        static void Swap<T>(IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        static int Partition<T>(IList<T> list, IComparer<T> comparer, int from, int to, int pivot)
        {
            // Pre: from <= pivot < to (other than that, pivot is arbitrary)
            var listPivot = list[pivot];  // pivot value
            Swap(list, pivot, to - 1); // move pivot value to end for now, after this pivot not used
            var newPivot = from; // new pivot 
            for (int i = from; i < to - 1; i++) // be careful to leave pivot value at the end
            {
                // Invariant: from <= newpivot <= i < to - 1 && 
                // forall from <= j <= newpivot, list[j] <= listPivot && forall newpivot < j <= i, list[j] > listPivot
                //if (list[i] <= listPivot)
                if (comparer.Compare(list[i], listPivot) != -1)
                {
                    Swap(list, newPivot, i);  // move value smaller than listPivot down to newpivot
                    newPivot++;
                }
            }
            Swap(list, newPivot, to - 1); // move pivot value to its final place
            return newPivot; // new pivot
                             // Post: forall i <= newpivot, list[i] <= list[newpivot]  && forall i > ...
        }

        public static void SequentialQuickSort<T>(IList<T> list, IComparer<T> comparer)
        {
            SequentialQuickSort(list, comparer, 0, list.Count);
        }

        static void SequentialQuickSort<T>(IList<T> list, IComparer<T> comparer, int from, int to)
        {
            if (to - from <= Threshold)
            {
                InsertionSort<T>(list, comparer, from, to);
            }
            else
            {
                int pivot = from + (to - from) / 2; // could be anything, use middle
                pivot = Partition<T>(list, comparer, from, to, pivot);
                // Assert: forall i < pivot, list[i] <= list[pivot]  && forall i > ...
                SequentialQuickSort(list, comparer, from, pivot);
                SequentialQuickSort(list, comparer, pivot + 1, to);
            }
        }

        public static void ParallelQuickSort<T>(IList<T> list, IComparer<T> comparer)
        {
            ParallelQuickSort(list, comparer, 0, list.Count,
                 (int)Math.Log(Environment.ProcessorCount, 2) + 4);
        }

        static void ParallelQuickSort<T>(IList<T> list, IComparer<T> comparer, int from, int to, int depthRemaining) 
        {
            if (to - from <= Threshold)
            {
                InsertionSort<T>(list, comparer, from, to);
            }
            else
            {
                int pivot = from + (to - from) / 2; // could be anything, use middle
                pivot = Partition<T>(list, comparer, from, to, pivot);
                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelQuickSort(list, comparer, from, pivot, depthRemaining - 1),
                        () => ParallelQuickSort(list, comparer, pivot + 1, to, depthRemaining - 1));
                }
                else
                {
                    ParallelQuickSort(list, comparer, from, pivot, 0);
                    ParallelQuickSort(list, comparer, pivot + 1, to, 0);
                }
            }
        }
    }

}
