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

namespace Jobs.Item.MKZ
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
            List<Comic> clst = comicmanager.Query(x => x.source == 9 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);
                    var cpli = doc.DocumentNode.SelectSingleNode("//ul[@class=\"chapter-lists-detail\"]").SelectNodes(".//li[@data-chapter_id]");

                    int sort = cpli.Count;
                    var last = cpli.First();
                    string updatestr = doc.DocumentNode.SelectSingleNode("//span[@class=\"update-time \"]").InnerText.Trim();
                    
                    string chaptername = last.SelectSingleNode(".//a").Attributes["title"].Value;
                    string chapterurl = "https://www.mkzhan.com" + last.SelectSingleNode(".//a").Attributes["href"].Value;
                    string chapterid = c.comicid + "_" + chapterurl.Split('/').Last().Replace(".html", "");

                    plst.Add(new Chapter()
                    {
                        chapterid = chapterid,
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 9,
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

            service.ChapterCompareBatchAdd(plst, 9, DateTime.Now.ToDateStr());
        }
    }
}
