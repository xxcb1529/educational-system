using Student.Achieve.Repository.Base;
using Student.Achieve.Model.Models;
using Student.Achieve.IRepository;
using System.Threading.Tasks;
using System.Linq;
using StackExchange.Redis;

namespace Student.Achieve.Repository
{
    /// <summary>
    /// UserRoleRepository
    /// </summary>	
    public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public async Task<UserRole> SaveUserRole(int uid, int rid)
        {
            UserRole userRole = new UserRole(uid, rid,0);

            UserRole model = new UserRole();
            var userList = await base.Query(a => a.UserId == userRole.UserId && a.RoleId == userRole.RoleId);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(userRole);
                model = await base.QueryById(id);
            }

            return model;

        }
        public async Task<int> GetRoleIdByUid(int uID)
        {
            int rid = 0;
            var Role =  await base.Query(a => a.UserId == uID);
            if(Role.Count > 0)
            {
                 rid = Role.FirstOrDefault().RoleId;
            }
            return rid;
        }
        public async Task<int> GetRoleIdByTid(int uID)
        {
            int Tid = 0;
            var Role = await base.Query(a => a.TId == uID);
            if (Role.Count > 0)
            {
                Tid = Role.FirstOrDefault().RoleId;
            }
            return Tid;
        }
    }
}
