using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressesApi.Data;
using AddressesApi.Models;
using System.Reflection;

namespace AddressesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressesContext _context;

        public AddressesController(AddressesContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet("{searchPhrase}/{field}/{order}")]
        public async Task<ActionResult<Address>> GetAddresses(string searchPhrase, string field, string order)
        {
            var list = _context.Addresses.ToList();
            var searchedAddresses = new List<Address>();

            if (string.IsNullOrEmpty(searchPhrase)) 
                searchedAddresses = list;
            else
            {
                foreach (var item in list)
                {
                    foreach (var prop in item.GetType().GetProperties().ToList())
                        if (item.GetType().GetProperty(prop.Name).GetValue(item, null).ToString().ToLower().Contains(searchPhrase.ToLower()))
                        {
                            searchedAddresses.Add(item);
                            break;
                        }
                }
            }

            if (order != "asc" && order != "desc")
                throw new Exception("Invalid ordering");
            
            searchedAddresses = order == "asc" ?
                searchedAddresses.OrderBy(f => f.GetType().GetProperty(field).GetValue(f)).ToList() :
                searchedAddresses.OrderByDescending(f => f.GetType().GetProperty(field).GetValue(f)).ToList();

            return Ok(searchedAddresses);
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(long id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(long id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.Id }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(long id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(long id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }
    }
}
