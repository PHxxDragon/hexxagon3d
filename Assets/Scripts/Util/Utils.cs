using System;
using System.Collections.Generic;

public static class Utils
{
    public static Random random = new Random();
    public static void Shuffle<T>(List<T> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = random.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}