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
using System.Text.RegularExpressions;

namespace Jobs.Item.Zhangyue
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Comic));
        private BaseManager<Comic> manager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int pagecount = 0;
                int sort = 0;
                DateTime dt = DateTime.Now;
                string shortdate = dt.ToString("yyyy-MM-dd");
                List<Comic> comiclst = new List<Comic>();
                while (true)
                {
                    HttpWebHelper _helper = new HttpWebHelper();
                    pagecount = pagecount + 1;
                    string bookurl = $"http://m.zhangyue.com/search/more?keyWord=%E6%BC%AB%E7%94%BB&currentPage={pagecount}";
                    var data = _helper.Get<zhangyue_comic_api>(bookurl, Encoding.GetEncoding("utf-8"));
                    if (data.html.IsNullOrWhiteSpace())
                    {
                        break;
                    }
                    string pattern2 = "<a href=\"(?<key1>.*?)\" data-js=\"pointBook\">";

                    MatchCollection matches2 = Regex.Matches(data.html, pattern2);

                    for (int j = 0; j < matches2.Count; j++)
                    {
                        try
                        {
                            var bookdetailurl = matches2[j].Groups["key1"].Value;
                            var bookdata = _helper.Get(bookdetailurl, Encoding.GetEncoding("utf-8"));
                            Regex reg1 = new Regex("<img class=\"bookcover\" data-js=\"book\" src=\"(?<key1>.*?)\" alt=\"(?<key2>.*?)\">");
                            Match match1 = reg1.Match(bookdata);
                            string comiccover = match1.Groups["key1"].Value;
                            string comicname = match1.Groups["key2"].Value;

                            Regex reg2 = new Regex("<span class=\"it red\">(?<key1>.*?)</span>");
                            Match match2 = reg2.Match(bookdata);
                            string authorname = match2.Groups["key1"].Value;

                            Regex reg4 = new Regex("<p data-js=\"brief_intro_txt\">(?<key1>.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            Match match4 = reg4.Match(bookdata);
                            string bookdesc = match4.Groups["key1"].Value.Trim().Replace("内容简介：", "");

                            Regex reg5 = new Regex("<dd class=\"lastline\"><span>(?<key1>.*?)</span></dd>");
                            Match match5 = reg5.Match(bookdata);
                            string isfinished = match5.Groups["key1"].Value.Contains("连载中") ? "连载中" : "已完结";


                            string pattern = "<span data-js=\"goCategory\" data-id=\"(?<key2>.*?)\" data-url=\"(?<key3>.*?)\">(?<key1>.*?)</span>";
                            MatchCollection matches = Regex.Matches(bookdata, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            string theme = "";
                            List<string> themelst = new List<string>();
                            for (int i = 0; i < matches.Count; i++)
                            {
                                themelst.Add(matches[i].Groups["key1"].Value.Trim());
                            }
                            theme = string.Join(",", themelst);

                            Regex reg6 = new Regex("<span class=\"yellow\">(?<key1>.*?)</span>");
                            Match match6 = reg6.Match(bookdata);
                            Regex reg7 = new Regex("<span class=\"f10\">(?<key2>.*?)</span>");
                            Match match7 = reg7.Match(bookdata);
                            string renqi = match6.Groups["key1"].Value + " | " + match7.Groups["key2"].Value;

                            sort = sort + 1;
                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                authorname = authorname,
                                bookurl = bookdetailurl,
                                comiccoversource = comiccover,
                                comiccoverlocal = "",
                                comicdesc = bookdesc,
                                comicid = "11" + "_" + bookdetailurl.Split('/')[4].Replace(".html", ""),
                                isfinished = isfinished,
                                theme = theme,
                                isvip = "0",
                                source = 11,
                                stopcrawer = true,
                                isoffline = false,
                                recrawer = false,
                                shortdate = shortdate,
                                modify = dt,
                                comicsort = sort,
                                hot = renqi,
                                area = "",
                                isdelete = false,
                                updatedatetime = ""
                            });
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }

                    }
                }
                service.ComicCompareBatchAdd(comiclst, 11, DateTime.Now.ToDateStr());
            }
            catch (Exception ex)
            {

                ;
            }
           


        }
    }

    public class zhangyue_comic_api
    {
        public string html { get; set; }
    }
}
