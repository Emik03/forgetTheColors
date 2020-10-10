using System;
using System.Linq;
using UnityEngine;

public class Init
{
    public Init(CoroutineScript Coroutine, FTCScript FTC, TPScript TP)
    {
        moduleId = ++moduleIdCounter;

        this.coroutine = Coroutine;
        this.FTC = FTC;
        this.TP = TP;

        calculate = new Calculate(FTC);
        render = new Render(FTC, this);
        selectable = new Selectable(calculate, FTC, this, render);
    }

    internal Calculate calculate;
    internal CoroutineScript coroutine;
    internal FTCScript FTC;
    internal Render render;
    internal static Rule[][] rules;
    internal Selectable selectable;
    internal TPScript TP;

    internal bool solved, legacy;
    internal static int moduleIdCounter;
    internal int moduleId, stage, fakeStage, maxStage = Arrays.EditorMaxStage;
    internal int[] cylinder, nixies, gear, largeDisplay, sineNumber, calculatedValues;
    internal string[] gearColor, ruleColor;

    internal void Start()
    {
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

        // Initalize Colorblind.
        render.colorblind = FTC.Colorblind.ColorblindModeActive;
        render.Colorblind(render.colorblind);

        FTC.NixieTexts[0].text = FTC.NixieTexts[1].text = "0";

        // Logs initalization.
        bool singleStage = maxStage == 1;
        Debug.LogFormat("[Forget The Colors #{0}]: {1} stage{2} using {3}.", moduleId, singleStage ? "a single" : maxStage.ToString(), singleStage ? "" : "s", Arrays.Version);

        // Initalizes the arrays.
        ResetArrays();
        coroutine.StartNewStage();

        // Debug: Starts up OLD FTC.
        new Legacy(coroutine, FTC, this).Start();
    }

    internal void ResetArrays()
    {
        cylinder = nixies = gear = largeDisplay = sineNumber = calculatedValues = new int[maxStage];
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

