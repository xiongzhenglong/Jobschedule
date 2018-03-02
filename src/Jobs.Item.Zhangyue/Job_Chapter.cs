using HtmlAgilityPack;
using Jobs.Core.APIModel;
using Jobs.Core.Business.Manager;
using Jobs.Core.Common;
using Jobs.Core.Common.Extension;
using Jobs.Core.Entity;
using Jobs.Core.Services;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jobs.Item.Zhangyue
{
    [DisallowConcurrentExecution]
    public sealed class Job_Chapter : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Chapter));
        private BaseManager<Chapter> chaptermanager = new BaseManager<Chapter>();
        private BaseManager<Comic> comicmanager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            string date = DateTime.Now.ToDateStr();
            List<Comic> clst = comicmanager.Query(x => x.source == 11 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();

            foreach (var c in clst)
            {
                try
                {
                    HttpWebHelper _helper = new HttpWebHelper();
                    var bookdata = _helper.Get(c.bookurl, Encoding.GetEncoding("utf-8"));
                    List<Chapter> chapterlst = new List<Chapter>();
                    Regex regp1 = new Regex("var maxChapterId = \"(?<key1>.*?)\";");
                    Match matchp1 = regp1.Match(bookdata);
                    string pcountstr = matchp1.Groups["key1"].Value;

                    Regex regp2 = new Regex("<title>(?<key1>.*?)</title>");
                    Match matchp2 = regp2.Match(bookdata);
                    string chaptername = matchp2.Groups["key1"].Value.Split('_')[1];


                    int sort = int.Parse(pcountstr);

                    string chapterurl = $"http://m.zhangyue.com/readbook/{c.comicid.Replace("11_", "")}/{sort}/";



                    plst.Add(new Chapter()
                    {
                        chapterid = c.comicid + "_" + sort,
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 11,
                        downstatus = 0,
                        isvip = "0",
                        chaptersource = "",
                        chapterlocal = "",
                        modify = DateTime.Now,
                        shortdate = DateTime.Now.ToDateStr(),
                        chapterupdate = DateTime.Now.ToDateStr()
                    });

                }
                catch (Exception ex)
                {

                    continue;
                }
            }

            service.ChapterCompareBatchAdd(plst, 11, DateTime.Now.ToDateStr());
        }
    }
}
