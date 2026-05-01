using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        public string GetVillas()
        {
            return "Get all villas";
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
