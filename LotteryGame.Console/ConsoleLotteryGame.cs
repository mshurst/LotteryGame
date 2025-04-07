using LotteryGame.Shared;
using LotteryGame.Shared.Services.Interfaces;
using LotteryGame.Shared.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace LotteryGame.Console;

public class ConsoleLotteryGame : ILotteryGame {
    private readonly ILogger<ConsoleLotteryGame> logger;
    private readonly IGameLogicService gameLogicService;
    private readonly ITicketService ticketService;
    private readonly LotteryGameSettings gameSettings;
    private readonly CurrencyHelper currencyHelper;

    public ConsoleLotteryGame(ILogger<ConsoleLotteryGame> logger, IGameLogicService gameLogicService, IOptions<LotteryGameSettings> gameSettings, CurrencyHelper currencyHelper, ITicketService ticketService) {
        this.logger = logger;
        this.gameLogicService = gameLogicService;
        this.currencyHelper = currencyHelper;
        this.ticketService = ticketService;
        this.gameSettings = gameSettings.Value;
    }
    public void PlayGame() {
        logger.LogInformation("Doing stuff...");
        logger.LogInformation("Max ticket size {tickets}", gameSettings.CostPerTicket);

        AnsiConsole.WriteLine("Welcome to the lottery game, Player 1!");
        AnsiConsole.WriteLine("Your current balance is {0}",currencyHelper.FormatCurrencyAsString(gameSettings.PlayerStartingBalance));
        AnsiConsole.WriteLine("Ticket Price : {0}", currencyHelper.FormatCurrencyAsString(gameSettings.CostPerTicket));
        AnsiConsole.WriteLine(Environment.NewLine);

        var playerTickets = AnsiConsole.Prompt(
            new TextPrompt<int>("How many tickets do you want to buy?")
                .PromptStyle("green")
                .Validate(tickets => tickets < gameSettings.MinNumberOfTicketsPerPlayer || tickets > gameSettings.MaxNumberOfTicketsPerPlayer ?
                    ValidationResult.Error($"Please enter a number between {gameSettings.MinNumberOfTicketsPerPlayer} and {gameSettings.MaxNumberOfTicketsPerPlayer}") :
                    ValidationResult.Success()));

        if (!ticketService.ValidateCanAffordTickets(playerTickets)) {
            int newTickets = ticketService.AllocateMaxNumberOfTickets(gameSettings.PlayerStartingBalance, gameSettings.CostPerTicket);
            AnsiConsole.WriteLine("You cannot afford to buy {0} tickets - you have purchased the maximum number available to you: {1}", playerTickets, newTickets);
            playerTickets = newTickets;
        }

        AnsiConsole.WriteLine($"You have bought {playerTickets} tickets.");
        AnsiConsole.WriteLine($"{gameLogicService.GetNumberOfCpuPlayers()} other CPU players also have purchased tickets.");

        var result = gameLogicService.GenerateResult(playerTickets);

        AnsiConsole.WriteLine(new string('*', 80));
        AnsiConsole.WriteLine("Results:" + Environment.NewLine);

        foreach (var prize in result.Prizes) {
            var winners = result.Winners[prize.Key];
            if (winners.Count > 1) {
                AnsiConsole.WriteLine("* {0}: Players {1} wins {2} each",
                    prize.Key,
                    string.Join(",", winners.Distinct().Select(x => x)),
                    currencyHelper.FormatCurrencyAsString(prize.Value / winners.Count));
            }
            else {
                AnsiConsole.WriteLine("* {0}: Player {1} wins {2}",
                    prize.Key,
                    winners.First(),
                    currencyHelper.FormatCurrencyAsString(prize.Value));
            }
            
        }

        AnsiConsole.WriteLine("Congratulations to the winners!");
        AnsiConsole.WriteLine("** House Share : {0} **", currencyHelper.FormatCurrencyAsString(result.HouseShare));
    }
}