
using Student.Achieve.IRepository.Base;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Student.Achieve.IRepository
{
    public partial interface IClassRepository : IBaseRepository<Class>
    {
        Task<PageModel<Class>> GetQueryPageOfMapperTb(Expression<Func<Class, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
    }
}