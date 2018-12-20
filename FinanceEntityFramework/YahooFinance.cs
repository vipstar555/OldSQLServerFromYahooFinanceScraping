using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using FinanceEntityFramework.Finance;
using System.Text.RegularExpressions;

namespace YahooFinance
{
    class YahooFinance
    {
        string htmlText;
        // HtmlDocumentオブジェクトを構築する
        HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
        int code;
        DateTime date;

        public YahooFinance(int code, out bool pageCheck)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using(var wc = new WebClient() )
            {
                this.code = code;
                try
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    htmlText = wc.DownloadString(string.Format("https://stocks.finance.yahoo.co.jp/stocks/detail/?code={0}.T", code));
                    htmlDoc.LoadHtml(htmlText);
                    //値幅制限の日付を取得
                    //string stringDate = htmlDoc.DocumentNode.SelectSingleNode(@"/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[7]/dl[1]/dd[1]/span[1]").InnerText;
                    var dateNodes = htmlDoc.DocumentNode.SelectNodes(@"//div//span[@class='date yjSt']");
                    string stringDate = dateNodes[6].InnerText;
                    stringDate = Regex.Replace(stringDate, "（", "");
                    stringDate = Regex.Replace(stringDate, "）", "");
                    if(DateTime.TryParse(stringDate, out date))
                    {
                        DateTime today = DateTime.Now;
                        if (today.Month < date.Month )
                        {
                            date.AddYears(-1);
                        }

                        pageCheck = true;
                    }
                    else
                    {
                        pageCheck = false;
                        return;
                    }

                }
                catch (Exception e)
                {
                    if (e is WebException)
                    {
                        Console.WriteLine("コード：{0}　で{1}", code, e);
                    }
                    //取り扱ってる証券コードが無い場合
                    if (e is NullReferenceException)
                    {
                        Console.WriteLine(e);
                        pageCheck = false;
                        return;
                    }
                    throw;                                        
                }
            }
        }

        public CodeList MakeCodeList()
        {
            try
            {
                CodeList codeList = new CodeList()
                {
                    code = this.code,
                    market = htmlDoc.DocumentNode.SelectSingleNode(@"//span[@class='stockMainTabName']").InnerText,        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[3]/div[1]/div[1]/span[1]"
                    name = htmlDoc.DocumentNode.SelectSingleNode(@"//th[@class='symbol']/h1").InnerText,          // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[5]/div[1]/div[2]/table[1]/tr[1]/th[1]/h1[1]/#text[1]"
                    sector = htmlDoc.DocumentNode.SelectSingleNode(@"//dd[@class='category yjSb']").InnerText,         // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[5]/div[1]/div[2]/dl[1]/dd[1]/a[1]"
                    date = this.date 
                };
                return codeList;
            }
            catch (Exception e)
            {
                if (e is NullReferenceException)
                {
                    return null;
                }

                throw;
            }
            
        }
        public Price MakePrice(CodeList codeList)
        {
            string[] limitArray = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[7]/dl/dd/strong").InnerText.Split('～');
            
            Price price = new Price()
            {
                codeList = codeList,
                code = this.code,
                lastClosePrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[1]/dl/dd/strong").InnerText),

                closePrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//td[@class='stoksPrice']").InnerText), //"/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[5]/div[1]/div[2]/table[1]/tr[1]/td[2]"　※, ,が入ってる
                closePriceDate = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[6]/dl/dd/span").InnerText,

                date = this.date,
                openPrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[2]/dl/dd/strong").InnerText), // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[2]/dl[1]/dd[1]/strong[1]"　※, ,が入ってる
                openPriceDate = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[2]/dl/dd/span").InnerText,

                highPrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[3]/dl/dd/strong").InnerText), // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[3]/dl[1]/dd[1]/strong[1]"　※, ,が入ってる
                highPriceDate = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[3]/dl/dd/span").InnerText,

                lowPrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[4]/dl/dd/strong").InnerText),          // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[4]/dl[1]/dd[1]/strong[1]"　※, ,が入ってる
                lowPriceDate = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[4]/dl/dd/span").InnerText,

                tradingVolume = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[6]/dl/dd/strong").InnerText) * 1000, // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[6]/dl[1]/dd[1]/strong[1]" ※, ,が入ってる
                volume = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@class='innerDate']/div[5]/dl/dd/strong").InnerText),// "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[5]/dl[1]/dd[1]/strong[1]"　※, ,が入ってる
                limitHighPrice = (double)doublePrice(limitArray[1]),                         // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[7]/dl[1]/dd[1]/strong[1]" ※　〇～〇
                limitLowPrice = (double)doublePrice(limitArray[0])                           // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[6]/div[2]/div[7]/dl[1]/dd[1]/strong[1]" ※　〇～〇
                
            };
            return price;
        }
        public TradeIndex MakeTradeIndex(Price price)
        {
            TradeIndex tradeIndex = new TradeIndex()
            {
                price = price,
                code = this.code,
                date = this.date,

                marketCapitalization = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[1]/dl/dd/strong").InnerText) * 1000000,　     // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[1]/dl[1]/dd[1]/strong[1]" (百万円)
                outstandingShares = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[2]/dl/dd/strong").InnerText),          // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[2]/dl[1]/dd[1]/strong[1]"
                
                //dividendYield = doublePrice(htmlDoc.DocumentNode.SelectSingleNode(           "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[3]/dl[1]/dd[1]/strong[1]").InnerText),              // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[3]/dl[1]/dd[1]/strong[1]"
                //DPS = doublePrice(htmlDoc.DocumentNode.SelectSingleNode(                     "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[4]/dl[1]/dd[1]/strong[1]/a[1]").InnerText),                        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[4]/dl[1]/dd[1]/strong[1]/a[1]"
                //PER = doublePrice(PER[1]),                        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[5]/dl[1]/dd[1]/strong[1]" ※（連）
                //PERrenrtan = PER[0],                // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[5]/dl[1]/dd[1]/strong[1]" ※（連）
                //PBR = 0,                        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[6]/dl[1]/dd[1]/strong[1]"
                //PBRrenrtan = "",                // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[6]/dl[1]/dd[1]/strong[1]"
                //EPS = 0,                        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[7]/dl[1]/dd[1]/strong[1]/a[1]"
                //EPSrenrtan = "",                // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[7]/dl[1]/dd[1]/strong[1]/a[1]"
                //BPS = 0,                        // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[8]/dl[1]/dd[1]/strong[1]/a[1]"
                //BPSrenrtan = "",                // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[8]/dl[1]/dd[1]/strong[1]/a[1]"

                minimumPrice = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[9]/dl/dd/strong").InnerText),               // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[9]/dl[1]/dd[1]/strong[1]"
                minimumUnit = intPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[10]/dl/dd/strong").InnerText),                // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[10]/dl[1]/dd[1]/strong[1]"
                yearHighPrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[11]/dl/dd/strong").InnerText),              // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[11]/dl[1]/dd[1]/strong[1]"
                yearLowPrice = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='rfindex']/div[2]/div[12]/dl/dd/strong").InnerText),               // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[9]/div[2]/div[2]/div[12]/dl[1]/dd[1]/strong[1]"

                marginBuy = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='margin']/div[1]/div[1]/div[1]/dl/dd/strong").InnerText),                  // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[11]/div[1]/div[1]/div[1]/dl[1]/dd[1]/strong[1]"
                WoWMarginBuy = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='margin']/div[1]/div[1]/div[2]/dl/dd/strong").InnerText),               // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[11]/div[1]/div[1]/div[2]/dl[1]/dd[1]/strong[1]"
                marginCell = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='margin']/div[1]/div[2]/div[1]/dl/dd/strong").InnerText),                 // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[11]/div[1]/div[2]/div[1]/dl[1]/dd[1]/strong[1]"
                WoWMarginCell = longPrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='margin']/div[1]/div[2]/div[2]/dl/dd/strong").InnerText),              // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[11]/div[1]/div[2]/div[2]/dl[1]/dd[1]/strong[1]"
                ratioMarginBalance = doublePrice(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='margin']/div[1]/div[1]/div[3]/dl/dd/strong").InnerText)          // "/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/div[11]/div[1]/div[1]/div[3]/dl[1]/dd[1]/strong[1]"
            };
            return tradeIndex;
        }

        private int intPrice(string priceData)
        {
            if (priceData == "---")
            {
                return 0;
            }

            string replacePriceData = Regex.Replace(priceData, ",", "");
            return int.Parse(replacePriceData);
        }

        private double? doublePrice(string priceData)
        {
            if (priceData == "---")
            {
                return null;
            }

            string replacePriceData = Regex.Replace(priceData, ",", "");
            return double.Parse(replacePriceData);
        }

        private long longPrice(string priceData)
        {
            if (priceData == "---")
            {
                return 0;
            }
            string replacePriceData = Regex.Replace(priceData, ",", "");
            return long.Parse(replacePriceData);
        }


    }
}
