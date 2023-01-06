namespace ILSA.Client.Views {
    using DevExpress.Mvvm.Native;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.Html.Internal;

    public sealed class ChipGroup : DxHtmlComponent {
        public static void Register() {
            DxHtmlDocument.Define<ChipGroupButton>("chip-group-button");
            DxHtmlDocument.Define<ChipGroup>("chip-group");
        }
        public string ItemClass {
            get { return GetAttribute(nameof(ItemClass)) as string ?? string.Empty; }
        }
        public string ItemTemplate {
            get { return GetAttribute(nameof(ItemTemplate)) as string ?? string.Empty; }
        }
        public override void ConnectedCallback() {
            base.ConnectedCallback();
            SetAttribute("self", this);
        }
        string selectedButtonId;
        void Toggle(string id) {
            if(selectedButtonId == id) return;
            selectedButtonId = id;
            UpdateItemStates();
        }
        void UpdateItemStates() {
            foreach(var item in Element.Children) {
                var button = item.GetAttribute("self") as ChipGroupButton;
                button.UpdateState(selectedButtonId == item.Id);
            }
        }
        public sealed class ChipGroupButton : DxHtmlComponent {
            public string Caption {
                get { return GetAttribute(nameof(Caption)) as string ?? string.Empty; }
            }
            bool IsSelected {
                get { return Equals(GetAttribute("selected"), true); }
            }
            ElementInternals _internals;
            public override void ConnectedCallback() {
                base.ConnectedCallback();
                var group = Element.ParentElement.GetAttribute("self") as ChipGroup;
                SetAttribute("chip-group", group);
                SetAttribute("self", this);
                EnsureInitialState(group);
                _internals = AttachInternals();
                int index = group.Element.Children.IndexOf(x => x == Element);
                EnsureButtonContent(group.ItemClass, group.ItemTemplate, index, 2);
                UpdateButtonState(IsSelected);
                AddEventListener("click", OnItemClick);
            }
            public override void DisconnectedCallback() {
                RemoveEventListener("click", OnItemClick);
                DetachInternals();
                _internals = null;
                base.DisconnectedCallback();
            }
            void EnsureButtonContent(string className, string templateId, int index, int count) {
                string actualClassName = string.IsNullOrEmpty(Element.ClassName) ?
                    className : Element.ClassName + " " + className;
                if(index == 0 && count > 1)
                    actualClassName += " first";
                if(index == count - 1 && count > 1)
                    actualClassName += " last";
                Element.ClassName = actualClassName;
                if(!string.IsNullOrEmpty(templateId)) {
                    var template = Element.RootElement.FindTemplate(templateId);
                    if(template != null)
                        AppendChild(template.CloneNode(true));
                    var captionElement = Element.FindElementById("caption");
                    if(captionElement != null)
                        captionElement.SetInnerText(Caption);
                }
            }
            void EnsureInitialState(ChipGroup chipGroup) {
                if(chipGroup.selectedButtonId == null) {
                    chipGroup.selectedButtonId = Element.Id;
                    SetAttribute("selected", true);
                }
            }
            public void UpdateState(bool isSelected) {
                if(IsSelected == isSelected)
                    return;
                SetAttribute("selected", isSelected);
                UpdateButtonState(isSelected);
            }
            void OnItemClick(DxHtmlElementEventArgs args) {
                var group = GetAttribute("chip-group") as ChipGroup;
                if(group != null) group.Toggle(Element.Id);
            }
            void UpdateButtonState(bool isChecked) {
                if(_internals == null)
                    return;
                if(isChecked)
                    _internals.States.Add("--selected");
                else
                    _internals.States.Delete("--selected");
            }
        }
    }
}