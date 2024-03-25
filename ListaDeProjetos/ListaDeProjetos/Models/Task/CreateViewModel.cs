using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.Models.Task
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Obrigatório Título da Tarefa")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Obrigatório Descrição da Tarefa")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Obrigatório Status da Tarefa")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Criação da Tarefa")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Entrega da Tarefa")]
        public DateTime FinishDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Obrigatório informar Usuário")]
        public int UserId { get; set; }

        public int ProjectId { get; set; }
    }
}
