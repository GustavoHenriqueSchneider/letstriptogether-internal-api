using LetsTripTogether.InternalApi.WebApi;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
        builder.ConfigureAppConfiguration(configBuilder => configBuilder.Build());
    })
    .Build()
    .RunAsync();