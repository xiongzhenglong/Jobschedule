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

namespace Jobs.Item.Manhuatai
{
    [DisallowConcurrentExecution]
    public sealed class Job_Chapter:IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Chapter));
        private BaseManager<Chapter> chaptermanager = new BaseManager<Chapter>();
        private BaseManager<Comic> comicmanager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            string date = DateTime.Now.ToDateStr();
            List<Comic> clst = comicmanager.Query(x => x.source == 5 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);


                    int sort = doc.DocumentNode.SelectSingleNode("//ul[@id=\"topic1\"]").SelectNodes("li").Count;
                    string updatestr = doc.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:update_time\"]").Attributes["content"].Value;
                    string chaptername = doc.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:latest_chapter_name\"]").Attributes["content"].Value;
                    string chapterurl = doc.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:latest_chapter_url\"]").Attributes["content"].Value;
                    string chapterid = c.comicid + "_" + chapterurl.Split('/').Last().Replace(".html", "");

                    plst.Add(new Chapter()
                    {
                        chapterid = chapterid,
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 5,
                        downstatus = 0,
                        isvip = "0",
                        chaptersource = "",
                        chapterlocal = "",
                        modify = DateTime.Now,
                        shortdate = DateTime.Now.ToDateStr(),
                        chapterupdate = updatestr
                    });
                }
                catch (Exception ex)
                {

                    continue;
                }
               
            }

            service.ChapterCompareBatchAdd(plst, 5, DateTime.Now.ToDateStr());
        }
    }
}
