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
        public string GetVillasById(int id)
        {
            return $"Get villa {id}";
        }
    }
}
