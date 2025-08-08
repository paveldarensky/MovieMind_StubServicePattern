using ServiceStub.Models;

namespace ServiceStub.Services
{
    public class StubLoginService : ILoginService
    {
        public bool Authenticate(LoginModel model)
        {
            return model.Username == "test" && model.Password == "123";
        }
    }
}
