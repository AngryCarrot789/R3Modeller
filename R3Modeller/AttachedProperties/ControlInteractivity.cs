using System;
using System.Windows;
using System.Windows.Input;
using R3Modeller.Core.Utils;

namespace R3Modeller.AttachedProperties {
    /// <summary>
    /// A class containing attached property for providing more advanced interactivity between view and view model
    /// </summary>
    public static class ControlInteractivity {
        private static readonly MouseButtonEventHandler MouseUpCommand_OnMouseUp = (s, e) => MouseCommand_OnMouseEvent((UIElement) s, e, true);
        private static readonly MouseButtonEventHandler MouseUpCommand_OnMouseDown = (s, e) => MouseCommand_OnMouseEvent((UIElement) s, e, false);


        #region Dependency Properties

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof(ICommand),
                typeof(ControlInteractivity),
                new PropertyMetadata(
                    null,
                    (d, e) => {
                        if (!(d is UIElement ui))
                            throw new Exception("This property must be applied to a UIElement");
                        ui.MouseUp -= MouseUpCommand_OnMouseUp;
                        if (e.NewValue != null)
                            ui.MouseUp += MouseUpCommand_OnMouseUp;
                    }));

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseDownCommand",
                typeof(ICommand),
                typeof(ControlInteractivity),
                new PropertyMetadata(
                    null,
                    (d, e) => {
                        if (!(d is UIElement ui))
                            throw new Exception("This property must be applied to a UIElement");
                        ui.MouseDown -= MouseUpCommand_OnMouseDown;
                        if (e.NewValue != null)
                            ui.MouseDown += MouseUpCommand_OnMouseDown;
                    }));

        public static readonly DependencyProperty HandleMouseUpProperty = DependencyProperty.RegisterAttached("HandleMouseUp", typeof(bool), typeof(ControlInteractivity), new PropertyMetadata(BoolBox.False));
        public static readonly DependencyProperty HandleMouseDownProperty = DependencyProperty.RegisterAttached("HandleMouseDown", typeof(bool), typeof(ControlInteractivity), new PropertyMetadata(BoolBox.False));
        public static readonly DependencyProperty HandleNumerDraggerEditStartedProperty = DependencyProperty.RegisterAttached("HandleNumerDraggerEditStarted", typeof(bool), typeof(ControlInteractivity), new PropertyMetadata(BoolBox.False));
        public static readonly DependencyProperty HandleNumerDraggerEditCompletedProperty = DependencyProperty.RegisterAttached("HandleNumerDraggerEditCompleted", typeof(bool), typeof(ControlInteractivity), new PropertyMetadata(BoolBox.False));


        #endregion

        #region Getter and Setters

        public static void SetMouseUpCommand(UIElement element, ICommand value) => element.SetValue(MouseUpCommandProperty, value);
        public static ICommand GetMouseUpCommand(UIElement element) => (ICommand) element.GetValue(MouseUpCommandProperty);
        public static void SetHandleMouseUp(UIElement element, bool value) => element.SetValue(HandleMouseUpProperty, value.Box());
        public static bool GetHandleMouseUp(UIElement element) => (bool) element.GetValue(HandleMouseUpProperty);

        public static void SetMouseDownCommand(UIElement element, ICommand value) => element.SetValue(MouseDownCommandProperty, value);
        public static ICommand GetMouseDownCommand(UIElement element) => (ICommand) element.GetValue(MouseDownCommandProperty);
        public static void SetHandleMouseDown(UIElement element, bool value) => element.SetValue(HandleMouseDownProperty, value.Box());
        public static bool GetHandleMouseDown(UIElement element) => (bool) element.GetValue(HandleMouseDownProperty);

        #endregion

        #region Event Handlers

        private static void MouseCommand_OnMouseEvent(UIElement element, MouseButtonEventArgs e, bool isUp) {
            if (element.GetValue(isUp ? MouseUpCommandProperty : MouseDownCommandProperty) is ICommand command) {
                if (command.CanExecute(null)) {
                    command.Execute(null);
                    if ((bool) element.GetValue(isUp ? HandleMouseUpProperty : HandleMouseDownProperty)) {
                        e.Handled = true;
                    }
                }
            }
        }

        #endregion
    }
}