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

namespace Jobs.Item.Buka
{
    //不允许此 Job 并发执行任务（禁止新开线程执行）
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
            int page = 0;
            List<Comic> comicall = new List<Comic>();
            List<Comic> arealst = new List<Comic>();
            List<Comic> themelst = new List<Comic>();

            
            #region area

            while (true)
            {
                try
                {
                    var count = page * 28;
                    var t = _helper.Post($"http://www.buka.cn/category/12053/%E6%97%A5%E9%9F%A9", $"start={count}", Encoding.GetEncoding("utf-8"), Encoding.GetEncoding("utf-8"), null, "", null, "http://www.buka.cn/category/12053/%E6%97%A5%E9%9F%A9.html");
                    var doc = new HtmlDocument();
                    doc.LoadHtml(t);
                    var obj = doc.DocumentNode.SelectNodes("//*[@id=\"mangawrap\"]/li");

                    if (obj != null)
                    {
                        foreach (var item in obj)
                        {
                            var alink = item.SelectNodes(".//div/a").First();

                            var bookurl = alink.Attributes["href"].Value;
                            var title = alink.Attributes["title"].Value;
                            var au = item.SelectNodes(".//*[@class=\"manga-author\"]").First().Attributes["title"].Value;
                            arealst.Add(new Comic()
                            {
                                comicname = title,
                                area = "日韩",
                                authorname = au,
                                bookurl = "http://www.buka.cn" + bookurl,
                                comiccoverlocal = "",
                                comiccoversource = "",
                                comicdesc = "",
                                comicid = "13_" + bookurl.Split('/').Last().Replace(".html", ""),
                                isfinished = "连载中",
                                isdelete = false,
                                isoffline = false,
                                isvip = "0",
                                recrawer = false,
                                source = 13,
                                stopcrawer = false,
                                shortdate = DateTime.Now.ToDateStr(),
                                modify = DateTime.Now,
                                comicsort = 1,
                                hot = "",
                                theme = "",
                                updatedatetime = ""

                            });


                        }

                    }
                    else
                    {
                        break;
                    }
                    page = page + 1;
                }
                catch (Exception ex)
                {

                    break;
                }


            }
            #endregion

            #region theme
            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("少女", "http://www.buka.cn/category/302/%E5%B0%91%E5%A5%B3");
            tags.Add("百合", "http://www.buka.cn/category/206/%E7%99%BE%E5%90%88");
            tags.Add("肥皂", "http://www.buka.cn/category/12009/%E8%82%A5%E7%9A%82");
            tags.Add("恋爱", "http://www.buka.cn/category/404/%E6%81%8B%E7%88%B1");
            tags.Add("玄幻", "http://www.buka.cn/category/12041/%E7%8E%84%E5%B9%BB");
            tags.Add("游戏", "http://www.buka.cn/category/12018/%E6%B8%B8%E6%88%8F");
            tags.Add("治愈", "http://www.buka.cn/category/202/%E6%B2%BB%E6%84%88");
            tags.Add("恐怖", "http://www.buka.cn/category/12075/%E6%81%90%E6%80%96");
            tags.Add("科幻", "http://www.buka.cn/category/403/%E7%A7%91%E5%B9%BB");
            tags.Add("搞笑", "http://www.buka.cn/category/10008/%E6%90%9E%E7%AC%91");
            tags.Add("鬼怪", "http://www.buka.cn/category/211/%E9%AC%BC%E6%80%AA");
            tags.Add("励志", "http://www.buka.cn/category/12023/%E5%8A%B1%E5%BF%97");
            tags.Add("格斗", "http://www.buka.cn/category/410/%E6%A0%BC%E6%96%97");
            tags.Add("少女漫", "http://www.buka.cn/category/12103/%E5%B0%91%E5%A5%B3%E6%BC%AB");
            tags.Add("少年漫", "http://www.buka.cn/category/12104/%E5%B0%91%E5%B9%B4%E6%BC%AB");
            tags.Add("布卡娘", "http://www.buka.cn/category/10035/布卡娘");
            foreach (var tag in tags)
            {
                page = 0;
                while (true)
                {
                    try
                    {
                        var count = page * 28;
                        var t = _helper.Post(tag.Value, $"start={count}", Encoding.GetEncoding("utf-8"), Encoding.GetEncoding("utf-8"));
                        var doc = new HtmlDocument();
                        doc.LoadHtml(t);
                        var obj = doc.DocumentNode.SelectNodes("//*[@id=\"mangawrap\"]/li");

                        if (obj != null)
                        {
                            foreach (var item in obj)
                            {
                                var alink = item.SelectNodes(".//div/a").First();

                                var bookurl = alink.Attributes["href"].Value;
                                var title = alink.Attributes["title"].Value;
                                var au = item.SelectNodes(".//*[@class=\"manga-author\"]").First().Attributes["title"].Value;
                                themelst.Add(new Comic()
                                {
                                    comicname = title,
                                    area = "",
                                    authorname = au,
                                    bookurl = "http://www.buka.cn" + bookurl,
                                    comiccoverlocal = "",
                                    comiccoversource = "",
                                    comicdesc = "",
                                    comicid = "13_" + bookurl.Split('/').Last().Replace(".html", ""),
                                    isfinished = "连载中",
                                    isdelete = false,
                                    isoffline = false,
                                    isvip = "0",
                                    recrawer = false,
                                    source = 13,
                                    stopcrawer = false,
                                    shortdate = DateTime.Now.ToDateStr(),
                                    modify = DateTime.Now,
                                    comicsort = 1,
                                    hot = "",
                                    theme = tag.Key,
                                    updatedatetime = ""

                                });


                            }

                        }
                        else
                        {
                            break;
                        }
                        page = page + 1;
                    }
                    catch (Exception ex)
                    {

                        break;
                    }


                }
            }
            #endregion
            page = 0;
            int sort = 0;
            while (true)
            {
                var count = page * 28;
                var url = $"http://www.buka.cn/all/{count}";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var obj = doc.DocumentNode.SelectNodes("//*[@id=\"mangawrap\"]/li");
                
                if (obj != null)
                {
                    foreach (var item in obj)
                    {
                        sort = sort + 1;
                        var alink = item.SelectNodes(".//div/a").First();

                        var bookurl = alink.Attributes["href"].Value;
                        var title = alink.Attributes["title"].Value;
                        var au = item.SelectNodes(".//*[@class=\"manga-author\"]").First().Attributes["title"].Value.Substring(3);
                        comicall.Add(new Comic()
                        {
                            comicname = title,
                            area = "",
                            authorname = au,
                            bookurl = "http://www.buka.cn" + bookurl,
                            comiccoverlocal = "",                         
                            comicid = "13_" + bookurl.Split('/').Last().Replace(".html", ""),
                            isfinished = "连载中",
                            isdelete = false,
                            isoffline = false,
                            isvip = "0",
                            recrawer = false,
                            source = 13,
                            stopcrawer = false,
                            shortdate = DateTime.Now.ToDateStr(),
                            modify = DateTime.Now,
                            comicsort = sort,
                            hot = "",
                            comicdesc = "",
                            theme ="",
                            comiccoversource = "",
                            updatedatetime =""

                        });

                    
                    }
                  
                }
                else
                {
                    break;
                }
                page = page + 1;
            }

            comicall.ForEach(x =>
            {
                string area = "";
                if (arealst.Where(y => y.comicid == x.comicid).FirstOrDefault()!=null)
                {
                    area = arealst.Where(y => y.comicid == x.comicid).FirstOrDefault().area;

                }
                string theme = "";
                if (themelst.Where(t=>t.comicid==x.comicid).Count()>0)
                {
                    theme = string.Join(",", themelst.Where(t => t.comicid == x.comicid).Select(t => t.theme).ToArray());

                }
                x.area = area;
                x.theme = theme;
            });
            List<Comic> exlst = new List<Comic>();
            foreach (var item in comicall)
            {
                try
                {
                    var web1 = new HtmlWeb();
                    var doc1 = web1.Load(item.bookurl);
                    var img1 = doc1.DocumentNode.SelectNodes("/html/body/section/aside/div[1]/img").First().Attributes["src"].Value;
                    var rate = doc1.DocumentNode.SelectNodes("//*[@class=\"manga-grade-num\"]").First().InnerText.Trim();
                    var desc = doc1.DocumentNode.SelectNodes("//*[@class=\"manga-desc-font\"]").First().InnerText.Trim();

                    item.comiccoversource = img1;
                    item.hot = rate;
                    item.comicdesc = desc;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(item.comicid);
                    exlst.Add(item);
                    continue;
                }
                
            }
            List<Comic> addlst = comicall.Except(exlst, new Comic_Comparer()).ToList();

            service.ComicCompareBatchAdd(addlst, 13, DateTime.Now.ToDateStr());
        }
    }
}