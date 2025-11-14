<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="HomePage.SignIn" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign In</title>
    <link href="Site.css" rel="stylesheet" />
    <style>
        .center-wrapper {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }

        .signin-panel {
            background: #fff;
            padding: 30px 40px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            width: 350px;
            text-align: center;
        }

        .signin-panel h2 {
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 15px;
            text-align: left;
        }

        .input-field {
            width: 100%;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .btn {
            width: 100%;
            padding: 10px;
        }

        .form-group a {
            color: #0366d6;
            text-decoration: none;
            font-size: 14px;
        }

        .form-group a:hover {
            text-decoration: underline;
        }

        .create-account-text {
            font-size: 14px;
            text-align: center;
        }

        hr {
            margin: 20px 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="center-wrapper">
            <asp:Panel ID="pnlSignIn" runat="server" CssClass="signin-panel">
                <h2>Sign In</h2>
                <asp:Label  ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                <div class="form-group">
                    <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtUsername"></asp:Label><br />
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblPassword" runat="server" Text="Password:" AssociatedControlID="txtPassword"></asp:Label><br />
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="input-field" TextMode="Password"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:CheckBox ID="chkRemember" runat="server" Text="" />
                </div>

                <div class="form-group">
                    <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn" OnClick="btnLogin_Click" />
                </div>

                

                <hr />

                <div class="form-group create-account-text">
                    <asp:Label ID="lblNoAccount" runat="server" Text="Don't have an account? "></asp:Label>
                    <asp:HyperLink ID="lnkSignUp" runat="server" NavigateUrl="SignUp.aspx">Create one</asp:HyperLink>
                </div>
            </asp:Panel>
        </div>

    </form>
</body>
</html>
