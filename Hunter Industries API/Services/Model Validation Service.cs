using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class ModelValidationService
    {
        /// <summary>
        /// Returns whether the model meets given requirements.
        /// </summary>
        public bool IsValid(object model, bool allRequired = false, string[] ignoreProperties = null)
        {
            bool validModel = false;

            if (model != null)
            {
                bool[] validProperties = Array.Empty<bool>();
                PropertyInfo[] properties = GetProperties(model);

                foreach (PropertyInfo property in properties)
                {
                    if (ignoreProperties != null && !ignoreProperties.Contains(property.Name))
                    {
                        validProperties = validProperties.Append(HasValue(property.GetValue(model))).ToArray();
                    }

                    if (ignoreProperties == null)
                    {
                        validProperties = validProperties.Append(HasValue(property.GetValue(model))).ToArray();
                    }
                }

                validModel = ModelValidity(validProperties, allRequired);
            }

            return validModel;
        }

        /// <summary>
        /// Returns all the properties in the model.
        /// </summary>
        private PropertyInfo[] GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }

        /// <summary>
        /// Returns whether the property has a value.
        /// </summary>
        private bool HasValue(object value = null)
        {
            bool propertyHasValue = false;

            if (value != null)
            {
                propertyHasValue = ConfirmValue(value);
            }

            return propertyHasValue;
        }

        /// <summary>
        /// Confirms whether the value matches the properties type.
        /// </summary>
        private bool ConfirmValue(object value)
        {
            bool valueConfirmed = false;

            if (value.GetType() == typeof(string))
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    if (Regex.IsMatch(value.ToString(), "^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/([0-9]{4})$"))
                    {
                        valueConfirmed = DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
                    }

                    else if (Regex.IsMatch(value.ToString(), "^(0[1-9]|1[0-2])/(0[1-9]|[12][0-9]|3[01])/([0-9]{4})$"))
                    {
                        valueConfirmed = DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
                    }

                    else
                    {
                        valueConfirmed = true;
                    }
                }
            }

            if (value.GetType() == typeof(StringValues))
            {
                valueConfirmed = !StringValues.IsNullOrEmpty(value.ToString());
            }

            if (value.GetType() == typeof(int))
            {
                valueConfirmed = int.TryParse(value.ToString(), out _);
            }

            if (value.GetType() == typeof(bool))
            {
                valueConfirmed = bool.TryParse(value.ToString(), out _);
            }

            return valueConfirmed;
        }

        /// <summary>
        /// Decides if the model is valid.
        /// </summary>
        private bool ModelValidity(bool[] validProperties, bool allRequired)
        {
            bool valid = false;

            if (allRequired)
            {
                valid = validProperties.All(isValid => isValid);
            }

            else
            {
                valid = Array.Find(validProperties, isValid => isValid);
            }

            return valid;
        }
    }
}