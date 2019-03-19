// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Contoso.Forms.Test
{
    public static class EventSharer
    {
        public delegate void EventCallbackHandler(EventData data);

        public static event EventCallbackHandler FailedToSendEvent;
        public static event EventCallbackHandler SentEvent;
        public static event EventCallbackHandler SendingEvent;

        public static void InvokeFailedToSendEvent(EventData data)
        {
            if (FailedToSendEvent != null)
            {
                FailedToSendEvent(data);
            }
        }

        public static void InvokeSentEvent(EventData data)
        {
            if (SentEvent != null)
            {
                SentEvent(data);
            }
        }

        public static void InvokeSendingEvent(EventData data)
        {
            if (SendingEvent != null)
            {
                SendingEvent(data);
            }
        }
    }
}
