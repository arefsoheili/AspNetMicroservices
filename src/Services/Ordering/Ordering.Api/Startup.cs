using MassTransit;
using Microsoft.OpenApi.Models;
using Ordering.Api.Consumers;
using Ordering.Api.Middleware;
using Ordering.Application;
using Ordering.Infrastructure;

namespace Ordering.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(Configuration);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BasketCheckoutConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(Configuration["EventBusSettings:Host"], "/", h =>
                {
                    h.Username(Configuration["EventBusSettings:UserName"] ?? "guest");
                    h.Password(Configuration["EventBusSettings:Password"] ?? "guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.Api", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.Api v1"));
        }

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
