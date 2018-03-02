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

namespace Jobs.Item.Webtoons
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
            var web = new HtmlWeb();
            var doc = web.Load("https://www.dongmanmanhua.cn/genre");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<string> bkids = new List<string>();
            var lis =  doc.DocumentNode.SelectNodes("//li[@data-title-no]");
            foreach (var li in lis)
            {
                var alink ="https:"+li.SelectSingleNode(".//a").Attributes["href"].Value;
                if (!dic.ContainsKey(alink))
                {
                    dic.Add(alink, alink);
                    bkids.Add(alink.Split('=').Last().Trim());
                }
            }

            int sort = 0;
            HttpWebHelper ht = new HttpWebHelper();
            var res = ht.Get<webtoons_comic>("https://www.dongmanmanhua.cn/v1/title/like/count?titleNos=" + string.Join(",", bkids.ToArray()), Encoding.GetEncoding("utf-8"));
            foreach (var item in dic)
            {
                try
                {
                    sort = sort + 1;
                    string bookurl = item.Value;
                    var detail = web.Load(bookurl);

                    string comiccover = detail.DocumentNode.SelectSingleNode("//meta[@property=\"og:image\"]").Attributes["content"].Value;
                    string authorname = detail.DocumentNode.SelectSingleNode("//meta[@property=\"com-dongman:webtoon:author\"]").Attributes["content"].Value;
                    string bookdesc = detail.DocumentNode.SelectSingleNode("//p[@class=\"summary\"]").InnerText.Trim();
                    string comicname = detail.DocumentNode.SelectSingleNode("//h1[@class=\"subj\"]").InnerText.Trim();
                    string isfinished = detail.DocumentNode.SelectNodes("//span[@class=\"txt_ico_up\"]")!=null?  "连载中" : "已完结";
                    string theme = detail.DocumentNode.SelectSingleNode("//h2[contains(@class,'genre')]").InnerText.Trim();

                    comiclst.Add(new Comic()
                    {
                        authorname = authorname,
                        bookurl = bookurl,
                        comiccoversource = comiccover,
                        comiccoverlocal = "",
                        comicdesc = bookdesc,
                        comicid = 2 + "_" + bookurl.Split('=').LastOrDefault(),
                        comicname = comicname,
                        isfinished = isfinished,
                        theme = theme,
                        isvip = "0",
                        source = 2,
                        stopcrawer = false,
                        isoffline = false,
                        recrawer = false,
                        shortdate = shortdate,
                        modify = dt,
                        comicsort = sort,
                        hot = res.data.Where(x => x.titleNo == item.Value.Split('=').Last().Trim()).First().count.ToString()
                    });

                }
                catch (Exception ex)
                {

                    continue;
                }
                
            }

            service.ComicCompareBatchAdd(comiclst, 2, DateTime.Now.ToDateStr());

        }
    }
}
