using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.Models.Task
{
    public class UpdateViewModel : CreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
