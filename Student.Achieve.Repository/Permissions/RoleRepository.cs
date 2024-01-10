using Student.Achieve.IRepository;
using Student.Achieve.Repository.Base;
using Student.Achieve.Model.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Student.Achieve.Repository
{	
	/// <summary>
	/// RoleRepository
	/// </summary>	
	public  class RoleRepository : BaseRepository<Role>, IRoleRepository
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> SaveRole(string roleName)
        {
            Role role = new Role(roleName);
            Role model = new Role();
            var userList = await base.Query(a => a.Name == role.Name && a.Enabled);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(role);
                model = await base.QueryById(id);
            }

            return model;

        }
        public async Task<string> GetUserRoleNameByRid(int roleId)
        {
            string name = null;
            var userRole = await base.Query(a => a.Id == roleId);
            if (userRole.Count > 0)
            {
                name = userRole.FirstOrDefault().Name;
            }
            return name;
        }
    }
}
