using SqlSugar;
using Student.Achieve.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    public class Team : RootEntity
    {
        [SugarColumn(Length = 255, IsNullable = true)]
        public string name { get; set; }
        [SugarColumn(IsNullable = true)]
        public int type { get; set; }
        [SugarColumn(IsNullable = true)]
        public int head_id { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string member_ids { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string task_ids { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string subordinate_team_ids { get; set; }
        [SugarColumn(IsNullable = true)]
        public int status { get; set; }
        [SugarColumn(Length = 100,IsNullable = true)]
        public string contact { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string belongs { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string remark { get; set; }
        [SugarColumn(Length = 255, IsNullable = true)]
        public string create_by { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public DateTime update_time { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string update_by { get; set; }
    }
}
