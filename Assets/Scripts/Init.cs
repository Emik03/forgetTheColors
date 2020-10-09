using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Init
{
    public Init(CoroutineScript Coroutine, FTCScript FTC, TPScript TP)
    {
        this.Coroutine = Coroutine;
        this.FTC = FTC;
        this.TP = TP;

        calculate = new Calculate(FTC);
        render = new Render(FTC, this);
        selectable = new Selectable(FTC, this, render);
    }

    internal Calculate calculate;
    internal CoroutineScript Coroutine;
    internal FTCScript FTC;
    internal Render render;
    internal static Rule[][] Rules;
    internal Selectable selectable;
    internal TPScript TP;

    internal bool solved, legacy;
    internal int moduleId, maxStage, stage;

    internal int[] cylinder, nixies, gear, largeDisplay, sineNumber, calculatedValues;
    internal string[] gearColor, ruleColor;

    private static int moduleIdCounter;

    internal void Start()
    {
        for (byte i = 0; i < FTC.Selectables.Length; i++)
            FTC.Selectables[i].OnInteract += selectable.Interact(i);

        // Module ID assignment.
        moduleId = ++moduleIdCounter;

        // Boss module handler assignment.
        if (FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore) != null)
            Arrays.Ignore = FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore);

        // Set maxStage to amount of modules.
        if (!Application.isEditor)
            maxStage = Math.Max(FTC.Info.GetSolvableModuleNames().Where(module => !Arrays.Ignore.Contains(module)).Count(), 10000);

        // Initalize RuleSeed.
        MonoRandom rnd = FTC.Rule.GetRNG();
        GenerateRules(rnd);
        Debug.LogFormat("[Forget The Colors #{0}]: Version {1}, RuleSeed {2}.", moduleId, Arrays.Version, rnd.Seed);

        // Initalize Colorblind.
        render.colorblind = FTC.Colorblind.ColorblindModeActive;
        render.Colorblind();

        // Log amount of stages.
        bool singleStage = maxStage == 1;
        Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have {1} stage{2}.", moduleId, singleStage ? "a single" : maxStage.ToString(), singleStage ? "" : "s");

        FTC.NixieTexts[0].text = FTC.NixieTexts[1].text = "0";
        FTC.Audio.PlaySoundAtTransform("start", FTC.Module.transform);
    
        // Initalizes the arrays.
        ResetArrays();

        // Debug: Starts up OLD FTC.
        new Legacy(Coroutine, FTC, this).Start();
    }

    internal void ResetArrays()
    {
        cylinder = nixies = gear = largeDisplay = sineNumber = calculatedValues = new int[maxStage];
        gearColor = ruleColor = new string[maxStage];
    }

    private static Rule[][] GenerateRules(MonoRandom rnd)
    {
        if (rnd.Seed == 1)
            return null;

        var rules = new Rule[4][] { new Rule[20], new Rule[10], new Rule[24], new Rule[8] };
        int[] ranges = { 10, 22, 10, 22 };

        for (int i = 0; i < rules.Length; i++)
        {
            for (int j = 0; j < rules[i].Length; j++)
            {
                rules[i][j] = new Rule
                {
                    Value = rnd.Next(ranges[i]),
                    Operator = i < 2 ? rnd.Next(5) : 0
                };
            }
        }

        return rules;
    }
}

