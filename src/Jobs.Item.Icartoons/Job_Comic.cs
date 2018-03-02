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

namespace Jobs.Item.Icartoons
{
    [DisallowConcurrentExecution]
    public sealed class Job_Comic:IJob
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
            var t = _helper.Get<icartoons_comic>("http://www.icartoons.cn/webapi/web/theme_contents.json?type=1&catid=0&content_type=1&page=1&pagesize=5000&sort=0", Encoding.GetEncoding("utf-8"));
            int sort = 0;
            foreach (var x in t.items)
            {
                try
                {
                    sort = sort + 1;
                    string bookurl = $"http://www.icartoons.cn/webapi/web/detail.json?type=1&content_id={x.serial_id}";

                    var data = _helper.Get<icartoons_comic_desc>(bookurl, Encoding.GetEncoding("utf-8"));
                    comiclst.Add(new Comic()
                    {
                        comicname = x.title,
                        authorname = data.author,
                        bookurl = $"http://www.icartoons.cn/portal/csource.html?content_id={x.serial_id}",
                        comiccoversource = x.cover,
                        comiccoverlocal = "",
                        comicdesc = data.description,
                        comicid = "8_" + x.serial_id,
                        isfinished = x.desc.StartsWith("全") ? "已完结" : "连载中",
                        theme = data.theme,
                        isvip = "0",
                        source = 8,
                        stopcrawer = false,
                        isoffline = false,
                        recrawer = false,
                        shortdate = shortdate,
                        modify = dt,
                        comicsort = sort,
                        hot = x.views

                    });
                }
                catch (Exception ex)
                {

                    continue;
                }
                
            }

            service.ComicCompareBatchAdd(comiclst, 8, DateTime.Now.ToDateStr());
        }
    }
}
