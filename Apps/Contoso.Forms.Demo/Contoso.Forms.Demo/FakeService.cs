using System.IO;
using System.Threading.Tasks;

namespace Contoso.Forms.Demo
{
    static class FakeService
    {
        internal static async Task DoStuffInBackground()
        {
            await Task.Run(() => throw new IOException("Server did not respond"));
        }
    }
}
