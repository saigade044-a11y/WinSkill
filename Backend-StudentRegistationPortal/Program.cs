using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UseCaseFinalSubmission.Database;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<Database1>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Main1"));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("Key"));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Please Enter Bearer [Space] and The Token Generated",
                Scheme = "Bearer",
                Name = "Authorization",
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                    {
                        Reference=new Microsoft.OpenApi.Models.OpenApiReference()
                        {
                            Type=Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        var app = builder.Build();

        // -----------------------------------------------
        // ✅ MIGRATION MODE (for Kubernetes Job)
        // -----------------------------------------------
        if (args.Contains("--migrate"))
        {
            Console.WriteLine("✔ Running EF Core migrations...");

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Database1>();

            db.Database.Migrate();

            Console.WriteLine("✔ Migration Completed. Exiting...");
            return; // 🔥 exit without starting the API
        }
        // -----------------------------------------------

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
