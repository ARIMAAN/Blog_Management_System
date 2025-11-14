using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HomePage
{
    public partial class SignIn : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            string hashedPassword = HashPassword(txtPassword.Text + "ablBlog");
            using (var con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash",
                    con
                );
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                try
                {
                    con.Open();
                    int userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        HttpCookie authCookie = new HttpCookie("AuthCookie");
                        authCookie.Values["Username"] = txtUsername.Text;
                        if (chkRemember.Checked)
                        {
                            authCookie.Expires = DateTime.Now.AddHours(3);
                        }
                        Response.Cookies.Add(authCookie);
                        Session["Username"] = txtUsername.Text;
                        Session["SessionStart"] = DateTime.Now;
                        Session.Timeout = 180; 
                        Response.Redirect("Workspace.aspx");
                    }
                    else
                    {
                        lblError.Text = "Incorrect username or password. Please try again.";
                        lblError.ForeColor = Color.Red;
                        lblError.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "An error occurred: " + ex.Message;
                    lblError.ForeColor = Color.Red;
                    lblError.Visible = true;
                }
            }
        }
    }
}