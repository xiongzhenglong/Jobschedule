using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.Entity
{
  
    public partial class Comic
    {
        [SugarColumn(IsIgnore = true)]
        public double comicsort2 { get; set; }
    }
}
