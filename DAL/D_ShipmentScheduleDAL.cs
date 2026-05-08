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
                        @DepoID,

                        mc.CompanyID, -- 会社ID

                        @DeliveryTimeClass, -- 便

                        CASE
                            WHEN h.VHJIKO = 'L6'
                                THEN N'三菱KD工場'
                            ELSE m.DEKANJ
                        END, -- 納入先名称

                        e.VINONO, -- 納品書番号

                        e.VIBUNM, -- 部品名称

                        e.VIBUNO, -- 部品番号

                        e.VIBUNO, -- 仕入先部品番号

                        e.VISRYO, -- 数量

                        ISNULL(
                            CEILING(
                                CAST(e.VISRYO AS DECIMAL(18, 2))
                                / NULLIF(p.LotQuantity, 0)
                            ),
                            0
                        ), -- 容器数

                        h.VHNOBA, -- 納入先工区

                        e.VIDATE, -- 納入日

                        h.VHJIKO, -- 納入場所

                        p.LotQuantity, -- 収容数

                        GETDATE(), -- 発行日

                        e.TorokuDateTime, -- 作成日時

                        e.KosinUserId, -- 作成者

                        GETDATE(), -- 更新日時

                        e.KosinUserId -- 更新者

                    FROM tozandbEDI.dbo.EDI_VI_nohin_meisai e

                    INNER JOIN tozandbEDI.dbo.EDI_VH_nohin_header h
                        ON  e.VINOKU = h.VHNOKU
                        AND e.VINOSE = h.VHNOSE
                        AND e.VINONO = h.VHNONO

                    LEFT JOIN tozandbEDI.dbo.BU_DE_UNYname_master m
                        ON m.DEMECD = h.VHJIKO

                    LEFT JOIN M_Company mc
                        ON mc.CompanyName =
                            CASE
                                WHEN h.VHNOSE IN ('T', 'B', 'M', 'N')
                                    THEN N'三菱ふそう'
                                ELSE N'三菱自動車'
                            END

                        AND mc.IsDeleted = 0

                    LEFT JOIN M_Delivery md
                        ON  md.CompanyID = mc.CompanyID
                        AND md.DeliveryName =
                            CASE
                                WHEN h.VHJIKO = 'L6'
                                    THEN N'三菱KD工場'
                                ELSE m.DEKANJ
                            END

                        AND md.DeliveryFactoryName = h.VHNOBA

                        AND md.IsDeleted = 0

                    LEFT JOIN M_Product p
                        ON  p.SupplierProductNumber = e.VIBUNO
                        AND  p.CompanyID = mc.CompanyID
                        AND p.IsDeleted = 0

                    WHERE
                        e.VITRCD = 'J019'

                        AND e.TorokuDateTime >= DATEADD(DAY, -2, GETDATE())

                        AND NOT EXISTS
                        (
                            SELECT 1
                            FROM D_ShipmentSchedule d
                            WHERE d.DeliverySlipNumber = e.VINONO
                        )
                ";
                var affected = conn.Execute(sql, new
                {
                    DepoID = Mitsubishi.DepoID,
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
