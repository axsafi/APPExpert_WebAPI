using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APPExpert_WebAPI.Entities;
using APPExpert_WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace APPExpert_WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        private readonly DBContext _dbcontext;

        public SecuredController(DBContext context)
        {
            _dbcontext = context;
        }

        // GET: api/Secured
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMaster>>> GetUserMaster()
        {
            return await _dbcontext.UserMaster.ToListAsync();
        }

        // GET: api/Secured/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserMaster>> GetUserMaster(int id)
        {
            var Username = new SqlParameter("@Username", "admin");
            var Password = new SqlParameter("@Password", "admin");

            var users = _dbcontext
                        .UserMaster
                        .FromSqlRaw("exec SpAPP_GetUser @Username, @Password", Username, Password)
                        .ToList().SingleOrDefault();

            var userMaster = await _dbcontext.UserMaster.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // PUT: api/Secured/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserMaster(int id, UserMaster userMaster)
        {
            if (id != userMaster.Id)
            {
                return BadRequest();
            }

            _dbcontext.Entry(userMaster).State = EntityState.Modified;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMasterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Secured
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserMaster>> PostUserMaster(UserMaster userMaster)
        {
            _dbcontext.UserMaster.Add(userMaster);
            await _dbcontext.SaveChangesAsync();

            return CreatedAtAction("GetUserMaster", new { id = userMaster.Id }, userMaster);
        }

        // DELETE: api/Secured/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserMaster>> DeleteUserMaster(int id)
        {
            var userMaster = await _dbcontext.UserMaster.FindAsync(id);
            if (userMaster == null)
            {
                return NotFound();
            }

            _dbcontext.UserMaster.Remove(userMaster);
            await _dbcontext.SaveChangesAsync();

            return userMaster;
        }

        private bool UserMasterExists(int id)
        {
            return _dbcontext.UserMaster.Any(e => e.Id == id);
        }
    }
}
