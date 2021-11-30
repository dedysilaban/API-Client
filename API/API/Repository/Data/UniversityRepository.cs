using API.Context;
using API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class UniversityRepository : GeneralRepository<MyContext, University, int>
    {
        private readonly MyContext context;
        public UniversityRepository(MyContext context) : base(context)
        {
            this.context = context;
        }
        public IEnumerable GetUniversity()
        {
            var result = from a in context.Accounts
                         join p in context.Profilings on a.NIK equals p.NIK
                         join ed in context.Educations on p.EducationId equals ed.EducationId
                         join u in context.Universities on ed.UniversityId equals u.UniversityId
                         group a by u.UniversityId into b
                         select new
                         {
                             universityId = b.Key,
                             value = b.Count()
                         };
            return result;
        }
    }
}
