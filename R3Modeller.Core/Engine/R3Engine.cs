using System;
using System.Threading;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine {
    public class R3Engine {
        private static R3Engine instance;
        public static R3Engine Instance => instance;

        private readonly Thread engineThread;
        private readonly Thread renderThread;
        private volatile bool isRunning;

        public bool IsRunning => this.isRunning;

        public R3Engine() {
            this.engineThread = Thread.CurrentThread;
            this.renderThread = new Thread(this.RenderMain);
        }

        /// <summary>
        /// Runs the engine in a new thread, setting instance once it has been created
        /// </summary>
        public static void RunEngine() {
            Thread thread = new Thread(() => {
                instance = new R3Engine();
                instance.Run();
            });

            thread.Start();
        }

        private void Run() {
            try {
                this.OnPreInitialise();
            }
            catch (Exception e) {
                throw this.OnLoadFailure("Failed to preinit engine", e);
            }

            try {
                this.OnInitialise();
            }
            catch (Exception e) {
                throw this.OnLoadFailure("Failed to initialise engine", e);
            }

            try {
                this.OnPostInitialise();
            }
            catch (Exception e) {
                throw this.OnLoadFailure("Failed to initialise engine", e);
            }

            Exception exception = null;
            try {
                this.isRunning = true;
                do {
                    this.OnTick();
                } while (this.isRunning);
            }
            catch (Exception e) {
                exception = e;
            }

            try {
                this.OnExit();
            }
            catch (Exception e) {
                if (exception == null) {
                    exception = e;
                }
                else {
                    exception.AddSuppressed(e);
                }
            }

            if (exception != null) {
                throw exception;
            }
        }

        #region Game Engine Thread



        #endregion

        private void OnPreInitialise() {
            // load plugins maybe
        }

        private void OnInitialise() {
            // load assets
        }

        private void OnPostInitialise() {
            // setup render thread
        }

        private void OnTick() {

        }

        private Exception OnLoadFailure(string message, Exception e) {
            try {
                this.OnExit();
            }
            catch (Exception ex) {
                e.AddSuppressed(ex);
            }

            return new Exception(message, e);
        }

        private void OnExit() {

        }

        #region Render Thread

        private void RenderMain() {

        }

        #endregion
    }
}