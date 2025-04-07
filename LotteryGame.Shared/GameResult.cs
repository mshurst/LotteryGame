namespace LotteryGame.Shared {
    public class GameResult {
        public Dictionary<string, List<int>> Winners = new Dictionary<string, List<int>>();
        public Dictionary<string, int> Prizes = new Dictionary<string, int>();
        public int HouseShare { get; set; }
    }
}
