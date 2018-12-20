using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceEntityFrameworkLib.Finance
{
    public class TradeIndex
    {
        public int id { get; set; }
        public virtual Price price { get; set; }
        public int code { get; set; }
        public DateTime date { get; set; } //日付
        //参考指標
        public long marketCapitalization { get; set; }
        public long outstandingShares { get; set; }
        //public double? dividendYield { get; set; }
        //public double? DPS { get; set; }
        //public double? PER { get; set; }
        //public string PERrenrtan { get; set; }
        //public double? PBR { get; set; }
        //public string PBRrenrtan { get; set; }
        //public double? EPS { get; set; }
        //public string EPSrenrtan { get; set; }
        //public double? BPS { get; set; }
        //public string BPSrenrtan { get; set; }
        public long minimumPrice { get; set; }
        public int minimumUnit { get; set; }
        public double? yearHighPrice { get; set; }
        public double? yearLowPrice { get; set; }
        //信用取引情報
        public long marginBuy { get; set; }
        public long WoWMarginBuy { get; set; }
        public long marginCell { get; set; }
        public long WoWMarginCell { get; set; }
        public double? ratioMarginBalance { get; set; }
    }
}
