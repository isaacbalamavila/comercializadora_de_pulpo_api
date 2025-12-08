using System.Text;
using Amazon.S3;
using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Repositories;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database Connection
builder.Services.AddDbContext<ComercializadoraDePulpoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AutoMapper Configuration
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var mapperConfig = new MapperConfiguration(
    cfg =>
    {
        cfg.AddProfile<AutoMapperProfile>();
    },
    loggerFactory
);

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//Ignore Cycle JSON References
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System
            .Text
            .Json
            .Serialization
            .ReferenceHandler
            .IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

//CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        }
    );
});

//Password Singleton
builder.Services.AddSingleton<Password>();
builder.Services.AddSingleton<S3Service>();

//JWT Singleton
builder.Services.AddSingleton<JWTHelper>();

// JWT Verification
builder
    .Services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(config =>
    {
        config.RequireHttpsMetadata = false;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!)
            ),
        };
    });

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<ISuppliersRepository, SuppliersRepository>();
builder.Services.AddScoped<IClientsRepository, ClientRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IRawMaterialsRepository, RawMaterialsRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchasesRepository>();
builder.Services.AddScoped<ISuppliesInventoryRepository, SuppliesInventoryRespository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISuppliersService, SuppliersService>();
builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IRawMaterialsService, RawMaterialsService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISuppliesInventoryService, SuppliesInventoryService>();

// Authorization Police
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOrManager",
        policy => policy.RequireRole("administrador", "supervisor")
    );

    options.AddPolicy("Admin", policy => policy.RequireRole("administrador"));

    options.AddPolicy("Employee", policy => policy.RequireRole("empleado general"));

    options.AddPolicy("Manager", policy => policy.RequireRole("gerente"));

    options.AddPolicy("Warehouse", policy => policy.RequireRole("almacenista"));
});

//AWS Services
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
