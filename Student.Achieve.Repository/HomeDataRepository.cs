using Student.Achieve.Repository.Base;
using System;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using Student.Achieve.IRepository;
using System.Collections.Generic;

namespace Student.Achieve.Repository
{
    public class HomeDataRepository : BaseRepository<HomeData>, IHomeDataRepository
    {
        public IEnumerable<Class> GetAllClasses() {
            return new Class[] { };
        
        }
        public IEnumerable<Class> GetClassesAddedWithinLastYear() {
            return new Class[] { };
        }
    }
}
