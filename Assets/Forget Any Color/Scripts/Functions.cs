using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all eases that FAC uses, which includes a few not present in the 'Easing' class.
/// </summary>
static class Functions
{
    internal static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    internal static int[] Random(int length, int min, int max)
    {
        // Create a range from min to max, and initalize an array with the specified size.
        int[] range = Enumerable.Range(min, --max).ToArray().Shuffle();

        // Failsafe: Should never happen, and will return unnatural values otherwise.
        if (range.Length < length)
            throw new ArgumentOutOfRangeException("range: " + range.Join(", "), "The length of the returned array (" + length + ") is larger than the range specified (" + range.Length + ")!");

        // Instance can be pulled linearly since the range has been shuffled anyway.
        return SubArray(range, 0, length);
    }

    public static bool TPCommandValidity(string command)
    {
        return command.All(c => "LRlr".Contains(c));
    }

    public static int GetColorIndex(int i, FACScript FAC)
    {
        return Array.IndexOf(FAC.ColorTextures, FAC.ColoredObjects[i].material.mainTexture);
    }

    public static float ElasticIn(float k)
    {
        return k % 1 == 0 ? k : -Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
    }

    public static float ElasticOut(float k)
    {
        return k % 1 == 0 ? k : Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
    }

    public static float BackIn(float k)
    {
        return k * k * ((1.70158f + 1f) * k - 1.70158f);
    }

    public static float BackOut(float k)
    {
        return (k -= 1f) * k * ((1.70158f + 1f) * k + 1.70158f) + 1f;
    }

    public static float CubicOut(float k)
    {
        return 1f + ((k -= 1f) * k * k);
    }
}
