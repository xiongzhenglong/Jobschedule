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

namespace Jobs.Item.MKZ
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            crawcomic("1");
            crawcomic("2");
        }

        private void crawcomic(string copyright)
        {
            int sort = 0;
            List<Comic> comiclst = new List<Comic>();
            var web = new HtmlWeb();
            int pageindex = 0;
            string copy = copyright == "1" ? "独家" : "合作";
            while (true)
            {
                pageindex = pageindex + 1;
                var docD = web.Load($"https://www.mkzhan.com/category/?copyright={copyright}&page={pageindex}");
                var alink = docD.DocumentNode.SelectNodes("//a[@class=\"comic-cover\"]");
                if (alink != null && alink.Count > 0)
                {
                    foreach (var a in alink)
                    {
                        try
                        {
                            sort = sort + 1;
                            var bookurl = "https://www.mkzhan.com" + a.Attributes["href"].Value;
                            var bookdata = web.Load(bookurl);
                            string comicname = bookdata.DocumentNode.SelectSingleNode("//h1[@class=\"title\"]").Attributes["title"].Value;
                            string authorname = bookdata.DocumentNode.SelectSingleNode("//p[@class=\"comic-author\"]").SelectSingleNode(".//a").InnerText.Trim();
                            string comiccover = bookdata.DocumentNode.SelectSingleNode("//div[@class=\"detail-image\"]/img").Attributes["src"].Value;
                            string bookdesc = bookdata.DocumentNode.SelectSingleNode("//div[@class=\"comic-summary\"]/span").InnerText.Trim();
                            string isfinished = bookdata.DocumentNode.SelectSingleNode("//p[@class=\"comic-status\"]").InnerText.Contains("完结") ? "已完结" : "连载中";
                            var tags = bookdata.DocumentNode.SelectNodes("//div[@class=\"title-box\"]/span");
                            var theme = "";
                            foreach (var tag in tags)
                            {
                                theme = theme + " " + tag.InnerText.Trim();
                            }
                            theme = theme.Trim().Replace(" ", ",");

                            string hot = bookdata.DocumentNode.SelectNodes("//p[@class=\"comic-status\"]/span/b")[1].InnerText.Trim();

                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                authorname = authorname,
                                bookurl = bookurl,
                                comiccoversource = comiccover,
                                comiccoverlocal = "",
                                comicdesc = bookdesc,
                                comicid = 9 + "_" + bookurl.Split('/')[3],

                                isfinished = isfinished,
                                theme = theme,
                                isvip = "0",
                                source = 9,
                                stopcrawer = false,
                                isoffline = true,
                                recrawer = false,
                                shortdate = DateTime.Now.ToDateStr(),
                                modify = DateTime.Now,
                                comicsort = sort,
                                hot = hot,
                                copyright = copy
                            });
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }
                      
                    }
                }
                else
                {
                    break;
                }
            }

            service.ComicCompareBatchAdd(comiclst, 9, DateTime.Now.ToDateStr());
        }
    }
}