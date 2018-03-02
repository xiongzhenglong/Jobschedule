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
    public sealed class Job_Chapter:IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Job_Chapter));
        private BaseManager<Chapter> chaptermanager = new BaseManager<Chapter>();
        private BaseManager<Comic> comicmanager = new BaseManager<Comic>();
        private CrawerService service = new CrawerService();

        public void Execute(IJobExecutionContext context)
        {
            string date = DateTime.Now.ToDateStr();
            List<Comic> clst = comicmanager.Query(x => x.source == 8 && x.shortdate == date);
            List<Chapter> plst = new List<Chapter>();
            HttpWebHelper _helper = new HttpWebHelper();

            foreach (var c in clst)
            {
                try
                {
                    var bookdata = _helper.Get<icartoons_chapter>($"http://www.icartoons.cn/webapi/web/serials.json?type=1&content_id={c.comicid.Replace("8_", "")}&sort=2", Encoding.GetEncoding("utf-8"));
                    var x = bookdata.items.First();
                    plst.Add(new Chapter()
                    {
                        chapterid = 8 + "_" + x.set_id,
                        chaptername = x.title,
                        chapterurl = $"http://www.icartoons.cn/portal/creader.html?content_id={c.comicid.Replace("8_", "")}&set_id={x.set_id}",
                        sort = x.sortid,
                        comicid = c.comicid,
                        retry = 0,
                        source = 8,
                        downstatus = 0,
                        isvip = "0",
                        chaptersource = "",
                        chapterlocal = "",
                        modify = DateTime.Now,
                        shortdate = DateTime.Now.ToDateStr(),
                        chapterupdate = DateTime.Now.ToDateStr()
                    });
                }
                catch (Exception ex)
                {

                    continue;
                }
                

            }

            service.ChapterCompareBatchAdd(plst, 8, DateTime.Now.ToDateStr());
        }
    }
}
