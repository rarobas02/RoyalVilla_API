using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla.DTO;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers.v2
{
    [Route("api/villa")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    //[Authorize(Roles = "Customer,Admin")]
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
        //[Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            //return _db.Villas.ToList();//not optimal
            // return Ok(await _db.Villas.ToListAsync()); //return status if everything was ok -> this way we no longer blocking the thread
            var villas = await _db.Villas.ToListAsync();
            var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
            var response = ApiResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas retrieved successfully");
            return Ok(response);
        }
        [HttpGet("{id:int}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillasById(int id)
        {
            try
            {
                if(id<=0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Id must be greater than 0"));
                }
                var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }
                return Ok(ApiResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villa), "Records retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while creating villa", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> CreateVilla(VillaCreateDTO villaDto)
        {
            try
            {
                if(villaDto is null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa data is required"));
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDto.Name.ToLower());
                if (duplicateVilla is not null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A villa name {villaDto.Name} already exists"));
                }
                Villa villa = _mapper.Map<Villa>(villaDto);
                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();
                var reponse = ApiResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), "Villa created successfully");
                return CreatedAtAction(nameof(CreateVilla),new {id=villa.Id}, reponse); //villaDTO because we want to return the id
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while creating villa", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> UpdateVilla(int id, VillaUpdateDTO villaDto)
        {
            try
            {
                if(villaDto is null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa data is required"));
                }
                if(id!= villaDto.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa ID in URL does not match Villa ID in request body"));
                }
                if(id!= villaDto.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa ID in URL does not match Villa ID in request body"));
                }
                var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVilla == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDto.Name.ToLower() && u.Id != id);
                if(duplicateVilla is not null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A villa name {villaDto.Name} already exists"));
                }
                _mapper.Map(villaDto, existingVilla);
                existingVilla.UpdatedDate = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                var reponse = ApiResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villaDto), "Villa updated successfully");
                return Ok(villaDto);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while updating villa", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVilla == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }
                _db.Villas.Remove(existingVilla);
                await _db.SaveChangesAsync();

                var response = ApiResponse<object>.NoContent("Villa deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while creating villa", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
