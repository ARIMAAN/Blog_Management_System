<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="HomePage.Home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NBlog - Home</title>
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

        <asp:Panel ID="pnlNavbar" runat="server" CssClass="navbar">
            <asp:Panel ID="pnlNavLeft" runat="server" CssClass="nav-left">
                <a href="Home.aspx" class="logo" style="display:inline-block;vertical-align:middle;">
                    <img src="NBLOGlogo.png" alt="NBlog Logo" style="height:32px;vertical-align:middle;" />
                </a>
                <asp:HyperLink ID="lnkBlogs" runat="server" NavigateUrl="Blog.aspx" Text="Blogs"></asp:HyperLink>
                <asp:HyperLink ID="lnkAbout" runat="server" NavigateUrl="AboutUs.aspx" Text="About Us"></asp:HyperLink>
                <asp:HyperLink ID="lnkWorkspace" runat="server" NavigateUrl="Workspace.aspx" Text="Workspace"></asp:HyperLink>
            </asp:Panel>

            <asp:Panel ID="pnlNavRight" runat="server" CssClass="nav-right" style="float:right; display:flex; align-items:center; gap:10px;">
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn" OnClick="btnSignIn_Click" />
                <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="btn" OnClick="btnSignUp_Click" />
                <div class="user-dropdown-container" runat="server" id="userDropdownContainer" style="display:inline-block; position:relative;">
                    <asp:Button ID="btnUserMenu" runat="server" CssClass="user-menu" Visible="false" OnClientClick="toggleUserDropdown(); return false;" />
                    <div id="userDropdown" class="user-dropdown" style="display:none; right:0; left:auto; min-width:220px;">
                        <asp:Label ID="lblSessionTime" runat="server" CssClass="session-time-label" />
                        <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" CssClass="btn signout-btn" OnClick="btnSignOut_Click" />
                    </div>
                </div>
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
                // Realtime session countdown
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
        </asp:Panel>

        <div class="content-section">
            <asp:Label ID="lblWelcome" runat="server" Text="Welcome to NBlog" Font-Size="XX-Large" Font-Bold="True"></asp:Label>
            <br /><br />
            <asp:Label ID="lblIntro" runat="server" Text="Your go-to platform for secure, collaborative, and user-friendly blogging." Font-Size="Large"></asp:Label>
            <br /><br />

            <div class="feature-box">
                <asp:Label ID="lblFeature1" runat="server" Text="🤝 Collaboration" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature1Desc" runat="server" Text="Work together with other authors in real-time to create amazing content." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature2" runat="server" Text="🔒 Security" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature2Desc" runat="server" Text="Your data is safe with our top-notch encryption and privacy measures." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature3" runat="server" Text="✨ User-Friendly" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature3Desc" runat="server" Text="A clean, simple, and intuitive interface designed for effortless blogging." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature4" runat="server" Text="🚀 Launch with Confidence" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature4Desc" runat="server" Text="Get a custom domain and reliable hosting so your site runs smoothly even during high traffic." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature5" runat="server" Text="📈 Grow Your Presence" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature5Desc" runat="server" Text="Reach more people with built-in SEO tools, Google Ads integration, email marketing, and social sharing." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature6" runat="server" Text="🎨 Design Freedom" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature6Desc" runat="server" Text="Start with AI-generated layouts or choose from our professionally designed templates and make them your own." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblFeature7" runat="server" Text="💡 Your Future Starts Here" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblFeature7Desc" runat="server" Text="Stand out online, share your voice, and grow your audience—NBlog is here to help you at every step." CssClass="feature-desc"></asp:Label>
            </div>
        </div>

        <div style="display:none;">
            <asp:Label ID="lblUserMenu" runat="server" />
        </div>

        <asp:Panel ID="pnlSessionInfo" runat="server" Visible="false" />

    </form>
</body>
</html>
