using System;
using System.Windows;
using NewaccNet.Contracts;
using NewaccNet.Plugin.DbMigrator.Views;

namespace NewaccNet.Plugin.DbMigrator
{
    public class DbMigratorPlugin : IPlugin
    {
        public string Id => "plugin.dbmigrator";
        public string Name => "Công cụ Chuyển CSDL lên SQL Server";
        public string Version => "1.0.0";
        public string Description => "Khởi tạo CSDL MS SQL Server và nhân bản dữ liệu từ CSDL Access hiện tại.";
        public string Author => "NewaccNet Development Team";

        public void Initialize(IPluginContext context)
        {
            context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = "Hệ thống",
                GroupName = "Cơ sở dữ liệu",
                Title = "Lên SQL Server",
                ToolTip = "Khởi tạo CSDL MS SQL Server và nhân bản dữ liệu từ CSDL đang kết nối",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/actions_database.svg",
                ExecuteAction = (owner) =>
                {
                    var window = new DbMigratorWindow
                    {
                        Owner = owner
                    };
                    window.ShowDialog();
                }
            });
        }

        public void Shutdown()
        {
        }
    }
}
