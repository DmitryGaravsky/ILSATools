namespace ILSA.Core.Tests.MethodsTree {
    using ILSA.Core.Hierarchy;
    using NUnit.Framework;

    [TestFixture]
    public class Nodes {
        readonly static INodesFactory factory = new NodesFactory();
        //
        class A {
            public override string ToString() {
                return "A";
            }
        }
        class B : A {
            public override string ToString() {
                return "B";
            }
            public int GetValue() {
                return 42;
            }
        }
        [Test]
        public void Test00_TypeWithoutBaseType() {
            var a = factory.Create(typeof(A));
            Assert.AreEqual(typeof(A).Name, a.Name);
            Assert.AreEqual(typeof(A).Namespace, a.Group);
            Assert.AreEqual(2, a.Nodes.Count);
            var c = ((Node[])a.Nodes)[0];
            Assert.AreEqual(".ctor()", c.Name);
            Assert.AreEqual(string.Empty, c.Group);
            Assert.AreEqual(0, c.Nodes.Count);
            var m = ((Node[])a.Nodes)[1];
            Assert.AreEqual("ToString() : string", m.Name);
            Assert.AreEqual(string.Empty, m.Group);
            Assert.AreEqual(0, m.Nodes.Count);
        }
        [Test]
        public void Test00_TypeWithBaseType() {
            var b = factory.Create(typeof(B));
            Assert.AreEqual(typeof(B).Name, b.Name);
            Assert.AreEqual(typeof(B).Namespace, b.Group);
            Assert.AreEqual(4, b.Nodes.Count);
            var c = ((Node[])b.Nodes)[1];
            Assert.AreEqual(".ctor()", c.Name);
            Assert.AreEqual(string.Empty, c.Group);
            Assert.AreEqual(0, c.Nodes.Count);
            var mToString = ((Node[])b.Nodes)[2];
            Assert.AreEqual("ToString() : string", mToString.Name);
            Assert.AreEqual(string.Empty, mToString.Group);
            Assert.AreEqual(0, mToString.Nodes.Count);
            var mGetValue = ((Node[])b.Nodes)[3];
            Assert.AreEqual("GetValue() : int", mGetValue.Name);
            Assert.AreEqual(string.Empty, mGetValue.Group);
            Assert.AreEqual(0, mGetValue.Nodes.Count);
            var baseTypesNode = ((Node[])b.Nodes)[0];
            Assert.AreEqual("Base Types", baseTypesNode.Name);
            Assert.AreEqual(string.Empty, baseTypesNode.Group);
            Assert.AreEqual(1, baseTypesNode.Nodes.Count);
            var a = ((Node[])baseTypesNode.Nodes)[0];
            Assert.AreEqual(typeof(A).Name, a.Name);
            Assert.AreEqual(string.Empty, a.Group);
            Assert.AreEqual(0, a.Nodes.Count);
        }
    }
}