using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController<Employee, EmployeeRepository, string>
    {
        private EmployeeRepository employeeRepository;

        public EmployeesController(EmployeeRepository employeeRepository) : base(employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Post(RegisterVM registerVm)
        {
            var result = employeeRepository.Register(registerVm);
            if (result == 1)
            {
                return Ok(new { status = HttpStatusCode.OK, result, message = "Berhasil Register" });
            }
            else if (result == 2)
            {
                return NotFound(new { status = HttpStatusCode.NotFound, result, messege = "Register Gagal, NIK sudah terdaftar" });
            }
            else if (result == 3)
            {
                return NotFound(new { status = HttpStatusCode.NotFound, result, messege = "Register Gagal, Email sudah terdaftar" });
            }
            else if (result == 4)
            {
                return NotFound(new { status = HttpStatusCode.NotFound, result, messege = "Register Gagal!!, Phone sudah terdaftar" });
            }
            {
                return NotFound(new { HttpStatusCode.NotFound, result, messege = "Register Gagal" });
            }
        }

        [Authorize(Roles = "Director, Manager")]
        [HttpGet]
        [Route("Registers")]
        public ActionResult<RegisterVM> GetProfile()
        {
            var result = employeeRepository.GetProfile();
            if (result == null)
            {
                return NotFound(new { status = HttpStatusCode.NoContent, result, messageResult = "Data masih kosong" });
            }
            return Ok(new { status = HttpStatusCode.OK, result, messageResult = "Semua data berhasil ditampilkan" });
        }

        [HttpGet("TestCORS")]
        public ActionResult TestCORS()
        {
            return Ok("Test CORS berhasil");
        }
    } 
}
















/*        [HttpPost]
        [Route("Login")]*/

/*        public ActionResult LoginAutentifikasi(LoginVM loginVM)
        {
            var result = employeeRepository.SignIn(loginVM);
            if (result.Item1 == 2)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Email atau nomor telepone salah, tidak bisa login" });
            }
            else if (result.Item1 == 3)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, result, message = "Password salah" });
            }
            else if (result.Item1 == 1)
            {
                *//*return Ok(new { status = HttpStatusCode.OK, result, message = "Berhasil Login" });*//*
                return RedirectToAction("GetProfile", "Employees", new { key = result.Item2, message = "Login Berhasil" });
            }
            return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Login gagal" });
        }*/

/* [HttpGet("Profile/{key}")]
 public ActionResult<RegisterVM> GetProfile(string key)
 {
     var result = employeeRepository.GetProfile(key);
     if (result != null)
     {
         return Ok(new { status = HttpStatusCode.OK, result = result, message = "data berhasil ditampilkan" });
     }
     return NotFound(new { status = HttpStatusCode.NotFound, result = result, message = $"Data Kosong" });
 }*/

