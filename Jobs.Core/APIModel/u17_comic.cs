using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class u17_comic
    {
        public int code { get; set; }
        public List<u17Comic_List> comic_list { get; set; }
        public string page_total { get; set; }
    }

    public class u17Comic_List
    {
        public string comic_id { get; set; }
        public string name { get; set; }
        public string cover { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
    }

}
