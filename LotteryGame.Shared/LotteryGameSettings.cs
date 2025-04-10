﻿using System.ComponentModel.DataAnnotations;

namespace LotteryGame.Shared {
    public class LotteryGameSettings {
        [Range(1, int.MaxValue)]
        public int MinNumberOfPlayers { get; set; }
        [Range(1, int.MaxValue)]
        public int MaxNumberOfPlayers { get; set; }
        [Range(1, int.MaxValue)]
        public int MinNumberOfTicketsPerPlayer { get; set; }
        [Range(1, int.MaxValue)]
        public int MaxNumberOfTicketsPerPlayer { get; set; }
        [Range(1, int.MaxValue)]
        public int PlayerStartingBalance { get; set; }
        [Range(1, int.MaxValue)]
        public int CostPerTicket { get; set; }
        [Required]
        public List<PrizeSetting> PrizeSettings { get; set; }
        [Required]
        public string CurrencySymbol { get; set; }

    }

    public class PrizeSetting {
        [Required]
        public string Name { get; set; }
        [Required]
        public double PrizeShare { get; set; }
        public int? NumberOfWinningTickets { get; set; }
        public double? PercentageOfWinningTickets { get; set; }
    }
}
