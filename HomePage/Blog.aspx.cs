using System;
using System.Configuration;
using System.Data;

namespace HomePage
{
    public partial class Blog : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["Username"] != null && Request.Cookies["AuthCookie"] != null;
            btnUserMenu.Visible = isLoggedIn;
            if (isLoggedIn)
            {
                btnUserMenu.Text = Session["Username"].ToString();
                DateTime sessionStart = Session["SessionStart"] != null ? (DateTime)Session["SessionStart"] : DateTime.Now;
                if (Session["SessionStart"] == null) Session["SessionStart"] = sessionStart;
                TimeSpan activeTime = DateTime.Now - sessionStart;
                lblSessionTime.Text = "Active session: " + activeTime.ToString(@"hh\:mm\:ss");
            }
            // Hide sign in/up buttons always for Blog page
            btnSignIn.Visible = false;
            btnSignUp.Visible = false;

            if (!IsPostBack)
                BindBlogs();
        }

        private void BindBlogs()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title");
            dt.Columns.Add("Author");
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Description");
            dt.Columns.Add("Link");

            using (var con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT b.BlogID, b.Title, u.Username AS Author, b.Description, b.CreatedAt FROM Blogs b INNER JOIN Users u ON b.UserID = u.UserID WHERE b.IsPublished = 1 AND b.IsActive = 1 ORDER BY b.CreatedAt DESC", con);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dt.Rows.Add(
                            rdr["Title"].ToString(),
                            rdr["Author"].ToString(),
                            Convert.ToDateTime(rdr["CreatedAt"]),
                            rdr["Description"].ToString(),
                            $"ReaderBlog.aspx?BlogID={rdr["BlogID"]}"
                        );
                    }
                }
            }
            rptBlogs.DataSource = dt;
            rptBlogs.DataBind();
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            if (Request.Cookies["AuthCookie"] != null)
            {
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Redirect("Home.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            DataTable dt = new DataTable();
            dt.Columns.Add("Title");
            dt.Columns.Add("Author");
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Description");
            dt.Columns.Add("Link");

            using (var con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT b.BlogID, b.Title, u.Username AS Author, b.Description, b.CreatedAt FROM Blogs b INNER JOIN Users u ON b.UserID = u.UserID WHERE b.IsPublished = 1 AND b.IsActive = 1 AND (b.Title LIKE @search OR b.Description LIKE @search) ORDER BY b.CreatedAt DESC", con);
                cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dt.Rows.Add(
                            rdr["Title"].ToString(),
                            rdr["Author"].ToString(),
                            Convert.ToDateTime(rdr["CreatedAt"]),
                            rdr["Description"].ToString(),
                            $"ReaderBlog.aspx?BlogID={rdr["BlogID"]}"
                        );
                    }
                }
            }
            rptBlogs.DataSource = dt;
            rptBlogs.DataBind();
        }
    }
}
