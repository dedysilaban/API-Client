using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountRolesController : BaseController<AccountRole, AccountRoleRepository, string>
    {
/*        private readonly Repository.Data.AccountRepository accountRepository;*/
        private readonly Repository.Data.AccountRoleRepository accountRoleRepository;
        public IConfiguration _configuration;
        public AccountRolesController(AccountRoleRepository accountRoleRepository, IConfiguration configuration) : base(accountRoleRepository)
        {
            this.accountRoleRepository = accountRoleRepository;
            this._configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult SignIn(LoginVM loginVm)
        {
            var ceks = accountRoleRepository.SignIn(loginVm);
            if (ceks == 2)
            {
                return NotFound(new JWTokenVM { Token = "", Messages = "Password Salah" });
            }
            else if (ceks == 1)
            {
                return NotFound(new JWTokenVM { Token = "", Messages = "Email dan  phone Salah" });
            }
            else if (ceks == 3)
            {
                var getUserData = accountRoleRepository.GetRole(loginVm.Email);
                var data = new LoginDataVM()
                {
                    Email = loginVm.Email,
                };
                var claims = new List<Claim>
                {
                new Claim("Email", data.Email),
                };

                foreach (var a in getUserData)
                {
                    claims.Add(new Claim(ClaimTypes.Role, a.ToString()));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                            signingCredentials: signIn
                            );
                var idToken = new JwtSecurityTokenHandler().WriteToken(token);
                claims.Add(new Claim("TokenSecurity", idToken.ToString()));

                return Ok(new { status = HttpStatusCode.OK, Token = idToken, data = accountRoleRepository.ProfileLogin(loginVm.Email), Messages = "Selamat berhasil login" });
            }
            return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Login gagal" });
        }

        [Authorize(Roles = "Director")]
        [HttpPut]
        [Route("addManager")]
        public ActionResult AddManager(AccountRole accountRole)
        {
            var result = accountRoleRepository.AddManager(accountRole);
            if (result == 1)
            {
                return Ok(new { status = HttpStatusCode.OK, message = "Data berhasil ditambahkan" });
            }
            return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Data gagal ditambahkan" });
        }
    }
}







/* var result = accountRepository.SignIn(loginVm);*/
/* if (result == 1)
 {
     return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Email atau nomor telepone salah, tidak bisa login" });
 }
 else if (result == 2)
 {
     return BadRequest(new { status = HttpStatusCode.BadRequest, result, message = "Password salah" });
 }
 else if (result == 3)
 {
     return Ok(new { status = HttpStatusCode.OK, Token = idToken, data = accountRepository.GetProfileLogin(loginVm.Email), message = "Berhasil Login" }); ;
 }
 return BadRequest(new { status = HttpStatusCode.BadRequest, message = "Login gagal" });*/




















