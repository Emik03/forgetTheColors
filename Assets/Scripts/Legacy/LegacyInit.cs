﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyForgetTheColors
{
    /// <summary>
    /// Initalizes Colorblind, RuleSeed, and all of the temporary lists to be the correct size.
    /// </summary>
    sealed class LegacyInit
    {
        public void Start(ref KMBossModule Boss, ref bool colorblind, ref KMColorblindMode Colorblind, ref KMRuleSeedable Rule, ref int moduleId, ref LegacyRule[][] rules, ref int maxStage, ref KMBombInfo BombInfo, ref List<byte> cylinder, ref List<byte> nixies, ref List<byte> gear, ref List<short> largeDisplay, ref List<int> calculatedValues, ref List<int> sineNumber, ref List<string> gearColor, ref List<string> ruleColor, ref TextMesh[] NixieTexts, ref KMAudio Audio, ref KMBombModule Module, ref TextMesh[] DisplayTexts, ref TextMesh DebugText)
        {
            //boss module handler
            if (Boss.GetIgnoredModules(Module, LegacyStrings.Ignore) != null)
                LegacyStrings.Ignore = Boss.GetIgnoredModules(Module, LegacyStrings.Ignore);

            //enables colorblind mode if needed
            colorblind = Colorblind.ColorblindModeActive;

            //gets seed
            MonoRandom rnd = Rule.GetRNG();
            Debug.LogFormat("[Forget The Colors #{0}]: Using version {1} with rule seed: {2}.", moduleId, LegacyStrings.Version, rnd.Seed);

            if (rnd.Seed == 1)
                rules = null;

            else
            {
                //establishes new variable
                rules = new LegacyRule[2][];
                rules[0] = new LegacyRule[21];
                rules[1] = new LegacyRule[10];

                //applies rule seeding for cylinders
                for (byte i = 0; i < rules[0].Length; i++)
                    rules[0][i] = new LegacyRule { Cylinder = (byte)rnd.Next(rules[0].Length), Operator = (byte)rnd.Next(5) };

                //applies rule seeding for edgework
                for (byte i = 0; i < rules[1].Length; i++)
                    rules[1][i] = new LegacyRule { Edgework = (byte)rnd.Next(rules[1].Length), Operator = (byte)rnd.Next(5) };
            }

            //if on unity, max stage should equal the initial value assigned, otherwise set it to the proper value
            if (!Application.isEditor)
                maxStage = BombInfo.GetSolvableModuleNames().Where(module => !LegacyStrings.Ignore.Contains(module)).Count();

            //proper grammar!!
            if (maxStage != 1)
                Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have {1} stages.", moduleId, maxStage);

            else
                Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have a single stage.", moduleId);

            //initialization of previous stage variables
            for (ushort i = 0; i < maxStage; i++)
            {
                for (byte j = 0; j < 4; j++)
                    cylinder.Add(0);

                nixies.Add(0);
                nixies.Add(0);
                gear.Add(0);
                largeDisplay.Add(0);
                calculatedValues.Add(0);
                sineNumber.Add(0);
                gearColor.Add("Red");
                ruleColor.Add("Red");
            }

            //begin module
            NixieTexts[0].text = "0";
            NixieTexts[1].text = "0";
            Audio.PlaySoundAtTransform("start", Module.transform);

            //show that it's debug mode
            if (Application.isEditor)
                DebugText.text = string.Empty;
        }
    }
}