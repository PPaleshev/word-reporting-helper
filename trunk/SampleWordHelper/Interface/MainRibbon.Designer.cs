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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.buttonSettings = this.Factory.CreateRibbonButton();
            this.buttonShowErrors = this.Factory.CreateRibbonButton();
            this.toggleStructureVisibility = this.Factory.CreateRibbonToggleButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "Отчётность";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.toggleStructureVisibility);
            this.group1.Items.Add(this.separator1);
            this.group1.Items.Add(this.buttonSettings);
            this.group1.Items.Add(this.buttonShowErrors);
            this.group1.Label = "Управление";
            this.group1.Name = "group1";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // buttonSettings
            // 
            this.buttonSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonSettings.Image = global::SampleWordHelper.Properties.Resources.sprocket_light;
            this.buttonSettings.Label = "Настройки";
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.ShowImage = true;
            // 
            // buttonShowErrors
            // 
            this.buttonShowErrors.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonShowErrors.Image = global::SampleWordHelper.Properties.Resources.warning_triangle;
            this.buttonShowErrors.Label = "Ошибки приложения";
            this.buttonShowErrors.Name = "buttonShowErrors";
            this.buttonShowErrors.ScreenTip = "Ошибки";
            this.buttonShowErrors.ShowImage = true;
            this.buttonShowErrors.SuperTip = "Ошибки, возникшие при загрузке приложения.";
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
            // MainRibbon
            // 
            this.Name = "MainRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleStructureVisibility;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonShowErrors;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
    }
}
