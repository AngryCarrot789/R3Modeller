using R3Modeller.Core.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Views.Dialogs.UserInputs;

namespace R3Modeller.Core.Engine.Objs.Actions {
    public class RenameObjectAction : AnAction {
        public RenameObjectAction() {

        }

        public override async Task<bool> ExecuteAsync(AnActionEventArgs e) {
            if (e.DataContext.TryGetContext(out SceneObjectViewModel obj)) {
                obj.DisplayName = await IoC.UserInput.ShowSingleInputDialogAsync("Rename object", "Input a new object name:", obj.DisplayName) ?? "";
                return true;
            }

            return false;
        }
    }
}