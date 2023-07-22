using System.ComponentModel;
using System.Windows.Markup;

namespace R3Modeller.CrossPlatform.AdvancedContextService.Static {
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class StaticShortcutMenuItem : StaticBaseMenuItem {
        public string ShortcutId { get; set; }
    }
}