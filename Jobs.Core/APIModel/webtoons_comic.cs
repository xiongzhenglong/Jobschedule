using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class webtoons_comic
    {
        public int code { get; set; }
        public List<webtoonsDatum> data { get; set; }
        public string message { get; set; }
    }

    public class webtoonsDatum
    {
        public string titleNo { get; set; }
        public int count { get; set; }
    }

}
