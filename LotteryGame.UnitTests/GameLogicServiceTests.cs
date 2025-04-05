using FluentAssertions;
using LotteryGame.Shared;
using LotteryGame.Shared.Services;
using LotteryGame.Shared.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LotteryGame.UnitTests {
    public class GameLogicServiceTests {
        private GameLogicService service;
        private Mock<ILogger<GameLogicService>> loggerMock = new Mock<ILogger<GameLogicService>>();
        private Mock<IRandomGenerator> randomGeneratorMock = new Mock<IRandomGenerator>();
        private Mock<ITicketService> ticketServiceMock = new Mock<ITicketService>();

        [Fact]
        public void GetNumberOfCpuPlayers_MinGreaterThanMax_ThrowsException() {
            // Arrange
            var settings = Options.Create(new LotteryGameSettings {
                MinNumberOfPlayers = 10,
                MaxNumberOfPlayers = 5
            });
            service = new GameLogicService(randomGeneratorMock.Object, settings, loggerMock.Object, ticketServiceMock.Object);
            // Act & Assert
            Assert.Throws<ApplicationException>(() => service.GetNumberOfCpuPlayers());
            randomGeneratorMock.Verify(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void GetNumberOfCpuPlayers_ValidMinAndMax_ReturnsCorrectValue() {
            // Arrange
            var settings = Options.Create(new LotteryGameSettings {
                MinNumberOfPlayers = 1,
                MaxNumberOfPlayers = 5
            });
            service = new GameLogicService(randomGeneratorMock.Object, settings, loggerMock.Object, ticketServiceMock.Object);
            randomGeneratorMock.Setup(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            // Act
            var result = service.GetNumberOfCpuPlayers();
            // Assert
            Assert.Equal(3, result);
            randomGeneratorMock.Verify(x => x.GetRandomNumber(1, 5), Times.Once);
        }

        [Fact]
        public void GetRandomNumberOfTickets_MinGreaterThanMax_ThrowsException() {
            // Arrange
            var settings = Options.Create(new LotteryGameSettings {
                MinNumberOfTicketsPerPlayer = 10,
                MaxNumberOfTicketsPerPlayer = 5
            });
            service = new GameLogicService(randomGeneratorMock.Object, settings, loggerMock.Object, ticketServiceMock.Object);
            // Act & Assert
            Assert.Throws<ApplicationException>(() => service.GetRandomNumberOfTickets());
            randomGeneratorMock.Verify(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void GetRandomNumberOfTickets_ValidMinAndMax_ReturnsCorrectValue() {
            // Arrange
            var settings = Options.Create(new LotteryGameSettings {
                MinNumberOfTicketsPerPlayer = 1,
                MaxNumberOfTicketsPerPlayer = 5
            });
            service = new GameLogicService(randomGeneratorMock.Object, settings, loggerMock.Object, ticketServiceMock.Object);
            randomGeneratorMock.Setup(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            // Act
            var result = service.GetRandomNumberOfTickets();
            // Assert
            Assert.Equal(3, result);
            randomGeneratorMock.Verify(x => x.GetRandomNumber(1, 5), Times.Once);
        }

        [Fact]
        public void GenerateResult_WithSinglePrizeWithNumberOfWinningTickets_ReturnsCorrectResult() {
            var settings = new LotteryGameSettings() {
                CostPerTicket = 1,
                PrizeSettings = new List<PrizeSetting>() {
                    new PrizeSetting() {
                        Name = "Grand prize",
                        NumberOfWinningTickets = 1,
                        PrizeShare = 0.5
                    }
                }
            };

            ticketServiceMock.Setup(x => x.GetAllTicketsForGame(It.IsAny<List<Player>>()))
                .Returns(new List<int>() {1, 1, 2, 2, 3, 3});
            randomGeneratorMock.Setup(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(2);

            service = new GameLogicService(randomGeneratorMock.Object, Options.Create(settings),
                loggerMock.Object, ticketServiceMock.Object);

            var result = service.GenerateResult(2);

            result.Winners.Should().ContainKey("Grand prize");
            result.Winners["Grand prize"].Should().HaveCount(1);
            result.Winners["Grand prize"].Should().Contain(1);
            result.Prizes.Should().ContainKey("Grand prize");
            result.Prizes["Grand prize"].Should().Be(3);
            result.HouseShare.Should().Be(3);
        }

        [Fact]
        public void GenerateResult_WithSinglePrizeWithPercentageOfWinningTickets_ReturnsCorrectResult() {
            var settings = new LotteryGameSettings() {
                CostPerTicket = 1,
                PrizeSettings = new List<PrizeSetting>() {
                    new PrizeSetting() {
                        Name = "Second prize",
                        PercentageOfWinningTickets = 0.1,
                        PrizeShare = 0.3
                    }
                },
                MinNumberOfPlayers = 3,
                MaxNumberOfPlayers = 3,
                MinNumberOfTicketsPerPlayer = 1,
                MaxNumberOfTicketsPerPlayer = 10
            };

            randomGeneratorMock.Setup(x => x.GetRandomNumber(settings.MinNumberOfPlayers, settings.MaxNumberOfPlayers))
                .Returns(3); //3 players
            randomGeneratorMock.Setup(x => x.GetRandomNumber(settings.MinNumberOfTicketsPerPlayer, settings.MaxNumberOfTicketsPerPlayer))
                .Returns(10); //10 tickets each

            ticketServiceMock.Setup(x => x.GetAllTicketsForGame(It.IsAny<List<Player>>()))
                .Returns(new List<int>() {
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3
                });

            service = new GameLogicService(randomGeneratorMock.Object, Options.Create(settings),
                loggerMock.Object, ticketServiceMock.Object);

            var result = service.GenerateResult(2);
            result.Winners.Should().ContainKey("Second prize");
            result.Winners["Second prize"].Should().HaveCount(3);
            result.Winners["Second prize"].Should().Contain(new List<int>() { 1, 2, 3 });
            result.Prizes.Should().ContainKey("Second prize");
            result.Prizes["Second prize"].Should().Be(9);
            result.HouseShare.Should().Be(21);
        }

        [Fact]
        public void GenerateResult_WithMultiplePrizes_ReturnsCorrectResult() {
            var settings = new LotteryGameSettings() {
                CostPerTicket = 1,
                PrizeSettings = new List<PrizeSetting>() {
                    new PrizeSetting() {
                        Name = "Grand prize",
                        NumberOfWinningTickets = 1,
                        PrizeShare = 0.5
                    },
                    new PrizeSetting() {
                        Name = "Second prize",
                        PercentageOfWinningTickets = 0.1,
                        PrizeShare = 0.3
                    }
                },
                MinNumberOfPlayers = 3,
                MaxNumberOfPlayers = 3,
                MinNumberOfTicketsPerPlayer = 1,
                MaxNumberOfTicketsPerPlayer = 10
            };


            randomGeneratorMock.Setup(x => x.GetRandomNumber(settings.MinNumberOfPlayers, settings.MaxNumberOfPlayers))
                .Returns(3); //3 players
            randomGeneratorMock.Setup(x => x.GetRandomNumber(settings.MinNumberOfTicketsPerPlayer, settings.MaxNumberOfTicketsPerPlayer))
                .Returns(10); //10 tickets each

            ticketServiceMock.Setup(x => x.GetAllTicketsForGame(It.IsAny<List<Player>>()))
                .Returns(new List<int>() {
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3,
                    1, 2, 3
                });

            service = new GameLogicService(randomGeneratorMock.Object, Options.Create(settings),
                loggerMock.Object, ticketServiceMock.Object);

            var result = service.GenerateResult(2);

            //todo finish this
        }
    }
}