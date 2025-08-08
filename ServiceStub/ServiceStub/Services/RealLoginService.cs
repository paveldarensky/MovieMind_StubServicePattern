using Dapper;
using MySql.Data.MySqlClient;
using ServiceStub.Models;
using System.Data;

namespace ServiceStub.Services
{
    public class RealLoginService : ILoginService
    {
        private readonly IConfiguration _configuration;

        public RealLoginService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Authenticate(LoginModel model)
        {
            using (IDbConnection db = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sqlQuery = "SELECT Id, Username, Password FROM Users WHERE Username = @Username";
                var user = db.QueryFirstOrDefault<User>(sqlQuery, new { model.Username });

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
