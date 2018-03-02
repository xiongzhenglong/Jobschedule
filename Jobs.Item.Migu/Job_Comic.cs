using HtmlAgilityPack;
using Jobs.Core.APIModel;
using Jobs.Core.Business.Manager;
using Jobs.Core.Common;
using Jobs.Core.Common.Comparer;
using Jobs.Core.Common.Extension;
using Jobs.Core.Entity;
using Jobs.Core.Services;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jobs.Item.Migu
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic:IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var domain = "http://www.migudm.cn";
                var web = new HtmlWeb();
                var doc = web.Load($"{domain}/comic/list/");
                var obj = doc.DocumentNode
                                .SelectNodes("//input[@id=\"pageTotalNo\"]").First();
                int pagetotal = int.Parse(obj.Attributes["value"].Value);
                List<Comic> comiclst = new List<Comic>();
                int sort = 0;
                for (int i = 1; i <= pagetotal; i++)
                {
                    var data = web.Load($"{domain}/comic/list_p{i}/");

                    var objdata = data.DocumentNode.SelectNodes("//h4[@class=\"title ellipsis\"]");
                    foreach (var item in objdata)
                    {
                        try
                        {
                            
                            sort = sort + 1;
                            var alink = item.SelectSingleNode(".//a").Attributes["href"].Value;
                            var itemdata = web.Load(domain + alink);
                            var comicname = itemdata.DocumentNode.SelectSingleNode($"//a[@href=\"{alink}\"]").InnerText.Trim();
                            var authorname = itemdata.DocumentNode.SelectSingleNode("//p[@class=\"authorName\"]").InnerText.Trim();
                            var comiccover = itemdata.DocumentNode.SelectSingleNode($"//img[@alt=\"{comicname}漫画\"]").Attributes["src"].Value;
                            var bookdesc = itemdata.DocumentNode.SelectSingleNode("//p[@id=\"worksDesc\"]").InnerText.Trim();
                            var isfinished = itemdata.DocumentNode.SelectSingleNode("//a[@class=\"status\"]").InnerText.Trim() == "连载中" ? "连载中" : "已完结";
                            var lbls = itemdata.DocumentNode.SelectSingleNode("//span[@class=\"labelBox\"]").SelectNodes(".//a");
                            string theme = "";
                            List<string> themelst = new List<string>();
                            foreach (var lbl in lbls)
                            {
                                themelst.Add(lbl.InnerText);
                            }

                            theme = string.Join(",", themelst);
                            var renqi = itemdata.DocumentNode.SelectSingleNode("//span[@class=\"clickCountStr\"]").InnerText.Trim();
                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                area = "",
                                authorname = authorname,
                                bookurl = domain + alink,
                                comiccoverlocal = "",
                                comiccoversource = comiccover,
                                comicdesc = "",
                                comicid = "10_" + alink.Split('/').Last().Replace(".html", ""),
                                isfinished = isfinished,
                                isdelete = false,
                                isoffline = false,
                                isvip = "0",
                                recrawer = false,
                                source = 10,
                                stopcrawer = false,
                                shortdate = DateTime.Now.ToDateStr(),
                                modify = DateTime.Now,
                                comicsort = sort,
                                hot = renqi,
                                theme = theme,
                                updatedatetime = ""
                            });
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }
                       
                    }
                }              

                service.ComicCompareBatchAdd(comiclst, 10, DateTime.Now.ToDateStr());
            }
            catch (Exception ex)
            {

                ;
            }
           
        }
    }
}
