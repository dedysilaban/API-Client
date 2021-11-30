using API.Context;
using API.Models;
using API.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class EmployeeRepository : GeneralRepository<MyContext, Employee, string>
    {
        private readonly MyContext context;

        public EmployeeRepository(MyContext context) : base(context)
        {
            this.context = context;
        }
        public static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }
        public int Register(RegisterVM registerVM)
        {

            Employee e = new Employee()
            {
                NIK = registerVM.NIK,
                Firstname = registerVM.Firstname,
                Lastname = registerVM.Lastname,
                Phone = registerVM.Phone,
                Birthdate = registerVM.Birthdate,
                Salary = registerVM.Salary,
                Email = registerVM.Email,
                Gender = (Models.Gender)registerVM.Gender
            };
            var cekNik = context.Employees.Find(registerVM.NIK);
            var cekEmail = context.Employees.Where(b => b.Email == registerVM.Email).FirstOrDefault();
            var cekPhone = context.Employees.Where(b => b.Phone == registerVM.Phone).FirstOrDefault();

            if (cekNik != null)
            {
                return 2;
            }
            if(cekEmail != null)
            {
                return 3;
            }
            if (cekPhone != null)
            {
                return 4;
            }
            context.Employees.Add(e);
            context.SaveChanges();

            Account a = new Account()
            {
                NIK = registerVM.NIK,

                Password = BCrypt.Net.BCrypt.HashPassword(registerVM.Password, GetRandomSalt())
            };
            context.Accounts.Add(a);
            context.SaveChanges();

            Education ed = new Education()
            {
                Degree = registerVM.Degree,
                GPA = registerVM.GPA,
                UniversityId = registerVM.UniversityId
            };
            context.Educations.Add(ed);
            context.SaveChanges();

            Profiling p = new Profiling()
            {
                NIK = e.NIK,
                EducationId = ed.EducationId
            };
            context.Profilings.Add(p);
            var result = context.SaveChanges();
            int cekRole = registerVM.RoleId;
            int idRole;
            if (cekRole == 0 || cekRole == 1)
            {
                idRole = 1;
            }
            else if(cekRole == 2)
            {
                idRole = 2;
            }
            else
            {
                idRole = 3;
            }

            AccountRole accountRole = new AccountRole()
            {
                NIK = a.NIK,
                RoleId = idRole
            };
            context.AccountRoles.Add(accountRole);
            context.SaveChanges();

            return result;
        }


        public IEnumerable<RegisterVM> GetProfile()
        {
            var getProfile = (from e in context.Employees
                              join a in context.Accounts on e.NIK equals a.NIK
                              join p in context.Profilings on a.NIK equals p.NIK
                              join ed in context.Educations on p.EducationId equals ed.EducationId
                              join u in context.Universities on ed.UniversityId equals u.UniversityId
                              join acr in context.AccountRoles on a.NIK equals acr.NIK
                              join r in context.Roles on acr.RoleId equals r.RoleId
                              select new RegisterVM
                              {
                                  NIK = e.NIK,
                                  Firstname = e.Firstname,
                                  Lastname = e.Lastname,
                                  Phone = e.Phone,
                                  Birthdate = e.Birthdate,
                                  Salary = e.Salary,
                                  Email = e.Email,
                                  Gender = (ViewModel.Gender)e.Gender,
                                  Password = a.Password,
                                  Degree = ed.Degree,
                                  GPA = ed.GPA,
                                  UniversityId = ed.UniversityId,
                                  Name = u.Name,
                                  RoleId = r.RoleId,
                                  NameRole = r.NameRole
                                  
                              });
            return getProfile.ToList();
        }
    }
}
