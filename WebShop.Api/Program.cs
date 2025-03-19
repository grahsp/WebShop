using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebShop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add services to the container.
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<Program>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["OAuth:Domain"];
                options.Audience = builder.Configuration["OAuth:Audience"];
            });

            builder.Services.AddAuthorization();

            // Configure the HTTP request pipeline.
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => "OK").RequireAuthorization();

            app.Run();
        }
    }
}
