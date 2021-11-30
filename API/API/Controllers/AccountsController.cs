using API.Base;
using API.Interface;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseController<Account, AccountRepository, string>
    {
        private AccountRepository accountRepository;
        public AccountsController(AccountRepository accountRepository) : base(accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [HttpPost]
        [Route("Login")]

        public ActionResult<LoginVM> GetLogin(LoginVM loginVM)
        {

            var result = accountRepository.SignIn(loginVM);
            if (result == 1)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Email atau nomor telepone salah, tidak bisa login" });
            }
            else if (result == 2)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, result, message = "Password salah" });
            }
            else if (result == 3)
            {
                return Ok(new { status = HttpStatusCode.OK, data = accountRepository.GetProfileLogin(loginVM.Email), message = "Berhasil Login" }); ;
            }
            return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Login gagal" });
        }

        [HttpPost]
        [Route("ChangePass")]
        public ActionResult ChangePass(ChangePassVM changePassVM)
        {
            var result = accountRepository.ChangePassword(changePassVM);
            if (result == 2)
            {
                return NotFound(new { status = HttpStatusCode.NotFound, result = result, messege = $"Email tidak ditemukan" });
            }
            else if (result == 3)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, result = result, messege = "Password Konfirmasi tidak sesuai dengan new password" });
            }
            else if (result == 1)
            {
                return Ok(new { status = HttpStatusCode.OK, result = result, messege = "Password Telah diperbaharui" });
            }
            else if (result == 4)
            {
                return BadRequest(new { status = HttpStatusCode.BadRequest, result = result, messege = "Password lama anda tidak sesuai!!" });
            }
            return NotFound(new { status = HttpStatusCode.NotFound, result = result, messege = "tidak ada data ditemukan" });
        }

        [HttpPost("ForgotPassword")]
        public ActionResult ForgotPassword(ForgotPassVM forgotPassVM)
        {
            var result = accountRepository.ForgotPassword(forgotPassVM);

            if (result == 1)
            {
                return Ok(new { status = HttpStatusCode.OK, result, message = "Password Baru Terkirim Ke Email Anda" });
            }
            return NotFound(new { status = HttpStatusCode.NotFound, result, message = "Data tidak ditemukan" });
        }



        [Authorize(Roles = "Director, Manager")]
        [HttpGet("TestJWT")]

        public ActionResult TestJWT()
        {
            return Ok("Test JWT Berhasil");
        }      
    }
}



