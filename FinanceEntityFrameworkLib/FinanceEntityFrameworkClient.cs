using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceEntityFrameworkLib.Finance;

namespace FinanceEntityFrameworkLib
{
    static public class FinanceEntityFrameworkClient
    {
        static public List<TradeIndex> ReadTradeIndexs()
        {
            using (var db = new CodeListDbContext())
            {
                return db.TradeIndexs.Include("Price").ToList();
            }
        }
        static public List<Price> ReadPrices()
        {
            using (var db = new CodeListDbContext())
            {
                return db.Prices.Include("CodeList").ToList();
            }
        }
        static public List<CodeList> ReadCodeLists()
        {
            using (var db = new CodeListDbContext())
            {
                return db.CodeLists.ToList();
            }
        }
    }
}
