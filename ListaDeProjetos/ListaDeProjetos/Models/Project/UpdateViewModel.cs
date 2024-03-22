using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.Models.Project
{
    public class UpdateViewModel : CreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
