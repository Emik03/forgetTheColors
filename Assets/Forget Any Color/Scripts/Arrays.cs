using ForgetTheColors;
using KModkit;
using System;
using System.Linq;
using UnityEngine;

namespace ForgetAnyColor
{
    /// <summary>
    /// Contains all the unchanging static lists and information.
    /// </summary>
    static class Arrays
    {
        public const int EditorMaxStage = 2;
        public const float TPAwardPerStage = 2;
        public const string Version = "v1.1.9";

        public static readonly int[,] ColorTable = 
        {
            { 1, 6, 8, 5, 2, 7, 3, 4 },
            { 7, 2, 5, 4, 6, 3, 1, 8 },
            { 3, 8, 1, 6, 4, 5, 7, 2 }
        };

        public static readonly string[] ColorLog =
        {
            "Red",
            "Orange",
            "Yellow",
            "Green",
            "Cyan",
            "Blue",
            "Purple",
            "White",
            "Gray"
        };

        public static string[] Ignore =
        {
            "14",
            "42",
            "501",
            "A>N<D",
            "Bamboozling Time Keeper",
            "Brainf---",
            "Busy Beaver",
            "Don't Touch Anything",
            "Forget Any Color",
            "Forget Enigma",
            "Forget Everything",
            "Forget It Not",
            "Forget Me Not",
            "Forget Me Later",
            "Forget Perspective",
            "Forget The Colors",
            "Forget Them All",
            "Forget This",
            "Forget Us Not",
            "Iconic",
            "Kugelblitz",
            "Multitask",
            "OmegaForget",
            "Organization",
            "Password Destroyer",
            "Purgatory",
            "RPS Judging",
            "Simon Forgets",
            "Simon's Stages",
            "Souvenir",
            "Tallordered Keys",
            "The Time Keeper",
            "The Troll",
            "The Twin",
            "The Very Annoying Button",
            "Timing Is Everything",
            "Turn The Key",
            "Ultimate Custom Night",
            "Übermodule",
            "Whiteout"
        };

        public static int GetEdgework(int index, FACScript FAC)
        {
            if (index > 22 || index < 0)
                throw new IndexOutOfRangeException("Arrays.GetEdgework recieved an out-of-range number: " + index + ".");
            return new[] 
            { 
                FAC.Info.GetBatteryCount(),
                FAC.Info.GetBatteryCount(Battery.AA) + FAC.Info.GetBatteryCount(Battery.AAx3) + FAC.Info.GetBatteryCount(Battery.AAx4),
                FAC.Info.GetBatteryCount(Battery.D), 
                FAC.Info.GetBatteryHolderCount(), 
                FAC.Info.GetIndicators().Count(),
                FAC.Info.GetOnIndicators().Count(),
                FAC.Info.GetOffIndicators().Count(), 
                FAC.Info.GetPortPlateCount(), 
                FAC.Info.GetPorts().Distinct().Count(), 
                FAC.Info.GetPorts().Count() - FAC.Info.GetPorts().Distinct().Count(), 
				FAC.Info.GetPortCount(), 
				FAC.Info.GetSerialNumberNumbers().First(), 
				FAC.Info.GetSerialNumberNumbers().Last(), 
				FAC.Info.GetSerialNumberNumbers().Count(), 
				FAC.Info.GetSerialNumberLetters().Count(), 
				FAC.Info.GetSolvedModuleNames().Count(), 
				FAC.init.maxStage, 
				FAC.Info.GetModuleNames().Count(), 
				FAC.Info.GetSolvableModuleNames().Count() - FAC.Info.GetSolvedModuleNames().Count(),
                FAC.init.maxStage - FAC.Info.GetSolvedModuleNames().Where(m => !Ignore.Contains(m)).Count(), 
				Application.isEditor ? 0 : int.Parse(FAC.DisplayTexts[1].text), 
				int.Parse(FAC.DisplayTexts[0].text) }[index];
        }
    }
}
