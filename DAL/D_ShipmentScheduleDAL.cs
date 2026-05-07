using Dapper;
using System.Data.SqlClient;
using tzn_sumaken_bat.Commons;
using static tzn_sumaken_bat.Commons.SystemConstants;

namespace tzn_sumaken_bat.DAL
{
    internal class D_ShipmentScheduleDAL
    {
        /// <summary>
        /// 出荷指示登録
        /// </summary>
        public static void ImportShipmentScheduleFromEDI()
        {
            try
            {
                using var conn = new SqlConnection(
                    ConnectToSQLServer.GetConnectionString("warehouse"));

                conn.Open();

                var sql = @"
                    INSERT INTO D_ShipmentSchedule
                    (
                        DepoID,
                        CompanyID,
                        DeliveryTimeClass,
                        DeliveryName,
                        DeliverySlipNumber,
                        DeliveryProductName,
                        DeliveryProductNumber,
                        SupplierProductNumber,
                        Quantity,
                        NumberOfBoxes,
                        DeliveryFactoryName,
                        DeliveryDate,
                        DeliveryLocation,
                        LotQuantity,
                        IssuedDate,
                        CreatedAt,
                        CreatedBy,
                        UpdatedAt,
                        UpdatedBy
                    )
                    SELECT
                        @DepoID,　　　　　　-- 倉庫ID
                        @CompanyID,      -- 会社ID
                        @DeliveryTimeClass, 　--便
                        e.VINOSE,             -- 納入製作所
                        e.VINONO,             -- 納品書番号
                        e.VIBUNM,             -- 部品名称
                        e.VIBUNO,             -- 部品番号
                        e.VIBUNO,             -- 部品番号
                        e.VISRYO,             -- 納入指示数
                        e.VIYOSU,             -- 容器数
                        e.VIHOAN,             -- 保安区分
                        e.VIDATE,             -- 納入指示日
                        e.VINOBA,             -- 納場
                        e.VILOSU,             -- 収容数
                        GETDATE(),           -- 発行日
                        e.TorokuDateTime,      -- 登録日時
                        e.KosinUserId,      -- 作成者
                        GETDATE(),      -- 更新日時
                        e.KosinUserId      -- 作成者
                    FROM tozandbEDI.dbo.EDI_VI_nohin_meisai e
                    WHERE 
                        e.VITRCD = 'J019'
                        AND e.TorokuDateTime >= DATEADD(DAY, -2, GETDATE())
                        AND NOT EXISTS (
                            SELECT 1
                            FROM D_ShipmentSchedule d
                            WHERE d.DeliverySlipNumber = e.VINONO
                        )
                ";

                var affected = conn.Execute(sql, new
                {
                    DepoID = Mitsubishi.DepoID,
                    CompanyID = Mitsubishi.CompanyID,
                    DeliveryTimeClass = Mitsubishi.DeliveryTimeClass
                });

                if (affected == 0)
                    Console.WriteLine("登録対象データはありません。");
                else
                    Console.WriteLine($"取込件数：{affected}件");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
