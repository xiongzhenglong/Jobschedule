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

namespace Jobs.Item.Kuaikan
{
    //不允许此 Job 并发执行任务（禁止新开线程执行）
    [DisallowConcurrentExecution]
    public sealed class Job_Chapter : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Chapter));
        private BaseManager<Chapter> chaptermanager = new BaseManager<Chapter>();
        private BaseManager<Comic> comicmanager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            
            try
            {
                
                string date = DateTime.Now.ToDateStr();
                List<Comic> clst = comicmanager.Query(x => x.source==12 && x.shortdate == date);
                List<Chapter> plst = new List<Chapter>();
                foreach (var c in clst)
                {
                    var url = c.bookurl;
                    var web = new HtmlWeb();
                    var doc = web.Load(url);

                    var obj = doc.DocumentNode
                        .SelectNodes("//table/tbody/tr");
                    if (obj != null)
                    {
                        int sort = obj.Count;
                        var alink = obj
                            .First().SelectNodes(".//a[@href]").First();
                        string chapterurl = alink.Attributes["href"].Value;
                        string title = alink.Attributes["title"].Value;
                        var riqi = obj.First().SelectNodes(".//td").Last().InnerText;

                        plst.Add(new Chapter()
                        {
                            chapterid = "12_" + chapterurl.Split('/')[3],
                            chaptername = title,
                            chapterurl = "http://www.kuaikanmanhua.com" + chapterurl,
                            sort = sort,
                            comicid = c.comicid,
                            retry = 0,
                            source = 12,
                            downstatus = 0,
                            isvip = "0",
                            chaptersource = "",
                            chapterlocal = "",
                            modify = DateTime.Now,
                            shortdate = DateTime.Now.ToDateStr(),
                            chapterupdate = riqi
                        });
                    }

                    

                }
                service.ChapterCompareBatchAdd(plst, 12, DateTime.Now.ToDateStr());
            }
            catch (Exception ex)
            {
                _logger.Error("Job_Chapter 执行过程中发生异常:" + ex.ToString());
            }
            finally
            {
                _logger.InfoFormat("Job_Chapter Execute end ");
            }
        }
    }
}
