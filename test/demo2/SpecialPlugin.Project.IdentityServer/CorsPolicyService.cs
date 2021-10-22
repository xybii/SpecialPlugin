using IdentityServer4.Services;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.IdentityServer
{
    public class CorsPolicyService : ICorsPolicyService
    {
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task<bool> IsOriginAllowedAsync(string origin)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            return true;
        }
    }
}
