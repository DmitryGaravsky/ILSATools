namespace ILSA.Client {
    using System;
    using System.Windows.Forms;
    using DevExpress.XtraEditors;

    internal static class ILSAClient {
        static ILSAClient() {
            WindowsFormsSettings.SetPerMonitorDpiAware();
            WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Fluent;
            WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.Office2019Colorful);
        }
        [STAThread]
        public static void Main() {
            Assets.Style.ResourcesRoot = "ILSA.Client.Assets.";
            Assets.Style.ResourcesAssembly = typeof(ILSAClient).Assembly;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainView());
        }
    }
}