using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all eases that FTC uses, which includes a few not present in the 'Easing' class.
/// </summary>
static class Functions
{
    /// <summary>
    /// Returns an array of random unique numbers with a few parameters.
    /// </summary>
    /// <param name="length">The length of the array.</param>
    /// <param name="min">The included minimum value.</param>
    /// <param name="max">The excluded maximum value.</param>
    /// <returns>A random integer array.</returns>
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

    /// <summary>
    /// Creates and returns a subarray of the given array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="data">The array itself to which to create a subarray of.</param>
    /// <param name="index">The inclusive starting index.</param>
    /// <param name="length">The length of the copy.</param>
    /// <returns></returns>
    internal static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    public static double Modulo(double num, int mod)
    {
        if (num < 0)
        {
            num += mod;
            num = Modulo(num, mod);
        }

        else if (num >= mod)
        {
            num -= mod;
            num = Modulo(num, mod);
        }

        return num;
    }

    public static int GetColorIndex(int i, FTCScript FTC)
    {
        return Array.IndexOf(FTC.ColorTextures, FTC.ColoredObjects[i].material.mainTexture);
    }

    public static float ElasticIn(float k)
    {
        return Modulo(k, 1) == 0 ? k : -Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
    }

    public static float ElasticOut(float k)
    {
        return Modulo(k, 1) == 0 ? k : Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
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
