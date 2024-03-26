using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student.Achieve.IRepository;
using Student.Achieve.Model.Models;
using Student.Achieve.Repository.Base;

namespace Student.Achieve.Repository
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        IUserRoleRepository _userRoleRepository;
        IRoleRepository _roleRepository;
        public TeacherRepository(IUserRoleRepository userRoleRepository, IRoleRepository roleRepository)
        {
            this._userRoleRepository = userRoleRepository;
            this._roleRepository = roleRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<string> GetTeacherRoleNameStr(string loginName, string loginPwd)
        {
            string roleName = "";
            var user = (await base.Query(a => a.TeacherNo == loginName && a.Password == loginPwd)).FirstOrDefault();
            var roleList = await _roleRepository.Query(a => a.IsDeleted == false);
            if (user != null)
            {
                var userRoles = await _userRoleRepository.Query(ur => ur.TId == user.Id);
                if (userRoles.Count > 0)
                {
                    var arr = userRoles.Select(ur => ur.RoleId.ObjToString()).ToList();
                    var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));

                    roleName = string.Join(',', roles.Select(r => r.Name).ToArray());
                }
            }
            return roleName;
        }
    }
}
