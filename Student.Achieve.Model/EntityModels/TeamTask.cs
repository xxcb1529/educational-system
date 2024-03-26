using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    public class TeamTask : RootEntity
    {
        [SugarColumn(IsNullable = true)]
        public int task_id { get; set; }
        [SugarColumn(IsNullable = true)]
        public int team_id { get; set; }
    }
}
