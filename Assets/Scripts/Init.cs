using System;
using System.Linq;
using UnityEngine;

namespace ForgetTheColors
{
    public class Init
    {
        public Init(CoroutineScript coroutine, LegacyFTC legacy, FTCScript FTC, TPScript TP)
        {
            moduleId = ++moduleIdCounter;

            this.coroutine = coroutine;
            this.FTC = FTC;
            this.legacy = legacy;
            this.TP = TP;

            calculate = new Calculate(FTC, this);
            render = new Render(calculate, FTC, this);
            selectable = new Selectable(calculate, coroutine, FTC, this, legacy, render);
        }

        internal Calculate calculate;
        internal CoroutineScript coroutine;
        internal FTCScript FTC;
        internal LegacyFTC legacy;
        internal Render render;
        internal static Rule[][] rules;
        internal Selectable selectable;
        internal TPScript TP;

        internal bool solved;
        internal static int moduleIdCounter;
        internal int fakeStage, moduleId, maxStage = Arrays.EditorMaxStage, stage;
        internal int[] edgeworks, sineNumber, values;
        internal int[,] cylinders, displays, gears, nixies;
        internal string[] gearColor, ruleColor;

        internal void Start()
        {
            // Add an event for each interactable element.
            for (byte i = 0; i < FTC.Selectables.Length; i++)
                FTC.Selectables[i].OnInteract += selectable.Interact(i);

            // Boss module handler assignment.
            if (FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore) != null)
                Arrays.Ignore = FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore);

            // Set maxStage to amount of modules.
            if (!Application.isEditor)
                maxStage = Math.Min(FTC.Info.GetSolvableModuleNames().Where(m => !Arrays.Ignore.Contains(m)).Count(), 10000);

            // Initalize RuleSeed.
            if (rules == null)
                rules = GenerateRules(FTC.Rule.GetRNG(), ref FTC);

            // Set gear to some value, which would conflict colorblind if not set.
            FTC.GearText.text = "0";

            // Initalize Colorblind.
            render.colorblind = FTC.Colorblind.ColorblindModeActive;
            render.Colorblind(render.colorblind);

            // Logs initalization.
            bool singleStage = maxStage == 1;
            Debug.LogFormat("[Forget The Colors #{0}]: {1} stage{2} using {3}.{4}", moduleId, singleStage ? "a single" : maxStage.ToString(), singleStage ? "" : "s", Arrays.Version, rules.GetLength(0) == 4 ? " Rule Seed " + FTC.Rule.GetRNG() : string.Empty);

            // Initalizes the arrays.
            ResetArrays();
            coroutine.StartNewStage();
        }

        internal void LegacyFTC()
        {
            TP.TwitchHelpMessage = @"!{0} submit <##> (Cycles through both nixies to match '##', then hits submit. If in strike mode, submitting will get you out of strike mode and back to submission | Valid numbers are from 0-99) !{0} preview <#> (If the module has struck, you can make # any valid stage number, which will show you what it displayed on that stage)";

            FTC.BackingTexts[0].color = FTC.BackingTexts[1].color = FTC.DisplayTexts[0].color = FTC.DisplayTexts[1].color = new Color32(0, 255, 0, 255);
            coroutine.enabled = FTC.enabled = false;

            legacy.Activate(ref moduleId);
        }

        internal void ResetArrays()
        {
            sineNumber = values = new int[maxStage];
            cylinders = new int[maxStage, 3];
            gears = displays = new int[maxStage, 2];
            gearColor = ruleColor = new string[maxStage];
        }

        private static Rule[][] GenerateRules(MonoRandom rnd, ref FTCScript FTC)
        {
            FTC.Audio.PlaySoundAtTransform("start", FTC.Module.transform);

            if (rnd.Seed == 1)
                return new Rule[0][];

            var rules = new Rule[4][] { new Rule[20], new Rule[10], new Rule[24], new Rule[8] };
            int[] ranges = { 10, 22, 10, 22 };

            for (int i = 0; i < rules.Length; i++)
            {
                for (int j = 0; j < rules[i].Length; j++)
                {
                    rules[i][j] = new Rule
                    {
                        Number = rnd.Next(ranges[i]),
                        Function = i < 2 ? rnd.Next(5) : 0
                    };
                }
            }

            return rules;
        }
    }
}
