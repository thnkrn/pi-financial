using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddChangeStatusAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("04bffc06-1405-4809-b32f-09115d66c08d"), "ChangeSetTradeStatusToSuccess", new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d") },
                    { new Guid("074d958f-1639-417a-a21b-36130c84e39a"), "ChangeStatusToFail", new Guid("222d19bd-92b9-4c40-bcea-3b404a14146a") },
                    { new Guid("0d201e4b-94f8-42aa-b708-a618cbbb14d8"), "ChangeStatusToFail", new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f") },
                    { new Guid("0f2ef9b5-6c84-4ceb-b794-b74e12bba61e"), "ChangeStatusToSuccess", new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f") },
                    { new Guid("1e5f4d1c-f65b-4033-ad7d-0a5c3196d3e5"), "ChangeSetTradeStatusToSuccess", new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c") },
                    { new Guid("2a3eff05-64bd-4a4b-a58e-aef6ffbcd80a"), "ChangeStatusToFail", new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90") },
                    { new Guid("2e9176ba-9dfe-48ba-836e-92059f1c9488"), "ChangeStatusToSuccess", new Guid("975cd24f-b648-402f-a17f-65fd053c9e72") },
                    { new Guid("3921769a-c1c7-48a3-9920-f06e9770775b"), "ChangeStatusToSuccess", new Guid("76c845bf-fb3a-490f-928c-54811f0a8739") },
                    { new Guid("4272eef0-736c-4f3a-bb81-5f7e2391f789"), "ChangeStatusToFail", new Guid("532d2838-9610-4704-a267-4e609032adf9") },
                    { new Guid("45828eb2-052a-4c36-9c95-c4268077817e"), "ChangeSetTradeStatusToFail", new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c") },
                    { new Guid("518c4ce1-6cda-4b07-911a-bf3d55666e4d"), "ChangeStatusToSuccess", new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d") },
                    { new Guid("524fe165-d582-4f6e-89f6-f815a02309e5"), "ChangeStatusToSuccess", new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b") },
                    { new Guid("5f747a23-eb7e-4dba-8442-38b9b4cb6fa1"), "ChangeStatusToSuccess", new Guid("532d2838-9610-4704-a267-4e609032adf9") },
                    { new Guid("744ba0f4-040b-4bb0-ab4e-54cd21f2a4e5"), "ChangeStatusToSuccess", new Guid("f9e00911-c580-48f2-9302-d7e10388507f") },
                    { new Guid("8303a6a4-657e-4937-a8f5-8c2f6c58958d"), "ChangeStatusToSuccess", new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90") },
                    { new Guid("907361e9-fc23-4d25-b32d-9bf12f75a9ec"), "ChangeStatusToFail", new Guid("975cd24f-b648-402f-a17f-65fd053c9e72") },
                    { new Guid("9807126c-e2b4-4b83-a530-579a5a291a36"), "ChangeStatusToFail", new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2") },
                    { new Guid("a55878c9-9a74-42bb-85c4-fb27298730bf"), "ChangeStatusToFail", new Guid("f9e00911-c580-48f2-9302-d7e10388507f") },
                    { new Guid("a679f695-898d-4ba3-94ee-c963a6b38ee7"), "ChangeStatusToSuccess", new Guid("0e1158b2-569d-4916-a68c-508c6813cb79") },
                    { new Guid("a6f84686-6ee2-432f-955f-fa8c570b210e"), "ChangeStatusToFail", new Guid("4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b") },
                    { new Guid("ac2b76d7-37c4-45af-a796-d8ecce065baf"), "ChangeStatusToSuccess", new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c") },
                    { new Guid("b5ef4ff0-97b8-4d08-8b3f-e215d8fb8f59"), "ChangeStatusToSuccess", new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d") },
                    { new Guid("b7b356f2-85c9-4fbe-ac86-7759610e3bc6"), "ChangeStatusToFail", new Guid("0e1158b2-569d-4916-a68c-508c6813cb79") },
                    { new Guid("c34a1032-49f5-497e-aab7-891529ca7363"), "ChangeStatusToFail", new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d") },
                    { new Guid("ccc455ce-0ccb-46fe-a729-371c6aa20b28"), "ChangeStatusToFail", new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d") },
                    { new Guid("cddf81a7-c5f7-434d-870a-198d653b83dc"), "ChangeStatusToSuccess", new Guid("4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b") },
                    { new Guid("d3b6668f-63e4-40d9-b282-ead905076903"), "ChangeSetTradeStatusToFail", new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d") },
                    { new Guid("d69c72b1-7eda-4fc6-9bd8-5fcc308444d0"), "ChangeStatusToFail", new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c") },
                    { new Guid("d7cb0c26-e1f8-43e9-ba4f-6449789535d6"), "ChangeStatusToFail", new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b") },
                    { new Guid("da0a09cb-6003-4879-83e6-24876d3ca863"), "ChangeStatusToFail", new Guid("76c845bf-fb3a-490f-928c-54811f0a8739") },
                    { new Guid("eb96e5ce-5dfd-4f7b-ae00-66ecefa3c4bb"), "ChangeStatusToSuccess", new Guid("222d19bd-92b9-4c40-bcea-3b404a14146a") },
                    { new Guid("f797b251-e55b-4077-99cf-4f4268788754"), "ChangeStatusToSuccess", new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2") },
                    { new Guid("f938909d-7b69-473b-93a1-4b2b171a2fd5"), "ChangeStatusToSuccess", new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b") },
                    { new Guid("fe29778c-4c72-46c1-aaa8-89efb10b6059"), "ChangeStatusToFail", new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b") }
                });

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("3dc91a3c-402b-45b5-80a5-b96c75e24391"), "Waiting for ats response", true, "Withdraw", null, "WaitingForAtsGatewayConfirmation", "Change transaction status" },
                    { new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936"), "SetTrade Trading Account Deposit Fail", true, "Deposit", null, "UpBackFailedRequireActionSetTrade", "Change transaction status" },
                    { new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28"), "Waiting for receiving freewill response", true, "Withdraw", null, "WithdrawWaitingForGateway", "Change transaction status" },
                    { new Guid("fe785ac2-09af-4cb0-a679-030310809bee"), "SetTrade Trading Account Deposit Fail", true, "Deposit", null, "TfexCashDepositFailed", "Change transaction status" }
                });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("103152af-c45c-462e-8eb8-e3b0cf77990e"), "ChangeStatusToFail", new Guid("fe785ac2-09af-4cb0-a679-030310809bee") },
                    { new Guid("15cf9ccf-054e-4061-8c8d-bc4e602191e0"), "ChangeStatusToFail", new Guid("3dc91a3c-402b-45b5-80a5-b96c75e24391") },
                    { new Guid("1ff86f44-9818-44f7-a421-89105a3a4a7c"), "ChangeStatusToFail", new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936") },
                    { new Guid("6d98f3cf-5654-44f8-850f-7cca6257af00"), "ChangeStatusToSuccess", new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28") },
                    { new Guid("a01bf92f-f69d-4a01-9349-325a2d2a296b"), "ChangeStatusToSuccess", new Guid("3dc91a3c-402b-45b5-80a5-b96c75e24391") },
                    { new Guid("aadb8248-f18d-438e-bd9e-c33753e71fee"), "ChangeStatusToSuccess", new Guid("fe785ac2-09af-4cb0-a679-030310809bee") },
                    { new Guid("ab6bab02-cd95-4ac1-846f-c3acfcc39541"), "ChangeSetTradeStatusToFail", new Guid("fe785ac2-09af-4cb0-a679-030310809bee") },
                    { new Guid("b43288c1-b3e1-413b-a0e2-fe52781ee1b0"), "ChangeStatusToFail", new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28") },
                    { new Guid("bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538"), "ChangeStatusToSuccess", new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936") },
                    { new Guid("f9ba7739-b55c-49de-8beb-ccc3d08beba8"), "ChangeSetTradeStatusToSuccess", new Guid("fe785ac2-09af-4cb0-a679-030310809bee") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("04bffc06-1405-4809-b32f-09115d66c08d"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("074d958f-1639-417a-a21b-36130c84e39a"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("0d201e4b-94f8-42aa-b708-a618cbbb14d8"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("0f2ef9b5-6c84-4ceb-b794-b74e12bba61e"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("103152af-c45c-462e-8eb8-e3b0cf77990e"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("15cf9ccf-054e-4061-8c8d-bc4e602191e0"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("1e5f4d1c-f65b-4033-ad7d-0a5c3196d3e5"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("1ff86f44-9818-44f7-a421-89105a3a4a7c"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("2a3eff05-64bd-4a4b-a58e-aef6ffbcd80a"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("2e9176ba-9dfe-48ba-836e-92059f1c9488"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("3921769a-c1c7-48a3-9920-f06e9770775b"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("4272eef0-736c-4f3a-bb81-5f7e2391f789"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("45828eb2-052a-4c36-9c95-c4268077817e"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("518c4ce1-6cda-4b07-911a-bf3d55666e4d"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("524fe165-d582-4f6e-89f6-f815a02309e5"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("5f747a23-eb7e-4dba-8442-38b9b4cb6fa1"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("6d98f3cf-5654-44f8-850f-7cca6257af00"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("744ba0f4-040b-4bb0-ab4e-54cd21f2a4e5"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("8303a6a4-657e-4937-a8f5-8c2f6c58958d"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("907361e9-fc23-4d25-b32d-9bf12f75a9ec"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("9807126c-e2b4-4b83-a530-579a5a291a36"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("a01bf92f-f69d-4a01-9349-325a2d2a296b"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("a55878c9-9a74-42bb-85c4-fb27298730bf"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("a679f695-898d-4ba3-94ee-c963a6b38ee7"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("a6f84686-6ee2-432f-955f-fa8c570b210e"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("aadb8248-f18d-438e-bd9e-c33753e71fee"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("ab6bab02-cd95-4ac1-846f-c3acfcc39541"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("ac2b76d7-37c4-45af-a796-d8ecce065baf"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b43288c1-b3e1-413b-a0e2-fe52781ee1b0"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b5ef4ff0-97b8-4d08-8b3f-e215d8fb8f59"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b7b356f2-85c9-4fbe-ac86-7759610e3bc6"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("c34a1032-49f5-497e-aab7-891529ca7363"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("ccc455ce-0ccb-46fe-a729-371c6aa20b28"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("cddf81a7-c5f7-434d-870a-198d653b83dc"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("d3b6668f-63e4-40d9-b282-ead905076903"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("d69c72b1-7eda-4fc6-9bd8-5fcc308444d0"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("d7cb0c26-e1f8-43e9-ba4f-6449789535d6"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("da0a09cb-6003-4879-83e6-24876d3ca863"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("eb96e5ce-5dfd-4f7b-ae00-66ecefa3c4bb"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("f797b251-e55b-4077-99cf-4f4268788754"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("f938909d-7b69-473b-93a1-4b2b171a2fd5"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("f9ba7739-b55c-49de-8beb-ccc3d08beba8"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("fe29778c-4c72-46c1-aaa8-89efb10b6059"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("3dc91a3c-402b-45b5-80a5-b96c75e24391"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("fe785ac2-09af-4cb0-a679-030310809bee"));
        }
    }
}
