using ListaDeProjetos.DBModels;
using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.Models.Project
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Obrigatório Título do Projeto")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 30 caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Obrigatório Descrição do Projeto")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Criação do Projeto")]
        //Necessario tornar data anulável para que o texto re Required escolhido seja exibidou e não a mensagem padrão genérica
        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Entrega do Projeto")]
        public DateTime? FinishDate { get; set; }

        [Required(ErrorMessage = "Obrigatório Status do Projeto")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Obrigatório Informar usuario(s)")]
        public virtual List<int> Users { get; set; }
    }
}
