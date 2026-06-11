// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
using Newtonsoft.Json.Linq;

namespace HunterIndustriesAPIControlPanel.Functions
{
    public class ConfigurationFunction
    {
        private readonly IFileSystem _FileSystem;
        private readonly APISettingsModel APISettings;

        // Sets the class's global variables.
        public ConfigurationFunction(
            IFileSystem _fileSystem,
            APISettingsModel apiSettings)
        {
            _FileSystem = _fileSystem;
            APISettings = apiSettings;
        }

        /// <summary>
        /// Returns the application name of the control panel.
        /// </summary>
        public string GetControlPanelApplication(List<ApplicationModel> applications)
        {
            string controlPanelApplication = "Application Not Found";

            JObject applicationPhraseJSON = JObject.Parse(_FileSystem.ReadAllText(APISettings.AuthPayloadLocation));

            if (applicationPhraseJSON.ContainsKey("phrase"))
            {
                string applicationPhrase = applicationPhraseJSON["phrase"].ToString();
                controlPanelApplication = applications.Find(a => a.Authorisation.Phrase == applicationPhrase)?.Name ?? "Application Not Found";
            }

            return controlPanelApplication;
        }

        /// <summary>
        /// Returns whether the application name is the control panel.
        /// </summary>
        public bool IsControlPanelApplication(ApplicationModel application)
        {
            bool isControlPanelApplication = false;

            JObject applicationPhraseJSON = JObject.Parse(_FileSystem.ReadAllText(APISettings.AuthPayloadLocation));

            if (applicationPhraseJSON.ContainsKey("phrase"))
            {
                string applicationPhrase = applicationPhraseJSON["phrase"].ToString();
                isControlPanelApplication = application.Authorisation.Phrase == applicationPhrase;
            }

            return isControlPanelApplication;
        }

        /// <summary>
        /// Returns whether the authorisation phrase is used by the control panel.
        /// </summary>
        public bool IsControlPanelAuthorisation(string phrase)
        {
            bool isControlPanelAuthorisation = false;

            JObject applicationPhraseJSON = JObject.Parse(_FileSystem.ReadAllText(APISettings.AuthPayloadLocation));

            if (applicationPhraseJSON.ContainsKey("phrase"))
            {
                string applicationPhrase = applicationPhraseJSON["phrase"].ToString();
                isControlPanelAuthorisation = phrase == applicationPhrase;
            }

            return isControlPanelAuthorisation;
        }
    }
}
