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
    }

    internal CoroutineScript Coroutine;
    internal FTCScript FTC;
    internal static Rule[][] Rules;
    internal TPScript TP;

    internal bool solved, legacy;
    internal int moduleId, maxStage, stage;

    internal byte[] cylinder, nixies, gear;
    internal short[] largeDisplay;
    internal int[] sineNumber, calculatedValues;
    internal string[] gearColor, ruleColor;

    private static int moduleIdCounter;

    protected internal void Start()
    {
        moduleId = ++moduleIdCounter;

        if (FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore) != null)
            Arrays.Ignore = FTC.Boss.GetIgnoredModules(FTC.Module, Arrays.Ignore);

        if (!Application.isEditor)
            maxStage = Math.Max(FTC.BombInfo.GetSolvableModuleNames().Where(module => !Arrays.Ignore.Contains(module)).Count(), 10000);

        MonoRandom rnd = FTC.Rule.GetRNG();
        GenerateRules(rnd);
        Debug.LogFormat("[Forget The Colors #{0}]: Version {1}, RuleSeed {2}.", moduleId, Arrays.Version, rnd.Seed);

        //Render.colorblind = FTC.Colorblind.ColorblindModeActive;


        bool singleStage = maxStage == 1;
        Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have {1} stage{2}.", moduleId, singleStage ? "a single" : maxStage.ToString(), singleStage ? "" : "s");

        FTC.NixieTexts[0].text = FTC.NixieTexts[1].text = "0";
        FTC.Audio.PlaySoundAtTransform("start", FTC.Module.transform);
        
        if (Application.isEditor)
        {
            FTC.DisplayTexts[1].fontSize = 35;
            FTC.DebugText.text = "";
        }

        new Legacy(FTC, this).Start();
    }

    private static Rule[][] GenerateRules(MonoRandom rnd)
    {
        if (rnd.Seed == 1)
            return null;

        var rules = new Rule[4][] { new Rule[20], new Rule[10], new Rule[24], new Rule[8] };
        int[] ranges = { 10, 22, 10, 22 };

        for (byte i = 0; i < rules.Length; i++)
        {
            for (byte j = 0; j < rules[i].Length; j++)
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

    private void ResetArrays()
    {
        cylinder = nixies = gear = new byte[maxStage];
        largeDisplay = new short[maxStage];
        sineNumber = calculatedValues = new int[maxStage];
        gearColor = ruleColor = new string[maxStage];
    }
}

