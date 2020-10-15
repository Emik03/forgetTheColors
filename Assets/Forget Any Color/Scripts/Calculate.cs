using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgetAnyColor
{
    public class Calculate
    {
        public Calculate(FACScript FAC, Init init)
        {
            this.FAC = FAC;
            this.init = init;

            figureSequences = new List<int>();
            sequences = new List<bool?>();
            modifiedSequences = new List<bool>();
        }

        internal List<int> figureSequences;
        internal List<bool?> sequences;
        internal List<bool> modifiedSequences;

        private readonly FACScript FAC;
        private readonly Init init;

        internal void Current()
        {
            int trigNumber = int.Parse(string.Concat((int.Parse(FAC.GearText.text.Last().ToString()) + (Init.rules.GetLength(0) == 4 ? Arrays.GetEdgework(Init.rules[3][Functions.GetColorIndex(3, FAC)].Number, FAC) : Edgework(Functions.GetColorIndex(3, FAC)))) % 10, FAC.DisplayTexts[1].text)),
                nixieL = int.Parse(FAC.NixieTexts[0].text), nixieR = int.Parse(FAC.NixieTexts[1].text);
            
            bool parity = nixieL % 2 == nixieR % 2;
            int trigResult = parity ? (int)(Math.Abs(Math.Sin(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000)
                                    : (int)(Math.Abs(Math.Cos(trigNumber * Mathf.Deg2Rad)) * 100000 % 100000);

            int[] decimals = new int[5], temp = Array.ConvertAll(trigResult.ToString().ToCharArray(), c => (int)char.GetNumericValue(c));
            Array.Copy(temp, decimals, temp.Length);
            var figure = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                int[][] cylinders = Figure.Create(decimals, ref i);
                int[] sums = Figure.Apply(cylinders, FAC);
                figure.Add(string.Concat(sums[0], sums[1], sums[2]));
            }

            FAC.DisplayTexts[0].text = figure.GroupBy(x => x).Where(g => g.Count() == 1).Select(y => y.Key).PickRandom();

            int figureUsed = figure.IndexOf(FAC.DisplayTexts[0].text);
            figureSequences.Add(figureUsed);

            bool? input = new bool?[] { false, null, true }[figureUsed % 3];
            sequences.Add(input);

            bool last = modifiedSequences.Count > 1 && modifiedSequences[modifiedSequences.Count - 1],
                 modifiedInput = input == null ? last : (bool)input;

            if (nixieL == 0 || nixieR == 0)
                modifiedInput = !modifiedInput;

            modifiedSequences.Add(modifiedInput);
            Debug.LogFormat("[Forget Any Color #{0}]: Stage {1} > Nixies are {2}, function({3}) = {4}, using figure {5}. Press {6}.", init.moduleId, init.stage + 1, nixieL.ToString() + nixieR.ToString(), trigNumber, trigResult, new[] { "LLLMR", "LMMMR", "LMRRR", "LMMRR", "LLMRR", "LLMMR" }[figureUsed], modifiedInput ? "Right" : "Left");
        }

        private int Edgework(int index)
        {
            if (Init.rules.GetLength(0) == 4)
                return Arrays.GetEdgework(Init.rules[3][init.render.GetGear()[1]].Number, FAC);

            switch (index)
            {
                case 0: return FAC.Info.GetBatteryCount();
                case 1: return FAC.Info.GetIndicators().Count();
                case 2: return FAC.Info.GetPortPlateCount();
                case 3: return FAC.Info.GetSerialNumberNumbers().First();
                case 4: return FAC.Info.GetBatteryHolderCount();
                case 5: return FAC.Info.GetOffIndicators().Count();
                case 6: return FAC.Info.GetPorts().Count();
                case 7: return FAC.Info.GetSerialNumberLetters().Count();

                default: throw new IndexOutOfRangeException("Calculate.Edgework recieved an out-of-range number: " + index + ".");
            }
        }
    }
}
