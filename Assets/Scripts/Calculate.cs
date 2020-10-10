using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Calculate 
{
    public Calculate(FTCScript FTC)
    {
        this.FTC = FTC;

        sequence = new List<bool?>();
        modifiedSequence = new List<bool>();
        legacySequence = new List<int>();
    }

    internal List<bool?> sequence;
    internal List<bool> modifiedSequence;
    internal List<int> legacySequence;

    private readonly FTCScript FTC;

    internal void Current()
    {
        int trigNumber = int.Parse(string.Concat((int.Parse(FTC.GearText.text.Last().ToString()) + Edgework(Functions.GetColorIndex(3, FTC))) % 10, FTC.DisplayTexts[1].text)),
            nixieL = int.Parse(FTC.NixieTexts[0].text), nixieR = int.Parse(FTC.NixieTexts[1].text);
        bool parity = nixieL % 2 == nixieR % 2;
        int trigResult = parity ? (int)(Math.Abs(Math.Sin(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000)
                                : (int)(Math.Abs(Math.Cos(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000);

        int[] decimals = new int[5], temp = Array.ConvertAll(trigResult.ToString().ToCharArray(), c => (int)char.GetNumericValue(c));
        Array.Copy(temp, decimals, temp.Length);
        var figure = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            int[][] cylinders = Figure.Create(decimals, ref i);
            int[] sums = Figure.Apply(cylinders, FTC);
            figure.Add(string.Concat(sums[0], sums[1], sums[2]));
        }

        FTC.DisplayTexts[0].text = figure.GroupBy(x => x).Where(g => g.Count() == 1).Select(y => y.Key).PickRandom();

        bool? input = new bool?[] { false, null, true }[figure.IndexOf(FTC.DisplayTexts[0].text) % 3];
        sequence.Add(input);

        bool last = modifiedSequence.Count > 1 ? modifiedSequence[modifiedSequence.Count - 1] : false,
             modifiedInput = input == null ? last : (bool)input;

        if (nixieL == 0 || nixieR == 0)
            modifiedInput = !modifiedInput;

        modifiedSequence.Add(modifiedInput);
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
