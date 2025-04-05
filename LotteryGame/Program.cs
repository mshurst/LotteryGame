using LotteryGame;
using LotteryGame.Services;
using LotteryGame.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) => {
        services.AddScoped<ILotteryGameService, LotteryGameService>();
        services.AddScoped<IGameLogicService, GameLogicService>();
        services.AddSingleton<IRandomGenerator, RandomGenerator>();
        services.AddOptions<LotteryGameSettings>().Bind(configuration.GetSection("LotteryGameSettings")).ValidateDataAnnotations();
    })
    .UseSerilog()
    .Build();

await host.StartAsync();

var lotteryGameService = host.Services.GetRequiredService<ILotteryGameService>();
await lotteryGameService.PlayGame();
Console.ReadKey();


//tests to write
//validate input prompt in AnsiConsole
