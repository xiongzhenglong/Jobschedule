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

namespace Jobs.Item.QQ
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
            List<Comic> clst = comicmanager.Query(x => x.source == 1 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();

            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var doc = web.Load(c.bookurl);
                    var cpa = doc.DocumentNode.SelectSingleNode("//ol[@class=\"chapter-page-all works-chapter-list\"]").SelectNodes(".//a[@title]");

                    int sort = cpa.Count;
                    var last = doc.DocumentNode.SelectSingleNode("//a[@class=\"works-ft-new\"]");
                    var up = doc.DocumentNode.SelectSingleNode("//span[@class=\"ui-pl10 ui-text-gray6\"]");
                    string updatestr = up == null ? DateTime.Now.ToDateStr() : up.InnerText.Trim();
                    
                    string chaptername = last.InnerText.Trim();
                    string chapterurl = "http://ac.qq.com" + last.Attributes["href"].Value;
                    string chapterid = c.comicid + "_" + chapterurl.Split('/').Last().Replace(".html", "");

                    plst.Add(new Chapter()
                    {
                        chapterid = chapterid,
                        chaptername = chaptername,
                        chapterurl = chapterurl,
                        sort = sort,
                        comicid = c.comicid,
                        retry = 0,
                        source = 1,
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

            service.ChapterCompareBatchAdd(plst, 1, DateTime.Now.ToDateStr());
        }
    }
}
