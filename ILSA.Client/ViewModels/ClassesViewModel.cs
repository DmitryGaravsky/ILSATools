namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ILReader.Readers;
    using ILSA.Core.Hierarchy;

    public class ClassesViewModel {
        public ClassesViewModel() {
            ShowOffset = true;
        }
        public virtual IReadOnlyCollection<Node> Nodes {
            get;
            protected set;
        }
        public virtual Node SelectedNode {
            get;
            set;
        }
        public virtual IEnumerable<IInstruction> SelectedMethod {
            get;
            protected set;
        }
        public virtual bool ShowOffset { 
            get; 
            protected set; 
        }
        public virtual bool ShowBytes { 
            get; 
            protected set; 
        }
        readonly static IInstruction[] Empty = new IInstruction[0];
        protected void OnSelectedNodeChanged() {
            var method = NodesFactory.GetMethod(SelectedNode);
            if(method == null)
                SelectedMethod = Empty;
            else {
                var cfg = ILReader.Configuration.Resolve(method);
                SelectedMethod = cfg.GetReader(method);
            }
        }
        INodesFactory factory;
        public void OnLoad() {
            factory = factory ?? new NodesFactory();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Nodes = assemblies
                .OrderBy(x => x.GetName().Name)
                .Select(factory.Create).ToArray();
        }
    }
}