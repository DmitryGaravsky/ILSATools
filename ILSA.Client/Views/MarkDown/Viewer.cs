namespace ILSA.Client.Views {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using DevExpress.Utils.Html;
    using DevExpress.XtraEditors;

    [ToolboxItem(false)]
    public class MarkdownViewer : HtmlContentControl {
        public MarkdownViewer() {
            HtmlImages = Assets.Style.SvgImages;
        }
        string markdownCore;
        [DefaultValue(null)]
        public string Markdown {
            get { return markdownCore; }
            set {
                if(markdownCore == value) return;
                markdownCore = value;
                LoadMarkdown();
            }
        }
        [DefaultValue(null)]
        public string MarkdownImageResourcesRootNamespace {
            get;
            set;
        }
        [DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Reflection.Assembly MarkdownImageResourcesAssembly {
            get;
            set;
        }
        public override Color BackColor {
            get { return DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Window); }
            set { base.BackColor = value; }
        }
        Dictionary<string, Image> resourceImageCache = new Dictionary<string, Image>();
        const string assetsRoot = "ILSA.Core.Assets.";
        protected override object GetImageCore(string imageId, bool field) {
            if(imageId.StartsWith("~/", System.StringComparison.Ordinal)) {
                string name = imageId.Substring(2).Replace("/", ".").Replace("images", "IMGS");
                string @namespace = MarkdownImageResourcesRootNamespace ?? (assetsRoot + "MD");
                string imgKey = @namespace + "." + name + "@" + ScaleDPI.ScaleDpi.ToString();
                Image image;
                if(!resourceImageCache.TryGetValue(imgKey, out image)) {
                    var asm = MarkdownImageResourcesAssembly ?? typeof(Core.WorkloadBase).Assembly;
                    using(var resourceImage = GetResourceImage(name, @namespace, asm)) {
                        var size = GetBestImageSizeForCurrentDpi(resourceImage);
                        var scaledBmp = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using(Graphics g = Graphics.FromImage(scaledBmp)) {
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.DrawImage(resourceImage, new Rectangle(Point.Empty, size));
                        }
                        resourceImageCache.Add(imgKey, image = scaledBmp);
                    }
                }
                return image;
            }
            return base.GetImageCore(imageId, field);
        }
        Image GetResourceImage(string name, string @namespace, System.Reflection.Assembly asm) {
            string imgName = @namespace + "." + name, imgExt = System.IO.Path.GetExtension(imgName);
            Image mipMapImg = null;
            if(ScaleDPI.ScaleDpi > 150) { // >144dpi
                var imgName2X = imgName.Replace(imgExt, "@2x" + imgExt);
                mipMapImg = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(imgName2X, asm);
            }
            if(ScaleDPI.ScaleDpi > 250) { // >240dpi
                var imgName3X = imgName.Replace(imgExt, "@3x" + imgExt);
                mipMapImg = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(imgName3X, asm);
            }
            return mipMapImg ?? DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(imgName, asm);
        }
        Size GetBestImageSizeForCurrentDpi(Image resourceImage) {
            int imgDPI = Round(resourceImage.HorizontalResolution), currentDpi = ScaleDPI.ScaleDpi;
            if(imgDPI >= currentDpi) {
                if(currentDpi < 100 && imgDPI == 192)
                    return new Size(resourceImage.Width / 2, resourceImage.Height / 2);
                if(currentDpi < 150 && imgDPI == 192)
                    return new Size(resourceImage.Width * 2 / 3, resourceImage.Height * 2 / 3);
            }
            if(imgDPI == 96)
                return ScaleDPI.ScaleSize(resourceImage.Size);
            return resourceImage.Size;
        }
        int Round(float value) {
            return (int)(value + 0.5f);
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new HtmlTemplate HtmlTemplate {
            get { return base.HtmlTemplate; }
        }
        Markdown.Result markdownParserResult;
        void LoadMarkdown() {
            if(!string.IsNullOrEmpty(Markdown)) {
                markdownParserResult = Views.Markdown.ToHtml(markdownCore);
                base.HtmlTemplate.Set(markdownParserResult.ToString(), MarkdownStyle.Instance.Css);
            }
            else base.HtmlTemplate.Set(string.Empty, string.Empty);
            ResetScroll();
            Refresh();
        }
        void ResetScroll() {
            var scrollInfo = ((IHtmlContentControl)this).ScrollInfo;
            if(scrollInfo != null && scrollInfo.VScrollBar != null) {
                scrollInfo.VScrollBar.Value = 0;
            }
        }
        public void PerformClick(DxHtmlElementMouseEventArgs args) {
            string id = args.ElementId ?? string.Empty;
            if(id.StartsWith("block-img:", StringComparison.OrdinalIgnoreCase)) {
                int elementId;
                if(int.TryParse(id.Substring("block-img:".Length), out elementId)) 
                    markdownParserResult.PerformElementClick(elementId);
            }
        }
        sealed class MarkdownStyle : Assets.Style {
            public static readonly Assets.Style Instance = new MarkdownStyle();
        }
    }
}