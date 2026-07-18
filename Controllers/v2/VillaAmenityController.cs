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
    [Route("api/villa-amenity")]
    [ApiController]
    //[Authorize(Roles = "Customer,Admin")]
    public class VillaAmenityController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAmenityController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        //[Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaAmenityDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaAmenityDTO>>>> GetVillaAmenity()
        {
            //return _db.Villas.ToList();//not optimal
            // return Ok(await _db.Villas.ToListAsync()); //return status if everything was ok -> this way we no longer blocking the thread
            var villaAmenity = await _db.VillaAmenities.ToListAsync();
            var dtoResponseVillaAmenity = _mapper.Map<List<VillaAmenityDTO>>(villaAmenity);
            var response = ApiResponse<IEnumerable<VillaAmenityDTO>>.Ok(dtoResponseVillaAmenity, "Villa amenity/ies retrieved successfully");
            return Ok(response);
        }
        [HttpGet("{id:int}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<VillaAmenityDTO>>> GetVillaAmenityById(int id)
        {
            try
            {
                if(id<=0)
                {
                    return NotFound(ApiResponse<object>.NotFound("VillaAmenity Id must be greater than 0"));
                }
                var villaAmenity = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                if(villaAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"VillaAmenity with ID {id} was not found"));
                }
                return Ok(ApiResponse<VillaAmenityDTO>.Ok(_mapper.Map<VillaAmenityDTO>(villaAmenity), "Villa amenity retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while retrieving villa amenity", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenityDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<VillaAmenityDTO>>> CreateVillaAmenity(VillaAmenityCreateDTO VillaAmenityDTO)
        {
            try
            {
                if(VillaAmenityDTO is null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenity data is required"));
                }
                var villaExists = await _db.Villas.FirstOrDefaultAsync(u => u.Id == VillaAmenityDTO.VillaId);
                if (villaExists is null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A villa with the Id '{VillaAmenityDTO.VillaId}' does not exists"));
                }
                VillaAmenity villaAmenity = _mapper.Map<VillaAmenity>(VillaAmenityDTO);
                villaAmenity.CreatedDate = DateTime.Now;
                await _db.VillaAmenities.AddAsync(villaAmenity);
                await _db.SaveChangesAsync();
                var reponse = ApiResponse<VillaAmenityDTO>.CreatedAt(_mapper.Map<VillaAmenityDTO>(villaAmenity), "Villa Amenity created successfully");
                return CreatedAtAction(nameof(CreateVillaAmenity),new {id= villaAmenity.Id}, reponse); //VillaAmenityDTO because we want to return the id
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while creating villa", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<VillaAmenityDTO>>> UpdateVillaAmenity(int id, VillaAmenityUpdateDTO VillaAmenityDTO)
        {
            try
            {
                if(VillaAmenityDTO is null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenity data is required"));
                }
                if(id != VillaAmenityDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenity ID in URL does not match Villa Amenity ID in request body"));
                }
                var villaExists = await _db.Villas.FirstOrDefaultAsync(u => u.Id == VillaAmenityDTO.VillaId);
                if (villaExists is null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A villa with the Id '{VillaAmenityDTO.VillaId}' does not exists"));
                }
                var existingVillaAmenity = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVillaAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa Amenity with ID {id} was not found"));
                }

                _mapper.Map(VillaAmenityDTO, existingVillaAmenity);
                existingVillaAmenity.UpdatedDate = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                var reponse = ApiResponse<VillaAmenityDTO>.Ok(_mapper.Map<VillaAmenityDTO>(existingVillaAmenity), "Villa Amenity updated successfully");
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while updating villa amenity", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var existingVillaAmenity = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                if(existingVillaAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"VillaAmenity with ID {id} was not found"));
                }
                _db.VillaAmenities.Remove(existingVillaAmenity);
                await _db.SaveChangesAsync();

                var response = ApiResponse<object>.NoContent("Villa Amenity deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred while deleting villa amenity", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
