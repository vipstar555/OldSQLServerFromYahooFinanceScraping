using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceEntityFramework.Finance
{
    public class CodeList
    {
        //コード
        public int id { get; set; }
        public int code { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string sector { get; set; }
        public DateTime date { get; set; }
        ICollection<Price> prices { get; set; }
    }
}
