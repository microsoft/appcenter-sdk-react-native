using System.Threading.Tasks;

namespace Contoso.Forms.Puppet
{
    public interface IFilePicker
    {
        Task<string> PickFile();
        byte[] GetFileContent(string file);
        string GetFileType(string file);
        string GetFileName(string file);
    }
}
