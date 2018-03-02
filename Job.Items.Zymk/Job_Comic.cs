using HtmlAgilityPack;
using Jobs.Core.APIModel;
using Jobs.Core.Business.Manager;
using Jobs.Core.Common;
using Jobs.Core.Common.Comparer;
using Jobs.Core.Common.Extension;
using Jobs.Core.Entity;
using Jobs.Core.Services;
using log4net;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jobs.Item.Zymk
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

            var jarraystr = _helper.Get("http://www.zymk.cn/app_api/v3/getallcomic/?callback=siftinit", Encoding.GetEncoding("utf-8"));
            jarraystr = jarraystr.Substring(jarraystr.IndexOf("[["), jarraystr.IndexOf("]]") - jarraystr.IndexOf("[[") + 2);

            var booklst = JsonHelper.DeserializeJsonToList<object>(jarraystr);
            foreach (var book in booklst)
            {
                var m = JArray.Parse(book.ToJson());
                try
                {
                    if (m[4].ToString().Contains(",28") || m[4].ToString().Contains(",51") || m[4].ToString().Contains(",30") || m[4].ToString().Contains(",52"))
                    {
                        string bookurl = $"http://www.zymk.cn/{m[0]}/";

                        var bookdata = _helper.Get(bookurl, Encoding.GetEncoding("utf-8"));

                        Regex reg1 = new Regex("<meta property=\"og:novel:book_name\" content=\"(?<key1>.*?)\">");
                        Match match1 = reg1.Match(bookdata);
                        string comicname = match1.Groups["key1"].Value;

                        Regex reg2 = new Regex("<meta property=\"og:novel:author\" content=\"(?<key1>.*?)\">");
                        Match match2 = reg2.Match(bookdata);
                        string authorname = match2.Groups["key1"].Value;
                        Regex reg3 = new Regex("<meta property=\"og:image\" content=\"(?<key1>.*?)\">");
                        Match match3 = reg3.Match(bookdata);
                        string comiccover = match3.Groups["key1"].Value;

                        Regex reg4 = new Regex("<div class=\"desc-con\">(?<key1>.*?)</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Match match4 = reg4.Match(bookdata);
                        string bookdesc = match4.Groups["key1"].Value.Trim();

                        Regex reg5 = new Regex("<meta property=\"og:novel:status\" content=\"(?<key1>.*?)\">");
                        Match match5 = reg5.Match(bookdata);
                        string isfinished = match5.Groups["key1"].Value == "连载" ? "连载中" : "已完结";

                        string theme = "";
                        Regex reg6 = new Regex("<meta property=\"og:novel:category\" content=\"(?<key1>.*?)\">");
                        Match match6 = reg6.Match(bookdata);
                        theme = string.Join(",", match6.Groups["key1"].Value.Trim().Split(' '));

                        comiclst.Add(new Comic()
                        {
                            comicname = comicname,
                            authorname = authorname,
                            bookurl = bookurl,
                            comiccoversource = comiccover,
                            comiccoverlocal = "",
                            comicdesc = bookdesc,
                            comicid = 4 + "_" + bookurl.Split('/')[3],
                            isfinished = isfinished,
                            theme = theme,
                            isvip = "0",
                            source = 4,
                            stopcrawer = false,
                            isoffline = true,
                            recrawer = false,
                            shortdate = shortdate,
                            modify = dt,
                            comicsort = 0,
                            comicsort2 = double.Parse(m[6].ToString()),
                            hot = m[6].ToString()
                        });


                    }
                }
                catch (Exception ex)
                {

                    continue;
                }
            }

            comiclst = comiclst.OrderBy(x => x.comicsort2).ToList();
            int sort = 0;
            comiclst.ForEach(x =>
            {
                sort = sort + 1;
                x.comicsort = sort;
            });
            service.ComicCompareBatchAdd(comiclst, 4, DateTime.Now.ToDateStr());

        }
    }
}
