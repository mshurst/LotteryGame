namespace LotteryGame.Shared {
    public class GameResult {
        public Dictionary<string, List<int>> Winners = new Dictionary<string, List<int>>();
        public Dictionary<string, double> Prizes = new Dictionary<string, double>();
        public double HouseShare { get; set; }
    }
}
