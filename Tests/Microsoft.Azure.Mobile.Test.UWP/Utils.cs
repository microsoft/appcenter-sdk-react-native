using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Test
{
    public static class Utils
    {
        public static bool StringDictionariesAreEqual(IDictionary<string, string> dictA, IDictionary<string, string> dictB)
        {
            if (dictA == dictB)
            {
                return true;
            }
            if (dictA == null && dictB == null)
            {
                return true;
            }
            if (dictA == null || dictB == null)
            {
                return false;
            }
            if (dictA.Count != dictB.Count)
            {
                return false;
            }
            
            foreach (var pair in dictA)
            {
                if (!dictB.ContainsKey(pair.Key))
                {
                    return false;
                }
                if (dictB[pair.Key] != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
