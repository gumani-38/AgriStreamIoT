using System.ComponentModel.DataAnnotations;

namespace AgriStreamIoT.Models
{
    public class DroneUploadViewModel
    {
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile ImageFile { get; set; } = default!;
    }
}
