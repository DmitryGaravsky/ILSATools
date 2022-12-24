namespace ILSA.Core.Loader {
    using System.Reflection;

    public interface IAssembliesSource {
        void Load(string path);
        Assembly[] Assemblies { get; }
        void Reset();
    }
}