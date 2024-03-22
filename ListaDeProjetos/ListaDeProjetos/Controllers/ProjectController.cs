using ListaDeProjetos.Database;
using ListaDeProjetos.DBModels;
using ListaDeProjetos.Models.Project;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ListaDeProjetos.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index([FromQuery] int? userId)
        {
            ViewBag.HasError = false;
            ViewBag.UserId = userId;

            using var dbContext = new ListaDeProjetosContext();

            IndexViewModel viewModel = new IndexViewModel();
            if (userId != null)
            {
                viewModel.Projects = dbContext.Projects.Where(x => x.UserProjects.Any(p => p.UserId == userId)).Include(x => x.UserProjects).ThenInclude(u => u.User).ToList();
            }
            else
            {
                viewModel.Projects = dbContext.Projects.Include(x => x.UserProjects).ThenInclude(u => u.User).ToList();
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(int? id)
        {
            using var dbContext = new ListaDeProjetosContext();

            var usersList = dbContext.Users.ToList();

            ViewBag.UsersList = new SelectList(usersList, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel projectVm)
        {
            using var dbContext = new ListaDeProjetosContext();

            var usersList = dbContext.Users.ToList();

            ViewBag.UsersList = new SelectList(usersList, "Id", "Name");

            if (ModelState.IsValid)
            {
                //Nessa lógica, o Entity Framework cuida do Id do Projeto, pois ele entende que se ele está sendo criado, logo não possuí Id no momento. Aqui também é passado o Id do Usuario passado na view para a propriedade UserId da entidade UserProject do banco. Essa etapa de coletar o Id do usuario é para poder fazer a ligação dele com o projeto no BD.

                //var userProjects = new List<UserProject>();
                //foreach (var userId in projectVm.Users)
                //{
                //    var userProject = new UserProject()
                //    {
                //        UserId = userId
                //    };
                //    userProjects.Add(userProject);
                //}

                //MAPEAMENTO: nova instância da Classe Project() de DBModels para receber os dados da CreateViewModel que é a view model que recebe os dados que o cliente informou na view.
                var project = new Project()
                {
                    CreatedDate = projectVm.CreatedDate.Value,
                    Description = projectVm.Description,
                    FinishDate = projectVm.FinishDate.Value,
                    Status = projectVm.Status,
                    Title = projectVm.Title,
                    //UserProjects = userProjects
                };
                // nesse momento o project.id é 0
                dbContext.Projects.Add(project);
                dbContext.SaveChanges();

                // Nessa lógica abaixo, o foreach substitui o passado acima, mas não tem o Entity Framework trabalhando para cuidar automaticamente do Id, aqui é necessário tratar a ausência do Id do Projeto. Acima já foi adicionado informações do Projeto no BD, então project.Id  já tem valor de Id > 0 no banco e agora é só adicionar ao ProjectId 

                var userProjects = new List<UserProject>();
                foreach (var userId in projectVm.Users)
                {
                    var userProject = new UserProject()
                    {
                        UserId = userId,
                        ProjectId = project.Id // O Id não definido em "var project = new Project()" é passado agora
                    };
                    userProjects.Add(userProject); //Como dessa forma não se trata o Id sozinho, não poderia passar "UserProjects = userProjects" acima, então é adicionado agora.
                }
                dbContext.UserProjects.AddRange(userProjects);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            using var dbContext = new ListaDeProjetosContext();

            var usersList = dbContext.Users.ToList();

            ViewBag.UsersList = new SelectList(usersList, "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }
            //Busca no BD dados de Projetos incluindo os usuarios ligados àqueles projetos
            Project project = dbContext.Projects.Include(x => x.UserProjects).FirstOrDefault(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            //MAPEAMENTO: dados do Projeto que tem no BD são passados para as propriedades do UdpateViewModel para serem exibidas na view
            var vm = new UpdateViewModel()
            {
                CreatedDate = project.CreatedDate,
                Description = project.Description,
                FinishDate = project.FinishDate,
                Id = project.Id,
                Status = project.Status,
                Title = project.Title,
                Users = project.UserProjects.Select(x => x.UserId).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(UpdateViewModel projectVm)
        {
            using var dbContext = new ListaDeProjetosContext();

            //É importante buscar no BD os usuários daquele projeto novamente, caso a tela seja recarregada por erro de envio. Se não for carregado os usuarios, a lista de usuarios aparecerá vazia. Erro que deve ser tatado.
            var usersList = dbContext.Users.ToList();

            ViewBag.UsersList = new SelectList(usersList, "Id", "Name");

            if (ModelState.IsValid)
            {
                Project project = dbContext.Projects.Include(x => x.UserProjects).FirstOrDefault(x => x.Id == projectVm.Id);
                if (project == null)
                {
                    return NotFound();
                }
                //MAPEAMENTO DA VIEWMODEL PARA O BD ATRAVÉS DA VARIAVEL PROJECT DO TIPO PROJECT CRIADA ACIMA
                project.CreatedDate = projectVm.CreatedDate.Value;
                project.Description = projectVm.Description;
                project.FinishDate = projectVm.FinishDate.Value;
                project.Status = projectVm.Status;
                project.Title = projectVm.Title;

                //Para salvar os usuarios que foram alterados, removidos ou adicionados na view para o BD, limpamos a lista de usuarios daquele projeto. Isso garante que, ao atualizar um projeto com uma nova lista de usuários associados, os registros antigos de UserProjects sejam removidos e os novos registros sejam adicionados, mantendo assim a integridade das associações entre projetos e usuários. Após isso, são adicionados novos registros de UserProject ao contexto do banco de dados com base nos IDs de usuários fornecidos na UpdateViewModel.
                dbContext.UserProjects.RemoveRange(project.UserProjects);

                var userProjects = new List<UserProject>();
                foreach (var userId in projectVm.Users)
                {
                    var userProject = new UserProject()
                    {
                        UserId = userId,
                        ProjectId = project.Id
                    };
                    userProjects.Add(userProject);
                }
                dbContext.UserProjects.AddRange(userProjects);

                dbContext.Projects.Update(project);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(projectVm);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using var dbContext = new ListaDeProjetosContext();
            var project = dbContext.Projects.FirstOrDefault(x => x.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            dbContext.Projects.Remove(project);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
