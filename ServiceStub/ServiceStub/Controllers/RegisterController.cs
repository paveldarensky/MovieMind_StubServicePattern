using ServiceStub.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

public class RegisterController : Controller
{
    private readonly IConfiguration _configuration;

    public RegisterController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            // Проверка совпадения паролей
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают.";
                return View();
            }

            using (IDbConnection db = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                // Проверяем, есть ли уже такой пользователь
                var existingUser = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Username = @Username", new { model.Username });

                if (existingUser != null)
                {
                    ViewBag.Error = "Пользователь с таким именем уже существует.";
                    return View();
                }

                // Хешируем пароль
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Добавляем нового пользователя в базу данных
                var newUser = new User
                {
                    Username = model.Username,
                    Password = hashedPassword
                };

                db.Execute("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", newUser);

                // После регистрации перенаправляем на страницу авторизации
                return RedirectToAction("Login", "Login");
            }
        }
        else
        {
            ViewBag.Error = "Ошибка регистрации. Пожалуйста, попробуйте снова.";
            return View();
        }
    }

}
