using Infrastructure;
using WebApi;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build();

await host.ApplyMigrationsAsync();
await host.RunAsync();