using Jobs.Core.Business.Manager;
using Jobs.Core.Common.Comparer;
using Jobs.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.Services
{
    public class CrawerService
    {
        
        public bool ComicCompareBatchAdd(List<Comic> clst,int source,string shortdate)
        {
            BaseManager<Comic> manager = new BaseManager<Comic>();
            List<Comic> comicdb = manager.Query(x =>x.source==source && x.shortdate == shortdate).ToList();
            List<Comic> add = clst.Except(comicdb, new Comic_Comparer()).ToList();
            if (add.Count>0)
            {
                return manager.BatchAdd(add);
            }
            return true;
        }

        public bool ChapterCompareBatchAdd(List<Chapter> cplst,int source, string shortdate)
        {
            BaseManager<Chapter> manager = new BaseManager<Chapter>();
            List<Chapter> chapterdb = manager.Query(x => x.source == source && x.shortdate == shortdate).ToList();
            List<Chapter> add = cplst.Except(chapterdb, new Chapter_Comparer()).ToList();
            if (add.Count>0)
            {
                return manager.BatchAdd(add);
            }
            return true;
        }
    }
}
