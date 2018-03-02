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

namespace Jobs.Item.Webtoons
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
            DateTime dt = DateTime.Now;
            string date = dt.ToDateStr();
            List<Comic> clst = comicmanager.Query(x => x.source == 2 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();

            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);
                    var cp = doc.DocumentNode.SelectSingleNode("//li[@data-episode-no]");
                    string chapterurl = cp.SelectSingleNode(".//a").Attributes["href"].Value;
                    int sort = int.Parse(cp.SelectSingleNode(".//span[@class=\"tx\"]").InnerText.Trim().Replace("#", ""));
                    string chaptername = cp.SelectSingleNode(".//span[@class=\"subj\"]").InnerText.Trim();
                    string chaptersource = cp.SelectSingleNode(".//img").Attributes["src"].Value;
                    string upstr = cp.SelectSingleNode(".//span[@class=\"date\"]").InnerText.Trim();
                    plst.Add(new Chapter()
                    {
                        chapterid = c.comicid + "_" + sort,
                        chaptername = chaptername,
                        chapterurl = "https:" + chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = c.source,
                        downstatus = 0,
                        isvip = "0",
                        chaptersource = chaptersource,
                        chapterlocal = "",
                        modify = dt,
                        shortdate = date,
                        chapterupdate = upstr
                    });
                }
                catch (Exception ex)
                {

                    continue;
                }
                
            }

            service.ChapterCompareBatchAdd(plst, 2, DateTime.Now.ToDateStr());

        }
    }
}
