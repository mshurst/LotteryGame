﻿using LotteryGame.Shared.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LotteryGame.Shared.Services;

public class TicketService : ITicketService {
    private LotteryGameSettings gameSettings;

    public TicketService(IOptions<LotteryGameSettings> gameSettings) {
        this.gameSettings = gameSettings.Value;
    }

    public List<int> GetAllTicketsForGame(IEnumerable<Player> players) {
        //tickets are generated by repeating the player number for the number of tickets they have
        var tickets = players
            .SelectMany(x => Enumerable.Repeat(x.PlayerNumber, x.NumberOfTickets))
            .ToList();
        tickets.Shuffle();
        return tickets;
    }

    public bool ValidateCanAffordTickets(int numberOfTicketsToBuy) {
        return (numberOfTicketsToBuy * (long)gameSettings.CostPerTicket) <= gameSettings.PlayerStartingBalance;
    }

    public int AllocateMaxNumberOfTickets(int balance, int costPerTicket) {
        return balance / costPerTicket;
    }
}