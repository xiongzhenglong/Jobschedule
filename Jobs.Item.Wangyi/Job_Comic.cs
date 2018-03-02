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
    public sealed class Job_Comic : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            var comiclst = new List<Comic>();
            HttpWebHelper _helper = new HttpWebHelper();
            DateTime dt = DateTime.Now;
            string shortdate = dt.ToDateStr();
            var web = new HtmlWeb();
            var t = _helper.Get<wangyi_comic>("https://manhua.163.com/category/getData.json?csrfToken=d56b46d665fd2a53ed222d302a572718&sort=2&pageSize=72&page=1", Encoding.GetEncoding("utf-8"));
            int pagecount = t.pageQuery.pageCount;

            int sort = 0;
            if (pagecount > 0)
            {
                for (int i = 1; i <= pagecount; i++)
                {
                    
                    var booklst = _helper.Get<wangyi_comic>($"https://manhua.163.com/category/getData.json?csrfToken=d56b46d665fd2a53ed222d302a572718&sort=2&pageSize=72&page={i}", Encoding.GetEncoding("utf-8"));

                    foreach (var bk in booklst.books)
                    {
                        try
                        {
                            sort = sort + 1;
                            string bookurl = $"https://manhua.163.com/source/{bk.bookId}";
                            var doc = web.Load(bookurl);
                            var meta = doc.DocumentNode.SelectSingleNode("//meta[@name=\"keywords\"]");

                            var arry = meta.Attributes["content"].Value.Split(',').ToList();
                            string comicname = arry[0];
                            string authorname = arry[arry.Count - 2];
                            arry.Remove(arry[0]);
                            arry.Remove(arry[0]);
                            arry.Remove(arry[arry.Count - 1]);
                            arry.Remove(arry[arry.Count - 1]);
                            string theme = string.Join(",", arry.Where(x => x.Length == 2).ToArray());

                            string comiccover = doc.DocumentNode.SelectSingleNode("//img[@class=\"sr-bcover\"]").Attributes["src"].Value;

                            string bookdesc = doc.DocumentNode.SelectSingleNode("//dl[@class=\"sr-dl multi-lines j-desc-inner\"]").SelectSingleNode(".//dd").InnerText.Trim();

                            string isfinished = doc.DocumentNode.SelectSingleNode("//dl[@class=\"sr-dl\"]").SelectSingleNode(".//a").InnerText.Trim() == "连载中" ? "连载中" : "已完结";

                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                authorname = authorname,
                                bookurl = bookurl,
                                comiccoversource = comiccover,
                                comiccoverlocal = "",
                                comicdesc = bookdesc,
                                comicid = 6 + "_" + bk.bookId,

                                isfinished = isfinished,
                                theme = theme,
                                isvip = "0",
                                source = 6,
                                stopcrawer = false,
                                isoffline = false,
                                recrawer = false,
                                shortdate = shortdate,
                                modify = dt,
                                comicsort = sort,
                                hot = bk.clickCount

                            });
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }
                        
                    }

                   
                }

                service.ComicCompareBatchAdd(comiclst, 6, DateTime.Now.ToDateStr());
            }
        }
    }
}