#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;

namespace NoCom_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventLogsController : ControllerBase
    {
        private readonly NoComContext _context;

        public EventLogsController(NoComContext context)
        {
            _context = context;
        }

        // GET: api/EventLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventLog>>> GetEventLogs()
        {
            return await _context.EventLogs.ToListAsync();
        }

        // GET: api/EventLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventLog>> GetEventLog(long id)
        {
            var eventLog = await _context.EventLogs.FindAsync(id);

            if (eventLog == null)
            {
                return NotFound();
            }

            return eventLog;
        }

        // PUT: api/EventLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventLog(long id, EventLog eventLog)
        {
            if (id != eventLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogExists(id))
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

        // POST: api/EventLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventLog>> PostEventLog(EventLog eventLog)
        {
            _context.EventLogs.Add(eventLog);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EventLogExists(eventLog.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEventLog", new { id = eventLog.Id }, eventLog);
        }

        // DELETE: api/EventLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventLog(long id)
        {
            var eventLog = await _context.EventLogs.FindAsync(id);
            if (eventLog == null)
            {
                return NotFound();
            }

            _context.EventLogs.Remove(eventLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventLogExists(long id)
        {
            return _context.EventLogs.Any(e => e.Id == id);
        }
    }
}
