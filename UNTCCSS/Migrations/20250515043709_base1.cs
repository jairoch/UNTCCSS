using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UNTCCSS.Migrations
{
    /// <inheritdoc />
    public partial class base1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RUC = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dni = table.Column<string>(type: "text", nullable: false),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    ImagenPerfil = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resolucion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Documentacion = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    AtCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resolucion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Temario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: true),
                    PerfilId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Perfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: true),
                    PermisosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolPermisos_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisosId",
                        column: x => x.PermisosId,
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Curso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdTemario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Curso_Temario_IdTemario",
                        column: x => x.IdTemario,
                        principalTable: "Temario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DNI = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    AtCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estudiantes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Certificado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoDocumento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdResolucion = table.Column<int>(type: "integer", nullable: false),
                    PromedioFinal = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Registro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CursoId = table.Column<int>(type: "integer", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaTermino = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    Archivo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificado_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certificado_Curso_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Curso",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certificado_Estudiantes_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certificado_Resolucion_IdResolucion",
                        column: x => x.IdResolucion,
                        principalTable: "Resolucion",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Descripcion", "Name", "NormalizedName", "Status" },
                values: new object[,]
                {
                    { "1", "6cda19c4-1f1e-4a54-9b9d-9e445a67b4e3", "Rol con acceso total al sistema", "Admin", "ADMIN", true },
                    { "2", "7f5a19c4-1f1e-4a54-9b9d-9e445a67b4e3", "Rol con permisos de administración de usuarios dentro de su empresa", "Empresa", "EMPRESA", true },
                    { "3", "d7a45bd1-edfd-444f-bb3f-dd61eef2eb8b", "Rol con permisos para registrar certificados", "Registrador", "REGISTRADOR", true },
                    { "4", "3f2c9b1d-9c4f-4e5b-b2c8-2e1d2a9c3d45", "Rol con permisos mínimos del sistema", "Predeterminado", "PREDETERMINADO", true }
                });

            migrationBuilder.InsertData(
                table: "Empresa",
                columns: new[] { "Id", "Direccion", "Nombre", "RUC", "RazonSocial", "Telefono" },
                values: new object[,]
                {
                    { 1, "Calle Ficticia 123", "Mas Saben Perú", "12345678901", "Empresa Demo", "987654321" },
                    { 2, "Av. Tecnológica 456", "Tech Solutions", "98765432109", "Tech Solutions SAC", "912345678" },
                    { 3, "Jr. Comercio 789", "Comercial Andina", "10293847561", "Comercial Andina EIRL", "923456789" },
                    { 4, "Calle Innovación 321", "Innova Corp", "56473829104", "Innovaciones Empresariales SAC", "934567890" },
                    { 5, "Av. Aduanas 654", "Global Import", "37482910562", "Global Importaciones SRL", "945678901" },
                    { 6, "Pasaje Verde 987", "EcoVerde", "91827364509", "Soluciones Ecológicas EIRL", "956789012" }
                });

            migrationBuilder.InsertData(
                table: "Perfil",
                columns: new[] { "Id", "Apellidos", "Direccion", "Dni", "Email", "ImagenPerfil", "Nombres", "Telefono" },
                values: new object[] { 1, "Chingo", "Los Tulipanes 465", "12345678", "jairochingo@outlook.com", null, "Jairo", 967607828 });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Descripcion", "Name", "Status" },
                values: new object[,]
                {
                    { 1, "Permite gestionar usuarios dentro de la empresa actual, acciones como agregar nuevo, eliminar, suspender y asignar roles.", "GestionarUsuarios", true },
                    { 2, "Permite crear, editar y eliminar roles y sus permisos.", "GestionarRoles", true },
                    { 3, "Permite ver certificados emitidos por la empresa a la que pertenece el usuario.", "VerCertificados", true },
                    { 4, "Permite ver los usuarios de la misma empresa del usuario actual.", "VerUsuarios", true },
                    { 5, "Permite ver la información de las empresas registradas.", "VerEmpresas", true },
                    { 6, "Permite acceder a la configuración del sistema.", "VerConfiguracion", true },
                    { 7, "Permite a un administrador ver certificados emitidos de todas las empresas.", "VerCertificadosAdmin", true },
                    { 8, "Permite a un administrador ver todos los usuarios del sistema.", "VerUsuariosAdmin", true },
                    { 9, "Permite crear certificados para los estudiantes.", "CrearCertificados", true },
                    { 10, "Permite crear nuevos usuarios dentro de la empresa a la que se le ha asignado.", "CrearUsuarios", true },
                    { 11, "Permite crear nuevos usuarios dentro de cualquier empresa.", "CrearUsuariosAdmin", true },
                    { 12, "Permite editar certificados previamente emitidos.", "EditarCertificados", true },
                    { 13, "Permite eliminar certificados emitidos.", "EliminarCertificados", true },
                    { 14, "Permite eliminar a los estudiantes y sus certificados.", "EliminarEstudiantes", true },
                    { 15, "Permite activar la consulta de los certificados expedidos a los clientes.", "ActivarVerificación", true },
                    { 16, "Permite pausar la consulta de los certificados expedidos a los clientes hasta que se vuelva a activar manualmente.", "PausarVerificación", true }
                });

            migrationBuilder.InsertData(
                table: "Resolucion",
                columns: new[] { "Id", "AtCreated", "Documentacion", "Name", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "", "N° 0001-A-2024-DUPG/CC.SS", true },
                    { 2, new DateTime(2025, 2, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "", "N° 0001-A-2022-DUPG/CC.SS", true }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IdEmpresa", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PerfilId", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0989809e-e4ee-4852-9abb-6d12abf5e781", 0, "ad877341-f227-4a43-9920-11bb53273269", "jairochingo@outlook.com", true, 1, false, false, null, "JAIROCHINGO@OUTLOOK.COM", "JAIRO", "AQAAAAIAAYagAAAAENjRHZEDEydSd2vpCMAkG9OUib8ydH98lT4rf1bziLvYOOqQ9oCP6C4BBp7SCALRKQ==", 1, "967607828", false, "6cda19c4-1f1e-4a54-9b9d-9e445a67b4e3", false, "Jairo" });

            migrationBuilder.InsertData(
                table: "RolPermisos",
                columns: new[] { "Id", "PermisosId", "RoleId" },
                values: new object[,]
                {
                    { 1, 2, "1" },
                    { 2, 5, "1" },
                    { 3, 6, "1" },
                    { 4, 7, "1" },
                    { 5, 8, "1" },
                    { 6, 11, "1" },
                    { 7, 3, "2" },
                    { 8, 4, "2" },
                    { 9, 9, "2" },
                    { 10, 10, "2" },
                    { 11, 12, "2" },
                    { 12, 13, "2" },
                    { 13, 14, "2" },
                    { 14, 15, "2" },
                    { 15, 16, "2" },
                    { 16, 3, "3" },
                    { 17, 9, "3" },
                    { 18, 12, "3" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "0989809e-e4ee-4852-9abb-6d12abf5e781" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdEmpresa",
                table: "AspNetUsers",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PerfilId",
                table: "AspNetUsers",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_AlumnoId",
                table: "Certificado",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_CursoId",
                table: "Certificado",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_IdResolucion",
                table: "Certificado",
                column: "IdResolucion");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_UserId",
                table: "Certificado",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Curso_IdTemario",
                table: "Curso",
                column: "IdTemario");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_UserId",
                table: "Estudiantes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_PermisosId",
                table: "RolPermisos",
                column: "PermisosId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_RoleId",
                table: "RolPermisos",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Certificado");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "Curso");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Resolucion");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Temario");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropTable(
                name: "Perfil");
        }
    }
}
