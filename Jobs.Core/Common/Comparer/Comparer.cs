using Jobs.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.Common.Comparer
{
    public class Comic_Comparer : IEqualityComparer<Comic>
    {
        public bool Equals(Comic x, Comic y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            return x != null && y != null && x.bookurl == y.bookurl&& x.shortdate==y.shortdate ;

        }

        public int GetHashCode(Comic obj)
        {
            int hashchapterid = (obj.bookurl+obj.shortdate).GetHashCode();
            return hashchapterid;
        }
    }
    public class Chapter_Comparer : IEqualityComparer<Chapter>
    {
        public bool Equals(Chapter x, Chapter y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            return x != null && y != null && x.chapterid == y.chapterid&& x.shortdate == y.shortdate;

        }

        public int GetHashCode(Chapter obj)
        {
           
            return obj.GetHashCode();
            
        }
    }
}
