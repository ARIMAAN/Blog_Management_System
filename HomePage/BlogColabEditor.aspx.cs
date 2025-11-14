using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace HomePage
{
    public partial class BlogColabEditor : System.Web.UI.Page
    {
        string basePath => Server.MapPath("~/Blog_Files");
        string connectionString = ConfigurationManager.ConnectionStrings["BlogServer"].ConnectionString;
        BlogEditor.BlogIndex blogIndex;
        string blogFolder;
        string indexPath;
        string ownerUsername;
        string blogTitle;
        string username => Session["Username"]?.ToString() ?? "Guest";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string blogIdStr = Request.QueryString["BlogID"];
                if (!string.IsNullOrEmpty(blogIdStr))
                {
                    int blogId;
                    if (int.TryParse(blogIdStr, out blogId))
                    {
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand("SELECT b.Title, b.Description, u.Username AS Owner FROM Blogs b INNER JOIN Users u ON b.UserID = u.UserID WHERE b.BlogID=@BlogID", con);
                            cmd.Parameters.AddWithValue("@BlogID", blogId);
                            using (var rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    blogTitle = rdr["Title"].ToString();
                                    ownerUsername = rdr["Owner"].ToString();
                                    lblBlogTitle.Text = blogTitle;
                                    lblOwner.Text = "Owner: " + ownerUsername;
                                    lblDescription.Text = "Description: " + rdr["Description"].ToString();
                                    hfOwnerUsername.Value = ownerUsername;
                                    hfBlogTitle.Value = blogTitle;
                                }
                            }
                        }
                        blogFolder = Path.Combine(basePath, ownerUsername, blogTitle);
                        indexPath = Path.Combine(blogFolder, "index.json");
                        if (File.Exists(indexPath))
                        {
                            blogIndex = JsonConvert.DeserializeObject<BlogEditor.BlogIndex>(File.ReadAllText(indexPath));
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
                            blogIndex = new BlogEditor.BlogIndex { blogTitle = blogTitle, history = new List<BlogEditor.VersionEntry>() };
                        }
                    }
                }
                // Show logged-in username in the navbar
                btnUserMenu.Text = username;
                // Session time logic for dropdown
                DateTime sessionStart = Session["SessionStart"] != null ? (DateTime)Session["SessionStart"] : DateTime.Now;
                if (Session["SessionStart"] == null) Session["SessionStart"] = sessionStart;
                TimeSpan remaining = TimeSpan.FromHours(3) - (DateTime.Now - sessionStart);
                lblSessionTime.Text = "Session expires in: <span id='session-timer'>" + remaining.ToString(@"hh\:mm\:ss") + "</span>";
            }
        }

        private void LoadBlogIndex()
        {
            string owner = hfOwnerUsername.Value;
            string title = hfBlogTitle.Value;
            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(title)) return;
            blogFolder = Path.Combine(basePath, owner, title);
            indexPath = Path.Combine(blogFolder, "index.json");
            if (File.Exists(indexPath))
                blogIndex = JsonConvert.DeserializeObject<BlogEditor.BlogIndex>(File.ReadAllText(indexPath));
            else
                blogIndex = new BlogEditor.BlogIndex { blogTitle = title, history = new List<BlogEditor.VersionEntry>() };
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
                    $"<img src='/Blog_Files/{hfOwnerUsername.Value}/{hfBlogTitle.Value}/Images/(.*?)'",
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
                var entry = new BlogEditor.VersionEntry
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