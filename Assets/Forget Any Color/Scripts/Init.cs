using System;
using System.Linq;
using UnityEngine;

namespace ForgetAnyColor
{
    public class Init
    {
        public Init(CoroutineScript coroutine, FACScript FAC, TPScript TP)
        {
            moduleId = ++moduleIdCounter;

            this.coroutine = coroutine;
            this.FAC = FAC;
            this.TP = TP;

            calculate = new Calculate(FAC, this);
            render = new Render(calculate, FAC, this);
            selectable = new Selectable(calculate, coroutine, FAC, this, render);
        }

        internal Calculate calculate;
        internal CoroutineScript coroutine;
        internal FACScript FAC;
        internal Render render;
        internal static Rule[][] rules;
        internal Selectable selectable;
        internal TPScript TP;

        internal bool solved;
        internal static int moduleIdCounter;
        internal int fakeStage, moduleId, stage, maxStage = Arrays.EditorMaxStage;
        internal int[,] cylinders;

        internal void Start()
        {
            // Add an event for each interactable element.
            for (byte i = 0; i < FAC.Selectables.Length; i++)
                FAC.Selectables[i].OnInteract += selectable.Interact(i);

            // Boss module handler assignment.
            if (FAC.Boss.GetIgnoredModules(FAC.Module, Arrays.Ignore) != null)
                Arrays.Ignore = FAC.Boss.GetIgnoredModules(FAC.Module, Arrays.Ignore);

            // Set maxStage to amount of modules.
            if (!Application.isEditor)
                maxStage = Math.Min(FAC.Info.GetSolvableModuleNames().Where(m => !Arrays.Ignore.Contains(m)).Count(), 10000);

            // Initalize RuleSeed.
            if (rules == null)
                rules = GenerateRules(FAC.Rule.GetRNG(), ref FAC);

            // Set gear to some value, which would conflict colorblind if not set.
            FAC.GearText.text = "0";

            // Initalize Colorblind.
            render.colorblind = FAC.Colorblind.ColorblindModeActive;
            render.Colorblind(render.colorblind);

            // Logs initalization.
            bool singleStage = maxStage == 1;
            Debug.LogFormat("[Forget Any Color #{0}]: {1} stage{2} using {3}.{4}", moduleId, singleStage ? "A single" : maxStage.ToString(), singleStage ? "" : "s", Arrays.Version, rules.GetLength(0) == 4 ? " Rule Seed " + FAC.Rule.GetRNG().Seed + '.' : string.Empty);

            // Initalizes the arrays.
            cylinders = new int[maxStage + 1, 3];

            // Automatically start a new stage.
            coroutine.StartNewStage();
        }

        private static Rule[][] GenerateRules(MonoRandom rnd, ref FACScript FAC)
        {
            FAC.Audio.PlaySoundAtTransform("start", FAC.Module.transform);

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
