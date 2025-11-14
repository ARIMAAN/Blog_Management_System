<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewBlog.aspx.cs" Inherits="HomePage.PreviewBlog" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blog Preview</title>
    <style>
        body { font-family: Arial, sans-serif; margin:0; padding:0; background: #f5f5f5; }
        .preview-container { width: 80vw; max-width: 900px; margin: 50px auto; background:#fff; border-radius:8px; padding:20px; position:relative; box-shadow:0 0 20px rgba(0,0,0,0.2);}
        .close-btn { position:absolute; top:10px; right:10px; font-size:24px; cursor:pointer; color:red; border:none; background:none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preview-container">
            <asp:Button ID="btnClose" runat="server" Text="X" CssClass="close-btn" OnClick="btnClose_Click" />
            <asp:Literal ID="litBlogContent" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
