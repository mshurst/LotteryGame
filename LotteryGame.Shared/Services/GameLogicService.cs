using LotteryGame.Shared.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LotteryGame.Shared.Services {
    public class GameLogicService : IGameLogicService {
        private readonly IRandomGenerator randomNumberGenerator;
        private readonly LotteryGameSettings lotteryGameSettings;
        private readonly ILogger<GameLogicService> logger;
        private readonly ITicketService ticketService;

        public GameLogicService(IRandomGenerator randomNumberGenerator, IOptions<LotteryGameSettings> settings, ILogger<GameLogicService> logger, ITicketService ticketService) {
            this.randomNumberGenerator = randomNumberGenerator;
            this.logger = logger;
            this.ticketService = ticketService;
            this.lotteryGameSettings = settings.Value;
        }

        public GameResult GenerateResult(int numberOfTicketsForPlayer) {
            var numberOfCpuPlayers = GetNumberOfCpuPlayers();

            var player = new Player(1, numberOfTicketsForPlayer);

            var allPlayers = new List<Player> {
                player
            };

            for (int i = 0; i < numberOfCpuPlayers; i++) {
                allPlayers.Add(new Player(i + 2, GetRandomNumberOfTickets()));
            }
            var result = new GameResult();

            var prizes = lotteryGameSettings.PrizeSettings;

            var totalTickets = ticketService.GetAllTicketsForGame(allPlayers);

            //we need to save this value here as tickets are removed from the array when they are not eligible for future prizes
            var totalTicketCount = totalTickets.Count();

            var totalPrizePot = totalTickets.Count() * lotteryGameSettings.CostPerTicket;

            logger.LogDebug("Total ticket count: {totalTicketCount}", totalTicketCount);
            logger.LogDebug("Total prize pot: {totalPrizePot}", totalPrizePot);

            foreach (var prize in prizes) {
                var numberOfWinners = prize.NumberOfWinningTickets 
                                      ?? (int)Math.Ceiling((double)totalTicketCount * prize.PercentageOfWinningTickets.Value);

                var winningTickets = totalTickets.Take(numberOfWinners).ToList();

                totalTickets.RemoveRange(0, numberOfWinners);

                result.Winners.Add(prize.Name, winningTickets);
                var prizeAmount = (int)Math.Round(totalPrizePot * prize.PrizeShare);
                result.Prizes.Add(prize.Name, prizeAmount);

                logger.LogDebug("Prize {prizeName} - number of winners {winners} - prize each {prizeEach} - total prize {totalPrize}",
                    prize.Name, numberOfWinners, prizeAmount/numberOfWinners, prizeAmount);
            }

            result.HouseShare = totalPrizePot - result.Prizes.Sum(x => x.Value);

            logger.LogDebug("House share : {houseShare}", result.HouseShare);

            return result;
        }

        public int GetNumberOfCpuPlayers() {
            if (lotteryGameSettings.MinNumberOfPlayers > lotteryGameSettings.MaxNumberOfPlayers) {
                throw new ApplicationException("MinNumberOfPlayers cannot be greater than MaxNumberOfPlayers");
            }

            return randomNumberGenerator.GetRandomNumber(lotteryGameSettings.MinNumberOfPlayers-1, lotteryGameSettings.MaxNumberOfPlayers-1);
        }

        public int GetRandomNumberOfTickets() {
            if (lotteryGameSettings.MinNumberOfTicketsPerPlayer > lotteryGameSettings.MaxNumberOfTicketsPerPlayer) {
                throw new ApplicationException("MinNumberOfTicketsPerPlayer cannot be greater than MaxNumberOfTicketsPerPlayer");
            }
            
            return randomNumberGenerator.GetRandomNumber(lotteryGameSettings.MinNumberOfTicketsPerPlayer, lotteryGameSettings.MaxNumberOfTicketsPerPlayer);
        }
    }
}
