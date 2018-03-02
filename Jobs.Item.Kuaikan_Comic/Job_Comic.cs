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

namespace Jobs.Item.Kuaikan
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
            HttpWebHelper _helper = new HttpWebHelper();
            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("19", "日常");
            tags.Add("20", "恋爱");
            tags.Add("22", "奇幻");
            tags.Add("23", "剧情");
            tags.Add("24", "爆笑");
            tags.Add("27", "治愈");
            tags.Add("40", "完结");
            tags.Add("41", "三次元");
            tags.Add("46", "古风");
            tags.Add("47", "校园");
            tags.Add("48", "都市");
            tags.Add("49", "少年");
            tags.Add("52", "总裁");
            tags.Add("53", "栏目");
            tags.Add("54", "正能量");
            tags.Add("56", "传统");
            tags.Add("57", "日漫");
            Dictionary<string, Comic> comicdic = new Dictionary<string, Comic>();
            try
            {
                foreach (var tag in tags)
                {
                    var t = _helper.Get<kuaikan_comic>($"http://www.kuaikanmanhua.com/web/tags/{tag.Key}?count=1800&page=0", Encoding.GetEncoding("utf-8"));
                    var comiclst = new List<Comic>();
                    if (t.status_code == 200)
                    {
                        foreach (var topic in t.data.topics)
                        {
                            string comicid = "12_" + topic.id;
                            if (!comicdic.ContainsKey(comicid))
                            {
                                comicdic.Add(comicid, new Comic()
                                {
                                    comicname = topic.title,
                                    authorname = topic.user == null ? "" : topic.user.nickname,
                                    bookurl = $"http://www.kuaikanmanhua.com/web/topic/{topic.id}/",
                                    comiccoversource = topic.vertical_image_url,
                                    comiccoverlocal = "",
                                    comicdesc = topic.description,
                                    comicid = "12_" + topic.id,
                                    isfinished = topic.description.Substring(topic.description.IndexOf('【') + 1).Split('/')[1].Contains("更新")? "连载中" : "已完结",
                                    theme = tag.Value,
                                    isvip = "0",
                                    source = 12,
                                    stopcrawer = false,
                                    isoffline = false,
                                    recrawer = false,
                                    shortdate = DateTime.Now.ToDateStr(),
                                    modify = DateTime.Now,
                                    comicsort = null,
                                    hot = topic.likes,
                                    copyright = topic.description.Substring(topic.description.IndexOf('【') + 1).Split('/')[0],
                                });
                            }
                           
                        }
                    }
                    

                }

                var t2 = _helper.Get<kuaikan_comic>($"http://www.kuaikanmanhua.com/web/tags/0?count=1800&page=0", Encoding.GetEncoding("utf-8"));
                var comiclst2 = new List<Comic>();
                if (t2.status_code == 200)
                {
                    int sort = 0;
                    foreach (var topic in t2.data.topics)
                    {
                        sort = sort + 1;
                        comiclst2.Add(new Comic()
                        {
                            comicname = topic.title,
                            authorname = topic.user == null ? "" : topic.user.nickname,
                            bookurl = $"http://www.kuaikanmanhua.com/web/topic/{topic.id}/",
                            comiccoversource = topic.vertical_image_url,
                            comiccoverlocal = "",
                            comicdesc = topic.description,
                            comicid = "12_" + topic.id,
                            isfinished = "连载中",
                            theme = "",
                            isvip = "0",
                            source = 12,
                            stopcrawer = false,
                            isoffline = false,
                            recrawer = false,
                            shortdate = DateTime.Now.ToDateStr(),
                            modify = DateTime.Now,
                            comicsort = sort,
                            hot = topic.likes

                        });
                    }

                }

                foreach (var item in comicdic)
                {
                    var comic = comiclst2.Where(x => x.comicid == item.Key).FirstOrDefault();
                    if (comic!=null)
                    {
                        item.Value.comicsort = comic.comicsort;
                    }
                   
                }
                List<Comic> addlst = new List<Comic>();
                foreach (var item in comicdic)
                {
                    addlst.Add(item.Value);
                }
                service.ComicCompareBatchAdd(addlst, 12, DateTime.Now.ToDateStr());
            }
            catch (Exception ex)
            {
                _logger.Error("Job_Comic 执行过程中发生异常:" + ex.ToString());
            }
            finally
            {
                _logger.InfoFormat("Job_Comic Execute end ");
            }
        }
    }
}
