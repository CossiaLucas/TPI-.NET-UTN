using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "direccion",
                columns: table => new
                {
                    IdDireccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Piso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Departamento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_direccion", x => x.IdDireccion);
                });

            migrationBuilder.CreateTable(
                name: "metodoPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metodoPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_producto_categoria_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDireccion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuario_direccion_IdDireccion",
                        column: x => x.IdDireccion,
                        principalTable: "direccion",
                        principalColumn: "IdDireccion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "precio",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    FechaDesde = table.Column<DateTime>(type: "datetime", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precio", x => new { x.IdProducto, x.FechaDesde });
                    table.ForeignKey(
                        name: "FK_precio_producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carrito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carrito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_carrito_usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favoritos",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    FechaAgregado = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favoritos", x => new { x.IdUsuario, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_favoritos_producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favoritos_usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venta",
                columns: table => new
                {
                    NroVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdMetodoPago = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venta", x => x.NroVenta);
                    table.ForeignKey(
                        name: "FK_venta_metodoPago_IdMetodoPago",
                        column: x => x.IdMetodoPago,
                        principalTable: "metodoPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_venta_usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "itemCarrito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCarrito = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemCarrito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_itemCarrito_carrito_IdCarrito",
                        column: x => x.IdCarrito,
                        principalTable: "carrito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_itemCarrito_producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalleVenta",
                columns: table => new
                {
                    IdDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalleVenta", x => x.IdDetalle);
                    table.ForeignKey(
                        name: "FK_detalleVenta_producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_detalleVenta_venta_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "venta",
                        principalColumn: "NroVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carrito_IdUsuario",
                table: "carrito",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_Nombre",
                table: "categoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_detalleVenta_IdProducto",
                table: "detalleVenta",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_detalleVenta_IdVenta",
                table: "detalleVenta",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_favoritos_IdProducto",
                table: "favoritos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_itemCarrito_IdCarrito_IdProducto",
                table: "itemCarrito",
                columns: new[] { "IdCarrito", "IdProducto" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itemCarrito_IdProducto",
                table: "itemCarrito",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_metodoPago_Nombre",
                table: "metodoPago",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_IdCategoria",
                table: "producto",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Dni",
                table: "usuario",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Email",
                table: "usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_IdDireccion",
                table: "usuario",
                column: "IdDireccion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Username",
                table: "usuario",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_venta_IdMetodoPago",
                table: "venta",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_venta_IdUsuario",
                table: "venta",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalleVenta");

            migrationBuilder.DropTable(
                name: "favoritos");

            migrationBuilder.DropTable(
                name: "itemCarrito");

            migrationBuilder.DropTable(
                name: "precio");

            migrationBuilder.DropTable(
                name: "venta");

            migrationBuilder.DropTable(
                name: "carrito");

            migrationBuilder.DropTable(
                name: "producto");

            migrationBuilder.DropTable(
                name: "metodoPago");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "direccion");
        }
    }
}
