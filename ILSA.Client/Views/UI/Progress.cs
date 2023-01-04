namespace ILSA.Client.Views {
    using DevExpress.Utils.Html;

    public class ProgressIndicator : DxHtmlComponent {
        public static void Register() {
            DxHtmlDocument.Define<ProgressIndicator>("progress-indicator");
        }
        public int MinValue {
            get { return GetInt(nameof(MinValue), 0); }
        }
        public int MaxValue {
            get { return GetInt(nameof(MaxValue), 100); }
        }
        public int Value {
            get { return GetInt(nameof(Value), 0); }
        }
        public string ProgressColor {
            get { return GetAttribute(nameof(ProgressColor)) as string ?? "@Blue"; }
        }
        public string ProgressHeight {
            get { return GetAttribute(nameof(ProgressHeight)) as string ?? "2px"; }
        }
        protected int GetInt(string name, int defaultValue) {
            object attrValue = GetAttribute(name);
            if(attrValue is int intValue)
                return intValue;
            return int.TryParse(attrValue as string ?? string.Empty, out int value) ? value : defaultValue;
        }
        DxHtmlElement progressElement;
        public override void ConnectedCallback() {
            base.ConnectedCallback();
            progressElement = CreateProgress();
            UpdateProgress(MaxValue - MinValue, Value - MinValue, progressElement);
            AppendChild(progressElement);
        }
        public override void DisconnectedCallback() {
            base.DisconnectedCallback();
            progressElement = null;
        }
        protected virtual DxHtmlElement CreateProgress() {
            var progressElement = DxHtmlDocument.CreateElement("div");
            progressElement.Style.SetProperty("margin-top", "-" + ProgressHeight);
            progressElement.Style.SetProperty("border-top", ProgressHeight + " solid " + ProgressColor);
            return progressElement;
        }
        public override void AttributeChangedCallback(string attributeName) {
            if(progressElement == null)
                return;
            if(attributeName == "value" || attributeName == "minvalue" || attributeName == "maxvalue")
                UpdateProgress(MaxValue - MinValue, Value - MinValue, progressElement);
        }
        protected virtual void UpdateProgress(int range, int value, DxHtmlElement element) {
            if(value > MinValue && Value < MaxValue) {
                double progress = System.Math.Round(100.0 * ((double)value / (double)range));
                element.Style.SetProperty("width", progress.ToString() + "%");
            }
            else element.Style.SetProperty("width", "0");
        }
    }
}