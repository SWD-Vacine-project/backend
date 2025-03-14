using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Vaccine.API.Models.ChildModel;
using Vaccine.API.Models.CustomerModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;
using VNPAY.NET;
using static Vaccine.API.Controllers.AuthController;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen();

// Add VNPAY service to the container.
builder.Services.AddSingleton<IVnpay, Vnpay>();

//allow cros
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true)
              .WithExposedHeaders("Content-Disposition");
        });

});

builder.Services.AddSwaggerExamplesFromAssemblyOf<ExampleCreateCustomerModel>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ExampleRequestCreateChildModel>();

// Thêm Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SWD392- Vaccine", Version = "v1" });
    c.EnableAnnotations(); // Optional for better documentation
    c.ExampleFilters();    // Register example filters
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "Access your profile" },
                    { "email", "Access your email" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "openid", "profile", "email" }
        }
    });
});

// Register UnitOfWork
builder.Services.AddScoped<UnitOfWork>();

// DbContext
builder.Services.AddDbContext<VaccineDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vaccine Shop");
        c.OAuthClientId("1006543489483-mrg7qa1pas18ulb0hvnadiagh8jajghs.apps.googleusercontent.com"); // Thay YOUR_GOOGLE_CLIENT_ID bằng Client ID đã lấy từ Google
        c.OAuthClientSecret("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");
        c.OAuthUsePkce(); // Bật PKCE
    });
//}
app.UseCors("MyPolicy");

app.UseHttpsRedirection();
//app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();


app.Run();
