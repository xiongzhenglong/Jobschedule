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

namespace Jobs.Item.U17
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
            List<Comic> clst = comicmanager.Query(x => x.source == 3 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);
                    var cpli = doc.DocumentNode.SelectSingleNode("//ul[@id=\"chapter\"]").SelectNodes(".//li[@id]");

                    int sort = cpli.Count;
                    var last = cpli.Last();
                    string updatestr = last.SelectSingleNode(".//a").Attributes["title"].Value;
                    updatestr = updatestr.Substring(updatestr.Length - 10);
                    string chaptername = last.SelectSingleNode(".//a").InnerText.Trim();
                    string chapterurl = last.SelectSingleNode(".//a").Attributes["href"].Value;
                    string chapterid = c.comicid + "_" + chapterurl.Split('/').Last().Replace(".html", "");

                    plst.Add(new Chapter()
                    {
                        chapterid = chapterid,
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 3,
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

            service.ChapterCompareBatchAdd(plst, 3, DateTime.Now.ToDateStr());
        }
    }
}
