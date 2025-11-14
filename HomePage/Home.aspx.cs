using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HomePage
{
    public partial class Home : System.Web.UI.Page
    {
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
                TimeSpan remaining = TimeSpan.FromHours(3) - (DateTime.Now - sessionStart);
                if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                lblSessionTime.Text = "Session countdown: <span id='session-timer'>" + remaining.ToString(@"hh\:mm\:ss") + "</span>";
                pnlSessionInfo.Visible = false; 
            }
            else
            {
                pnlSessionInfo.Visible = false;
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignIn.aspx");
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
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
    }
}