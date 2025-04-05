namespace LotteryGame.Shared {
    public class Player {
        public int PlayerNumber { get; set; }
        public int NumberOfTickets { get; set; }

        public Player(int playerNumber, int numberOfTickets) {
            PlayerNumber = playerNumber;
            NumberOfTickets = numberOfTickets;
        }
    }
}
