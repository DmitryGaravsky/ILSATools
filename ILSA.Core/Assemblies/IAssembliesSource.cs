namespace ILSA.Core.Sources {
    using System.Reflection;

    public interface IAssembliesSource {
        void Load(string path);
        Assembly[] Assemblies { get; }
        void Reset();
    }
}