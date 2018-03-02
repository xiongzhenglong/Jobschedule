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

namespace Jobs.Item.Buka
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

                string date = DateTime.Now.ToDateStr();
                List<Comic> clst = comicmanager.Query(x =>x.source==13 &&  x.shortdate == date);
                List<Chapter> plst = new List<Chapter>();
                foreach (var c in clst)
                {
                    try
                    {
                        var url = c.bookurl;
                        var web = new HtmlWeb();
                        var doc = web.Load(url);

                        var obj = doc.DocumentNode
                            .SelectNodes("//div[@id=\"episodes\"]").First().SelectNodes(".//a[@class=\"epsbox-eplink \"]");
                        var riqi = doc.DocumentNode.SelectNodes("//span[@class=\"time\"]").First().InnerText.Trim();
                        if (obj != null)
                        {
                            int sort = obj.Count;
                            var alink = obj
                                .First();
                            string chapterurl = "";
                            var t = obj.Where(x => x.InnerText.Contains("话")).ToList();
                           
                            if (t.Count>1)
                            {
                                
                                string n1 = System.Text.RegularExpressions.Regex.Replace(t.First().InnerText.Trim(), @"[^0-9]+", "");
                               
                                string n2 = System.Text.RegularExpressions.Regex.Replace(t.Last().InnerText.Trim(), @"[^0-9]+", "");
                                if (n1.IsNumeric() && n2.IsNumeric()&&(int.Parse(n1) < int.Parse(n2)))
                                {
                                    
                                    alink = obj.Last();
                                }
                                
                            }
                            if (alink.Attributes["href"]==null)
                            {
                                var ss = alink.Attributes["onclick"].Value;                                                         
                               

                                chapterurl = ss.Split(',').First().Replace("payChapter('", "").Replace("'", "");
                            }
                            else
                            {
                                chapterurl = alink.Attributes["href"].Value;
                            }
                             
                            string title = alink.InnerText.Replace("￥","").Trim();


                            plst.Add(new Chapter()
                            {
                                chapterid = c.comicid+"_" + chapterurl.Split('/').Last().Replace(".html", ""),
                                chaptername = title,
                                chapterurl = "http://www.buka.cn" + chapterurl,
                                sort = sort,
                                comicid = c.comicid,
                                retry = 0,
                                source = 13,
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
                service.ChapterCompareBatchAdd(plst, 13, DateTime.Now.ToDateStr());
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
