using API.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController <Entity, Repository, Key>: ControllerBase
        where Entity : class
        where Repository : IRepository<Entity, Key>
    {
        private readonly Repository repository;
        public BaseController(Repository repository)
        {
            this.repository = repository;

        }
        [HttpPost]
        public ActionResult Post(Entity entity, Key key)
        {
            var result = repository.Insert(entity, key);
            return Ok(result);
        }

        [HttpGet]
        public IEnumerable<Entity> Get()
        {
            var result = repository.Get();
            return result;
        }

        [HttpGet("{key}")]
        public ActionResult Get(Key key)
        {
            var exist = repository.Get(key);

            if(exist != null)
            {
                return Ok(exist);
            }
            return NotFound(new { status = HttpStatusCode.NotFound, result = exist, messege = $"Data dengan NIK: {key} tidak ditemukan"});
        }

        [HttpPut("{key}")]
        public ActionResult<Entity> Update(Entity entity, Key key)
        {
            try
            {
                var result = repository.Update(entity, key);
                return Ok(result);
            }
            catch
            {
                return NotFound(new { status = HttpStatusCode.NotFound, message = $"Data dengan NIK: {key} tidak ditemukan" });
            }
        }

        [HttpDelete("{key}")]
        public ActionResult Delete(Key key)
        {
            var exist = repository.Get(key);
            try
            {
                var result = repository.Delete(key);
                return Ok(result);
            }
            catch
            {
                return NotFound(new { status = HttpStatusCode.NotFound, result = exist, message = $"Data dengan NIK: {key} tidak ditemukan" });
            }
        }

    }
}
