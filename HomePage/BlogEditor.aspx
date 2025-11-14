<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlogEditor.aspx.cs" Inherits="HomePage.BlogEditor" ValidateRequest="false" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blog Editor</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/summernote/dist/summernote-lite.min.css" rel="stylesheet" />
    <link href="Site.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/summernote/dist/summernote-lite.min.js"></script>

    <style>
        .note-editable img, .note-editable video { display: block; margin: 0 auto; }
        .user-dropdown-container {
            position: relative;
        }
        .user-menu {
            cursor: pointer;
            font-weight: bold;
            color: #0366d6;
            margin-right: 20px;
            padding: 6px 12px;
            border-radius: 4px;
            background: #f3f3f3;
            transition: background 0.2s;
            border: none;
        }
        .user-menu:hover {
            background: #e2e2e2;
        }
        .user-dropdown {
            position: absolute;
            right: 0;
            top: 40px;
            background: #fff;
            border: 1px solid #ccc;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.12);
            padding: 18px 24px;
            z-index: 1000;
            min-width: 220px;
        }
        .session-time-label {
            display: block;
            margin-bottom: 12px;
            color: #333;
            font-size: 15px;
        }
        .signout-btn {
            background: #d73a49;
            color: #fff;
            border: none;
            border-radius: 4px;
            padding: 6px 18px;
            cursor: pointer;
        }
        .signout-btn:hover {
            background: #c12233;
        }
        .nav-right {
            float: right;
            display: flex;
            align-items: center;
            gap: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlNavbar" runat="server" CssClass="navbar" style="width:100vw;min-width:100vw;margin:0;padding:0;">
            <asp:Panel ID="pnlNavLeft" runat="server" CssClass="nav-left">
                <a href="Home.aspx" class="logo" style="display:inline-block;vertical-align:middle;">
                    <img src="NBLOGlogo.png" alt="NBlog Logo" style="height:32px;vertical-align:middle;" />
                </a>
                <asp:HyperLink ID="lnkBlogs" runat="server" NavigateUrl="Blog.aspx" Text="Blogs"></asp:HyperLink>
                <asp:HyperLink ID="lnkWorkspace" runat="server" NavigateUrl="Workspace.aspx" Text="Workspace"></asp:HyperLink>
            </asp:Panel>
            <asp:Panel ID="pnlNavRight" runat="server" CssClass="nav-right" style="float:right; display:flex; align-items:center; gap:10px;">
                <div class="user-dropdown-container" runat="server" id="userDropdownContainer" style="display:inline-block; position:relative;">
                    <asp:Button ID="btnUserMenu" runat="server" CssClass="user-menu" Visible="true" />
                    <div id="userDropdown" class="user-dropdown" style="display:none; right:0; left:auto; min-width:220px;">
                        <asp:Label ID="lblSessionTime" runat="server" CssClass="session-time-label" />
                        <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" CssClass="btn signout-btn" OnClick="btnSignOut_Click" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlMainContent" runat="server" style="display:flex;flex-direction:column;align-items:center;justify-content:center;width:100vw;min-height:80vh;">
            <div style="width:80%;max-width:900px;margin:40px auto;">
                <h2 class="mb-4" style="text-align:center;">Blog Editor</h2>
                <asp:Panel ID="pnlBlogName" runat="server">
                    <div class="form-group">
                        <label for="txtBlogName">Blog Title</label>
                        <asp:TextBox ID="txtBlogName" runat="server" CssClass="form-control" Placeholder="Enter blog title"></asp:TextBox>
                        <asp:Label ID="lblBlogNameError" runat="server" CssClass="text-danger"></asp:Label>
                    </div>
                    <div class="form-group">
                        <label for="txtBlogDesc">Blog Description</label>
                        <asp:TextBox ID="txtBlogDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" Placeholder="Enter blog description"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnCreateBlog" runat="server" Text="Create Blog" CssClass="btn btn-primary" OnClick="btnCreateBlog_Click" />
                </asp:Panel>
                <asp:Panel ID="pnlEditor" runat="server" Visible="false">
                    <h4 style="text-align:center;"><asp:Literal ID="lblBlogTitle" runat="server"></asp:Literal></h4>
                    <asp:HiddenField ID="hfBlogTitleEditor" runat="server" />
                    <asp:HiddenField ID="hfBlogTitle" runat="server" />
                    <div class="form-group">
                        <label for="txtBlogDescription">Content</label>
                        <asp:TextBox ID="txtBlogDescription" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtCommitMessage">Commit Message</label>
                        <asp:TextBox ID="txtCommitMessage" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnSaveVersion" runat="server" Text="Save Version" CssClass="btn btn-success" OnClick="btnSaveVersion_Click" />
                    <asp:Label ID="lblUploadStatus" runat="server" CssClass="text-success ml-2"></asp:Label>
                    <div class="mt-4">
                        <h5>Previous Versions</h5>
                        <div style="margin-bottom:10px;">
                            <strong>Live Version:</strong>
                            <asp:Label ID="lblCurrentVersion" runat="server" CssClass="badge badge-success" />
                        </div>
                        <asp:ListBox ID="lstVersions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstVersions_SelectedIndexChanged"
                            CssClass="form-control" Height="200px"></asp:ListBox>
                        <asp:Button ID="btnMakeLive" runat="server" Text="Make Live" CssClass="btn btn-warning mt-2" OnClick="btnMakeLive_Click" />
                        <asp:Label ID="lblVersionMeta" runat="server" CssClass="mt-2 d-block"></asp:Label>
                    </div>
                    <asp:Panel ID="pnlCollaborators" runat="server" CssClass="mt-4">
                        <h5>Add Collaborators</h5>
                        <asp:TextBox ID="txtCollaborator" runat="server" CssClass="form-control" Placeholder="Enter username"></asp:TextBox>
                        <asp:Button ID="btnAddCollaborator" runat="server" Text="Add" CssClass="btn btn-info mt-2" OnClick="btnAddCollaborator_Click" />
                        <asp:ListBox ID="lstCollaborators" runat="server" CssClass="form-control mt-2" Height="120px" />
                        <asp:Button ID="btnRemoveCollaborator" runat="server" Text="Remove Selected" CssClass="btn btn-danger mt-2" OnClick="btnRemoveCollaborator_Click" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </asp:Panel>
    </form>
    <script>
        let isDirty = false;
        $(document).ready(function () {
            const editorId = '#<%= txtBlogDescription.ClientID %>';
            $(editorId).summernote({
                placeholder: 'Type your blog content here...',
                height: 300,
                callbacks: {
                    onChange: function(contents, $editable) {
                        isDirty = true;
                    },
                    onImageUpload: function(files) {
                        isDirty = true;
                        for (var i = 0; i < files.length; i++) {
                            uploadImage(files[i]);
                        }
                    }
                }
            });
            $('.navbar a, .navbar .logo').on('click', function(e) {
                var href = $(this).attr('href');
                if (isDirty && href && href.indexOf('Workspace.aspx') === -1) {
                    e.preventDefault();
                    if (confirm('You have unsaved changes. Save before leaving?')) {
                        $('#<%= btnSaveVersion.ClientID %>').click();
                        setTimeout(function() { window.location.href = href; }, 1000);
                    } else {
                        isDirty = false;
                        window.location.href = href;
                    }
                }
            });
            $('#<%= btnSaveVersion.ClientID %>').on('click', function() {
                isDirty = false;
            });
        });
        function uploadImage(file) {
            var blogTitle = $('#<%= hfBlogTitleEditor.ClientID %>').val();
            var data = new FormData();
            data.append("file", file);
            data.append("blogTitle", blogTitle);
            $.ajax({
                url: 'UploadImage.ashx',
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function(url) {
                    $('#<%= txtBlogDescription.ClientID %>').summernote('insertImage', url);
                },
                error: function (err) {
                    alert("Image upload failed: " + err.responseText);
                }
            });
        }
    </script>
    <script type="text/javascript">
    function startSessionCountdown() {
        var timerSpan = document.getElementById('session-timer');
        if (!timerSpan) return;
        var timeParts = timerSpan.innerText.split(':');
        var totalSeconds = parseInt(timeParts[0],10)*3600 + parseInt(timeParts[1],10)*60 + parseInt(timeParts[2],10);
        var interval = setInterval(function() {
            if (totalSeconds <= 0) {
                timerSpan.innerText = '00:00:00';
                clearInterval(interval);
                return;
            }
            totalSeconds--;
            var h = Math.floor(totalSeconds/3600).toString().padStart(2,'0');
            var m = Math.floor((totalSeconds%3600)/60).toString().padStart(2,'0');
            var s = (totalSeconds%60).toString().padStart(2,'0');
            timerSpan.innerText = h+':'+m+':'+s;
        }, 1000);
    }
    document.addEventListener('DOMContentLoaded', startSessionCountdown);
    </script>
</body>
</html>
