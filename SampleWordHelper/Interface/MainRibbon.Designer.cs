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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainRibbon));
            this.toggleEnabled = this.Factory.CreateRibbonToggleButton();
            this.toggleStructureVisibility = this.Factory.CreateRibbonToggleButton();
            this.buttonReload = this.Factory.CreateRibbonButton();
            this.separator2 = this.Factory.CreateRibbonSeparator();
            this.buttonSettings = this.Factory.CreateRibbonButton();
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group2 = this.Factory.CreateRibbonGroup();
            group1 = this.Factory.CreateRibbonGroup();
            group1.SuspendLayout();
            this.tab1.SuspendLayout();
            this.group2.SuspendLayout();
            // 
            // group1
            // 
            group1.Items.Add(this.toggleEnabled);
            group1.Label = "Управление";
            group1.Name = "group1";
            // 
            // toggleEnabled
            // 
            this.toggleEnabled.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.toggleEnabled.Image = ((System.Drawing.Image)(resources.GetObject("toggleEnabled.Image")));
            this.toggleEnabled.Label = "Включить";
            this.toggleEnabled.Name = "toggleEnabled";
            this.toggleEnabled.ScreenTip = "Управление активностью";
            this.toggleEnabled.ShowImage = true;
            this.toggleEnabled.SuperTip = "Включает надстройку по управлению базой знаний: загружает данные каталога, выполн" +
    "яет индексацию и начинает отслеживание состояния документов.";
            // 
            // toggleStructureVisibility
            // 
            this.toggleStructureVisibility.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.toggleStructureVisibility.Image = ((System.Drawing.Image)(resources.GetObject("toggleStructureVisibility.Image")));
            this.toggleStructureVisibility.Label = "Показать\\скрыть структуру";
            this.toggleStructureVisibility.Name = "toggleStructureVisibility";
            this.toggleStructureVisibility.ScreenTip = "Структура";
            this.toggleStructureVisibility.ShowImage = true;
            this.toggleStructureVisibility.SuperTip = "Показывает\\скрывает панель со структурой базы знаний.";
            // 
            // buttonReload
            // 
            this.buttonReload.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonReload.Image = ((System.Drawing.Image)(resources.GetObject("buttonReload.Image")));
            this.buttonReload.Label = "Обновить каталог";
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.ScreenTip = "Обновить каталог";
            this.buttonReload.ShowImage = true;
            this.buttonReload.SuperTip = "Полностью перечитывает данные из текущего каталога.";
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            // 
            // buttonSettings
            // 
            this.buttonSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonSettings.Image = global::SampleWordHelper.Properties.Resources.warning;
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
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "База знаний";
            this.tab1.Name = "tab1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.toggleStructureVisibility);
            this.group2.Items.Add(this.buttonReload);
            this.group2.Items.Add(this.separator2);
            this.group2.Items.Add(this.buttonSettings);
            this.group2.Label = "Основное";
            this.group2.Name = "group2";
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
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleStructureVisibility;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonReload;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleEnabled;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
    }
}
