using Microsoft.OpenApi.Models;
using Play.Catelog.Service.Entities;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Common.MassTransit;

public class Startup
{
    private ServiceSettings serviceSettings;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

        // calls two extenstion methods
        services.AddMongo()
            .AddMongoRepository<Item>("items")
            .AddMassTransitWithRabbitMq();

        // services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen();

        
        // at runtime ASP.NET will remove the async suffix from our methods
        // this will cause our 'created at action' method (in POST) to throw an error
        // fix this by suppressing this action
        services.AddControllers(options =>
            options.SuppressAsyncSuffixInActionNames = false
        );
        // services.AddRazorPages();

        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Play.Catalogue.Service", Version = "v1"});
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseStaticFiles();
    }
}