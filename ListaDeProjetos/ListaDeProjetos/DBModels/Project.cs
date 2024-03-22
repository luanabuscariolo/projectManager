using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.DBModels
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório Título do Projeto")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Obrigatório Descrição do Projeto")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Criação do Projeto")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Obrigatório Data de Entrega do Projeto")]
        public DateTime FinishDate { get; set; }

        [Required(ErrorMessage = "Obrigatório Status do Projeto")]
        public string Status { get; set; }

        public virtual List<UserProject> UserProjects { get; set; }

    }
}
