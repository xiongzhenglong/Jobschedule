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
using System.Web;

namespace Jobs.Item.QQ
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic:IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            List<Comic> comiclst = new List<Comic>();
            DateTime dt = DateTime.Now;
            string shortdate = dt.ToDateStr();
            var web = new HtmlWeb();
            int pageindex = 0;
            int sort = 0;
            while (true)
            {
                pageindex = pageindex + 1;
                var doc = web.Load($"http://ac.qq.com/Comic/all/search/hot/page/{pageindex}");
                var alink =  doc.DocumentNode.SelectNodes("//a[@class=\"mod-cover-list-thumb mod-cover-effect ui-db\"]");
                if (alink!=null && alink.Count>0)
                {
                    foreach (var a in alink)
                    {
                        try
                        {
                            sort = sort + 1;
                            var bookurl = "http://ac.qq.com" + a.Attributes["href"].Value;
                            var bookdata = web.Load(bookurl);
                            string authorname = bookdata.DocumentNode.SelectSingleNode("//input[@id=\"input_artistName\"]").Attributes["value"].Value;
                            var works_cover = bookdata.DocumentNode.SelectSingleNode("//div[@class=\"works-cover ui-left\"]");
                            string comicname = works_cover.SelectSingleNode(".//img").Attributes["alt"].Value;
                            string comiccover = works_cover.SelectSingleNode(".//img").Attributes["src"].Value;
                            string isfinished = works_cover.SelectSingleNode(".//label").InnerText.Trim();
                            string bookdesc = bookdata.DocumentNode.SelectSingleNode("//p[contains(@class,'works-intro-short')]").InnerText.Trim();
                            string metatag = bookdata.DocumentNode.SelectSingleNode("//meta[@name=\"Description\"]").Attributes["content"].Value;                           
                            string theme = metatag.Substring(metatag.IndexOf("标签：") + 3);
                            string hot = bookdata.DocumentNode.SelectSingleNode("//span[contains(text(),'人气：')]").InnerText.Replace("人气：", "").Trim();
                            string copyright = bookdata.DocumentNode.SelectSingleNode("//div[@class=\"works-intro-head clearfix\"]").SelectSingleNode(".//i").InnerText.Trim();
                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                authorname = authorname,
                                bookurl = bookurl,
                                comiccoversource = comiccover,
                                comiccoverlocal = "",
                                comicdesc = bookdesc,
                                comicid = 1 + "_" + bookurl.Split('/').Last(),

                                isfinished = isfinished,
                                theme = theme,
                                isvip = "0",
                                source = 1,
                                stopcrawer = false,
                                isoffline = false,
                                recrawer = false,
                                shortdate = shortdate,
                                modify = dt,
                                comicsort = sort,
                                hot = hot,
                                copyright = copyright
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

            service.ComicCompareBatchAdd(comiclst, 1, DateTime.Now.ToDateStr());
        }
    }
}
