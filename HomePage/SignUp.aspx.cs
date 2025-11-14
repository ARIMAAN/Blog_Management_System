using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace HomePage
{
    public partial class SignUp : System.Web.UI.Page
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

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsStrongPassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            lblErrorUsername.Text = "";
            lblErrorEmail.Text = "";
            lblErrorPassword.Text = "";
            lblErrorConfirmPassword.Text = "";

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblErrorUsername.Text = "Username is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblErrorEmail.Text = "Email is required.";
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                lblErrorEmail.Text = "Invalid email format.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblErrorPassword.Text = "Password is required.";
                isValid = false;
            }
            else if (!IsStrongPassword(txtPassword.Text))
            {
                lblErrorPassword.Text = "Password must be at least 8 characters, include uppercase, lowercase, digit, and special character.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblErrorConfirmPassword.Text = "Please confirm your password.";
                isValid = false;
            }
            else if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblErrorConfirmPassword.Text = "Passwords do not match.";
                isValid = false;
            }

            if (!isValid)
            {
                return;
            }
            string hashedPassword = HashPassword(txtPassword.Text + "ablBlog");
            using (var con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)", con);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Response.Redirect("SignIn.aspx");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        con.Close();
                        using (var checkCmd = new SqlCommand("SELECT Username, Email FROM Users WHERE Username = @Username OR Email = @Email", con))
                        {
                            checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                            checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            con.Open();
                            using (var reader = checkCmd.ExecuteReader())
                            {
                                bool duplicateUsername = false;
                                bool duplicateEmail = false;
                                while (reader.Read())
                                {
                                    if (reader["Username"].ToString().Equals(txtUsername.Text, StringComparison.OrdinalIgnoreCase))
                                        duplicateUsername = true;
                                    if (reader["Email"].ToString().Equals(txtEmail.Text, StringComparison.OrdinalIgnoreCase))
                                        duplicateEmail = true;
                                }
                                if (duplicateUsername)
                                    lblErrorUsername.Text = "Username already registered.";
                                if (duplicateEmail)
                                    lblErrorEmail.Text = "Email already registered.";
                                if (!duplicateUsername && !duplicateEmail)
                                    lblErrorEmail.Text = "A unique constraint error occurred. Please check your details.";
                            }
                            con.Close();
                        }
                    }
                    else
                    {
                        lblErrorEmail.Text = "Registration failed. Please check your details and try again.";
                    }
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignIn.aspx");
        }
    }
}