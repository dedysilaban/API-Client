using API.Base;
using API.Models;
using API.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversitiesController : BaseController<University, UniversityRepository, int>
    {
        private Repository.Data.UniversityRepository universityRepository;

        public UniversitiesController(UniversityRepository universityRepository) : base(universityRepository)
        {
            this.universityRepository = universityRepository;
        }


        [HttpGet]
        [Route("University")]
        public ActionResult GetUniversity()
        {
            var result = universityRepository.GetUniversity(); 
            return Ok(new { status = HttpStatusCode.OK, result , messsage = "Data ditampilkan" });
           
        }
    }
}
