using ListaDeProjetos.Database;
using ListaDeProjetos.DBModels;
using ListaDeProjetos.Models;
using ListaDeProjetos.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ListaDeProjetos.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            using var dbContext = new ListaDeProjetosContext();

            IndexViewModel viewModel = new IndexViewModel();
            viewModel.Users = dbContext.Users.Include(u => u.UserProjects).ThenInclude(x => x.Project).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            using var dbContext = new ListaDeProjetosContext();

            if (ModelState.IsValid)
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                TempData["MensagemSucesso"] = "Cadastro registrado com sucesso!";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar cadastrar novo usuário!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Modify(int? id)
        {
            using var dbContext = new ListaDeProjetosContext();

            if (id == null)
            {
                return NotFound();
            }

            User user = dbContext.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Modify(User user)
        {
            using var dbContext = new ListaDeProjetosContext();

            if (ModelState.IsValid)
            {
                dbContext.Users.Update(user);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            using var dbContext = new ListaDeProjetosContext();

            if (id == null || id == 0)
            {
                return NotFound();
            }

            User user = dbContext.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(User user)
        {
            using var dbContext = new ListaDeProjetosContext();

            if (user == null)
            {
                return NotFound();
            }

            dbContext.Users.Remove(user);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
