<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReaderBlog.aspx.cs" Inherits="HomePage.ReaderBlog" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blog Reader</title>
    <link href="Site.css" rel="stylesheet" />
    <style>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlNavbar" runat="server" CssClass="navbar">
            <asp:Panel ID="pnlNavLeft" runat="server" CssClass="nav-left">
                <a href="Home.aspx" class="logo" style="display:inline-block;vertical-align:middle;">
                    <img src="NBLOGlogo.png" alt="NBlog Logo" style="height:32px;vertical-align:middle;" />
                </a>
                <asp:HyperLink ID="lnkBlogs" runat="server" NavigateUrl="Blog.aspx" Text="Blogs"></asp:HyperLink>
                <asp:HyperLink ID="lnkAbout" runat="server" NavigateUrl="AboutUs.aspx" Text="About Us"></asp:HyperLink>
                <asp:HyperLink ID="lnkWorkspace" runat="server" NavigateUrl="Workspace.aspx" Text="Workspace"></asp:HyperLink>
            </asp:Panel>
            <asp:Panel ID="pnlNavRight" runat="server" CssClass="nav-right">
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn" Visible="false" OnClick="btnSignIn_Click1" />
                <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="btn" Visible="false" OnClick="btnSignUp_Click1" />
                <div class="user-dropdown-container" runat="server" id="userDropdownContainer" style="display:inline-block; position:relative;">
                    <asp:Button ID="btnUserMenu" runat="server" CssClass="user-menu" Visible="false" OnClientClick="toggleUserDropdown(); return false;" />
                    <div id="userDropdown" class="user-dropdown" style="display:none; right:0; left:auto; min-width:220px;">
                        <asp:Label ID="lblSessionTime" runat="server" CssClass="session-time-label" />
                        <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" CssClass="btn signout-btn" OnClick="btnSignOut_Click" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <div class="main-row" style="display:flex; justify-content:center; align-items:center;">
            <div class="blog-col" style="width:100%; max-width:700px; margin:0 auto;">
                <asp:Panel ID="pnlInfo" runat="server" CssClass="info-panel" style="margin-bottom:20px; text-align:center; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.12); padding:18px;">
                    <asp:Label ID="lblOwner" runat="server" Font-Bold="true" CssClass="owner-text" />
                    <asp:Label ID="lblCreated" runat="server" CssClass="blog-meta" />
                    <asp:Label ID="lblDescription" runat="server" CssClass="blog-meta" />
                </asp:Panel>
                <asp:Panel ID="pnlBlogContent" runat="server" CssClass="blog-content-panel" style="background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.12); padding:32px; text-align:left;">
                    <asp:Literal ID="litBlogContent" runat="server"></asp:Literal>
                </asp:Panel>
                <asp:Panel ID="pnlCollaborators" runat="server" CssClass="collab-panel" style="margin-top:24px; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.12); padding:18px;">
                    <h5>Collaborator</h5>
                    <div style="margin-top:16px;">
                        <strong>Collaborators:</strong>
                        <asp:Label ID="lblCollaborators" runat="server" CssClass="collaborator-text" />
                    </div>
                    <div style="margin-top:16px;">
                        <asp:Button ID="btnRequestCollab" runat="server" Text="Request Collaboration" CssClass="btn btn-primary" OnClick="btnRequestCollab_Click" />
                        <asp:Button ID="btnSignInCollab" runat="server" Text="Sign In to Collaborate" CssClass="btn btn-secondary" OnClick="btnSignIn_Click1" />
                        <asp:Button ID="btnSignUpCollab" runat="server" Text="Sign Up to Collaborate" CssClass="btn btn-secondary" OnClick="btnSignUp_Click1" />
                        <asp:Label ID="lblCollabMsg" runat="server" CssClass="collab-msg" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        function toggleUserDropdown() {
            var dropdown = document.getElementById('userDropdown');
            if (dropdown) {
                dropdown.style.display = (dropdown.style.display === 'block') ? 'none' : 'block';
            }
        }
        document.addEventListener('click', function(event) {
            var dropdown = document.getElementById('userDropdown');
            var button = document.getElementById('<%= btnUserMenu.ClientID %>');
            if (dropdown && button && !button.contains(event.target) && !dropdown.contains(event.target)) {
                dropdown.style.display = 'none';
            }
        });
    </script>
</body>
</html>
