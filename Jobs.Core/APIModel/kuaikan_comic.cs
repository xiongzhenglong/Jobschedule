using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.APIModel
{

    public class kuaikan_comic
    {
        public kuaikan_Data data { get; set; }
        public string message { get; set; }
        public int status_code { get; set; }
    }

    public class kuaikan_Data
    {
        public List<kuaikan_Topic> topics { get; set; }
    }

    public class kuaikan_Topic
    {
        public int comic_type { get; set; }
        public int comics_count { get; set; }
        public string cover_image_url { get; set; }
        public int created_at { get; set; }
        public string description { get; set; }
        public string discover_image_url { get; set; }
        public int exclusive_flag { get; set; }
        public int gender_bias { get; set; }
        public int id { get; set; }
        public int label_id { get; set; }
        public string likes { get; set; }
        public string male_cover_image { get; set; }
        public string male_vertical_cover_image { get; set; }
        public int order { get; set; }
        public int quality_certified { get; set; }
        public string title { get; set; }
        public string update_day { get; set; }
        public int update_status { get; set; }
        public int updated_at { get; set; }
        public User user { get; set; }
        public string vertical_image_url { get; set; }
    }

    public class User
    {
        public string avatar_url { get; set; }
        public int grade { get; set; }
        public int id { get; set; }
        public string nickname { get; set; }
        public int pub_feed { get; set; }
        public string reg_type { get; set; }
    }

}
