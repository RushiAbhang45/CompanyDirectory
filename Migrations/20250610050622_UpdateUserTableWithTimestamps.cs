using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyDirectory.Migrations
{
    public partial class UpdateUserTableWithTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            // Drop Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            // Add new Id column as bigint with identity
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            // Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            // Drop Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            // Re-add Id as int with identity
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}

