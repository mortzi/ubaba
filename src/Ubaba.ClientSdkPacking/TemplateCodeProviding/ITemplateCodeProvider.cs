using System.Threading;
using System.Threading.Tasks;

namespace Ursa.ClientSdkPacking.TemplateCodeProviding
{
    internal interface ITemplateCodeProvider
    {
        Task<string> GetTemplateCodeAsync(CancellationToken cancellationToken);
    }
}