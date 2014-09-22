namespace SampleWordHelper.Interface
{
    partial class MainRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MainRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
            this.toggleStructureVisibility = this.Factory.CreateRibbonToggleButton();
            this.separator = this.Factory.CreateRibbonSeparator();
            this.buttonSettings = this.Factory.CreateRibbonButton();
            this.tab1 = this.Factory.CreateRibbonTab();
            group1 = this.Factory.CreateRibbonGroup();
            group1.SuspendLayout();
            this.tab1.SuspendLayout();
            // 
            // group1
            // 
            group1.Items.Add(this.toggleStructureVisibility);
            group1.Items.Add(this.separator);
            group1.Items.Add(this.buttonSettings);
            group1.Label = "Управление";
            group1.Name = "group1";
            // 
            // toggleStructureVisibility
            // 
            this.toggleStructureVisibility.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.toggleStructureVisibility.Image = global::SampleWordHelper.Properties.Resources.view_outline_detail;
            this.toggleStructureVisibility.Label = "Показать\\скрыть структуру";
            this.toggleStructureVisibility.Name = "toggleStructureVisibility";
            this.toggleStructureVisibility.ScreenTip = "Структура";
            this.toggleStructureVisibility.ShowImage = true;
            this.toggleStructureVisibility.SuperTip = "Показывает\\скрывает панель со структурой базы знаний.";
            // 
            // separator
            // 
            this.separator.Name = "separator";
            // 
            // buttonSettings
            // 
            this.buttonSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonSettings.Image = global::SampleWordHelper.Properties.Resources.sprocket_light;
            this.buttonSettings.Label = "Настройки";
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.ScreenTip = "Основные настройки";
            this.buttonSettings.ShowImage = true;
            this.buttonSettings.SuperTip = "Позволяет установить требуемые параметры работы надстройки для повышения удобства" +
    " работы.";
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(group1);
            this.tab1.Label = "Отчётность";
            this.tab1.Name = "tab1";
            // 
            // MainRibbon
            // 
            this.Name = "MainRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            group1.ResumeLayout(false);
            group1.PerformLayout();
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleStructureVisibility;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator;
    }
}
