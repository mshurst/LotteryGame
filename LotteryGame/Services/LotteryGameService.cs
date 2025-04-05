using LotteryGame.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services {
    public class LotteryGameService : ILotteryGameService {
        private readonly ILogger<ILotteryGameService> logger;
        private readonly IGameLogicService gameLogic;
        private readonly LotteryGameSettings lotteryGameSettings;

        public LotteryGameService(ILogger<ILotteryGameService> logger, IOptions<LotteryGameSettings> lotteryGameSettings, IGameLogicService gameLogic) {
            this.logger = logger;
            this.lotteryGameSettings = lotteryGameSettings.Value;
            this.gameLogic = gameLogic;
        }

        public async Task PlayGame() {
            logger.LogInformation("Doing stuff...");
            logger.LogInformation("Max ticket size {tickets}", lotteryGameSettings.CostPerTicket);

            AnsiConsole.WriteLine("Welcome to the lottery game, Player 1!");
            AnsiConsole.WriteLine("Your current balance is {0}", lotteryGameSettings.PlayerStartingBalance / 100.00);
            AnsiConsole.WriteLine("Ticket Price : {0}", lotteryGameSettings.CostPerTicket / 100.00);

            var playerTickets = AnsiConsole.Prompt(
                new TextPrompt<int>("How many tickets do you want to buy?")
                    .PromptStyle("green")
                    .Validate(tickets => tickets < lotteryGameSettings.MinNumberOfTicketsPerPlayer || tickets > lotteryGameSettings.MaxNumberOfTicketsPerPlayer ? ValidationResult.Error($"Please enter a number between {lotteryGameSettings.MinNumberOfTicketsPerPlayer} and {lotteryGameSettings.MaxNumberOfTicketsPerPlayer}") : ValidationResult.Success()));

            var numberOfCpuPlayers = gameLogic.GetNumberOfCpuPlayers();

            var player = new Player(1, playerTickets);

            var allPlayers = new List<Player> {
                player
            };

            for (int i = 0; i < numberOfCpuPlayers; i++) {
                allPlayers.Add(new Player(i + 2, gameLogic.GetRandomNumberOfTickets()));
            }

            AnsiConsole.WriteLine($"You have bought {playerTickets} tickets.");
            AnsiConsole.WriteLine($"There are {numberOfCpuPlayers} CPU players.");


            var result = gameLogic.GenerateResult(allPlayers);

            AnsiConsole.WriteLine(new string('*', 80));
            AnsiConsole.WriteLine("Results:");

            foreach (var prize in result.Prizes)
            {
                var winners = result.Winners[prize.Key];
                AnsiConsole.WriteLine("* {0}: Players {1} wins ${2} each",
                    prize.Key,
                    string.Join(",", winners.Distinct().Select(x => x)),
                    prize.Value/winners.Count/100.00);
            }

            AnsiConsole.WriteLine("Congratulations to the winners!");
            AnsiConsole.WriteLine("** House Share : ${0} **", result.HouseShare/100.00);
        }
    }
}
