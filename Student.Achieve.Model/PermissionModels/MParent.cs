using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Achieve.Model.Models
{
    public class MParent: RootEntity
    {
        public MParent()
        {
            //this.ChildModule = new List<Module>();
            //this.ModulePermission = new List<ModulePermission>();
            //this.RoleModulePermission = new List<RoleModulePermission>();
        }
        /// <summary>
        /// 接口名称
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string IName { get; set; }
        /// <summary>
        /// 接口链接
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Description { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string CreateBy { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public int Enabled { get; set; }
    }
}
