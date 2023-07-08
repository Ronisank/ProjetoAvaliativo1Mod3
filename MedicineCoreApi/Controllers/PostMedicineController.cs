using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicineCoreApi.Context;

namespace MedicineCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostMedicineController : ControllerBase
    {
        private readonly MedicineCoreApiContext _context;

        public PostMedicineController(MedicineCoreApiContext context)
        {
            _context = context;
        }

        // GET: api/PostMedicine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostMedicine>>> GetMedicines()
        {
          if (_context.Medicines == null)
          {
              return NotFound();
          }
            return await _context.Medicines.ToListAsync();
        }

        // GET: api/PostMedicine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostMedicine>> GetPostMedicine(int id)
        {
          if (_context.Medicines == null)
          {
              return NotFound();
          }
            var postMedicine = await _context.Medicines.FindAsync(id);

            if (postMedicine == null)
            {
                return NotFound();
            }

            return postMedicine;
        }

        // PUT: api/PostMedicine/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostMedicine(int id, PostMedicine postMedicine)
        {
            if (id != postMedicine.Id)
            {
                return BadRequest();
            }

            _context.Entry(postMedicine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostMedicineExists(id))
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

        // POST: api/PostMedicine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostMedicine>> PostPostMedicine(PostMedicine postMedicine)
        {
          if (_context.Medicines == null)
          {
              return Problem("Entity set 'MedicineCoreApiContext.Medicines'  is null.");
          }
            _context.Medicines.Add(postMedicine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostMedicine", new { id = postMedicine.Id }, postMedicine);
        }

        // DELETE: api/PostMedicine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostMedicine(int id)
        {
            if (_context.Medicines == null)
            {
                return NotFound();
            }
            var postMedicine = await _context.Medicines.FindAsync(id);
            if (postMedicine == null)
            {
                return NotFound();
            }

            _context.Medicines.Remove(postMedicine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostMedicineExists(int id)
        {
            return (_context.Medicines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
