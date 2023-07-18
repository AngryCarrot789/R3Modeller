using System.ComponentModel;
using System.Windows.Markup;

namespace R3Modeller.AdvancedContextService.Static {
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class StaticShortcutMenuItem : StaticBaseMenuItem {
        public string ShortcutId { get; set; }
    }
}