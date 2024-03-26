
using Student.Achieve.IRepository.Base;
using Student.Achieve.Model.Models;
using System.Threading.Tasks;

namespace Student.Achieve.IRepository
{
    public partial interface ITeacherRepository : IBaseRepository<Teacher>
    {
        Task<string> GetTeacherRoleNameStr(string loginName, string loginPwd);
    }
}