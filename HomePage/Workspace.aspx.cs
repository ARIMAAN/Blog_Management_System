using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace HomePage
{
    public partial class Workspace : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null && Request.Cookies["AuthCookie"] != null && !string.IsNullOrEmpty(Request.Cookies["AuthCookie"].Values["Username"]))
            {
                Session["Username"] = Request.Cookies["AuthCookie"].Values["Username"];
                Session["SessionStart"] = DateTime.Now;
                Session.Timeout = 180; // 3 hours
            }

            bool isLoggedIn = Session["Username"] != null && Request.Cookies["AuthCookie"] != null;
            btnSignIn.Visible = !isLoggedIn;
            btnSignUp.Visible = !isLoggedIn;
            btnUserMenu.Visible = isLoggedIn;
            DateTime sessionStart;
            if (isLoggedIn)
            {
                btnUserMenu.Text = Session["Username"].ToString();
                sessionStart = Session["SessionStart"] != null ? (DateTime)Session["SessionStart"] : DateTime.Now;
                if (Session["SessionStart"] == null) Session["SessionStart"] = sessionStart;
                TimeSpan remaining = TimeSpan.FromHours(3) - (DateTime.Now - sessionStart);
                if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                lblSessionTime.Text = "Session countdown: <span id='session-timer'>" + remaining.ToString(@"hh\:mm\:ss") + "</span>";
                pnlSessionInfo.Visible = false;
            }
            else
            {
                pnlSessionInfo.Visible = false;
                sessionStart = DateTime.Now;
            }

            pnlNotLoggedIn.Visible = !isLoggedIn;
            pnlFeatures.Visible = isLoggedIn;
            pnlUserBlogs.Visible = isLoggedIn;
            pnlSort.Visible = isLoggedIn;
            lblWelcome.Text = isLoggedIn ? "Welcome to Your Workspace, " + Session["Username"].ToString() : "Sign in or Sign up to create your works";

            if (!isLoggedIn)
            {
                return;
            }

            if (!IsPostBack)
            {
                BindUserBlogs();
            }
        }

        private void BindUserBlogs()
        {
            rptUserBlogs.DataSource = GetWorkspaceData();
            rptUserBlogs.DataBind();
        }

        private DataTable GetWorkspaceData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Title");
            dt.Columns.Add("Status");
            dt.Columns.Add("Date");
            dt.Columns.Add("Link");
            dt.Columns.Add("PreviewLink");
            dt.Columns.Add("Role"); 

            string username = Session["Username"].ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
                cmdUser.Parameters.AddWithValue("@Username", username);
                object userIdObj = cmdUser.ExecuteScalar();
                if (userIdObj == null) return dt;
                int userId = Convert.ToInt32(userIdObj);

                SqlCommand cmdOwned = new SqlCommand("SELECT BlogID, Title, Description, IsPublished, UpdatedAt FROM Blogs WHERE UserID=@UserID AND IsActive=1", con);
                cmdOwned.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader rdr = cmdOwned.ExecuteReader();
                while (rdr.Read())
                {
                    int blogId = Convert.ToInt32(rdr["BlogID"]);
                    string title = rdr["Title"].ToString();
                    bool isPublished = Convert.ToBoolean(rdr["IsPublished"]);
                    DateTime date = Convert.ToDateTime(rdr["UpdatedAt"]);

                    dt.Rows.Add(blogId.ToString(), title, isPublished ? "Published" : "Draft",
                        date.ToString("dd MMM yyyy"),
                        "BlogEditor.aspx?BlogID=" + blogId,
                        "PreviewBlog.aspx?BlogID=" + blogId,
                        "owner");
                }
                rdr.Close();

                SqlCommand cmdCollab = new SqlCommand(
                    "SELECT b.BlogID, b.Title, b.IsPublished, b.UpdatedAt " +
                    "FROM Blogs b INNER JOIN Collaborators c ON b.BlogID=c.BlogID " +
                    "WHERE c.UserID=@UserID", con);
                cmdCollab.Parameters.AddWithValue("@UserID", userId);
                rdr = cmdCollab.ExecuteReader();
                while (rdr.Read())
                {
                    int blogId = Convert.ToInt32(rdr["BlogID"]);
                    string title = rdr["Title"].ToString();
                    bool isPublished = Convert.ToBoolean(rdr["IsPublished"]);
                    DateTime date = Convert.ToDateTime(rdr["UpdatedAt"]);

                    dt.Rows.Add(blogId.ToString(), title, isPublished ? "Published" : "Draft",
                        date.ToString("dd MMM yyyy"),
                        "BlogColabEditor.aspx?BlogID=" + blogId,
                        "PreviewBlog.aspx?BlogID=" + blogId,
                        "collaborator");
                }
                rdr.Close();
            }

            return dt;
        }

        protected void ddlSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = GetWorkspaceData();

            if (ddlSortBy.SelectedValue == "Date")
                dt.DefaultView.Sort = "Date DESC";
            else if (ddlSortBy.SelectedValue == "Status")
                dt.DefaultView.Sort = "Status ASC";

            rptUserBlogs.DataSource = dt;
            rptUserBlogs.DataBind();
        }

        protected void rptUserBlogs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string blogId = e.CommandArgument.ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                if (e.CommandName == "ToggleStatus")
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Blogs SET IsPublished = CASE WHEN IsPublished=1 THEN 0 ELSE 1 END, UpdatedAt=GETDATE() WHERE BlogID=@BlogID", con);
                    cmd.Parameters.AddWithValue("@BlogID", blogId);
                    cmd.ExecuteNonQuery();
                }
                else if (e.CommandName == "DeleteBlog")
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Blogs SET IsActive=0 WHERE BlogID=@BlogID", con);
                    cmd.Parameters.AddWithValue("@BlogID", blogId);
                    cmd.ExecuteNonQuery();
                }
            }

            BindUserBlogs();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect("BlogEditor.aspx");
        }

        protected void btnSignIn_Click(object sender, EventArgs e) { Response.Redirect("SignIn.aspx"); }
        protected void btnSignUp_Click(object sender, EventArgs e) { Response.Redirect("SignUp.aspx"); }
        protected void btnPanelSignIn_Click(object sender, EventArgs e) { Response.Redirect("SignIn.aspx"); }
        protected void btnPanelSignUp_Click(object sender, EventArgs e) { Response.Redirect("SignUp.aspx"); }
        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            if (Request.Cookies["AuthCookie"] != null)
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("Home.aspx");
        }
    }
}
