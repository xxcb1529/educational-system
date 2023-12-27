using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Student.Achieve.Model.Models
{
    /// <summary>
    /// 班级表
    /// </summary>
    public class Class : RootEntity
    {

        /// <summary>
        /// 班级编号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ClassNo { get; set; }




        [SugarColumn(Length = 50, IsNullable = true)]
        public string Name { get; set; }//名称


        public int GradeId { get; set; } //年级ID
        [SugarColumn(IsIgnore = true)]
        public Grade Grade { get; set; } //班级所属年级


        /// <summary>
        /// 院系
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Department { get; set; }//系别

        /// <summary>
        /// 系主任
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string DepManager { get; set; }//
        /// <summary>
        /// 专业
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Specialized { get; set; }//

        /// <summary>
        /// 班主任
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string TeacherCharge { get; set; }//
        /// <summary>
        /// 辅导员
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Counsellor { get; set; }//


        [SugarColumn(IsIgnore = true)]
        public List<Students> studentList { get; set; } = new List<Students>(); //班级下的学生


    }
}
