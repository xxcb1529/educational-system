using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    public class Task : RootEntity
    {
        [SugarColumn(Length = 255, IsNullable = true)]
        public string name { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string description { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string priority { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string assign_team_ids { get; set; }

        [SugarColumn(Length = 11, IsNullable = true)]
        public int status { get; set; } = 4;

        [SugarColumn(IsNullable = true)]
        public DateTime start_time { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime end_time { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string remark { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string create_by { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public DateTime update_time { get; set; }

        [SugarColumn(Length = 255, IsNullable = true)]
        public string update_by { get; set; }

        public List<Task> children = new List<Task>();
        public string team_name;
    }
}
