using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AteracaoNaTabelaTarefas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Tarefas",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Not Started",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Moderate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tarefas",
                newName: "Nome");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Tarefas",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Not Started");
        }
    }
}
