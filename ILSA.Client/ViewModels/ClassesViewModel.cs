namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ILSA.Core.Hierarchy;

    public class ClassesViewModel {
        public virtual IReadOnlyCollection<Node> Nodes {
            get;
            protected set;
        }
        public void OnLoad() {
            INodesFactory factory = new NodesFactory();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Nodes = assemblies
                .OrderBy(x => x.GetName().Name)
                .Select(factory.Create).ToArray();
        }
    }
}