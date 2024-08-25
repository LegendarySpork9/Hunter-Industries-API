// Copyright © - unpublished - Toby Hunter
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HunterIndustriesAPI.Services
{
    public class ModelValidationService
    {
        public bool IsValid(object model, bool allRequired = false, string[]? ignoreProperties = null)
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

        private PropertyInfo[] GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }

        private bool HasValue(object? value)
        {
            bool propertyHasValue = false;

            if (value != null)
            {
                propertyHasValue = ConfirmValue(value);
            }

            return propertyHasValue;
        }

        private bool ConfirmValue(object value)
        {
            bool valueConfirmed = false;

            if (value.GetType() == typeof(string))
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
#pragma warning disable CS8604 // Possible null reference argument.
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
#pragma warning restore CS8604 // Possible null reference argument.
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

            return valueConfirmed;
        }

        private bool ModelValidity(bool[] validProperties, bool allRequired)
        {
            bool valid = false;

            if (allRequired)
            {
                valid = validProperties.All(valid => valid);
            }

            else
            {
                valid = Array.Find(validProperties, valid => valid);
            }

            return valid;
        }
    }
}
