using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VillaController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            //return _db.Villas.ToList();//not optimal
            return Ok(await _db.Villas.ToListAsync()); //return status if everything was ok -> this way we no longer blocking the thread
        }
        [HttpGet("{id:int}")]
        public string GetVillasById([FromRoute] int id) // data come from url itself
        {
            return $"Get villa {id}";
        }
        [HttpGet("{id:int}/{name}")] //string is the default type so name is empty
        public string GetVillasByIdAndName([FromRoute] int id,[FromRoute] string name) 
        {
            return $"Get Villa: {id} : {name}";
        }
    }
}
