using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class buka_comic
    {
        public int ret { get; set; }
        public List<buka_comicItem> items { get; set; }
        public int hasnext { get; set; }
        public string title { get; set; }
    }

    public class buka_comicItem
    {
        public string mid { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string logos { get; set; }
        public string logodir { get; set; }
        public string logo { get; set; }
        public string lastchap { get; set; }
        public string finish { get; set; }
        public string rate { get; set; }
        public string type { get; set; }
        public string lastup { get; set; }
    }

}
