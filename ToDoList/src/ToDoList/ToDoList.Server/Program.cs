using Serilog;
using ToDoList.Server.Configurations;
using ToDoList.Server.EndPoints;
using ToDoList.Server.Filters;
using ToDoList.Server.Middlewares;

namespace ToDoList.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.ConfigureSerilog();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ApiExceptionFilterAttribute>();
            //options.Filters.Add<ToDoListCountHeaderFilter>();
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Configure();
        builder.Configuration();
        builder.ConfigurationJwtAuth();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });


        var app = builder.Build();
        app.UseGlobalExceptionHandling();
        app.UseMiddleware<RequestDurationMiddleware>();

        //app.Use(async (context, next) =>
        //{

        //    await next();
        //});

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowAll");


        app.MapControllers();

        app.MapUserEndpoints();

        app.Run();
    }
}
