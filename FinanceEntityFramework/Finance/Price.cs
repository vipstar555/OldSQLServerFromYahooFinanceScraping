using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceEntityFramework.Finance
{
    public class Price
    {
        public int id { get; set; }
        public virtual CodeList codeList { get; set; }
        public int code { get; set; }
        //ヘッダー
        public DateTime date { get; set; } //日付
        public double? lastClosePrice { get; set; }
        
        public double? closePrice { get; set; }
        public string closePriceDate { get; set; } //引け値日付
        //詳細情報
        public double? openPrice { get; set; }
        public string openPriceDate { get; set; } //引け値日付
        public double? highPrice { get; set; }
        public string highPriceDate { get; set; } //引け値日付
        public double? lowPrice { get; set; }
        public string lowPriceDate { get; set; } //引け値日付
        public long volume { get; set; }
        public long tradingVolume { get; set; }
        public double limitHighPrice { get; set; }
        public double limitLowPrice { get; set; }
        public virtual ICollection<TradeIndex> tradeIndex { get; set; }
    }
}
