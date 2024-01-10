using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    /// <summary>
    /// 教师表
    /// </summary>
    public class Teacher : RootEntity
    {


        [SugarColumn(IsIgnore = true)]
        public List<CCT> cct { get; set; } = new List<CCT>();


        /// <summary>
        /// 教师编号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string TeacherNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Password { get; set; }



        [SugarColumn(IsIgnore = true)]
        public Grade Grade { get; set; } //所属年级
        public int gradeId { get; set; }

        /// <summary>
        /// 班级id
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Class_ids { get; set; }

        /// <summary>
        /// 科目id
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string courseIds { get; set; }


    }
}
