using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class Calculate 
{
    public Calculate(FTCScript FTC)
    {
        this.FTC = FTC;
        sequence = new List<bool?>();
        legacySequence = new List<int>();
    }

    internal List<bool?> sequence;
    internal List<int> legacySequence;

    private readonly FTCScript FTC;

    internal void Current()
    {
        int trigNumber = int.Parse(string.Concat(FTC.DisplayTexts[1].text, (int.Parse(FTC.GearText.text) + Edgework(Functions.GetColorIndex(3, FTC))) % 10));

        int nixie1 = int.Parse(FTC.NixieTexts[0].text), nixie2 = int.Parse(FTC.NixieTexts[1].text);

        bool parity = nixie1 % 2 == nixie1 % 2;
        int trigResult = parity ? (int)(Math.Abs(Math.Sin(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000)
                                : (int)(Math.Abs(Math.Cos(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000);

        int[] decimals = Array.ConvertAll(trigResult.ToString().ToCharArray(), c => (int)char.GetNumericValue(c));
        List<string> figure = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            int[][] cylinders = Figure.Create(decimals, ref i);
            int[] sums = Figure.Apply(cylinders, FTC);
            figure.Add(string.Concat(sums[0], sums[1], sums[2]));
        }

        FTC.DisplayTexts[0].text = figure.GroupBy(x => x).Where(g => g.Count() == 1).Select(y => y.Key).PickRandom();
        sequence.Add(new bool?[] { false, null, true }[figure.IndexOf(FTC.DisplayTexts[0].text) % 3]);
    }

    internal void LegacyCurrent()
    {

    }

    internal void LegacyFinal()
    {

    }

    private int Edgework(int index)
    {
        switch (index)
        {
            case 0: return FTC.Info.GetBatteryCount();
            case 1: return FTC.Info.GetIndicators().Count();
            case 2: return FTC.Info.GetPortPlateCount();
            case 3: return FTC.Info.GetSerialNumberNumbers().First();
            case 4: return FTC.Info.GetBatteryCount();
            case 5: return FTC.Info.GetOffIndicators().Count();
            case 6: return FTC.Info.GetPorts().Count();
            case 7: return FTC.Info.GetSerialNumberLetters().Count();

            default: throw new IndexOutOfRangeException("Calculate.Edgework recieved an out-of-range number: " + index + ".");
        }
    }
}
