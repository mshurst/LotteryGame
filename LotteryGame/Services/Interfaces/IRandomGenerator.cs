using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.Interfaces {
    public interface IRandomGenerator {
        int GetRandomNumber(int min, int max);
    }
}
