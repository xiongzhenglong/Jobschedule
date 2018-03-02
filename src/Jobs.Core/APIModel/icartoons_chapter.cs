using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class icartoons_chapter
    {
        public int record_count { get; set; }
        public List<icartoons_chapterItem> items { get; set; }
        public string serial_title { get; set; }
    }

    public class icartoons_chapterItem
    {
        public string title { get; set; }
        public string content_id { get; set; }
        public string set_id { get; set; }
        public int sortid { get; set; }
        public int is_free { get; set; }
        public string next_chapter_id { get; set; }
        public string last_chapter_id { get; set; }
    }

}
