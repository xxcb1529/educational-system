using NPOI.HSSF.Record;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    /// <summary>
    /// 用户跟角色关联表
    /// </summary>
    public class UserRole : RootEntity
    {
        public UserRole() { }

        public UserRole(int uid, int rid, int tId)
        {
            UserId = uid;
            RoleId = rid;
            TId = tId;
            CreateTime = DateTime.Now;
            IsDeleted = false;
            CreateTime = DateTime.Now;
            TId = tId;
        }



        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
        public int TId { get; set; }

    }
}
