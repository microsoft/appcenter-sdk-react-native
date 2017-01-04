using System;
using System.Collections.Generic;

namespace Contoso.Forms.Test
{
    public static class EventData
    {
        //TODO add bools for testing which callbacks were called and a reset method

        public static string Name;
        public static Dictionary<string, string> Properties;

        public static event Action UpdatedEvent;

        public static void Updated()
        {
            if (UpdatedEvent != null)
            {
                UpdatedEvent();
            }
        }
    }
}
