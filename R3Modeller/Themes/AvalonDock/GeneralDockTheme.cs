using System;
using AvalonDock.Themes;

namespace R3Modeller.Themes.AvalonDock {
    /// <inheritdoc/>
    public class GeneralDockTheme : Theme {
        /// <inheritdoc/>
        public override Uri GetResourceUri() {
            return new Uri("/R3Modeller;component/Themes/AvalonDock/GeneralDockTheme.xaml", UriKind.Relative);
        }
    }
}