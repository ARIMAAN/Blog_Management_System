<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="HomePage.AboutUs" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>About Us - NBlog</title>
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
        </asp:Panel>

        <!-- About Us Section -->
        <div class="content-section">
            <asp:Label ID="lblHeading" runat="server" Text="About Us" Font-Size="XX-Large" Font-Bold="True"></asp:Label>
            <br /><br />
            <asp:Label ID="lblIntro" runat="server" 
                Text="At NBlog, we believe in the power of words to inspire, educate, and connect people across the globe. Our platform is built for passionate writers and engaged readers." 
                Font-Size="Large"></asp:Label>
            <br /><br />
            <!-- Mission -->
            <div class="feature-box">
                <asp:Label ID="lblMissionTitle" runat="server" Text="🎯 Our Mission" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblMissionDesc" runat="server" Text="To empower creators by providing a secure, collaborative, and accessible space where ideas can flourish without limits." CssClass="feature-desc"></asp:Label>
            </div>

            <!-- Vision -->
            <div class="feature-box">
                <asp:Label ID="lblVisionTitle" runat="server" Text="🌍 Our Vision" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblVisionDesc" runat="server" Text="A world where every voice can be heard, every story can be shared, and every reader can find content that resonates with them." CssClass="feature-desc"></asp:Label>
            </div>

            <!-- Values -->
            <div class="feature-box">
                <asp:Label ID="lblValue1Title" runat="server" Text="🤝 Collaboration" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblValue1Desc" runat="server" Text="We believe the best content comes from collective creativity and teamwork." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblValue2Title" runat="server" Text="💡 Innovation" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblValue2Desc" runat="server" Text="We embrace new ideas and technologies to make blogging smarter and easier." CssClass="feature-desc"></asp:Label>
            </div>

            <div class="feature-box">
                <asp:Label ID="lblValue3Title" runat="server" Text="🔒 Integrity" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblValue3Desc" runat="server" Text="We prioritize honesty, transparency, and trust in everything we do." CssClass="feature-desc"></asp:Label>
            </div>

            <!-- Team Section -->
            <div class="feature-box">
                <asp:Label ID="lblTeamTitle" runat="server" Text="👨‍💻 Meet Our Team" CssClass="feature-title"></asp:Label>
                <asp:Label ID="lblTeamDesc" runat="server" Text="A diverse group of developers, writers, and designers dedicated to building a platform that serves our global community of creators." CssClass="feature-desc"></asp:Label>
            </div>
        </div>

        <asp:Panel ID="pnlSessionInfo" runat="server" Visible="false"></asp:Panel>
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
</body>
</html>
