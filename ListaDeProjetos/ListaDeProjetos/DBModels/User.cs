using System.ComponentModel.DataAnnotations;

namespace ListaDeProjetos.DBModels
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome de usuário obrigatório")]
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public virtual List<UserProject> UserProjects { get; set; }
    }
}
