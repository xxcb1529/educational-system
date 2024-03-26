using System;
using System.Threading.Tasks;
using Student.Achieve.IRepository.Base;
using Student.Achieve.Model.Models;

namespace Student.Achieve.IRepository
{	
	/// <summary>
	/// IUserRoleRepository
	/// </summary>	
	public interface IUserRoleRepository : IBaseRepository<UserRole>//类名
    {
        Task<int> GetRoleIdByUid(int uID);
        Task<UserRole> SaveUserRole(int uid, int rid);
        Task<int> GetRoleIdByTid(int uID);

    }
}

	