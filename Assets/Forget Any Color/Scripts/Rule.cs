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
        public int Number { get; set; }

        /// <summary>
        /// This value is exclusive for legacy FTC, where operators are used alongside the numbers.
        /// </summary>
        public int Function { get; set; }
    }
}
