using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Web;

namespace HomePage
{
    public class BlogEditor1 : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        string basePath = @"F:\Blog Management\Blog_Files";

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

            // Safely get username from session
            string username = "Guest";
            if (context.Session != null && context.Session["Username"] != null)
                username = context.Session["Username"].ToString();

            string blogTitle = context.Request.Form["blogTitle"] ?? "GuestBlog";

            string userFolder = Path.Combine(basePath, username);
            string blogFolder = Path.Combine(userFolder, blogTitle);
            string imagesFolder = Path.Combine(blogFolder, "Images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(imagesFolder, fileName);

            // Check if same image exists by MD5 hash
            if (File.Exists(filePath))
            {
                using (var md5 = MD5.Create())
                using (var stream = file.InputStream)
                {
                    byte[] uploadedHash = md5.ComputeHash(stream);
                    stream.Position = 0;

                    byte[] existingHash;
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        existingHash = md5.ComputeHash(fs);
                    }

                    bool same = StructuralComparisons.StructuralEqualityComparer.Equals(uploadedHash, existingHash);
                    if (!same)
                    {
                        fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.Ticks + Path.GetExtension(fileName);
                        filePath = Path.Combine(imagesFolder, fileName);
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
