<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="HomePage.SignUp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }
        .center-container {
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .signup-panel {
            background-color: #fff;
            padding: 25px 35px;
            border-radius: 8px;
            box-shadow: 0 0 15px rgba(0,0,0,0.1);
            width: 350px;
            text-align: center;
        }
        .panel-title {
            font-size: 22px;
            margin-bottom: 20px;
            color: #333;
        }
        .form-group {
            margin-bottom: 15px;
            text-align: left;
        }
        .input-box {
            width: 100%;
            padding: 10px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        .btn {
            width: 100%;
            padding: 10px;
            font-size: 16px;
            background-color: #28a745;
            border: none;
            border-radius: 4px;
            color: white;
            cursor: pointer;
        }
        .btn:hover {
            background-color: #218838;
        }
        .form-footer {
            margin-top: 15px;
            font-size: 14px;
        }
        .error {
            color: red;
            font-size: 12px;
            display: block;
            margin-top: 3px;
        }
    </style>
    <title>Sign Up</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="center-container">
            <asp:Panel ID="pnlSignUp" runat="server" CssClass="signup-panel">
                <h2 class="panel-title">Create an Account</h2>
                
                <div class="form-group">
                    <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" Text="Username"></asp:Label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="input-box"></asp:TextBox>
                    <asp:Label ID="lblErrorUsername" runat="server" CssClass="error"></asp:Label>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email"></asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="input-box"></asp:TextBox>
                    <asp:Label ID="lblErrorEmail" runat="server" CssClass="error"></asp:Label>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password"></asp:Label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-box"></asp:TextBox>
                    <asp:Label ID="lblErrorPassword" runat="server" CssClass="error"></asp:Label>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="txtConfirmPassword" Text="Confirm Password"></asp:Label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="input-box"></asp:TextBox>
                    <asp:Label ID="lblErrorConfirmPassword" runat="server" CssClass="error"></asp:Label>
                </div>

                <div class="form-group">
                    <asp:Button ID="btnSignUp" runat="server" Text="Register" CssClass="btn" OnClick="btnSignUp_Click" />
                </div>

                <div class="form-footer">
                    Already have an account? <a href="SignIn.aspx">Sign In</a>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
