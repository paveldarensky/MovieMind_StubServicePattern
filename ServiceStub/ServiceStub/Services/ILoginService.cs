using ServiceStub.Models;

namespace ServiceStub.Services
{
    public interface ILoginService
    {
        bool Authenticate(LoginModel model);
    }
}
