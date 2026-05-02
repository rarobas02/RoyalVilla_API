using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            //return _db.Villas.ToList();//not optimal
            return Ok(await _db.Villas.ToListAsync()); //return status if everything was ok -> this way we no longer blocking the thread
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Villa>> GetVillasById(int id)
        {
            try
            {
                if(id<=0)
                {
                    return BadRequest("Villa Id must be greater than 0");
                }
                var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(villa == null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }
                return Ok(villa);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An Error occurred while retrieving villa with Id{id}: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<Villa>> CreateVilla(VillaCreateDTO villaDto)
        {
            try
            {
                if(villaDto is null)
                {
                    return BadRequest("Villa data is required");
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDto.Name.ToLower());
                if (duplicateVilla is not null)
                {
                    return Conflict($"A villa name {villaDto.Name} already exists");
                }
                Villa villa = _mapper.Map<Villa>(villaDto);
                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(CreateVilla),new {id=villa.Id},villa);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An Error occurred while creating villa: {ex.Message}");
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Villa>> UpdateVilla(int id, VillaUpdateDTO villaDto)
        {
            try
            {
                if(villaDto is null)
                {
                    return BadRequest("Villa data is required");
                }
                if(id!= villaDto.Id)
                {
                    return BadRequest("Villa ID in URL does not match Villa ID in request body");
                }
                var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVilla == null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDto.Name.ToLower() && u.Id != id);
                if(duplicateVilla is not null)
                {
                    return Conflict($"A villa name {villaDto.Name} already exists");
                }
                _mapper.Map(villaDto, existingVilla);
                existingVilla.UpdatedDate = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return Ok(villaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An Error occurred while updating villa: {ex.Message}");
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Villa>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVilla == null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }
                _db.Villas.Remove(existingVilla);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An Error occurred while deleting villa: {ex.Message}");
            }
        }
    }
}
