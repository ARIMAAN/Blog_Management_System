using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace HomePage
{
    public partial class PreviewBlog : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["BlogID"] != null)
                {
                    int blogId = Convert.ToInt32(Request.QueryString["BlogID"]);
                    LoadBlogHtml(blogId);
                }
            }
        }

        private void LoadBlogHtml(int blogId)
        {
            string blogTitle = null;
            string ownerUsername = null;
            string htmlFilePath = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Title, UserID FROM Blogs WHERE BlogID=@BlogID", con);
                cmd.Parameters.AddWithValue("@BlogID", blogId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        blogTitle = rdr["Title"].ToString();
                        int ownerId = Convert.ToInt32(rdr["UserID"]);
                        using (SqlConnection con2 = new SqlConnection(connectionString))
                        {
                            con2.Open();
                            SqlCommand cmdOwner = new SqlCommand("SELECT Username FROM Users WHERE UserID=@UserID", con2);
                            cmdOwner.Parameters.AddWithValue("@UserID", ownerId);
                            object ownerResult = cmdOwner.ExecuteScalar();
                            if (ownerResult != null)
                                ownerUsername = ownerResult.ToString();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(blogTitle) && !string.IsNullOrEmpty(ownerUsername))
            {
                string basePath = Server.MapPath("~/Blog_Files");
                string blogFolder = Path.Combine(basePath, ownerUsername, blogTitle);
                string indexPath = Path.Combine(blogFolder, "index.json");
                if (File.Exists(indexPath))
                {
                    var blogIndex = Newtonsoft.Json.JsonConvert.DeserializeObject<BlogEditor.BlogIndex>(File.ReadAllText(indexPath));
                    if (!string.IsNullOrEmpty(blogIndex?.current))
                    {
                        htmlFilePath = Path.Combine(blogFolder, blogIndex.current);
                        if (File.Exists(htmlFilePath))
                        {
                            litBlogContent.Text = File.ReadAllText(htmlFilePath);
                            return;
                        }
                    }
                }
            }
            litBlogContent.Text = "<p>Blog preview not available.</p>";
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("Workspace.aspx");
        }
    }
}
