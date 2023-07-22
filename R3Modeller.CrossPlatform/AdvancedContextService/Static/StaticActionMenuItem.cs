using System.ComponentModel;
using System.Windows.Markup;

namespace R3Modeller.CrossPlatform.AdvancedContextService.Static {
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class StaticActionMenuItem : StaticBaseMenuItem {
        public string ActionID { get; set; }
    }
}