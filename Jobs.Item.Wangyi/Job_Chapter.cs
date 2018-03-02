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

namespace Jobs.Item.Wangyi
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
            List<Comic> clst = comicmanager.Query(x => x.source == 6 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            HttpWebHelper _helper = new HttpWebHelper();
            var web = new HtmlWeb();

            foreach (var c in clst)
            {
                try
                {
                    var data = web.Load(c.bookurl);
                    var updatestr = data.DocumentNode.SelectSingleNode("//div[@class=\"sr-notice__text f-toe\"]").InnerText.Trim();
                    if (updatestr.Contains("最近更新"))
                    {
                        updatestr = updatestr.Substring(0, updatestr.IndexOf("最近更新"));
                      
                    }
                    else
                    {
                        updatestr = "";
                    }
                    var chapterdata = _helper.Get<wangyi_chapter>($"https://manhua.163.com/book/catalog/{c.comicid.Replace("6_", "")}.json", Encoding.GetEncoding("utf-8"));
                    if (chapterdata.code == 200)
                    {
                        var t = chapterdata.catalog.sections.First();
                        var l = t.sections.Last();

                        plst.Add(new Chapter()
                        {
                            chapterid = c.comicid + "_" + l.sectionId,
                            chaptername = l.title,
                            chapterurl = $"https://manhua.163.com/reader/{l.bookId}/{l.sectionId}#imgIndex=0",
                            sort = t.sections.Count,
                            comicid = c.comicid,
                            retry = 0,
                            source = 6,
                            downstatus = 0,
                            isvip = "0",
                            chaptersource = "",
                            chapterlocal = "",
                            modify = DateTime.Now,
                            shortdate = DateTime.Now.ToDateStr(),
                            chapterupdate = updatestr
                        });
                    }
                }
                catch (Exception ex)
                {

                    continue;
                }
                
            }

            service.ChapterCompareBatchAdd(plst, 6, DateTime.Now.ToDateStr());
        }
    }
}
