using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class wangyi_comic
    {
        public List<wangyi_Book> books { get; set; }
        public int code { get; set; }
        public wangyi_Pagequery pageQuery { get; set; }
        public string msg { get; set; }
    }



    public class wangyi_Pagequery
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int count { get; set; }
        public int limit { get; set; }
        public int pageCount { get; set; }
        public int offset { get; set; }
    }

   

    public class wangyi_Book
    {
        public string bookId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int subCount { get; set; }
        public string updateTime { get; set; }
        public int vipPrice { get; set; }
        public string author { get; set; }
        public int payType { get; set; }
        public int totalCount { get; set; }
        public int serialStatus { get; set; }
        public int commentCount { get; set; }
        public string shortIntro { get; set; }
        public int turnStyle { get; set; }
        public int readerType { get; set; }
        public int level { get; set; }
        public string announcement { get; set; }
        public string clickCount { get; set; }
        public int baoyueType { get; set; }
        public int tucaoCount { get; set; }
        public string latestPublishTime { get; set; }
        public int tucaoEnable { get; set; }
        public string pushupdatecount { get; set; }
        public int topCommentCount { get; set; }
        public int bestWorks { get; set; }
        public string publishTime { get; set; }
        public string yuepiaoCount { get; set; }
        public int area { get; set; }
        public int recommend { get; set; }
        public string fansValue { get; set; }
        public int spamLevel { get; set; }
        public int sectionCount { get; set; }
        public int imageCount { get; set; }
        public string latestSectionId { get; set; }
        public string latestSectionTitle { get; set; }
        public string latestSectionTitleOrder { get; set; }
        public string latestSectionTitleText { get; set; }
        public bool bookRecommend { get; set; }
        public string cover { get; set; }
        public string authorUserId { get; set; }
        public string latestSectionOrderDecorate { get; set; }
        public string latestSectionFullTitle { get; set; }
        public string latestSectionTitleOrderDecorate { get; set; }
        public string tagNames { get; set; }
        public string latestPublishTimeStr { get; set; }
        public string bookTypeStr { get; set; }
        public string latestSectionOrderTextDecorate { get; set; }
        public bool finished { get; set; }
        public string payTypeEnum { get; set; }
        public string bookStatus { get; set; }
        public bool statusNormal { get; set; }
        public bool statusDepend { get; set; }
    }

}
