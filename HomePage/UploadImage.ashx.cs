using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Web;

namespace HomePage
{
    public class UploadImage : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (context.Request.Files.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("No file uploaded.");
                return;
            }

            var file = context.Request.Files[0];

            string username = context.Session?["Username"]?.ToString() ?? "Guest";
            string blogTitle = context.Request.Form["blogTitle"] ?? "GuestBlog";

            string blogFolder = context.Server.MapPath($"~/Blog_Files/{username}/{blogTitle}/Images/");
            if (!Directory.Exists(blogFolder)) Directory.CreateDirectory(blogFolder);

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(blogFolder, fileName);

            // Check duplicate by hash
            if (File.Exists(filePath))
            {
                using (var md5 = MD5.Create())
                using (var stream = file.InputStream)
                {
                    byte[] uploadedHash = md5.ComputeHash(stream);
                    stream.Position = 0;

                    byte[] existingHash;
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        existingHash = md5.ComputeHash(fs);

                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(uploadedHash, existingHash))
                    {
                        fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.Ticks + Path.GetExtension(fileName);
                        filePath = Path.Combine(blogFolder, fileName);
                    }
                    else
                    {
                        context.Response.Write($"/Blog_Files/{username}/{blogTitle}/Images/{fileName}");
                        return;
                    }
                }
            }

            file.SaveAs(filePath);
            context.Response.Write($"/Blog_Files/{username}/{blogTitle}/Images/{fileName}");
        }

        public bool IsReusable => false;
    }
}
