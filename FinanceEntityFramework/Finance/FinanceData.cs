using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceEntityFramework.Finance
{
    public class FinanceData
    {
        public CodeList codeList { get; set; }
        public Price price { get; set; }
        public TradeIndex tradeIndex { get; set; }
    }
}
