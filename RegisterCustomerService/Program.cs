
using Microsoft.EntityFrameworkCore;
using RegisterCustomerService.Data;
using RegisterCustomerService.Middleware;
using RegisterCustomerService.Proxy.AuthProxy;
using SharedAuth.Extensions;
using SharedAuth.Handlers;

namespace RegisterCustomerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSharedAuth(builder.Configuration);
            builder.Services.AddCustomSwagger(builder.Configuration);
            builder.Services.AddJwtAuthenticationFromConfig(builder.Configuration);
            builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    // use camelCase and ignore nulls for cleaner JSON payloads
                    opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    opts.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            // CORS policy for browser clients (adjust origins in production)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            builder.Services.AddHttpClient<IAuthProxyService, AuthProxyService>(opt =>
            {
                opt.BaseAddress = new Uri(builder.Configuration["AuthService:BaseUrl"] ?? "http://localhost:5001");
            }).AddHttpMessageHandler<TokenDelegatingHandler>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure EF Core with Npgsql (PostgreSQL) and connection resiliency + pooling
            builder.Services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsql =>
                    npgsql.EnableRetryOnFailure(maxRetryCount: 3)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCustomSwagger();
            // Global exception handling
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("DefaultCors");

            app.UseAuthorization();

            app.MapControllers();

            // Apply EF Core migrations at startup (preferred over EnsureCreated for production scenarios)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
