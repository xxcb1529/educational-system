using Student.Achieve.IRepository.Base;
using Student.Achieve.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student.Achieve.IRepository
{
    public partial interface ITaskRepository : IBaseRepository<Model.Models.Task>
    {
        Task<List<Model.Models.Task>> GetByIdsAsync(int[] ids);
    }
}
