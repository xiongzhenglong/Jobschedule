using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class wangyi_chapter
    {
        public wangyiCatalog catalog { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
    }

    public class wangyiCatalog
    {
        public int level { get; set; }
        public bool leaf { get; set; }
        public List<wangyiSection> sections { get; set; }
        public string bookId { get; set; }
    }

    public class wangyiSection
    {
        public string bookId { get; set; }
        public string sectionId { get; set; }
        public int level { get; set; }
        public string title { get; set; }
        public int needPay { get; set; }
        public int price { get; set; }
        public int wordCount { get; set; }
        public bool leaf { get; set; }
        public bool paied { get; set; }
        public string titleOrder { get; set; }
        public string titleText { get; set; }
        public string fullTitle { get; set; }
        public string orderDecorate { get; set; }
        public string orderTextDecorate { get; set; }
        public List<wangyiSection1> sections { get; set; }
        public bool _new { get; set; }
    }

    public class wangyiSection1
    {
        public string bookId { get; set; }
        public string sectionId { get; set; }
        public int level { get; set; }
        public string title { get; set; }
        public int needPay { get; set; }
        public int price { get; set; }
        public int wordCount { get; set; }
        public bool leaf { get; set; }
        public bool paied { get; set; }
        public string titleOrder { get; set; }
        public string titleText { get; set; }
        public string fullTitle { get; set; }
        public string orderDecorate { get; set; }
        public string orderTextDecorate { get; set; }
        public bool _new { get; set; }
    }

}
