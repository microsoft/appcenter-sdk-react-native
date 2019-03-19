// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Ingestion.Models
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
            CannotBeEmpty,
            MaxItems,
            MinItems,
            MaxLength,
            Pattern,
            InclusiveMinimum,
            InclusiveMaximum
        }

        /// <summary>
        /// Gets a string message that describes a given validation rule
        /// </summary>
        /// <param name="rule">The rule to create a string for</param>
        /// <param name="extraValue">An extra detail to include with the rule.</param>
        /// <returns>A string describing the rule</returns>
        private static string GetRuleString(Rule rule, string extraValue)
        {
            switch (rule)
            {
                case Rule.CannotBeNull:
                    return "Cannot be null";
                case Rule.CannotBeEmpty:
                    return "Cannot be empty";
                case Rule.MaxItems:
                    return $"Number of items exceeded maximum of {extraValue}";
                case Rule.MinItems:
                    return $"Number of items less than minimum of {extraValue}";
                case Rule.MaxLength:
                    return $"Maximum length of {extraValue} exceeded";
                case Rule.Pattern:
                    return $"Does not match expected pattern: {extraValue}";
                case Rule.InclusiveMaximum:
                    return $"Item exceeds maximum value of {extraValue}";
                case Rule.InclusiveMinimum:
                    return $"Item is less than minimum value of {extraValue}";
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
        private static string GetErrorString(Rule validationRule, string propertyName, object detail)
        {
            return $"Validation failed due to property '{propertyName}': {GetRuleString(validationRule, detail?.ToString())}";
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
        public ValidationException(Rule validationRule, string propertyName, object detail = null) : base(GetErrorString(validationRule, propertyName, detail))
        {
        }
    }
}
