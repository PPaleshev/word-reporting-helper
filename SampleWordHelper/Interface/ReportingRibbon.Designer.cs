namespace SampleWordHelper.Interface
{
    partial class ReportingRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ReportingRibbon()
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
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.toggleStructureVisibility = this.Factory.CreateRibbonToggleButton();
            this.buttonSettings = this.Factory.CreateRibbonButton();
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
            ribbonDialogLauncherImpl1.ScreenTip = "Настройки";
            ribbonDialogLauncherImpl1.SuperTip = "Параметры системы управления документами. Позволяет задавать различные параметры " +
    "источников данных.";
            this.group1.DialogLauncher = ribbonDialogLauncherImpl1;
            this.group1.Items.Add(this.buttonSettings);
            this.group1.Items.Add(this.toggleStructureVisibility);
            this.group1.Label = "Управление";
            this.group1.Name = "group1";
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
            // buttonSettings
            // 
            this.buttonSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonSettings.Image = global::SampleWordHelper.Properties.Resources.warning_triangle;
            this.buttonSettings.Label = "Необходима настройка";
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.ScreenTip = "Надстройка не может быть использована, так как не заданы некоторые параметры. Что" +
    "бы выполнить настройку, нажмите на это сообщение.";
            this.buttonSettings.ShowImage = true;
            this.buttonSettings.SuperTip = "Проблемы";
            // 
            // ReportingRibbon
            // 
            this.Name = "ReportingRibbon";
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
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSettings;
    }
}
