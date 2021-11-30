using API.Context;
using API.Models;
using API.ViewModel;
using API.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using System.Net.Mail;

namespace API.Repository.Data
{
    public class AccountRepository : GeneralRepository<MyContext, Account, string>
    {
        private readonly MyContext context;
        public AccountRepository(MyContext context) : base(context)
        {
            this.context = context;
        }
        public int SignIn(LoginVM loginVM)
        {
            var result = 0;
            try
            {
                var cekEmail = context.Employees.Where(e => e.Email == loginVM.Email).FirstOrDefault();
                var cekPhone = context.Employees.Where(e => e.Phone == loginVM.Phone).FirstOrDefault();

                if (cekEmail != null || cekPhone != null)
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

        public IEnumerable <RegisterVM> GetProfileLogin(string key)
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

        public int ForgotPassword(ForgotPassVM forgotPassVM)
        {
            var cekEmail = context.Employees.Where(b => b.Email == forgotPassVM.Email).FirstOrDefault();

            if (cekEmail != null)
            {
                string passNew = Guid.NewGuid().ToString().Substring(0, 12);
                var name = cekEmail.Firstname + " " + cekEmail.Lastname; 
                var original = context.Accounts.Find(cekEmail.NIK);
                DateTimeOffset now = (DateTimeOffset)DateTime.Now;
                original.Password = Hashing.Hashing.HashPassword(passNew);
                context.SaveChanges();

                if (original != null)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(forgotPassVM.Email);
                    mail.From = new MailAddress("dedysilaban01@gmail.com", "Forgot Password!!", System.Text.Encoding.UTF8);
                    mail.Subject = "Forgot Password System" + now;
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = "Hi" + " " + name + " Your <br /> New Password is : " + passNew;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    SmtpClient client = new SmtpClient();
                    client.Credentials = new System.Net.NetworkCredential("dedysilaban01@gmail.com", "silaban_12");
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(mail);
                        return 1;
                    }
                    catch (Exception)
                    {
                        return 3;
                    }
                }
            }
            return 2;
        }

        public int ChangePassword(ChangePassVM changePassVM)
        {
            var cekEmail = context.Employees.Where(b => b.Email == changePassVM.Email).FirstOrDefault();

            if (cekEmail != null)
            {
                if (changePassVM.NewPassword == changePassVM.ConfirmPassword)
                {
                    var password = (from e in context.Employees
                                    where e.Email == changePassVM.Email
                                    join a in context.Accounts on e.NIK equals a.NIK
                                    select a.Password).Single();
                    var nik = (from e in context.Employees
                               where e.Email == changePassVM.Email
                               join a in context.Accounts on e.NIK equals a.NIK
                               select e.NIK).Single();
                    var cekPass = Hashing.Hashing.ValidatePassword(changePassVM.OldPassword, password);
                    if(cekPass == false)
                    {
                        return 4;
                    }
                    var original = context.Accounts.Find(nik);
                    if(original != null)
                    {
                        original.Password = Hashing.Hashing.HashPassword(changePassVM.NewPassword);
                        context.SaveChanges();
                        return 1;
                    }
                }
                else
                {
                    return 3;
                }
            }
            return 2;
        }
    }
}