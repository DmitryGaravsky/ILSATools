namespace ILSA.Client.Views {
    using DevExpress.Utils;

    static class CoreSvgImages {
        static SvgImageCollection svgImages;
        public static SvgImageCollection SvgImages {
            get { return svgImages ?? (svgImages = SvgImageCollection.FromResources("ILSA.Core.Assets.Svg", typeof(ILSA.Core.Node).Assembly)); }
        }
    }
}