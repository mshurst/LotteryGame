using LotteryGame.Console;
using LotteryGame.Shared;
using LotteryGame.Shared.Services;
using LotteryGame.Shared.Services.Interfaces;
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
        services.AddScoped<IGameLogicService, GameLogicService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddSingleton<IRandomGenerator, RandomGenerator>();
        services.AddScoped<ILotteryGame, ConsoleLotteryGame>();
        services.AddOptions<LotteryGameSettings>().Bind(configuration.GetSection("LotteryGameSettings")).ValidateDataAnnotations();
    })
    .UseSerilog()
    .Build();

await host.StartAsync();

var lotteryGameService = host.Services.GetRequiredService<ILotteryGame>();
lotteryGameService.PlayGame();
Console.ReadKey();