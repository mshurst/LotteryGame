using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame {
    public class Player {
        public int PlayerNumber { get; set; }
        public int NumberOfTickets { get; set; }

        public Player(int playerNumber, int numberOfTickets) {
            PlayerNumber = playerNumber;
            NumberOfTickets = numberOfTickets;
        }
    }
}
