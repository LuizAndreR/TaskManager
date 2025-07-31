using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoColunaStatusPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tarefas",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "Pendente",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Not Started");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "Tarefas",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "Média",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Moderate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Not Started",
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldDefaultValue: "Pendente");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Moderate",
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldDefaultValue: "Média");
        }
    }
}
