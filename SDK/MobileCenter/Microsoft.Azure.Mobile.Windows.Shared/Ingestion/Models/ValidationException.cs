namespace Microsoft.AAppCenterIngestion.Models
{
    /// <summary>
    /// Exception thrown when ingestion models fail to validate
    /// </summary>
    public class ValidationException : IngestionException
    {
        private const string DefaultMessage = "Validation failed";

        /// <summary>
        /// Validation rules that <see cref="ValidationException"/> recognizes
        /// </summary>
        public enum Rule
        {
            CannotBeNull,
            CannotBeEmpty
        }

        /// <summary>
        /// Gets a string message that describes a given validation rule
        /// </summary>
        /// <param name="rule">The rule to create a string for</param>
        /// <returns>A string describing the rule</returns>
        private static string GetRuleString(Rule rule)
        {
            switch (rule)
            {
                case Rule.CannotBeNull:
                    return "Cannot be null";
                case Rule.CannotBeEmpty:
                    return "Cannot be empty";
                default:
                    return "Unknown rule";
            }
        }

        /// <summary>
        /// Constructs a full error string to be used as an exception message
        /// </summary>
        /// <param name="validationRule">The rule that was broken</param>
        /// <param name="propertyName">The name of the property that broke the rule</param>
        /// <returns></returns>
        private static string GetErrorString(Rule validationRule, string propertyName)
        {
            return $"Validation failed due to property '{propertyName}': {GetRuleString(validationRule)}";
        }

        public ValidationException() : base(DefaultMessage)
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a validation exception and populates its message
        /// </summary>
        /// <param name="validationRule">The rule that was broken</param>
        /// <param name="propertyName">The name of the property that broke the rule</param>
        public ValidationException(Rule validationRule, string propertyName) : base(GetErrorString(validationRule, propertyName))
        {
        }
    }
}
