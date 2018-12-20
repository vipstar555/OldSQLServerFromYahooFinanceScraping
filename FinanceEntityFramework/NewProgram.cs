using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FinanceEntityFramework.Finance;

namespace FinanceEntityFramework
{
    class NewProgram
    {
        static void NewMain()
        {
            int errorSleep = 1000;
            int sleepTime = 1000;
            List<CodeList> codeListList;
            List<Price> priceList;
            List<TradeIndex> tradeIndexList;

            List<FinanceData> financeDataList = new List<FinanceData>();

            using (var db = new CodeListDbContext())
            {
                codeListList = db.CodeLists.ToList();
                priceList = db.Prices.ToList();
                tradeIndexList = db.TradeIndexs.ToList();
            }

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
                    FinanceData financeData = new FinanceData();

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
                    //codeList追加
                    financeData.codeList = codeList;

                    //Price追加
                    var price = yahoo.MakePrice(codeList);
                    if (price == null)
                    {
                        Console.WriteLine("{0}　はPrice情報がありません", code);
                        financeDataList.Add(financeData);
                        ExportLog(financeData);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    financeData.price = price;

                    //TradeIndex追加
                    var tradeIndex = yahoo.MakeTradeIndex(price); // ETFは取得しない
                    if (tradeIndex == null)
                    {
                        Console.WriteLine("{0}　はtradeIndex情報がありません", code);
                        financeDataList.Add(financeData);
                        ExportLog(financeData);
                        System.Threading.Thread.Sleep(errorSleep);
                        continue;
                    }
                    financeData.tradeIndex = tradeIndex;
                }
                catch (System.Net.WebException e)
                {
                    Console.WriteLine("10分待機 {0} に コード:{1} で {2}", DateTime.Now, code, e);
                    System.Threading.Thread.Sleep(600000);
                    code--;
                    continue;
                }
                catch
                {
                    Console.WriteLine("{0}　で異常終了 確認待ち", code);
                    Console.ReadLine();
                }
            }
        }

        static private void ExportLog(FinanceData financeData)
        {

        }
    }
}
