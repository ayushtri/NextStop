using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStopApp.Migrations
{
    /// <inheritdoc />
    public partial class DbForth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BusOperators_Email",
                table: "BusOperators",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusOperators_Email",
                table: "BusOperators");
        }
    }
}
