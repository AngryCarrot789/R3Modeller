using System;
using R3Modeller.Core.Exceptions;
using R3Modeller.Core.Utils;
using R3Modeller.Core.Views.Windows;

namespace R3Modeller.Views.Exceptions {
    public class ExceptionViewerService {
        // singleton to soon support IoC
        public static ExceptionViewerService Instance { get; } = new ExceptionViewerService();

        public IWindow ShowExceptionStack(ExceptionStack stack) {
            ExceptionViewerWindow window = new ExceptionViewerWindow();
            ExceptionStackViewModel vm = new ExceptionStackViewModel(stack);
            window.DataContext = vm;
            window.Show();
            return window;
        }

        public IWindow ShowException(Exception exception) {
            ExceptionViewerWindow window = new ExceptionViewerWindow();
            using (ExceptionStack stack = new ExceptionStack(null, false)) {
                stack.Add(exception);

                ExceptionStackViewModel vm = new ExceptionStackViewModel(stack);
                window.DataContext = vm;
                window.Show();
                return window;
            }
        }
    }
}