using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundNotificationAndUpdateExpireDateTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
              table: "notifications_data",
              keyColumn: "id",
              keyValue: 7);
            migrationBuilder.InsertData(
                table: "notifications_data",
                columns: new[] { "id", "action_path", "action_type", "created_at", "description", "payload", "sending_mechanism", "template_values", "title", "visibility" },
                values: new object[] { 7, "_", 1, new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "برجاء مشاركتنا رأيك فى التطبيق لنعمل على تحسينه", "{\"Image\":null}", "Automatic", "{}", "تم الغاء الاشتراك", "Private" });

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_user_expire_data_after_purchase_insert()
                RETURNS TRIGGER 
                LANGUAGE PLPGSQL
                AS
                $$
                declare 
                    current_purchase record;
                    calc_expire_date TimeStamp := null;
                begin 
                    for current_purchase in 
                            select  used_at , number_of_days from (
                            select used_at , number_of_days from public.influencer_code_users_link where user_id = NEW.user_id
                                union 
                            select used_at , number_of_days from public.user_promo_codes where user_id = NEW.user_id and used_at is not null 
                                union 
                            select purchase_date as used_at , number_of_days from public.purchases where user_id = NEW.user_id and refunded_at is null) temp
                            order by used_at
                        loop 
                            if calc_expire_date is null or current_purchase.used_at > calc_expire_date then 
                                calc_expire_date  = current_purchase.used_at + (current_purchase.number_of_days || ' days') ::interval;
                            else 
                                calc_expire_date  = calc_expire_date + (current_purchase.number_of_days || ' days') ::interval;
                            end if; 
                            
                        end loop ; 
                        
                    update users 
                    set expire_date = calc_expire_date	
                    where users.id = NEW.user_id ;

                    RETURN NEW;
                end;
                $$;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS Insert_into_purchases ON purchases;");
            migrationBuilder.Sql(@"
                CREATE TRIGGER Insert_into_purchases
                After INSERT ON purchases
                FOR EACH ROW
                EXECUTE PROCEDURE update_user_expire_data_after_purchase_insert();
            ");
            migrationBuilder.Sql(@"
                CREATE TRIGGER Update_purchase
                After UPDATE OF refunded_at ON purchases
                FOR EACH ROW
                EXECUTE PROCEDURE update_user_expire_data_after_purchase_insert();
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 7);
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS Insert_into_purchases ON purchases;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS Update_purchase ON purchases;");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS update_user_expire_data_after_purchase_insert();");
        }
    }
}
