using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Project.Scripts.Helpers.ShuffleHelper
{
    /// <summary>
    /// Simple class for shuffling objects in a list
    /// </summary>
    public static class Shuffle
    {
        public static void FisherYatesShuffle<T>(this IList<T> list)
        {
            Random rng = new();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        public static void FisherYatesShuffle<T>(this Queue<T> queue)
        {
            var list = queue.ToList();

            list.FisherYatesShuffle();

            queue.Clear();
            
            foreach (T item in list)
            {
                queue.Enqueue(item);
            }
        }        
        
        public static void FisherYatesShuffle<T>(this T[] array)
        {
            var list = array.ToList();

            list.FisherYatesShuffle();

            for (int index = 0; index < list.Count; index++)
            {
                array[index] = list[index];
            }
        }
    }
}