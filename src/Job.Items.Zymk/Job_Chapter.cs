using HtmlAgilityPack;
using Jobs.Core.Business.Manager;
using Jobs.Core.Common.Extension;
using Jobs.Core.Entity;
using Jobs.Core.Services;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jobs.Item.Zymk
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
            List<Comic> clst = comicmanager.Query(x => x.source == 4 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            var web = new HtmlWeb();
            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);
                    var alink = doc.DocumentNode.SelectSingleNode("//ul[@id=\"chapterList\"]").SelectNodes(".//a");
                    var cpupdate = doc.DocumentNode.SelectSingleNode("//p[@class=\"update\"]").InnerText.Trim().Replace("最后更新: ", "");
                    int sort = alink.Count;

                    var lastcp = alink.First();

                    string chapterurl = $"http://www.zymk.cn/{c.comicid.Replace("4_", "")}/{lastcp.Attributes["href"].Value}";
                    string chaptername = lastcp.Attributes["title"].Value;

                    plst.Add(new Chapter()
                    {
                        chapterid = c.comicid + "_" + chapterurl.Split('/').Last().Replace(".html", ""),
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 4,
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

            service.ChapterCompareBatchAdd(plst, 4, DateTime.Now.ToDateStr());
        }
    }
}