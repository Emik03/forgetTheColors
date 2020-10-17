namespace ForgetAnyColor
{
    /// <summary>
    /// Datatype for use in RuleSeed, containing 2 integers.
    /// </summary>
    internal class Rule
    {
        /// <summary>
        /// This value is set from 0 through 9, for use in tables.
        /// </summary>
        internal int Number { get; set; }

        /// <summary>
        /// This value is exclusive for Forget The Colors, which while unused is still needed to be in-sync with the manual.
        /// </summary>
        internal int Function { get; set; }
    }
}
