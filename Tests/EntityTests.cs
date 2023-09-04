using cpGames.core;
using cpGames.core.EntityComponentFramework.impl;

namespace Tests
{
    [TestClass]
    public class EntityTests
    {
        #region Methods
        [TestMethod]
        public void ParentingTest1()
        {
            var idGenerator = new IdGenerator(4);
            var root = new Entity(0, "root") { IdGenerator = idGenerator };

            Assert.AreEqual(root.Root, root);

            var child1 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child1");
            var child2 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child2");
            var child3 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child3");

            Entity? parent = null;
            Assert.IsTrue(child1.ParentChangedSignal.AddCommand(() =>
            {
                parent = child1.Parent;
                return Outcome.Success();
            }));
            Assert.IsTrue(child1.SetParent(root));
            Assert.IsTrue(child2.SetParent(root));
            Assert.IsTrue(child3.SetParent(root));
            Assert.AreEqual(root.ChildCount, 3);
            Assert.AreEqual(parent, root);
            Assert.IsTrue(root.RemoveChild(child1.Id));
            Assert.AreEqual(root.ChildCount, 2);
            Assert.AreEqual(parent, null);

            Assert.IsFalse(root.RemoveChild(child1.Id));

            Assert.AreEqual(root.ChildCount, 2);
            Assert.AreEqual(parent, null);
        }

        [TestMethod]
        public void ParentingTest2()
        {
            var idGenerator = new IdGenerator(4);
            var root = new Entity(0, "root");
            var numChildren = 2000000;

            for (var i = 0; i < numChildren; i++)
            {
                var child = new Entity(TestHelpers.GenerateId(idGenerator, root), "clone");
                Assert.IsTrue(child.SetParent(root));
            }
            Assert.AreEqual(root.ChildCount, numChildren);
        }

        [TestMethod]
        public void ParentingTest3()
        {
            var idGenerator = new IdGenerator(4);
            var root = new Entity(0, "root");
            var numChildren = 2000000;

            for (var i = 0; i < numChildren; i++)
            {
                Assert.IsTrue(root.AddChild(TestHelpers.GenerateId(idGenerator, root), "clone", out _));
            }
            Assert.AreEqual(root.ChildCount, numChildren);
        }

        [TestMethod]
        public void ParentingTest4()
        {
            var idGenerator = new IdGenerator(4);
            var root = new Entity(0, "root");
            var child1 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child1");
            var child2 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child2");
            var child3 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child3");
            var child4 = new Entity(TestHelpers.GenerateId(idGenerator, root), "child4");

            Assert.IsTrue(child1.SetParent(root));
            Assert.IsTrue(child2.SetParent(child1));
            Assert.IsTrue(child3.SetParent(child2));
            Assert.IsTrue(child4.SetParent(child2));

            Assert.AreEqual(root.Root, root);
            Assert.AreEqual(child1.Root, root);
            Assert.AreEqual(child2.Root, root);
            Assert.AreEqual(child3.Root, root);
            Assert.AreEqual(child4.Root, root);

            Assert.AreEqual(root.ChildCount, 1);
            Assert.AreEqual(child1.ChildCount, 1);
            Assert.AreEqual(child2.ChildCount, 2);
            Assert.AreEqual(child3.ChildCount, 0);
            Assert.AreEqual(child4.ChildCount, 0);

            Assert.AreEqual(root.Parent, null);
            Assert.AreEqual(child1.Parent, root);
            Assert.AreEqual(child2.Parent, child1);
            Assert.AreEqual(child3.Parent, child2);
            Assert.AreEqual(child4.Parent, child2);

            Assert.IsTrue(root.GetChildByAddress(root.Address, out var child));
            Assert.AreEqual(child, root);

            Assert.IsTrue(root.GetChildByAddress(child1.Address, out child));
            Assert.AreEqual(child, child1);

            Assert.IsTrue(root.GetChildByAddress(child2.Address, out child));
            Assert.AreEqual(child, child2);

            Assert.IsTrue(root.GetChildByAddress(child3.Address, out child));
            Assert.AreEqual(child, child3);

            Assert.IsTrue(root.GetChildByAddress(child4.Address, out child));
            Assert.AreEqual(child, child4);

            // unparent
            Assert.IsTrue(child2.SetParent(null));
            Assert.AreEqual(child2.Parent, null);
            Assert.AreEqual(child2.Root, child2);
            Assert.AreEqual(child3.Root, child2);
            Assert.AreEqual(child4.Root, child2);
            Assert.AreEqual(child2.Address.IdCount, 1);
            Assert.AreEqual(child1.ChildCount, 0);

            Assert.IsFalse(root.GetChildByAddress(child2.Address, out _));
        }
        #endregion
    }
}