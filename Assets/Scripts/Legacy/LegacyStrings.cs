using ForgetTheColors;

namespace LegacyForgetTheColors
{
    /// <summary>
    /// Contains all the static strings used in FTC.
    /// </summary>
    sealed class LegacyStrings
    {
        public const string Version = Arrays.Version;

        public static string[] Ignore = Arrays.Ignore;

        public static readonly string[] FailPhrases = Arrays.Lose, WinPhrases = Arrays.Win;

        public static readonly string[] ColorLog =
        {
            "Red",
            "Orange",
            "Yellow",
            "Green",
            "Cyan",
            "Blue",
            "Purple",
            "Pink",
            "Maroon",
            "White",
            "Gray"
        };

        public static readonly string[] DebugText =
        {
            "largeH",
            "largeD",
            "large",
            "cylA",
            "cylB",
            "cylC",
            "gear#",
            "gearC",
            "nixieL",
            "nixieR",
            "stage",
            "quit"
        };
    }
}