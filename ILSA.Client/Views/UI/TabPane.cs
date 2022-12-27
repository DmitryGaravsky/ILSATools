namespace ILSA.Client.Views {
    using DevExpress.Utils.Html;
    using DevExpress.Utils.Html.Internal;

    public sealed class TabPane : DxHtmlComponent {
        public static void Register() {
            DxHtmlDocument.Define<TabPaneButton>("tab-pane-button");
            DxHtmlDocument.Define<TabPane>("tab-pane");
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
                var button = item.GetAttribute("self") as TabPaneButton;
                button.UpdateState(selectedButtonId == item.Id);
            }
        }
        public sealed class TabPaneButton : DxHtmlComponent {
            public string Icon {
                get { return GetAttribute(nameof(Icon)) as string ?? string.Empty; }
            }
            public string Caption {
                get { return GetAttribute(nameof(Caption)) as string ?? string.Empty; }
            }
            bool IsSelected {
                get { return Equals(GetAttribute("selected"), true); }
            }
            ElementInternals _internals;
            public override void ConnectedCallback() {
                base.ConnectedCallback();
                var tabPane = Element.ParentElement.GetAttribute("self") as TabPane;
                SetAttribute("tab-pane", tabPane);
                SetAttribute("self", this);
                EnsureInitialState(tabPane);
                _internals = AttachInternals();
                EnsureButtonContent(tabPane.ItemClass, tabPane.ItemTemplate);
                UpdateButtonState(IsSelected);
                AddEventListener("click", OnItemClick);
            }
            public override void DisconnectedCallback() {
                RemoveEventListener("click", OnItemClick);
                DetachInternals();
                _internals = null;
                base.DisconnectedCallback();
            }
            void EnsureButtonContent(string className, string templateId) {
                Element.ClassName = string.IsNullOrEmpty(Element.ClassName) ? className : Element.ClassName + " " + className;
                if(!string.IsNullOrEmpty(templateId)) {
                    var template = Element.RootElement.FindTemplate(templateId);
                    if(template != null)
                        AppendChild(template.CloneNode(true));
                    var iconElement = Element.FindElementById("icon");
                    if(iconElement != null)
                        iconElement.SetAttribute("src", Icon);
                    var captionElement = Element.FindElementById("caption");
                    if(captionElement != null)
                        captionElement.SetInnerText(Caption);    
                }
            }
            void EnsureInitialState(TabPane tabPane) {
                if(tabPane.selectedButtonId == null) {
                    tabPane.selectedButtonId = Element.Id;
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
                var tabPane = GetAttribute("tab-pane") as TabPane;
                if(tabPane != null) tabPane.Toggle(Element.Id);
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