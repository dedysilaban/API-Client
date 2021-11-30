using API.Context;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interface
{
    public class EmployeeRepository : OldIEmployeeRepository
    {
        private readonly MyContext context;

        public EmployeeRepository(MyContext context)
        {
            this.context = context;
        }

        public IEnumerable<Employee> Get()
        {
            return context.Employees.ToList();
        }
           
        public Employee Get(string NIK)
        {
 
            return context.Employees.Find(NIK);
        }

        public int Insert(Employee employee)
        {
            var cekNik = context.Employees.Find(employee.NIK);

            if (cekNik != null)
            {
                return 2;
            }
            else {
                context.Employees.Add(employee);
                var result = context.SaveChanges();
                return result;
            }
        }

        public int Update(Employee employee)
        {
            context.Entry(employee).State = EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }

        public int Delete(string NIK)
        {
            var entity = context.Employees.Find(NIK);
            context.Remove(entity);
            var result = context.SaveChanges();
            return result;
        }

    }
   


}
