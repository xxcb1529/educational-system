using Student.Achieve.IRepository;
using Student.Achieve.Repository.Base;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using System.Collections.Generic;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Achieve.Repository
{
    public class TaskRepository : BaseRepository<Model.Models.Task>, ITaskRepository
    {
        public async Task<List<Model.Models.Task>> GetByIdsAsync(int[] ids)
        {
            RefAsync<int> totalCount = 0;
            var list = await Db.Queryable<Model.Models.Task>()
                .Where(task => ids.Contains(task.Id) && task.IsDeleted == false)
                .ToPageListAsync(1, ids.Length, totalCount); // 使用ids的长度作为页面大小

            return list;
        }
    }
}
