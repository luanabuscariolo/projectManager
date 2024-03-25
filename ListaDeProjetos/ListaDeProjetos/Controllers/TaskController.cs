using ListaDeProjetos.Database;
using ListaDeProjetos.DBModels;
using ListaDeProjetos.Models.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Task = ListaDeProjetos.DBModels.Task;

namespace ListaDeProjetos.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index([FromQuery] int? projectId)
        {
            if (projectId == null)
            {
                ViewBag.ErrorMsg = "ERRO: Informar Projeto";
                ViewBag.HasError = true;

                return View(new IndexViewModel());
            }
            else
            {
                ViewBag.HasError = false;
                ViewBag.UserId = projectId;
            }

            using var dbContext = new ListaDeProjetosContext();

            var project = dbContext.Projects
                .Include(x => x.UserProjects)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            IndexViewModel viewModel = new IndexViewModel();

            viewModel.Tasks = dbContext.Tasks.Where(x => x.ProjectId == projectId).Include(x => x.Project).Include(u => u.User).ToList();
            viewModel.Project = project;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create([FromQuery] int projectId)
        {
            using var dbContext = new ListaDeProjetosContext();

            var userList = dbContext.Users.ToList();
            var projectList = dbContext.Projects.Where(x => x.Id == projectId).ToList();

            ViewBag.UsersList = new SelectList(userList, nameof(DBModels.User.Id), nameof(DBModels.User.Name));
            ViewBag.ProjectsList = new SelectList(projectList, nameof(DBModels.Project.Id), nameof(DBModels.Project.Title));

            var initialViewModel = new CreateViewModel()
            {
                ProjectId = projectId
            };

            return View(initialViewModel);
        }

        [HttpPost]
        public IActionResult Create([FromForm] CreateViewModel taskVm)
        {
            using var dbContext = new ListaDeProjetosContext();

            var userList = dbContext.Users.ToList();
            var projectList = dbContext.Projects.ToList();

            ViewBag.UsersList = new SelectList(userList, nameof(DBModels.User.Id), nameof(DBModels.User.Name));
            ViewBag.ProjectsList = new SelectList(projectList, nameof(DBModels.Project.Id), nameof(DBModels.Project.Title));

            if (ModelState.IsValid)
            {
                var task = new Task()
                {
                    Title = taskVm.Title,
                    Description = taskVm.Description,
                    Status = taskVm.Status,
                    UserId = taskVm.UserId,
                    ProjectId = taskVm.ProjectId,
                    CreatedDate = taskVm.CreatedDate,
                    FinishDate = taskVm.FinishDate,
                };

                dbContext.Tasks.Add(task);
                dbContext.SaveChanges();

                return RedirectToAction("Index", new { projectId = task.ProjectId });
            }
            else
            {
                return View(taskVm);
            }
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            using var dbContext = new ListaDeProjetosContext();
            var userList = dbContext.Users.ToList();
            var projectList = dbContext.Projects.ToList();

            ViewBag.UsersList = new SelectList(userList, nameof(DBModels.User.Id), nameof(DBModels.User.Name));
            ViewBag.ProjectList = new SelectList(projectList, nameof(DBModels.Project.Id), nameof(DBModels.Project.Title));

            if (id == null)
            {
                return NotFound();
            }

            Task task = dbContext.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            var vm = new UpdateViewModel()
            {
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                UserId = task.UserId,
                ProjectId = task.ProjectId,
                CreatedDate = task.CreatedDate,
                FinishDate = task.FinishDate,
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(UpdateViewModel taskVm)
        {
            using var dbContext = new ListaDeProjetosContext();
            var userList = dbContext.Users.ToList();
            var projectList = dbContext.Projects.ToList();
            ViewBag.UserList = new SelectList(userList, nameof(DBModels.User.Id), nameof(DBModels.User.Name));
            ViewBag.ProjectList = new SelectList(projectList, nameof(DBModels.Project.Id), nameof(DBModels.Project.Title));

            if (ModelState.IsValid)
            {
                Task task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskVm.Id);
                if (task == null)
                {
                    return NotFound();
                }
                task.Title = taskVm.Title;
                task.Description = taskVm.Description;
                task.Status = taskVm.Status;
                task.UserId = taskVm.UserId;
                //task.ProjectId = taskVm.ProjectId;
                task.CreatedDate = taskVm.CreatedDate;
                task.FinishDate = taskVm.FinishDate;

                dbContext.Tasks.Update(task);
                dbContext.SaveChanges();

                return RedirectToAction("Index", new { projectId = task.ProjectId });
            }

            return View(taskVm);
        }

        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using var dbContext = new ListaDeProjetosContext();

            var task = dbContext.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            dbContext.Tasks.Remove(task);
            dbContext.SaveChanges();

            return RedirectToAction("Index", new { projectId = task.ProjectId });
        }
    }
}
