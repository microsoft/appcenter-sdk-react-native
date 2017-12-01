using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Text.Format;
using Android.Webkit;
using Contoso.Forms.Puppet.Droid;
using Xamarin.Forms;

// Make this class visible to the DependencyService manager.
[assembly: Dependency(typeof(FilePicker))]

namespace Contoso.Forms.Puppet.Droid
{
    public class FilePicker : IFilePicker
    {
        public Task<string> PickFile()
        {
            Intent intent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                intent = new Intent(Intent.ActionOpenDocument);
                intent.AddCategory(Intent.CategoryOpenable);
            }
            else
            {
                intent = new Intent(Intent.ActionGetContent);
            }
            intent.SetType("*/*");
            var activity = Xamarin.Forms.Forms.Context as MainActivity;
            activity.StartActivityForResult(Intent.CreateChooser(intent, "Select attachment file"), MainActivity.FileAttachmentId);
            activity.FileAttachmentTaskCompletionSource = new TaskCompletionSource<string>();
            return activity.FileAttachmentTaskCompletionSource.Task;
        }

        public Tuple<byte[], string, string> ReadFile(string file)
        {
            var uri = Android.Net.Uri.Parse(file);
            var activity = Xamarin.Forms.Forms.Context as MainActivity;

            // Data
            byte[] data = null;
            using (var inputStream = activity.ContentResolver.OpenInputStream(uri))
            using (var memoryStream = new MemoryStream())
            {
                inputStream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }

            // Name
            string name = "";
            var cursor = activity.ContentResolver.Query(uri, null, null, null, null);
            try
            {
                if (cursor != null && cursor.MoveToFirst())
                {
                    var nameIndex = cursor.GetColumnIndex(OpenableColumns.DisplayName);
                    if (!cursor.IsNull(nameIndex))
                    {
                        name = cursor.GetString(nameIndex);
                    }
                }
            }
            finally
            {
                cursor?.Close();
            }

            // Mime
            string mime = null;
            if (uri.Scheme == ContentResolver.SchemeContent)
            {
                mime = activity.ContentResolver.GetType(uri);
            }
            else
            {
                var extension = MimeTypeMap.GetFileExtensionFromUrl(file);
                mime = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.ToLower());
            }
            return new Tuple<byte[], string, string>(data, name, mime);
        }

        public string GetFileDescription(string file)
        {
            var uri = Android.Net.Uri.Parse(file);
            var activity = Xamarin.Forms.Forms.Context as MainActivity;

            string name = null;
            string size = null;
            var cursor = activity.ContentResolver.Query(uri, null, null, null, null);
            try
            {
                if (cursor != null && cursor.MoveToFirst())
                {
                    var nameIndex = cursor.GetColumnIndex(OpenableColumns.DisplayName);
                    var sizeIndex = cursor.GetColumnIndex(OpenableColumns.Size);
                    if (!cursor.IsNull(nameIndex))
                    {
                        name = cursor.GetString(nameIndex);
                    }
                    if (!cursor.IsNull(sizeIndex))
                    {
                        size = Formatter.FormatFileSize(activity, cursor.GetLong(sizeIndex));
                    }
                }
            }
            finally
            {
                cursor?.Close();
            }

            string result = "";
            if (name != null)
            {
                result += "File: " + name;
            }
            if (size != null)
            {
                result += " Size: " + size;
            }
            return result;
        }
    }
}
