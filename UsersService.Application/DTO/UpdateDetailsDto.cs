using Newtonsoft.Json;

namespace UsersService.Application.DTO
{
    public class UpdateDetailsDto
    {
        public string Name { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
