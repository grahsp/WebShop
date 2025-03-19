namespace WebShop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add services to the container.
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configure the HTTP request pipeline.
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
