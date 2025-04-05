using LotteryGame.Shared;
using LotteryGame.Shared.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace LotteryGame.Console;

public class ConsoleLotteryGame : ILotteryGame {
    private readonly ILogger<ConsoleLotteryGame> logger;
    private readonly IGameLogicService gameLogicService;
    private readonly LotteryGameSettings gameSettings;

    public ConsoleLotteryGame(ILogger<ConsoleLotteryGame> logger, IGameLogicService gameLogicService, IOptions<LotteryGameSettings> gameSettings) {
        this.logger = logger;
        this.gameLogicService = gameLogicService;
        this.gameSettings = gameSettings.Value;
    }
    public void PlayGame() {
        logger.LogInformation("Doing stuff...");
        logger.LogInformation("Max ticket size {tickets}", gameSettings.CostPerTicket);

        AnsiConsole.WriteLine("Welcome to the lottery game, Player 1!");
        AnsiConsole.WriteLine("Your current balance is {0}", gameSettings.PlayerStartingBalance / 100.00);
        AnsiConsole.WriteLine("Ticket Price : {0}", gameSettings.CostPerTicket / 100.00);

        var playerTickets = AnsiConsole.Prompt(
            new TextPrompt<int>("How many tickets do you want to buy?")
                .PromptStyle("green")
                .Validate(tickets => tickets < gameSettings.MinNumberOfTicketsPerPlayer || tickets > gameSettings.MaxNumberOfTicketsPerPlayer ? ValidationResult.Error($"Please enter a number between {gameSettings.MinNumberOfTicketsPerPlayer} and {gameSettings.MaxNumberOfTicketsPerPlayer}") : ValidationResult.Success()));

        

        AnsiConsole.WriteLine($"You have bought {playerTickets} tickets.");
        AnsiConsole.WriteLine($"There are {gameLogicService.GetNumberOfCpuPlayers()} CPU players.");


        var result = gameLogicService.GenerateResult(playerTickets);

        AnsiConsole.WriteLine(new string('*', 80));
        AnsiConsole.WriteLine("Results:");

        foreach (var prize in result.Prizes) {
            var winners = result.Winners[prize.Key];
            AnsiConsole.WriteLine("* {0}: Players {1} wins ${2} each",
                prize.Key,
                string.Join(",", winners.Distinct().Select(x => x)),
                prize.Value / winners.Count / 100.00);
        }

        AnsiConsole.WriteLine("Congratulations to the winners!");
        AnsiConsole.WriteLine("** House Share : ${0} **", result.HouseShare / 100.00);
    }
}