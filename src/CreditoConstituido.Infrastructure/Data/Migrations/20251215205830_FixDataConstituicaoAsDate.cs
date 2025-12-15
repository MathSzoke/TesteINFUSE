using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditoConstituido.Infrastructure.Data.Migrations
{
    public partial class FixDataConstituicaoAsDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "data_constituicao",
                table: "credito",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_constituicao",
                table: "credito",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
