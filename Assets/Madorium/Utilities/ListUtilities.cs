using System;
using System.Collections.Generic;

static public class ListUtilities
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T PickRandom<T>(this IList<T> list)
    {
        int index = 0;

        if(list.Count > 1)
            index = rng.Next(0, list.Count);

        return list[index];
    }

    public static int PickRandomIndex<T>(this IList<T> list)
    {
        int index = 0;

        if (list.Count > 1)
            index = rng.Next(0, list.Count);

        return index;
    }
}