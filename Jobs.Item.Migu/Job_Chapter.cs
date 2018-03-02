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

namespace Jobs.Item.Migu
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
            try
            {
                var domain = "http://www.migudm.cn";
                string date = DateTime.Now.ToDateStr();
                List<Comic> clst = comicmanager.Query(x => x.source == 10 && x.shortdate == date);
                List<Chapter> plst = new List<Chapter>();
                foreach (var c in clst)
                {
                    try
                    {
                        var url = c.bookurl;
                        var web = new HtmlWeb();
                        var doc = web.Load(url);

                        var obj = doc.DocumentNode
                            .SelectNodes("//a[@class=\"item ellipsis\"]");
                        var riqi = doc.DocumentNode.SelectSingleNode("//span[@class=\"date\"]").InnerText.Trim();
                        if (obj != null)
                        {
                            int sort = obj.Count;
                            var alink = obj
                                .Last();
                            string chapterurl = domain + alink.Attributes["href"].Value;
                            string title = alink.Attributes["title"].Value;
                            

                            plst.Add(new Chapter()
                            {
                                chapterid = c.comicid+"_"+chapterurl.Split('/').Last().Replace(".html",""),
                                chaptername = title,
                                chapterurl = chapterurl,
                                sort = sort,
                                comicid = c.comicid,
                                retry = 0,
                                source = 10,
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
                    catch (Exception ex)
                    {

                        continue;
                    }




                }
                service.ChapterCompareBatchAdd(plst, 10, DateTime.Now.ToDateStr());
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
