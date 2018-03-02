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

namespace Jobs.Item.Manhuatai
{
    [DisallowConcurrentExecution]
    public class Job_Comic:IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            List<Comic> comiclst = new List<Comic>();
            DateTime dt = DateTime.Now;
            string shortdate = dt.ToDateStr();
            int pageindex = 0;
            var web = new HtmlWeb();
            int sort = 0;
            while (true)
            {
                pageindex = pageindex + 1;
                var doc = web.Load($"http://www.manhuatai.com/all_p{pageindex}.html");
                var alst = doc.DocumentNode.SelectNodes("//a[@class=\"sdiv\"]");
                
                foreach (var a in alst)
                {
                    try
                    {
                        sort = sort + 1;
                        string bookurl = "http://www.manhuatai.com" + a.Attributes["href"].Value;
                        var data = web.Load(bookurl);
                        var comicname = data.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:book_name\"]").Attributes["content"].Value;
                        var authorname = data.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:author\"]").Attributes["content"].Value;
                        var comiccover = data.DocumentNode.SelectSingleNode("//meta[@property=\"og:image\"]").Attributes["content"].Value;
                        var bookdesc = data.DocumentNode.SelectSingleNode("//div[@class=\"wz clearfix t1\"]").InnerText.Trim();
                        var isfinished = data.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:status\"]").Attributes["content"].Value == "连载中" ? "连载中" : "已完结";
                        var theme = data.DocumentNode.SelectSingleNode("//meta[@property=\"og:novel:category\"]").Attributes["content"].Value.Replace(" ", ",");
                        HttpWebHelper ht = new HttpWebHelper();
                        var numres = ht.Get($"http://api.share.baidu.com/getnum?url=http://www.manhuatai.com{a.Attributes["href"].Value}&callback=bdShare.fn._getShare&type=load", Encoding.GetEncoding("utf-8"));
                        numres = numres.Split(',').Last().Replace("]})", "").Replace("\"", "").Replace("\\", "").Trim();
                        if (numres.IndexOf('u') > 0)
                        {
                            numres = numres.Substring(0, numres.IndexOf('u')) + StringHelper.UnicodeToString(numres.Substring(numres.IndexOf('u')));
                        }

                        comiclst.Add(new Comic()
                        {
                            comicname = comicname,
                            authorname = authorname,
                            bookurl = bookurl,
                            comiccoversource = comiccover,
                            comiccoverlocal = "",
                            comicdesc = bookdesc,
                            comicid = 5 + "_" + bookurl.Split('/')[3],

                            isfinished = isfinished,
                            theme = theme,
                            isvip = "0",
                            source = 5,
                            stopcrawer = false,
                            isoffline = false,
                            recrawer = false,
                            shortdate = shortdate,
                            modify = dt,
                            comicsort = sort,
                            hot = numres
                        });
                    }
                    catch (Exception ex)
                    {

                        continue;
                    }
                    
                }

                if (pageindex>1 &&doc.DocumentNode.SelectNodes("//a[@class=\"page nolink\"]")!=null)
                {
                    break;
                }
                

            }

            service.ComicCompareBatchAdd(comiclst, 5, DateTime.Now.ToDateStr());
        }
    }
}
