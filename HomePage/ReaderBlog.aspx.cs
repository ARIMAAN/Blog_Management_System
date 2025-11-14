using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace HomePage
{
    public partial class ReaderBlog : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["Username"] != null && Request.Cookies["AuthCookie"] != null;
            btnSignIn.Visible = !isLoggedIn;
            btnSignUp.Visible = !isLoggedIn;
            btnUserMenu.Visible = isLoggedIn;
            if (isLoggedIn)
            {
                btnUserMenu.Text = Session["Username"].ToString();
                DateTime sessionStart = Session["SessionStart"] != null ? (DateTime)Session["SessionStart"] : DateTime.Now;
                if (Session["SessionStart"] == null) Session["SessionStart"] = sessionStart;
                TimeSpan activeTime = DateTime.Now - sessionStart;
                lblSessionTime.Text = "Active session: " + activeTime.ToString(@"hh\:mm\:ss");
            }
            if (!IsPostBack && Request.QueryString["BlogID"] != null)
            {
                int blogId = Convert.ToInt32(Request.QueryString["BlogID"]);
                LoadBlogHtml(blogId);
                LoadBlogInfo(blogId);
                SetupCollabPanel(blogId);
            }
        }

        private void LoadBlogHtml(int blogId)
        {
            string blogTitle = null;
            string htmlFilePath = null;
            int userId = 0;
            string ownerUsername = "";

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
                        userId = Convert.ToInt32(rdr["UserID"]);
                    }
                }
                if (userId > 0)
                {
                    SqlCommand cmdUser = new SqlCommand("SELECT Username FROM Users WHERE UserID=@UserID", con);
                    cmdUser.Parameters.AddWithValue("@UserID", userId);
                    object result = cmdUser.ExecuteScalar();
                    if (result != null) ownerUsername = result.ToString();
                }
            }

            if (!string.IsNullOrEmpty(blogTitle))
            {
                string basePath = Server.MapPath("~/Blog_Files");
                string blogFolder = Path.Combine(basePath, ownerUsername, blogTitle);
                string indexPath = Path.Combine(blogFolder, "index.json");
                if (File.Exists(indexPath))
                {
                    var blogIndex = JsonConvert.DeserializeObject<HomePage.BlogEditor.BlogIndex>(File.ReadAllText(indexPath));
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
            litBlogContent.Text = "<p>Blog content not available.</p>";
        }

        private void LoadBlogInfo(int blogId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT b.Title, b.Description, b.CreatedAt, u.Username AS Owner FROM Blogs b INNER JOIN Users u ON b.UserID = u.UserID WHERE b.BlogID=@BlogID", con);
                cmd.Parameters.AddWithValue("@BlogID", blogId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        lblOwner.Text = "Owner: " + rdr["Owner"].ToString();
                        lblCreated.Text = "Created: " + Convert.ToDateTime(rdr["CreatedAt"]).ToString("dd MMM yyyy");
                        lblDescription.Text = "Description: " + rdr["Description"].ToString();
                    }
                }
                // Collaborators
                SqlCommand cmdCollab = new SqlCommand("SELECT u.Username FROM Collaborators c INNER JOIN Users u ON c.UserID = u.UserID WHERE c.BlogID=@BlogID", con);
                cmdCollab.Parameters.AddWithValue("@BlogID", blogId);
                var collabs = new System.Text.StringBuilder();
                using (var rdr = cmdCollab.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (collabs.Length > 0) collabs.Append(", ");
                        collabs.Append(rdr["Username"].ToString());
                    }
                }
                lblCollaborators.Text = "Collaborators: " + (collabs.Length > 0 ? collabs.ToString() : "None");
            }
        }

        private void SetupCollabPanel(int blogId)
        {
            bool isLoggedIn = Session["Username"] != null && Request.Cookies["AuthCookie"] != null;
            string username = isLoggedIn ? Session["Username"].ToString() : null;
            bool isOwner = false, isCollaborator = false;
            int userId = 0, ownerId = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmdBlog = new SqlCommand("SELECT UserID FROM Blogs WHERE BlogID=@BlogID", con);
                cmdBlog.Parameters.AddWithValue("@BlogID", blogId);
                object ownerIdObj = cmdBlog.ExecuteScalar();
                if (ownerIdObj != null) ownerId = Convert.ToInt32(ownerIdObj);
                if (isLoggedIn)
                {
                    SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
                    cmdUser.Parameters.AddWithValue("@Username", username);
                    object userIdObj = cmdUser.ExecuteScalar();
                    if (userIdObj != null) userId = Convert.ToInt32(userIdObj);
                    isOwner = (userId == ownerId);
                    SqlCommand cmdCollab = new SqlCommand("SELECT COUNT(*) FROM Collaborators WHERE BlogID=@BlogID AND UserID=@UserID", con);
                    cmdCollab.Parameters.AddWithValue("@BlogID", blogId);
                    cmdCollab.Parameters.AddWithValue("@UserID", userId);
                    isCollaborator = Convert.ToInt32(cmdCollab.ExecuteScalar()) > 0;
                }
            }
            btnRequestCollab.Visible = isLoggedIn && !isOwner && !isCollaborator;
            btnSignInCollab.Visible = !isLoggedIn;
            btnSignUpCollab.Visible = !isLoggedIn;
            // Hide add collaborator controls for everyone (including owner)
            
            if (isLoggedIn)
            {
                if (isOwner)
                    lblCollabMsg.Text = "You are the owner of this blog.";
                else if (isCollaborator)
                    lblCollabMsg.Text = "You are a collaborator on this blog.";
                else
                    lblCollabMsg.Text = "Request to collaborate on this blog.";
            }
            else
            {
                lblCollabMsg.Text = "You need an account to collaborate.";
            }
        }

        protected void btnRequestCollab_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null) return;
            string username = Session["Username"].ToString();
            int blogId = Convert.ToInt32(Request.QueryString["BlogID"]);
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
                cmdUser.Parameters.AddWithValue("@Username", username);
                object userIdObj = cmdUser.ExecuteScalar();
                if (userIdObj == null) return;
                int userId = Convert.ToInt32(userIdObj);
                SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Collaborators WHERE BlogID=@BlogID AND UserID=@UserID", con);
                cmdCheck.Parameters.AddWithValue("@BlogID", blogId);
                cmdCheck.Parameters.AddWithValue("@UserID", userId);
                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                if (count == 0)
                {
                    SqlCommand cmdAdd = new SqlCommand("INSERT INTO Collaborators (BlogID, UserID) VALUES (@BlogID, @UserID)", con);
                    cmdAdd.Parameters.AddWithValue("@BlogID", blogId);
                    cmdAdd.Parameters.AddWithValue("@UserID", userId);
                    cmdAdd.ExecuteNonQuery();
                }
            }
            SetupCollabPanel(blogId);
        }

        protected void btnSignIn_Click(object sender, EventArgs e) { Response.Redirect("SignIn.aspx"); }
        protected void btnSignUp_Click(object sender, EventArgs e) { Response.Redirect("SignUp.aspx"); }
        protected void btnClose_Click(object sender, EventArgs e) { Response.Redirect("Blog.aspx"); }
        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            if (Request.Cookies["AuthCookie"] != null)
            {
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Redirect("Home.aspx");
        }

        protected void btnSignIn_Click1(object sender, EventArgs e)
        {
            Response.Redirect("SignIn.aspx");
        }

        protected void btnSignUp_Click1(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");   
        }
    }
}