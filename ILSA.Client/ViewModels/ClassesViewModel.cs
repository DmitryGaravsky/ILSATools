namespace ILSA.Client.ViewModels {
    using System.Collections.Generic;
    using ILSA.Core.Hierarchy;

    public class ClassesViewModel {
        public virtual IReadOnlyCollection<Node> Nodes { 
            get; 
            protected set; 
        }
        public void OnLoad() {
            INodesFactory factory = new NodesFactory();
            Nodes = new Node[] {
                factory.Create(typeof(Node).Assembly),
                factory.Create(typeof(MainView).Assembly),
            };
        }
    }
}
