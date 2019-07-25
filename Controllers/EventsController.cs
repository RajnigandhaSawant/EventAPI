using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventAPI.Infrastructure;
using EventAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventAPI.Controllers
{
    [Route("api/[controller]")] //Route Prefix
    [ApiController]
    public class EventsController : ControllerBase
    {
        private EventDbContext db;

        public EventsController(EventDbContext dbContext)
        {
            db = dbContext;
        }
        
        //GET /api/events
        [Produces("text/json","text/xml")]
        [HttpGet(Name ="GetAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        
        public ActionResult<List<EventInfo>> GetEvents()
        {
            var events = db.Events.ToList();
            return Ok(events); //returns with status code 200
        }

        //POST /api/events
        [Authorize]
        [HttpPost(Name ="AddEvent")]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<EventInfo>> AddEvent([FromBody]EventInfo eventInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await db.Events.AddAsync(eventInfo);

                    await db.SaveChangesAsync();

                    //return Created("", result.Entity); //returns the status code as 201

                    //If you want to use Action for redirection then give action name
                    //return CreatedAtAction(nameof(GetEvent), new { id = result.Entity.Id }, result.Entity); //returns the status code as 201

                    //If you want to use route for redirection then give route name
                    return CreatedAtRoute("GetById", new { id = result.Entity.Id }, result.Entity); //returns the status code as 201
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET /api/events/{id}
        [HttpGet("{id}", Name = "GetById")] //Here name is route name
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<EventInfo>> GetEvent([FromRoute] int id)
        {
            var eventInfo = await db.Events.FindAsync(id);

            if (eventInfo != null)
                return Ok(eventInfo);
            else
                return NotFound("Item you are searching not found");
        }

    }
}