using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Configuration;

namespace HomePage
{
    public partial class BlogEditor : System.Web.UI.Page
    {
        string basePath => Server.MapPath("~/Blog_Files");
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;

        public class VersionEntry
        {
            public string version { get; set; }
            public string user { get; set; }
            public string timestamp { get; set; }
            public string file_path { get; set; }
            public string changeNotes { get; set; }
        }

        public class BlogIndex
        {
            public string blogTitle { get; set; }
            public string current { get; set; }
            public List<VersionEntry> history { get; set; } = new List<VersionEntry>();
        }

        BlogIndex blogIndex;
        string blogFolder;
        string indexPath;
        string username => Session["Username"]?.ToString() ?? "Guest";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["Username"] != null && Request.Cookies["AuthCookie"] != null;
            if (isLoggedIn)
            {
                btnUserMenu.Text = Session["Username"].ToString();
                DateTime sessionStart = Session["SessionStart"] != null ? (DateTime)Session["SessionStart"] : DateTime.Now;
                if (Session["SessionStart"] == null) Session["SessionStart"] = sessionStart;
                TimeSpan remaining = TimeSpan.FromHours(3) - (DateTime.Now - sessionStart);
                if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                lblSessionTime.Text = "Session countdown: <span id='session-timer'>" + remaining.ToString(@"hh\:mm\:ss") + "</span>";
            }

            string blogIdStr = Request.QueryString["BlogID"];
            int blogId = 0;
            if (!string.IsNullOrEmpty(blogIdStr))
                int.TryParse(blogIdStr, out blogId);

            if (!IsPostBack)
            {
                if (blogId > 0)
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
                    string blogTitle = null;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Title, Description FROM Blogs WHERE BlogID=@BlogID", con);
                        cmd.Parameters.AddWithValue("@BlogID", blogId);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                blogTitle = rdr["Title"].ToString();
                                txtBlogDesc.Text = rdr["Description"].ToString();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(blogTitle))
                    {
                        hfBlogTitle.Value = blogTitle;
                        hfBlogTitleEditor.Value = blogTitle;
                        blogFolder = Path.Combine(basePath, username, blogTitle);
                        indexPath = Path.Combine(blogFolder, "index.json");
                        pnlBlogName.Visible = false;
                        pnlEditor.Visible = true;
                        lblBlogTitle.Text = blogTitle;
                        if (File.Exists(indexPath))
                        {
                            blogIndex = JsonConvert.DeserializeObject<BlogIndex>(File.ReadAllText(indexPath));
                            if (!string.IsNullOrEmpty(blogIndex?.current))
                            {
                                string currentFile = Path.Combine(blogFolder, blogIndex.current);
                                if (File.Exists(currentFile))
                                    txtBlogDescription.Text = File.ReadAllText(currentFile);
                            }
                            LoadVersionList();
                        }
                        else
                        {
                            blogIndex = new BlogIndex { blogTitle = blogTitle, history = new List<VersionEntry>() };
                    }
                }
            }
            else
            {
                pnlEditor.Visible = false;
                pnlBlogName.Visible = true;
            }
        }
        if (blogId > 0)
        {
            LoadCollaborators(blogId);
        }
    }

    protected void btnCreateBlog_Click(object sender, EventArgs e)
    {
        string blogName = txtBlogName.Text.Trim();
        string blogDesc = txtBlogDesc.Text.Trim();
        if (string.IsNullOrEmpty(blogName))
        {
            lblBlogNameError.Text = "Blog name cannot be empty.";
            return;
        }
        if (string.IsNullOrEmpty(blogDesc))
        {
            lblBlogNameError.Text = "Blog description cannot be empty.";
            return;
        }

        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
        int userId = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Blogs WHERE Title=@Title AND UserID=(SELECT UserID FROM Users WHERE Username=@Username)", con);
            cmdCheck.Parameters.AddWithValue("@Title", blogName);
            cmdCheck.Parameters.AddWithValue("@Username", username);
            int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
            if (count > 0)
            {
                lblBlogNameError.Text = "A blog with this name already exists.";
                return;
            }

            SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
            cmdUser.Parameters.AddWithValue("@Username", username);
            object userIdObj = cmdUser.ExecuteScalar();
            if (userIdObj == null)
            {
                lblBlogNameError.Text = "User not found.";
                return;
            }
            userId = Convert.ToInt32(userIdObj);

            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Blogs (UserID, Title, Description) VALUES (@UserID, @Title, @Desc)", con);
            cmdInsert.Parameters.AddWithValue("@UserID", userId);
            cmdInsert.Parameters.AddWithValue("@Title", blogName);
            cmdInsert.Parameters.AddWithValue("@Desc", blogDesc);
            cmdInsert.ExecuteNonQuery();
        }

        hfBlogTitle.Value = blogName;
        hfBlogTitleEditor.Value = blogName;

        blogFolder = Path.Combine(basePath, username, blogName);
        indexPath = Path.Combine(blogFolder, "index.json");

        if (!Directory.Exists(blogFolder))
        {
            Directory.CreateDirectory(blogFolder);
            Directory.CreateDirectory(Path.Combine(blogFolder, "Images"));
        }

        blogIndex = new BlogIndex { blogTitle = blogName, current = "", history = new List<VersionEntry>() };
        File.WriteAllText(indexPath, Newtonsoft.Json.JsonConvert.SerializeObject(blogIndex, Newtonsoft.Json.Formatting.Indented));

        pnlBlogName.Visible = false;
        pnlEditor.Visible = true;
        lblBlogTitle.Text = blogName;

        LoadVersionList();
        LoadExistingImages();
    }

    private void LoadBlogIndex()
    {
        string blogName = hfBlogTitle.Value;
        if (string.IsNullOrEmpty(blogName)) return;

        hfBlogTitleEditor.Value = blogName;

        blogFolder = Path.Combine(basePath, username, blogName);
        indexPath = Path.Combine(blogFolder, "index.json");

        if (File.Exists(indexPath))
            blogIndex = JsonConvert.DeserializeObject<BlogIndex>(File.ReadAllText(indexPath));
        else
            blogIndex = new BlogIndex { blogTitle = blogName, history = new List<VersionEntry>() };
    }

    private void LoadVersionList()
    {
        LoadBlogIndex();
        if (blogIndex == null) return;

        lstVersions.Items.Clear();
        foreach (var v in blogIndex.history)
            lstVersions.Items.Add(v.version);

        if (!string.IsNullOrEmpty(blogIndex.current))
        {
            var currentEntry = blogIndex.history.Find(v => v.file_path == blogIndex.current);
            lblCurrentVersion.Text = currentEntry != null ? currentEntry.version : blogIndex.current;
        }
        else
        {
            lblCurrentVersion.Text = "None";
        }
    }

    protected void btnSaveVersion_Click(object sender, EventArgs e)
    {
        try
        {
            LoadBlogIndex();
            if (blogIndex == null)
            {
                lblUploadStatus.Text = "Error: Blog not initialized.";
                return;
            }

            if (string.IsNullOrEmpty(txtCommitMessage.Text))
            {
                lblUploadStatus.Text = "Commit message is required.";
                return;
            }

            string blogContent = Request.Unvalidated["txtBlogDescription"] ?? "";

            blogContent = System.Text.RegularExpressions.Regex.Replace(
                blogContent,
                $"<img src='/Blog_Files/{username}/{hfBlogTitle.Value}/Images/(.*?)'",
                "<img src='Images/$1'",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (blogIndex.history.Count > 0)
            {
                string latestFile = Path.Combine(blogFolder, blogIndex.history[blogIndex.history.Count - 1].file_path);
                if (File.Exists(latestFile))
                {
                    string latestContent = File.ReadAllText(latestFile);
                    if (latestContent.Trim() == blogContent.Trim())
                    {
                        lblUploadStatus.Text = "No changes detected.";
                        return;
                    }
                }
            }

            int nextVersion = blogIndex.history.Count + 1;
            string versionName = "v" + nextVersion + ".html";
            string filePath = Path.Combine(blogFolder, versionName);
            File.WriteAllText(filePath, blogContent);

            var entry = new VersionEntry
            {
                version = "v" + nextVersion,
                user = username,
                timestamp = DateTime.Now.ToString("s"),
                file_path = versionName,
                changeNotes = txtCommitMessage.Text
            };

            blogIndex.history.Add(entry);

            if (blogIndex.history.Count == 1)
            {
                blogIndex.current = versionName;
            }

            File.WriteAllText(indexPath, JsonConvert.SerializeObject(blogIndex, Formatting.Indented));

            lblUploadStatus.Text = $"Version {versionName} saved successfully!";
            LoadVersionList();
        }
        catch (Exception ex)
        {
            lblUploadStatus.Text = "Error: " + ex.Message;
        }
    }

    protected void lstVersions_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlogIndex();
        if (blogIndex == null) return;

        string selectedVersion = lstVersions.SelectedValue;
        var version = blogIndex.history.Find(v => v.version == selectedVersion);
        if (version != null)
        {
            string path = Path.Combine(blogFolder, version.file_path);
            if (File.Exists(path))
                txtBlogDescription.Text = File.ReadAllText(path);

            lblVersionMeta.Text = $"Version: {version.version}<br/>User: {version.user}<br/>Time: {version.timestamp}<br/>Message: {version.changeNotes}";
        }
    }

    private void LoadExistingImages()
    {
        if (string.IsNullOrEmpty(blogFolder)) return;
        if (!File.Exists(indexPath)) return; // Only for new blogs

        string imagesFolder = Path.Combine(blogFolder, "Images");
        if (!Directory.Exists(imagesFolder)) return;

        string[] files = Directory.GetFiles(imagesFolder);
        foreach (string file in files)
        {
            string relativeUrl = $"/Blog_Files/{username}/{hfBlogTitle.Value}/Images/{Path.GetFileName(file)}";
            txtBlogDescription.Text += $"<img src='{relativeUrl}' style='max-width:100%;'><br/>";
        }
    }
    protected global::System.Web.UI.WebControls.TextBox txtBlogDesc;

    protected void lnkWorkspace_Click(object sender, EventArgs e)
    {
        if (pnlEditor.Visible && !string.IsNullOrEmpty(txtBlogDescription.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "confirmSave", @"
                if(confirm('Do you want to save your changes before leaving?')) {
                    document.getElementById('btnSaveVersion').click();
                    window.location.href='Workspace.aspx';
                } else {
                    window.location.href='Workspace.aspx';
                }
            ", true);
        }
        else
        {
            Response.Redirect("Workspace.aspx");
        }
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

    protected void btnMakeLive_Click(object sender, EventArgs e)
{
    LoadBlogIndex();
    if (blogIndex == null) return;
    string selectedVersion = lstVersions.SelectedValue;
    var version = blogIndex.history.Find(v => v.version == selectedVersion);
    if (version != null)
    {
        blogIndex.current = version.file_path;
        File.WriteAllText(indexPath, Newtonsoft.Json.JsonConvert.SerializeObject(blogIndex, Newtonsoft.Json.Formatting.Indented));
        lblCurrentVersion.Text = version.version;
        lblUploadStatus.Text = $"{version.version} is now live.";
    }
}

protected void btnAddCollaborator_Click(object sender, EventArgs e)
{
    string collaboratorUsername = txtCollaborator.Text.Trim();
    lblUploadStatus.Text = ""; // Clear previous status
    if (string.IsNullOrEmpty(collaboratorUsername))
    {
        lblUploadStatus.Text = "Please enter a username.";
        return;
    }
    string blogIdStr = Request.QueryString["BlogID"];
    int blogId = 0;
    if (!string.IsNullOrEmpty(blogIdStr))
        int.TryParse(blogIdStr, out blogId);
    if (blogId == 0)
    {
        lblUploadStatus.Text = "Blog not found.";
        return;
    }
    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
    int collaboratorUserId = 0;
    try
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
            cmdUser.Parameters.AddWithValue("@Username", collaboratorUsername);
            object userIdObj = cmdUser.ExecuteScalar();
            if (userIdObj == null)
            {
                lblUploadStatus.Text = "User not found.";
                return;
            }
            collaboratorUserId = Convert.ToInt32(userIdObj);
            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Collaborators WHERE BlogID=@BlogID AND UserID=@UserID", con);
            cmdCheck.Parameters.AddWithValue("@BlogID", blogId);
            cmdCheck.Parameters.AddWithValue("@UserID", collaboratorUserId);
            int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
            if (count > 0)
            {
                lblUploadStatus.Text = "User is already a collaborator.";
                return;
            }
            SqlCommand cmdAdd = new SqlCommand("INSERT INTO Collaborators (BlogID, UserID) VALUES (@BlogID, @UserID)", con);
            cmdAdd.Parameters.AddWithValue("@BlogID", blogId);
            cmdAdd.Parameters.AddWithValue("@UserID", collaboratorUserId);
            int rowsAffected = cmdAdd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                lblUploadStatus.Text = "Collaborator added successfully.";
                LoadCollaborators(blogId); // Refresh collaborator list
            }
            else
            {
                lblUploadStatus.Text = "Failed to add collaborator.";
            }
        }
    }
    catch (Exception ex)
    {
        lblUploadStatus.Text = "Error: " + ex.Message;
    }
}

private void LoadCollaborators(int blogId)
{
    lstCollaborators.Items.Clear();
    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
    using (SqlConnection con = new SqlConnection(connectionString))
    {
        con.Open();
        SqlCommand cmd = new SqlCommand("SELECT u.Username FROM Collaborators c INNER JOIN Users u ON c.UserID = u.UserID WHERE c.BlogID=@BlogID", con);
        cmd.Parameters.AddWithValue("@BlogID", blogId);
        using (var rdr = cmd.ExecuteReader())
        {
            while (rdr.Read())
            {
                var item = new System.Web.UI.WebControls.ListItem(rdr["Username"].ToString(), rdr["Username"].ToString());
                lstCollaborators.Items.Add(item);
            }
        }
    }
    if (lstCollaborators.Items.Count > 0)
    {
        lstCollaborators.SelectedIndex = 0;
    }
}

protected void btnRemoveCollaborator_Click(object sender, EventArgs e)
{
    string selectedUser = lstCollaborators.SelectedValue;
    if (string.IsNullOrEmpty(selectedUser))
    {
        lblUploadStatus.Text = "Please select a collaborator to remove.";
        return;
    }
    string blogIdStr = Request.QueryString["BlogID"];
    int blogId = 0;
    if (!string.IsNullOrEmpty(blogIdStr))
        int.TryParse(blogIdStr, out blogId);
    if (blogId == 0)
    {
        lblUploadStatus.Text = "Blog not found.";
        return;
    }
    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
    using (SqlConnection con = new SqlConnection(connectionString))
    {
        con.Open();
        SqlCommand cmdUser = new SqlCommand("SELECT UserID FROM Users WHERE Username=@Username", con);
        cmdUser.Parameters.AddWithValue("@Username", selectedUser);
        object userIdObj = cmdUser.ExecuteScalar();
        if (userIdObj != null)
        {
            int userId = Convert.ToInt32(userIdObj);
            SqlCommand cmdRemove = new SqlCommand("DELETE FROM Collaborators WHERE BlogID=@BlogID AND UserID=@UserID", con);
            cmdRemove.Parameters.AddWithValue("@BlogID", blogId);
            cmdRemove.Parameters.AddWithValue("@UserID", userId);
            int rowsAffected = cmdRemove.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                lblUploadStatus.Text = "Collaborator removed successfully.";
                LoadCollaborators(blogId);
            }
            else
            {
                lblUploadStatus.Text = "Failed to remove collaborator.";
            }
        }
        else
        {
            lblUploadStatus.Text = "User not found.";
        }
    }
}
    }
}
