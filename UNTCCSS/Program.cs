using UNTCCSS.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blazored.Modal;
using UNTCCSS.Components.Account;
using UNTCCSS.Helper;
using UNTCCSS.Components.Modal.ModalServicios;
using UNTCCSS.Servicios.Email;
using UNTCCSS.Servicios.AWS;
using UNTCCSS.Servicios.HunterIO.IHunterIO;
using UNTCCSS.Repositorios;
using UNTCCSS.Data;
using UNTCCSS.Servicios.HunterIO;
using UNTCCSS.Servicios.JCASoftware;
using UNTCCSS.Repositorios.IRepositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

/*Cadena de Conexión*/
var connectionString = builder.Configuration.GetConnectionString("DataBase") ?? throw new InvalidOperationException("Connection string 'SQL' No econtrada.");
/*SQLServer
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString));*/

/*PostgresSQL*/
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
     options.UseNpgsql(connectionString));

/*Pagina de Errores solo Entorno Desarollo*/
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*Servicio de Identity*/
builder.Services.AddIdentity<ApplicationUser,ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<ApplicationRole>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

    options.AddPolicy("Dashboard", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "VerDashboard"));
    });

    options.AddPolicy("Usuarios", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "VerUsuarios") ||
            context.User.HasClaim("permission", "VerUsuariosAdmin"));
    });

    options.AddPolicy("Empresas", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "VerEmpresas"));
    });

    options.AddPolicy("Certificates", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "VerCertificados") ||
            context.User.HasClaim("permission", "VerCertificadosAdmin"));
    });

    options.AddPolicy("RegistrarCertificados", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "CrearCertificados"));
    });

    options.AddPolicy("Configuracion", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "VerConfiguracion"));
    });

    options.AddPolicy("Roles", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("permission", "GestionarRoles"));
    });

    options.AddPolicy("EliminarCertificados", policy =>
    {
        policy.RequireClaim("permission", "EliminarCertificados");
    });
});

builder.Services.AddBlazoredModal();
builder.Services.AddScoped<ConfirmacionServicio>();
builder.Services.AddTransient<IEmailSenderJCA, EmailSender>();
builder.Services.AddScoped<IAutenticacionRepositorio, AutenticacionRepositorio>();
builder.Services.AddScoped<IRoleRepositorio, RoleRepositorio>();
builder.Services.AddScoped<IHunterIOMaster, HunterIOMaster>();
builder.Services.AddScoped<IUsersRepositorio, UsersRepositorio>();
builder.Services.AddScoped<IEmpresaRepositorio, EmpresasRepositorio>();
builder.Services.AddScoped<IEstudianteRepositorio, EstudianteRepositorio>();
builder.Services.AddScoped<ICursoRepositorio, CursoRepositorio>();
builder.Services.AddScoped<IResolucionRepositorio, ResolucionRepositorio>();
builder.Services.AddScoped<ICertificadoRepositorio, CertificadoRepositorio>();
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddHttpClient<IJCARepositorio, JCARepositorio>(client =>
{
    client.BaseAddress = new Uri(ApiHelper.BaseUrl);
});


//builder.Environment.EnvironmentName = "Production";
//builder.WebHost.UseUrls("http://85.31.60.81:5208", "https://85.31.60.81:7032");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();
app.Run();
