<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Blog.aspx.cs" Inherits="HomePage.Blog" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Blog</title>
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
        
        <!-- User Session Info Panel (hidden by default, shown on user menu click) -->
        <asp:Panel ID="pnlSessionInfo" runat="server" Visible="false">
            <!-- You can add session info content here if needed -->
        </asp:Panel>
        
        <!-- Navbar -->
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
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Search blog..."></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn" OnClick="btnSearch_Click" />
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn" Visible="false" />
                <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="btn" Visible="false" />
            </asp:Panel>

            <div class="user-dropdown-container" runat="server" id="userDropdownContainer" style="display:inline-block; position:relative;">
                <asp:Button ID="btnUserMenu" runat="server" CssClass="user-menu" Visible="false" OnClientClick="toggleUserDropdown(); return false;" />
                <div id="userDropdown" class="user-dropdown" style="display:none; right:0; left:auto; min-width:220px;">
                    <asp:Label ID="lblSessionTime" runat="server" CssClass="session-time-label" />
                    <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" CssClass="btn signout-btn" OnClick="btnSignOut_Click" />
                </div>
            </div>
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
        </asp:Panel>

        <asp:Panel ID="pnlBlogContainer" runat="server" CssClass="blog-container">
            <asp:Label ID="lblLatestBlogs" runat="server" Text="Latest Blogs" Font-Size="XX-Large" Font-Bold="true"></asp:Label>

            <asp:Repeater ID="rptBlogs" runat="server">
                <ItemTemplate>
                    <asp:Panel runat="server" CssClass="blog-card">
                        <asp:Label runat="server" Text='<%# Eval("Title") %>' Font-Size="X-Large" Font-Bold="true"></asp:Label><br />
                        <asp:Label runat="server" CssClass="blog-meta" 
                            Text='<%# "By " + Eval("Author") + " | " + String.Format("{0:MMM dd, yyyy}", Eval("Date")) %>'></asp:Label>
                        <br />
                        <asp:Label runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                        <br />
                        <asp:HyperLink runat="server" CssClass="read-more" NavigateUrl='<%# Eval("Link") %>' Text="Read More →"></asp:HyperLink>
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <asp:Label ID="lblError" runat="server" CssClass="text-danger" Style="display:block;margin:10px 0;font-weight:bold;" />
    </form>
</body>
</html>
