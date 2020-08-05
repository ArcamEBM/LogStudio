using LogStudio.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LogStudio.Framework
{
    /// <summary>
    /// Log report module interface
    /// </summary>
    public interface IModule : INotifyPropertyChanged
    {
        /// <summary>
        /// Priority of display order. The higher priority the higher up in the hierarchy
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// Display name of module
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// True if the module can be called with sendto 
        /// </summary>
        bool SupportsSendToCalls { get; }

        /// <summary>
        /// The control of this module
        /// </summary>
        Control ModuleControl { get; }

        /// <summary>
        /// Database containing values
        /// </summary>
        IItemDatabase Database { get; set; }

        /// <summary>
        /// True if printing is allowed
        /// </summary>
        bool CanPrint { get; }

        /// <summary>
        /// Called when print button is pressed
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        void Print(Graphics g, RectangleF rect);

        /// <summary>
        /// True if module needs to show an item tree
        /// </summary>
        bool TreeVisible { get; }

        /// <summary>
        /// Contains the currently selected item
        /// </summary>
        string SelectedItem { get; set; }

        /// <summary>
        /// Contains a list with checked items
        /// </summary>
        BindingListEx<string> CheckedItems { get; }

        /// <summary>
        /// Initialization method, must be called first
        /// </summary>
        /// <param name="manager"></param>
        void Initialize(IModuleManager manager);

        /// <summary>
        /// Loads settings from file
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="prefix">Prefix of a settings (used for graph tabs, etc...)</param>
        /// <param name="readOnly">These settings are read only</param>
        void LoadSettings(SettingsGraph settings, string prefix, bool readOnly);

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings(SettingsGraph settings);

        /// <summary>
        /// Called after settings are loaded.
        /// </summary>
        void ModuleLoaded();

        /// <summary>
        /// Clear all settings
        /// </summary>
        void Clear();

        /// <summary>
        /// Used to determine if module settings has changed and needs to be saved
        /// </summary>
        bool HasSettingsChanged { get; }

        /// <summary>
        /// Called when a module is shown
        /// </summary>
        void Shown(out ToolStripItem[] menuItems);

        /// <summary>
        /// Called when a module is hidden
        /// </summary>
        void Hidden();

        /// <summary>
        /// Ensure visibility of timestamp
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="timeStamp"></param>
        void GotoTime(string itemId, DateTime timeStamp);
    }
}
