// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using System.ComponentModel;

namespace HunterIndustriesAPIControlPanel.Functions
{
    public static class SettingValidatorFunction
    {
        /// <summary>
        /// Returns a list of validation errors for the supplied application's settings,
        /// resolving each setting's effective post-save value from pending-new → current → null.
        /// </summary>
        public static List<string> ValidateApplicationSettings(ApplicationModel application,
            UserSettingModel? currentUserSettings,
            List<UserSettingRequestModel> pendingNewSettings)
        {
            List<string> errors = [];

            foreach (ApplicationSettingModel applicationSetting in application.Settings)
            {
                string? value = pendingNewSettings.FirstOrDefault(ns => ns.SettingName == applicationSetting.Name)?.SettingValue ?? currentUserSettings?.Settings.FirstOrDefault(s => s.Name == applicationSetting.Name)?.Value;

                if (applicationSetting.Required && string.IsNullOrWhiteSpace(value))
                {
                    errors.Add($"{applicationSetting.Name} is required.");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(value) && !TryConvert(applicationSetting.Type, value))
                {
                    errors.Add($"{applicationSetting.Name} must be a valid {applicationSetting.Type}.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Returns whether the value can be converted to the specified type.
        /// </summary>
        public static bool TryConvert(string typeName,
            string value)
        {
            Type? resolvedType = Type.GetType($"System.{typeName}", false, true);

            if (resolvedType == null)
            {
                return false;
            }

            try
            {
                return TypeDescriptor.GetConverter(resolvedType).IsValid(value);
            }

            catch
            {
                return false;
            }
        }
    }
}
