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
using System.Web;

namespace Jobs.Item.U17
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic : IJob
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
            HttpWebHelper _helper = new HttpWebHelper();
            string cataurl = "http://www.u17.com/comic/ajax.php?mod=comic_list&act=comic_list_new_fun&a=get_comic_list";
            string paras = "data%5Bgroup_id%5D=no&data%5Btheme_id%5D=no&data%5Bis_vip%5D=no&data%5Baccredit%5D=no&data%5Bcolor%5D=no&data%5Bcomic_type%5D=no&data%5Bseries_status%5D=no&data%5Border%5D=0&data%5Bpage_num%5D={0}&data%5Bread_mode%5D=no";
            var data = _helper.Post<u17_comic>(cataurl, string.Format(paras, 1), Encoding.GetEncoding("utf-8"), Encoding.GetEncoding("utf-8"));
            int pagecount = int.Parse(data.page_total);
            int sort = 0;
            for (int i = 1; i <= pagecount; i++)
            {

                var booklst = _helper.Post<u17_comic>(cataurl, string.Format(paras, i), Encoding.GetEncoding("utf-8"), Encoding.GetEncoding("utf-8"));
                foreach (var x in booklst.comic_list)
                {
                    try
                    {
                        sort = sort + 1;
                        string bookurl = $"http://www.u17.com/comic/{x.comic_id}.html";
                        var doc = web.Load(bookurl);
                        if (doc.DocumentNode.SelectSingleNode("//title") != null)
                        {
                            string title = doc.DocumentNode.SelectSingleNode("//title").InnerText.Trim();
                            string comicname = title.Split('_')[0];
                            string authorname = title.Split('_')[1];

                            //var scriptnode = doc.DocumentNode.SelectSingleNode("//script[contains(text(),'var cover_url =')]");
                            //Match match = Regex.Match(scriptnode.InnerText, "cover_url = \"(?<key1>.*?)\"");
                            string comiccover = doc.DocumentNode.SelectSingleNode("//div[@class=\"cover\"]").SelectSingleNode(".//img").Attributes["src"].Value;
                            

                            string bookdesc = doc.DocumentNode.SelectSingleNode("//div[@class=\"info\"]").SelectNodes(".//p").Last().InnerText.Trim();
                            var line1 = doc.DocumentNode.SelectSingleNode("//div[@class=\"line1\"]");
                            string isfinished = "";
                            string hot = "";
                            string theme = "";
                            if (line1.InnerText.Contains("类别："))
                            {
                                isfinished = line1.InnerText.Contains("连载中") ? "连载中" : "已完结";
                                hot = line1.InnerText.Substring(line1.InnerText.IndexOf("总点击：")).Replace("总点击：", "").Trim();
                                theme = line1.SelectSingleNode(".//span").InnerText.Replace(" ", "").Replace("/", ",").Trim();
                            }
                            else
                            {
                                var line2 = doc.DocumentNode.SelectSingleNode("//div[@class=\"cf line2\"]").SelectNodes(".//span");
                                isfinished = line2[0].InnerText.Contains("连载中") ? "连载中" : "已完结";
                                hot = line2[1].InnerText.Trim();
                                var tags = line1.SelectNodes(".//a");
                                foreach (var tag in tags)
                                {
                                    if (!tag.InnerText.Trim().IsNullOrWhiteSpace())
                                    {
                                        theme = theme + " " + tag.InnerText.Trim();
                                    }
                                }
                                theme = theme.Trim().Replace(" ", ",");
                            }

                            var cpnode = doc.DocumentNode.SelectSingleNode("//div[@class=\"left_tag\"]").SelectSingleNode(".//img");

                            var copyright = cpnode==null?"":cpnode.Attributes["title"].Value.Replace("有","").Replace("妖", "").Replace("气","").Replace("作品", "").Replace("刊载", "");

                            comiclst.Add(new Comic()
                            {
                                comicname = comicname,
                                authorname = authorname,
                                bookurl = bookurl,
                                comiccoversource = comiccover,
                                comiccoverlocal = "",
                                comicdesc = bookdesc,
                                comicid = 3 + "_" + bookurl.Split('/').LastOrDefault().Replace(".html", ""),

                                isfinished = isfinished,
                                theme = theme,
                                isvip = "0",
                                source = 3,
                                stopcrawer = false,
                                isoffline = true,
                                recrawer = false,
                                shortdate = shortdate,
                                modify = dt,
                                comicsort = sort,
                                hot = hot,
                                copyright = copyright
                            });
                        }
                    }
                    catch (Exception ex)
                    {

                        continue;
                    }
                    
                }


            }

            service.ComicCompareBatchAdd(comiclst, 3, DateTime.Now.ToDateStr());
        }
    }
}
