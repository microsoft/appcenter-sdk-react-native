using System;
using System.Threading.Tasks;

namespace Contoso.Forms.Puppet
{
    public static class EventFilterHolder
    {
        public interface IImplementation
        {
            Type BindingType { get; }

            Task<bool> IsEnabledAsync();

            Task SetEnabledAsync(bool enabled);
        }

        public static IImplementation Implementation { get; set; }
    }
}
