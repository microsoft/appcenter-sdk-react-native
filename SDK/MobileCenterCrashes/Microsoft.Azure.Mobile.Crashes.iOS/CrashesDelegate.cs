using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class CrashesDelegate : MSCrashesDelegate
    {
        private readonly PlatformCrashes _owner;
        CrashesDelegate(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport errorReport)
        {
            ErrorReport report = new ErrorReport(errorReport);
            return _owner.ShouldProcessErrorReport(report);
        }

        public override MSErrorAttachment AttachmentWithCrashes(MSCrashes crashes, MSErrorReport errorReport)
        {
            ErrorReport report = new ErrorReport(errorReport);
            return _owner.GetErrorAttachment(report).ToMSErrorAttachment();
        }

        public override void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            
        }
        public override void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport);
        public override void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error);
    }
}


// @optional -(BOOL)crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport;
[Export("crashes:shouldProcessErrorReport:")]
bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport errorReport);

// @optional -(MSErrorAttachment *)attachmentWithCrashes:(MSCrashes *)crashes forErrorReport:(MSErrorReport *)errorReport;
[Export("attachmentWithCrashes:forErrorReport:")]
MSErrorAttachment AttachmentWithCrashes(MSCrashes crashes, MSErrorReport errorReport);

// @optional -(void)crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport;
[Export("crashes:willSendErrorReport:")]
void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport errorReport);

// @optional -(void)crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport;
[Export("crashes:didSucceedSendingErrorReport:")]
void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport errorReport);

// @optional -(void)crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)error;
[Export("crashes:didFailSendingErrorReport:withError:")]
void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport errorReport, NSError error);