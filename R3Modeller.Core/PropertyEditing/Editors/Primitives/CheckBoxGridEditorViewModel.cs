using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace R3Modeller.Core.PropertyEditing.Editors.Primitives {
    public class CheckBoxGridEditorViewModel : BasePropertyEditorViewModel, IEnumerable<CheckBoxEditorViewModel> {
        public ObservableCollection<CheckBoxEditorViewModel> Editors { get; }

        public CheckBoxGridEditorViewModel(Type applicableType) : base(applicableType) {
            this.Editors = new ObservableCollection<CheckBoxEditorViewModel>();
        }

        public void Add(CheckBoxEditorViewModel editor) {
            this.Editors.Add(editor);
        }

        public IEnumerator<CheckBoxEditorViewModel> GetEnumerator() => this.Editors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Editors.GetEnumerator();
    }
}