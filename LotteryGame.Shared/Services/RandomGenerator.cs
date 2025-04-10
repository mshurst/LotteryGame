﻿using LotteryGame.Shared.Services.Interfaces;

namespace LotteryGame.Shared.Services {
    public class RandomGenerator : IRandomGenerator {
        private static Random random = new Random();

        public int GetRandomNumber(int min, int max) {
            return random.Next(min, max + 1);
        }
    }
}
