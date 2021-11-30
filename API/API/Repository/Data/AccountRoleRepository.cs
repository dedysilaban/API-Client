using API.Context;
using API.Models;
using API.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class AccountRoleRepository : GeneralRepository<MyContext, AccountRole, string>
    {
        private readonly MyContext context;
        public AccountRoleRepository(MyContext context) : base(context)
        {
            this.context = context;
        }

        public int SignIn(LoginVM loginVM)
        {
            var result = 0;
            try
            {
                var cekEmail = context.Employees.
                    Include("Account").Where(e => e.Email == loginVM.Email || e.Phone == loginVM.Phone).FirstOrDefault();

                if (cekEmail != null)
                {
                    var getPass = (from e in context.Employees
                                   where e.Email == loginVM.Email || e.Phone == loginVM.Phone
                                   join a in context.Accounts on e.NIK equals a.NIK
                                   select a.Password).Single();

                    var validPass = BCrypt.Net.BCrypt.Verify(loginVM.Password, getPass);

                    if (validPass == true)
                    {
                        result = 3;
                    }
                    else
                    {
                        result = 2;
                    }
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        public string[] GetRole(string email)
        {
            var getData = context.Employees.Where(e => e.Email == email).FirstOrDefault();
            var getRole = (from acr in context.AccountRoles
                           join r in context.Roles
                           on acr.RoleId equals r.RoleId
                           select new
                           {
                               NIK = acr.NIK,
                               RoleName = r.NameRole
                           }).Where(acr => acr.NIK == getData.NIK).ToList();
            List<string> result = new List<string>();
            foreach (var item in getRole)
            {
                result.Add(item.RoleName);
            }
            return result.ToArray();
        }
        public IEnumerable<RegisterVM>ProfileLogin(string key)
        {
            var getProfile = (from e in context.Employees
                              join a in context.Accounts on e.NIK equals a.NIK
                              join p in context.Profilings on a.NIK equals p.NIK
                              join ed in context.Educations on p.EducationId equals ed.EducationId
                              join u in context.Universities on ed.UniversityId equals u.UniversityId
                              join acr in context.AccountRoles on a.NIK equals acr.NIK
                              join r in context.Roles on acr.RoleId equals r.RoleId
                              where key == e.Email
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
                                  Degree = ed.Degree,
                                  GPA = ed.GPA,
                                  UniversityId = ed.UniversityId,
                                  Name = u.Name,
                                  RoleId = r.RoleId,
                                  NameRole = r.NameRole
                              });
            return getProfile.ToList();
        }

        public int AddManager(AccountRole accountRole)
        {
            try
            {
                context.AccountRoles.Add(accountRole);
                var result = context.SaveChanges();
                return result;
            }
            catch
            {
                return 0;
            }
        }

    }
}
