<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCategoryOrProfile.aspx.cs" Inherits="DejaVu.SelfHealthCheck.MonitorNew.pages.AddCategoryOrProfile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Self Health Check | Add Category Or Profile</title>
    <!-- Tell the browser to be responsive to screen width -->
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport">
    <!-- Bootstrap 3.3.5 -->
    <link rel="stylesheet" href="../../bootstrap/css/bootstrap.min.css">
    <!-- Font Awesome -->
    <link href="../bower_components/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css">
    <link rel="stylesheet" href="../../dist/css/AdminLTE.min.css">
    <link rel="stylesheet" href="../../plugins/iCheck/all.css">

    <!-- Select2 -->
    <link rel="stylesheet" href="../../plugins/select2/select2.min.css">

    <link rel="stylesheet" href="../../dist/css/skins/_all-skins.min.css">
    <link rel="stylesheet" href="../../bower_components/formvalidation-master/dist/css/formValidation.min.css">
     <!-- MY CSS-->
    <link rel="stylesheet" href="../../bower_components/style.css" />

</head>
<body class="hold-transition skin-blue sidebar-mini">

    <div class="wrapper">

        <header class="main-header">
            <!-- Logo -->
            <a href="/index.html" class="logo">
                <!-- mini logo for sidebar mini 50x50 pixels -->
                <span class="logo-mini"><b>H</b>CM</span>
                <!-- logo for regular state and mobile devices -->
                <span class="logo-lg">Health Check Monitor</span>
            </a>
            <!-- Header Navbar: style can be found in header.less -->
            <nav class="navbar navbar-static-top" role="navigation">
                <!-- Sidebar toggle button-->
                <a href="#" class="sidebar-toggle" data-toggle="offcanvas" role="button">
                    <span class="sr-only">Toggle navigation</span>
                </a>
                <div class="navbar-custom-menu">
                    <ul class="nav navbar-nav">
                        <!-- Control Sidebar Toggle Button -->
                        <li>
                            <a href="#" data-toggle="control-sidebar"><i class="fa fa-gears"></i></a>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
        <!-- Left side column. contains the logo and sidebar -->
        <aside class="main-sidebar">
            <!-- sidebar: style can be found in sidebar.less -->
            <section class="sidebar">
                <!-- sidebar menu: : style can be found in sidebar.less -->
                <ul class="sidebar-menu">
                    <li><a href="/index.html"><i class="fa fa-dashboard"></i><span>DashBoard</span></a></li>
                    <li class="treeview">
                        <a href="#">
                            <i class="fa fa-folder"></i><span>Applications</span>
                            <i class="fa fa-angle-left pull-right"></i>
                        </a>
                        <ul class="treeview-menu">
                            <li><a href="AddApplication.aspx"><i class="fa fa-plus-circle"></i>Add Application</a></li>
                            <li><a href="view-applications.html"><i class="fa fa-th-list"></i>View Applications</a></li>
                        </ul>
                    </li>
                    <li class="treeview">
                        <a href="#">
                            <i class="fa fa-folder"></i><span>Category/Profile</span>
                            <i class="fa fa-angle-left pull-right"></i>
                        </a>
                        <ul class="treeview-menu">
                            <li><a href="AddCategoryOrProfile.aspx"><i class="fa fa-plus-circle"></i>Add Category/Profile</a></li>
                            <li><a href="ViewCategoriesOrProfiles.html"><i class="fa fa-th-list"></i>View Category/Profile</a></li>
                        </ul>
                    </li>

                </ul>
            </section>
            <!-- /.sidebar -->
        </aside>
        <div class="content-wrapper">
            <!-- Content Header (Page header) -->
            <section class="content-header">
                <h1>Add Category/Profile Application
                </h1>
            </section>

            <!-- Main content -->
            <section class="content">

                <!-- SELECT2 EXAMPLE -->
                <div class="box box-default">
                    <div class="box-header with-border">
                        <h3 class="box-title">Set Up a Profile </h3>

                    </div>
                    <!-- /.box-header -->
                    <div class="box-body">
                        <div class="row">
                            <form id="mainForm" name="mainForm" role="form" runat="server">
                                <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" EnablePartialRendering="true" runat="server" />

                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Profile Name</label>
                                        <asp:TextBox ID="txtProfileName" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>

                                    <!-- /.form-group -->
                                </div>
                                <!-- /.col -->
                                <div class="col-md-6">
                                    <div class="form-group">

                                        <asp:HiddenField ID="hdnCategories" runat="server" />
                                        <label>Set Up Dependent Categories</label>

                                        <select id="txtCategoryName" class="form-control" runat="server" multiple="true"></select>
                                    </div>
                                </div>
                                <asp:Button ID="SaveButton"  runat="server" type="submit" class="btn btn-default" Text="Save" OnClientClick="sendCategories()" OnClick="SaveButton_Click1"/>

                            </form>
                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->

            </section>
            <!-- /.content -->
        </div>
        <!-- /.content-wrapper -->
        <footer class="main-footer">
            <div class="pull-right hidden-xs">
                <b>Version</b> 2.3.2
   
            </div>

            <strong>Copyright &copy; <% DateTime.Now.Year.ToString();%> <a href="#">AppZone</a>.</strong> All rights
    reserved.
        </footer>
        <div>
            <div class="control-sidebar-bg"></div>
        </div>
    </div>
    <!-- jQuery 2.2.0 -->
    <script src="../../plugins/jQuery/jQuery-2.2.0.min.js"></script>
    <!-- Bootstrap 3.3.5 -->
    <script src="../../bootstrap/js/bootstrap.min.js"></script>
    <!-- Alertify 0.3.11 -->
    <script src="../../bower_components/formvalidation-master/dist/js/formValidation.min.js"></script>
    <script src="../../bower_components/formvalidation-master/dist/js/framework/bootstrap.min.js"></script>
    <!-- Select2 -->
    <script src="../../plugins/select2/select2.full.min.js"></script>
    <!-- AdminLTE App -->
    <script src="../../dist/js/app.min.js"></script>
    <!-- AdminLTE for demo purposes -->
    <script src="../../dist/js/demo.js"></script>
    <script type="text/javascript">
        function sendCategories() {
            var categories = $("#txtCategoryName").val();
            console.log(categories);
            document.getElementById('hdnCategories').value = categories;
            $(".select2-selection__rendered li").each(function (data) {
                console.log(data);
                debugger;
            })
        }
        $(document).ready(function () {
            $("#txtCategoryName").select2({
                placeholder: 'Add Categories here and seperate each category with commas',
                dropdownCss: { display: 'none' },
                tags: [''],
                tokenSeparators: ','
            });

            
           
        });
    </script>
</body>
</html>
