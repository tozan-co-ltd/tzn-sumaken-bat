using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Dapper;
using System.Data.SqlClient;
//using tec_correct_empty_box_supply_request_datetime_bat.Commons;

namespace tec_correct_empty_box_supply_request_datetime_bat.DAL
{
    internal class TEmptyBoxSupplyRequestDAL
    {
        /// <summary>
        /// 
        /// </summary>
        public static void UpdateEmptyBoxSupplyRequest()
        {
            ////DB接続              
            //string connectionString = ConnectToSQLServer.GetSQLServerConnectionString();
            //using (SqlConnection con = new SqlConnection(connectionString))
            //{

            //    con.Open();
            //    DefaultTypeMap.MatchNamesWithUnderscores = true;
            //    //SQL実行
            //    var param = new
            //    {
            //        TargetDate = targetDate

            //    };
            //    //デバッグ用
            //    string tableName = "t_daily_shipping_plans";//test_making_daily_shipping_plans
            //    var IsTxCommitedForPlan = false;
            //    using (var tx = con.BeginTransaction())
            //    {
            //        try
            //        {
            //            //デバッグ用                       
            //            var count = con.Execute(CreateSQLToDailyShippingPlan(tableName), param, tx);
            //            tx.Commit();
            //            IsTxCommitedForPlan = true;

            //            if (count == 0)
            //            {
            //                throw new Exception("更新件数が0件です。");
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            if (!IsTxCommitedForPlan)
            //            {
            //                tx.Rollback();
            //            }
            //            throw;
            //        }
            //    }
            //}
        }

        private static string CreateSQLToUpdateEmptyBoxSupplyRequest()
        {
            string sql = $@"

             ";
            return sql;

        }
    }

}
