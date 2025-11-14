<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Workspace.aspx.cs" Inherits="HomePage.Workspace" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Workspace</title>
    <link href="Site.css" rel="stylesheet" />
    <style>
        .workspace-title {
            text-align: center;
            font-size: 32px;
            font-weight: bold;
            margin-top: 30px;
            margin-bottom: 10px;
            color: #2c974b;
        }
        .my-works {
            text-align: center;
            font-size: 24px;
            font-weight: bold;
            margin: 20px 0;
            color: #333;
        }
        .sort-panel {
            width: 90%;
            margin: 0 auto 20px auto;
            text-align: right;
            font-size: 14px;
        }
        .blog-card {
            background: white;
            padding: 15px;
            margin: 15px auto;
            width: 90%;
            border-radius: 8px;
            box-shadow: 0px 2px 6px rgba(0,0,0,0.1);
        }
        .blog-card-buttons {
            margin-top: 10px;
            text-align: right;
        }
        .toggle-btn, .delete-btn, .preview-btn {
            margin-left: 5px;
            padding: 5px 10px;
            border-radius: 4px;
            border: none;
            cursor: pointer;
        }
        .toggle-btn { background-color: #2ea44f; color: white; }
        .toggle-btn:hover { background-color: #2c974b; }
        .delete-btn { background-color: #d73a49; color: white; }
        .delete-btn:hover { background-color: #c12233; }
        .preview-btn { background-color: #0366d6; color: white; }
        .preview-btn:hover { background-color: #024e9b; }

        .owner-title { color: green; }
        .collaborator-title { color: blue; }

        .center-panel {
            margin: 80px auto;
            width: 400px;
            text-align: center;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.12);
            padding: 40px 20px;
        }

        .center-vertical {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 180px;
        }

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
            left: auto;
            top: 40px;
            background: #fff;
            border: 1px solid #ccc;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.12);
            padding: 18px 24px;
            z-index: 1000;
            min-width: 220px;
            max-width: 320px;
            width: max-content;
            box-sizing: border-box;
            overflow-x: auto;
        }
        @media (max-width: 600px) {
            .user-dropdown {
                left: 0;
                right: auto;
                width: 90vw;
                min-width: unset;
                max-width: unset;
            }
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

        <asp:Panel ID="pnlSessionInfo" runat="server" Visible="false"></asp:Panel>

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
                <div class="user-dropdown-container" runat="server" id="userDropdownContainer" style="display:inline-block;">
                    <asp:Button ID="btnUserMenu" runat="server" CssClass="user-menu" Visible="false" OnClientClick="toggleUserDropdown(); return false;" />
                    <div id="userDropdown" class="user-dropdown" style="display:none;">
                        <asp:Label ID="lblSessionTime" runat="server" CssClass="session-time-label" />
                        <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" CssClass="btn signout-btn" OnClick="btnSignOut_Click" />
                    </div>
                </div>
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn" Visible="true" OnClick="btnSignIn_Click" />
                <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="btn" Visible="true" OnClick="btnSignUp_Click" />
            </asp:Panel>
        </asp:Panel>

        <div class="center-vertical">
            <asp:Label ID="lblWelcome" runat="server"  CssClass="workspace-title" />
        </div>

        <asp:Panel ID="pnlFeatures" runat="server" CssClass="content-section" style="text-align:center;">
            <asp:Button ID="btnCreate" runat="server" Text="Create New Blog" CssClass="feature-box" OnClick="btnCreate_Click" />
        </asp:Panel>

        <div class="my-works">My Works</div>

        <asp:Panel ID="pnlSort" runat="server" CssClass="sort-panel">
            Sort By:
            <asp:DropDownList ID="ddlSortBy" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortBy_SelectedIndexChanged">
                <asp:ListItem Text="Date" Value="Date" />
                <asp:ListItem Text="Status" Value="Status" />
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="pnlUserBlogs" runat="server">
            <asp:Repeater ID="rptUserBlogs" runat="server" OnItemCommand="rptUserBlogs_ItemCommand">
                <ItemTemplate>
                    <div class="blog-card">
                        <asp:Label runat="server" 
                                   Text='<%# Eval("Title") %>' 
                                   Font-Size="X-Large" 
                                   Font-Bold="true" 
                                   CssClass='<%# Eval("Role").ToString() == "owner" ? "owner-title" : "collaborator-title" %>'>
                        </asp:Label><br />

                        <asp:Label runat="server" CssClass="blog-meta" 
                                   Text='<%# "Status: " + Eval("Status") + " | " + Eval("Date") %>'>
                        </asp:Label>
                        <br />
                        <asp:HyperLink runat="server" CssClass="read-more" NavigateUrl='<%# Eval("Link") %>' Text="Edit →"></asp:HyperLink>
                        <asp:HyperLink runat="server" CssClass="preview-btn" NavigateUrl='<%# Eval("PreviewLink") %>' Target="_blank" Text="Preview"></asp:HyperLink>

                        <div class="blog-card-buttons">
                            <asp:Button ID="btnToggleStatus" runat="server" 
                                Text='<%# Eval("Status").ToString() == "Published" ? "Make Private" : "Publish" %>' 
                                CssClass="toggle-btn" 
                                CommandName="ToggleStatus" 
                                CommandArgument='<%# Eval("ID") %>' 
                                Visible='<%# Eval("Role").ToString() == "owner" %>' />
                            <asp:Button ID="btnDelete" runat="server" 
                                Text="Delete" 
                                CssClass="delete-btn" 
                                CommandName="DeleteBlog" 
                                CommandArgument='<%# Eval("ID") %>' 
                                Visible='<%# Eval("Role").ToString() == "owner" %>' />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <asp:Panel ID="pnlNotLoggedIn" runat="server" CssClass="center-panel" Visible="false" style="margin: 80px auto; width: 400px; text-align: center; background: #fff; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.12); padding: 40px 20px;">
            <asp:Label ID="lblNeedAccount" runat="server" Text="Need an account to continue" Font-Size="Large" Font-Bold="true" Style="display:block; margin-bottom: 30px; color: #333;" />
            <asp:Button ID="btnPanelSignIn" runat="server" Text="Sign In" CssClass="btn" Style="margin: 0 10px 0 0; width: 120px;" OnClick="btnPanelSignIn_Click" />
            <asp:Button ID="btnPanelSignUp" runat="server" Text="Sign Up" CssClass="btn" Style="margin: 0 0 0 10px; width: 120px;" OnClick="btnPanelSignUp_Click" />
        </asp:Panel>

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
            function startSessionCountdown() {
                var timerSpan = document.getElementById('session-timer');
                if (!timerSpan) return;
                var timeParts = timerSpan.innerText.split(':');
                var totalSeconds = parseInt(timeParts[0], 10) * 3600 + parseInt(timeParts[1], 10) * 60 + parseInt(timeParts[2], 10);
                var interval = setInterval(function () {
                    if (totalSeconds <= 0) {
                        timerSpan.innerText = '00:00:00';
                        clearInterval(interval);
                        return;
                    }
                    totalSeconds--;
                    var h = Math.floor(totalSeconds / 3600).toString().padStart(2, '0');
                    var m = Math.floor((totalSeconds % 3600) / 60).toString().padStart(2, '0');
                    var s = (totalSeconds % 60).toString().padStart(2, '0');
                    timerSpan.innerText = h + ':' + m + ':' + s;
                }, 1000);
            }
            document.addEventListener('DOMContentLoaded', startSessionCountdown);
        </script>

    </form>
</body>
</html>
