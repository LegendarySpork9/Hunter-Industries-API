using HunterIndustriesAPI.Models;
using System.Configuration;

namespace HunterIndustriesAPI.Tests.Functions
{
    internal static class ConfigurationLoaderFunction
    {
        // Loads the configuration app settings into the needed models.
        public static void LoadConfig()
        {
            DatabaseModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            DatabaseModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            ValidationModel.Issuer = ConfigurationManager.AppSettings["Issuer"];
            ValidationModel.Audience = ConfigurationManager.AppSettings["Audience"];
            ValidationModel.SecretKey = ConfigurationManager.AppSettings["SecretKey"];
        }
    }
}
