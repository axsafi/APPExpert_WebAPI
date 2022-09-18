using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APPExpert_WebAPI.Entities;
using APPExpert_WebAPI.Helpers;

namespace APPExpert_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        private readonly DataContext _context;

        public SecuredController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Secured
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMaster>>> GetUserMaster()
        {
            return await _context.UserMaster.ToListAsync();
        }

        // GET: api/Secured/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserMaster>> GetUserMaster(int id)
        {
            var userMaster = await _context.UserMaster.FindAsync(id);

            if (userMaster == null)
            {
                return NotFound();
            }

            return userMaster;
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

            _context.Entry(userMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            _context.UserMaster.Add(userMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserMaster", new { id = userMaster.Id }, userMaster);
        }

        // DELETE: api/Secured/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserMaster>> DeleteUserMaster(int id)
        {
            var userMaster = await _context.UserMaster.FindAsync(id);
            if (userMaster == null)
            {
                return NotFound();
            }

            _context.UserMaster.Remove(userMaster);
            await _context.SaveChangesAsync();

            return userMaster;
        }

        private bool UserMasterExists(int id)
        {
            return _context.UserMaster.Any(e => e.Id == id);
        }
    }
}
