using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Properties;

namespace SampleWordHelper.Interface
{
    public class CommandView : BasicDisposable, ICommandView
    {
        readonly ContextMenuStrip strip;
        readonly List<Tuple<ToolStripItem, Func<bool>>> menuItems;

        public object RawObject { get { return strip; } }

        public CommandView()
        {
            strip = new ContextMenuStrip();
            strip.Opening += OnOpening;
            strip.ShowItemToolTips = true;
            strip.ShowCheckMargin = false;
            strip.ShowImageMargin = false;
            menuItems = new List<Tuple<ToolStripItem,Func<bool>>>();
        }

        /// <summary>
        /// Вызывается при открытии меню.
        /// </summary>
        void OnOpening(object sender, CancelEventArgs e)
        {
            strip.Items.Clear();
            e.Cancel = true;
            foreach (var item in menuItems.Where(entry => entry.Item2()))
            {
                strip.Items.Add(item.Item1);
                e.Cancel = false;
            }
        }

        public void Add(string text, string toolTip, string imageKey, Func<bool> isEnabled, Action execute)
        {
            var item = new ToolStripButton(text, (Image) Resources.ResourceManager.GetObject(imageKey));
            item.ToolTipText = toolTip;
            item.Click += (sender, args) => execute();
            menuItems.Add(Tuple.Create((ToolStripItem) item, isEnabled));
        }

        protected override void DisposeManaged()
        {
            strip.SafeDispose();
        }
    }
}