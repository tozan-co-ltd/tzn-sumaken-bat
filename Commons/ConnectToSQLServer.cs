using Microsoft.Extensions.Configuration;

namespace tec_correct_emptybox_supply_request_datetime_bat.Commons
{
    /// <summary>
    /// SQLServer接続に関する関数
    /// </summary>
    public static class ConnectToSQLServer
    {
        /// <summary>
        /// SQLServer接続文字列取得
        /// </summary>
        /// <returns></returns>
        public static string GetSQLServerConnectionString()
        {
            var databaseName = "tec-shipping-management";
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            return configuration.GetSection("connectionString").GetValue<string>(databaseName);
        }
    }
}