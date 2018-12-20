using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FinanceEntityFramework.Finance;

namespace FinanceEntityFramework
{
    class OldMain
    {
        static void oldMain()
        {
            int errorSleep = 1000;
            int sleepTime = 1000;
            using (var db = new CodeListDbContext())
            {
                List<CodeList> codeListList = db.CodeLists.ToList();
                List<Price> priceList = db.Prices.ToList();
                List<TradeIndex> tradeIndexList = db.TradeIndexs.ToList();

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

                        //コードリストが存在するか確認する
                        CodeList dbCodeList;
                        if (codeListCheck(codeListList, codeList))
                        {
                            dbCodeList = db.CodeLists.Single(x => x.code == codeList.code);

                            dbCodeList.name = codeList.name;
                            dbCodeList.market = codeList.market;
                            dbCodeList.sector = codeList.sector;
                            dbCodeList.date = codeList.date;
                        }
                        else
                        {
                            db.CodeLists.Add(codeList);
                            dbCodeList = codeList;
                        }
                        //CodeList更新
                        db.SaveChanges();

                        var price = yahoo.MakePrice(dbCodeList);
                        if (price == null)
                        {
                            Console.WriteLine("{0}　はPrice情報がありません", code);
                            System.Threading.Thread.Sleep(errorSleep);
                            continue;
                        }

                        var updatePrice = priceCheck(priceList, price);
                        if (updatePrice == null)
                        {
                            updatePrice = price;
                            db.Prices.Add(updatePrice);
                        }
                        else
                        {
                            var tmpPrice = db.Prices.SingleOrDefault(x => x.id == updatePrice.id);
                            tmpPrice.lastClosePrice = price.lastClosePrice;
                            tmpPrice.closePrice = price.closePrice;
                            tmpPrice.closePriceDate = price.closePriceDate;
                            tmpPrice.highPrice = price.highPrice;
                            tmpPrice.highPriceDate = price.highPriceDate;

                            tmpPrice.limitHighPrice = price.limitHighPrice;
                            tmpPrice.limitLowPrice = price.limitLowPrice;
                            tmpPrice.lowPrice = price.lowPrice;
                            tmpPrice.lowPriceDate = price.lowPriceDate;
                            tmpPrice.openPrice = price.openPrice;
                            tmpPrice.openPriceDate = price.openPriceDate;
                            tmpPrice.tradingVolume = price.tradingVolume;
                            tmpPrice.volume = price.volume;

                            updatePrice = tmpPrice;
                        }

                        //Price更新
                        db.SaveChanges();

                        var tradeIndex = yahoo.MakeTradeIndex(updatePrice); // ETFは取得しない
                        if (tradeIndex == null)
                        {
                            Console.WriteLine("{0}　はtradeIndex情報がありません", code);
                            System.Threading.Thread.Sleep(errorSleep);
                            continue;
                        }

                        var updateTradeIndex = tradeindexCheck(tradeIndexList, tradeIndex);
                        if (updateTradeIndex == null)
                        {
                            updateTradeIndex = tradeIndex;
                            db.TradeIndexs.Add(updateTradeIndex);
                        }
                        else
                        {
                            var tmpTradeIndex = db.TradeIndexs.SingleOrDefault(x => x.id == updateTradeIndex.id);
                            tmpTradeIndex.marginBuy = tradeIndex.marginBuy;
                            tmpTradeIndex.marginCell = tradeIndex.marginCell;
                            tmpTradeIndex.marketCapitalization = tradeIndex.marketCapitalization;
                            tmpTradeIndex.minimumPrice = tradeIndex.minimumPrice;
                            tmpTradeIndex.minimumUnit = tradeIndex.minimumUnit;
                            tmpTradeIndex.outstandingShares = tradeIndex.outstandingShares;
                            tmpTradeIndex.price = tradeIndex.price;
                            tmpTradeIndex.ratioMarginBalance = tradeIndex.ratioMarginBalance;
                            tmpTradeIndex.WoWMarginBuy = tradeIndex.WoWMarginBuy;
                            tmpTradeIndex.WoWMarginCell = tradeIndex.WoWMarginCell;
                            tmpTradeIndex.yearHighPrice = tradeIndex.yearHighPrice;
                            tmpTradeIndex.yearLowPrice = tradeIndex.yearLowPrice;
                            updateTradeIndex = tmpTradeIndex;
                        }
                        //TradeIndex更新
                        db.SaveChanges();

                        Console.WriteLine("{0}　完了", code);
                        System.Threading.Thread.Sleep(sleepTime);
                    }
                    catch (System.Net.WebException e)
                    {
                        Console.WriteLine("10分待機 {0} に コード:{1} で {2}", DateTime.Now, code, e);
                        System.Threading.Thread.Sleep(600000);
                        code--;
                        continue;
                    }
                }
            }
        }

        static private bool codeListCheck(List<CodeList> codeListList, CodeList codeList)
        {
            int code = codeList.code;

            foreach(var checkList in codeListList)
            {
                if (code == checkList.code)
                {
                    return true;
                }
            }
            return false;
        }

        static Price priceCheck(List<Price> priceList, Price price)
        {
            foreach(var checkPrice in priceList)
            {
                if (checkPrice.date == price.date && checkPrice.code == price.code)
                {
                    return checkPrice;
                }
            }
            return null;
        }

        static TradeIndex tradeindexCheck(List<TradeIndex> tradeIndexList, TradeIndex tradeIndex)
        {
            foreach (var checkTradeIndex in tradeIndexList)
            {
                if (checkTradeIndex.date == tradeIndex.date && checkTradeIndex.code == tradeIndex.code)
                {
                    return checkTradeIndex;
                }
            }
            return null;
        }
    }
}
