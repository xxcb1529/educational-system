using Student.Achieve.IRepository.Base;
using Student.Achieve.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Student.Achieve.IRepository
{
    public interface IHomeDataRepository : IBaseRepository<HomeData>
    {
        IEnumerable<Class> GetAllClasses();
        IEnumerable<Class> GetClassesAddedWithinLastYear();
    }
}
