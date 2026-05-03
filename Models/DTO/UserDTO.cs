using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!; //the default! is used to suppress the warning about non-nullable properties not being initialized in the constructor. Since this is a DTO, we expect these properties to be set when the object is created, so we can safely use default! here.
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
