using Dapper;
using System.Data.SqlClient;
using tec_correct_empty_box_supply_request_datetime_bat.Commons;

namespace tec_correct_empty_box_supply_request_datetime_bat.DAL
{
    internal class TEmptyBoxSupplyRequestDAL
    {
        /// <summary>
        /// 空箱供給依頼日時補正
        /// </summary>
        /// <remarks>運搬終了していない依頼がある場合は、補正依頼日時を本日06:00に更新する(カウントダウン・カウントアップをリセットするため)</remarks>
        public static void UpdateEmptyBoxSupplyRequest()
        {
            // 空箱供給依頼日時更新SQL作成
            string sql = CreateSQLToUpdateEmptyBoxSupplyRequest();

            using SqlConnection connection = new(ConnectToSQLServer.GetSQLServerConnectionString());
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                // DB接続
                using SqlCommand command = new(sql, connection, transaction);
                var count = command.ExecuteNonQuery();
                transaction.Commit();

                if (count == 0)
                {
                    throw new Exception("更新件数が0件です。");
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 空箱供給依頼日時更新SQL作成
        /// </summary>
        /// <returns>SQL文</returns>
        private static string CreateSQLToUpdateEmptyBoxSupplyRequest()
        {
            // 空箱供給状態名=依頼中、準備完了、運搬開始のいずれか
            // 完了フラグ = 0
            // 削除フラグ = 0

            var sql = $@"UPDATE t_empty_box_supply_request 
                        SET corrected_request_datetime = GETDATE()
                        FROM t_empty_box_supply_request     
                        WHERE 
                            empty_box_supply_status_id IN 
                                    ({(int)EnumEmptyBoxSupplyStatus.Requesting}, 
                                    {(int)EnumEmptyBoxSupplyStatus.Ready}, 
                                    {(int)EnumEmptyBoxSupplyStatus.TransportationStart})
                            AND is_completed = 0
                            AND is_deleted = 0
            ";
            return sql;

        }
    }

}
