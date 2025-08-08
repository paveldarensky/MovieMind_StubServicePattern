using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServiceStub.Models;
using ServiceStub.Services;
using System.Data;


public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    private readonly IConfiguration _configuration;

    public LoginController(ILoginService loginService, IConfiguration configuration)
    {
        _loginService = loginService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.UseStub = _configuration.GetValue<bool>("UseStubLogin");
        ViewBag.ServiceInfo = ViewBag.UseStub
    ? "<div class='alert alert-warning'>" +
      "<strong>⚠ Используется StubLoginService</strong><br/>" +
      "Проводятся временные технические работы.<br/>" +
      "Для входа в систему используйте логин <code>test</code>, пароль <code>123</code>." +
      "</div>"
    : "<div class='alert alert-success'><strong>✔ Используется RealLoginService</strong></div>";

        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginModel model)
    {
        ViewBag.UseStub = _configuration.GetValue<bool>("UseStubLogin");
        ViewBag.ServiceInfo = ViewBag.UseStub
    ? "<div class='alert alert-warning'>" +
      "<strong>⚠ Используется StubLoginService</strong><br/>" +
      "Проводятся временные технические работы.<br/>" +
      "Для входа в систему используйте логин <code>test</code>, пароль <code>123</code>." +
      "</div>"
    : "<div class='alert alert-success'><strong>✔ Используется RealLoginService</strong></div>";


        if (ModelState.IsValid)
        {
            if (_loginService.Authenticate(model))
            {
                return RedirectToAction("Index", "MovieMindService");
            }

            ViewBag.Error = "Неправильное имя пользователя или пароль.";
            return View();
        }

        ViewBag.Error = "Неправильное имя пользователя или пароль.";
        return View();
    }
}


