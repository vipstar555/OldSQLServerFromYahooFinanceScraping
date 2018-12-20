using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceEntityFramework.Finance;

namespace FinanceEntityFramework
{
    public class FinanceData
    {
        public CodeList codeList { get; set; }
        public Price price { get; set; }
        public TradeIndex tradeIndex { get; set; }
    }

    public class NewMain
    {
        static public void newMain()
        {
            int errorSleep = 1000;
            int sleepTime = 1000;
            List<FinanceData> financeDataList = new List<FinanceData>();

            for (int code = 1300; code <= 9999; code++)
            {
                try
                {
                    bool pageCheck;
                    var yahoo = new YahooFinance.YahooFinance(code, out pageCheck);
                    if (pageCheck == false)
                    {
                        Console.WriteLine("{0}　は存在しません。", code);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    var codeList = yahoo.MakeCodeList();
                    //codeListが存在しない場合（現状 空ページorETF）
                    if (codeList == null)
                    {
                        Console.WriteLine("{0}　はETFか存在しません。", code);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    //1301の日付が今日じゃない場合、飛ばす
                    if (codeList.date != DateTime.Now.Date && codeList.code == 1301)
                    {
                        Console.WriteLine("今日の日付ではありません 停止します（確認待ち）");
                        Console.ReadLine();
                        return;
                    }
                    //ファイナンスデータを入れる
                    FinanceData financeData = new FinanceData()
                    {
                        codeList = codeList
                    };

                    var price = yahoo.MakePrice(codeList);
                    if (price == null)
                    {
                        Console.WriteLine("{0}　はPrice情報がありません", code);
                        //ファイナンスデータを入れる
                        financeDataList.Add(financeData);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    //ファイナンスデータを入れる
                    financeData.price = price;

                    if (price.codeList.sector == "ETF")
                    {
                        Console.WriteLine("{0}　はETF、tradeIndex情報がありません", code);
                        //ファイナンスデータを入れる
                        financeDataList.Add(financeData);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }

                    var tradeIndex = yahoo.MakeTradeIndex(price); // ETFは取得しない
                    if (tradeIndex == null)
                    {
                        Console.WriteLine("{0}　はtradeIndex情報がありません", code);
                        //ファイナンスデータを入れる
                        financeDataList.Add(financeData);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    //ファイナンスデータを入れる
                    financeData.tradeIndex = tradeIndex;
                    //ファイナンスデータを入れる
                    financeDataList.Add(financeData);
                    Console.WriteLine("{0}　完了", code);
                    System.Threading.Thread.Sleep(sleepTime);

                }
                catch (Exception e)
                {
                    Console.WriteLine("10分待機 {0} に コード:{1} で {2}", DateTime.Now, code, e);
                    System.Threading.Thread.Sleep(600000);
                    code--;
                    continue;
                }
            }
         
            //ここにDB登録を書く
            using (var db = new CodeListDbContext())
            {
                List<CodeList> codeListList = db.CodeLists.ToList();

                foreach (var finance in financeDataList)
                {
                    var codeList = codeListList.Where(x => x.code == finance.codeList.code).FirstOrDefault();

                    if (codeList == null)
                    {
                        if(finance.tradeIndex != null)
                        {
                            db.TradeIndexs.Add(finance.tradeIndex);
                            continue;
                        }
                        if (finance.price != null)
                        {
                            db.Prices.Add(finance.price);
                            continue;
                        }
                        db.CodeLists.Add(finance.codeList);
                    }
                    codeList.date = finance.codeList.date;
                    finance.price.codeList = codeList;
                    if (finance.tradeIndex != null)
                    {
                        db.TradeIndexs.Add(finance.tradeIndex);
                        continue;
                    }
                    if (finance.price != null)
                    {
                        db.Prices.Add(finance.price);
                        continue;
                    }
                    db.CodeLists.Add(finance.codeList);
                }
                db.SaveChanges();
            }            
        }
    }
}
