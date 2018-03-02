using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class icartoons_comic
    {
        public int record_count { get; set; }
        public List<icartoonsItem> items { get; set; }
    }

    public class icartoonsItem
    {
        public string title { get; set; }
        public string desc { get; set; }
        public string cover { get; set; }
        public string serial_id { get; set; }
        public string views { get; set; }
        public string author { get; set; }
        public string userid { get; set; }
        public string photo { get; set; }
    }



    public class icartoons_comic_desc
    {
        public string title { get; set; }
        public string cover { get; set; }
        public string description { get; set; }
        public string views { get; set; }
        public string author { get; set; }
        public string state { get; set; }
        public int is_up { get; set; }
        public string theme { get; set; }
    }

}
