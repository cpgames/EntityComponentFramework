using cpGames.core;
using cpGames.core.EntityComponentFramework;
using cpGames.core.EntityComponentFramework.impl;

namespace Tests;

[TestClass]
public class ComponentTests
{
    #region Nested type: BaseComponent
    private class BaseComponent : Component, IBaseComponent { }
    #endregion

    #region Nested type: IBaseComponent
    private interface IBaseComponent { }
    #endregion

    #region Nested type: TestComponent1
    private class TestComponent1 : Component
    {
        #region Fields
        public bool connected;
        #endregion

        #region Constructors
        public TestComponent1()
        {
            var addCommandsOutcome =
                ConnectedSignal.AddCommand(OnConnected) &&
                DisconnectedSignal.AddCommand(OnDisconnected);
            if (!addCommandsOutcome)
            {
                throw new Exception(addCommandsOutcome.ErrorMessage);
            }
        }
        #endregion

        #region Methods
        private Outcome OnConnected()
        {
            connected = true;
            return Outcome.Success();
        }

        private Outcome OnDisconnected()
        {
            connected = false;
            return Outcome.Success();
        }
        #endregion
    }
    #endregion

    #region Nested type: TestComponent2
    private class TestComponent2 : Component { }
    #endregion

    #region Nested type: TestComponent3
    private class TestComponent3 : Component { }
    #endregion

    #region Nested type: TestComponent4
    private class TestComponent4 : Component
    {
        #region Properties
        [RequiredComponent]
        public TestComponent3? Component3 { get; set; }
        #endregion
    }
    #endregion

    #region Nested type: TestComponent5
    private class TestComponent5 : Component
    {
        #region Properties
        [RequiredComponent(searchParent = true)]
        public IBaseComponent? BaseComponent { get; set; }
        #endregion
    }
    #endregion

    #region Methods
    [TestMethod]
    public void ComponentTest1()
    {
        var obj = new Entity(0, "obj");
        var cmp = new TestComponent1();
        var addComponentOutcome = obj.AddComponent(cmp);
        if (!addComponentOutcome)
        {
            Assert.Fail(addComponentOutcome.ErrorMessage);
        }

        Assert.IsTrue(obj.HasComponent<TestComponent1>());
        Assert.IsTrue(cmp.connected);

        // add same component again
        addComponentOutcome = obj.AddComponent(new TestComponent1());
        Assert.IsFalse(addComponentOutcome);
        var removeComponentOutcome = obj.RemoveComponent<TestComponent1>();
        if (!removeComponentOutcome)
        {
            Assert.Fail(removeComponentOutcome.ErrorMessage);
        }

        Assert.IsFalse(obj.HasComponent<TestComponent1>());
        Assert.IsFalse(cmp.connected);
        addComponentOutcome = obj.AddComponent(cmp);
        if (!addComponentOutcome)
        {
            Assert.Fail(addComponentOutcome.ErrorMessage);
        }

        Assert.IsTrue(obj.HasComponent<TestComponent1>());
        Assert.IsTrue(cmp.connected);
        removeComponentOutcome = obj.RemoveComponent<TestComponent1>();
        if (!removeComponentOutcome)
        {
            Assert.Fail(removeComponentOutcome.ErrorMessage);
        }

        Assert.IsFalse(obj.HasComponent<TestComponent1>());
        Assert.IsFalse(cmp.connected);
    }

    [TestMethod]
    public void ComponentTest2()
    {
        var obj = new Entity(0, "obj");
        Assert.IsFalse(obj.AddComponent(new TestComponent4()));
        Assert.IsFalse(obj.Components.Any());
        Assert.IsTrue(obj.AddComponent(new TestComponent3()));
        Assert.IsTrue(obj.HasComponent<TestComponent3>());
        Assert.IsTrue(obj.AddComponent(new TestComponent4()));
        Assert.IsTrue(obj.HasComponent<TestComponent4>());
        Assert.IsFalse(obj.RemoveComponent<TestComponent3>());
        Assert.IsTrue(obj.HasComponent<TestComponent3>());
        Assert.IsTrue(obj.HasComponent<TestComponent4>());
        Assert.IsTrue(obj.RemoveComponent<TestComponent4>());
        Assert.IsFalse(obj.HasComponent<TestComponent4>());
        Assert.IsTrue(obj.HasComponent<TestComponent3>());
        Assert.IsTrue(obj.RemoveComponent<TestComponent3>());
        Assert.IsFalse(obj.Components.Any());
    }

    [TestMethod]
    public void ComponentTest3()
    {
        var parent = new Entity(0, "parent");
        var child = new Entity(1, "child");
        Assert.IsTrue(child.SetParent(parent));
        Assert.IsFalse(child.AddComponent(new TestComponent5()));
        Assert.IsTrue(parent.AddComponent(new BaseComponent()));
        Assert.IsTrue(child.AddComponent(new TestComponent5()));
        Assert.IsFalse(parent.RemoveComponent<BaseComponent>());
        Assert.IsTrue(child.RemoveComponent<TestComponent5>());
        Assert.IsTrue(parent.RemoveComponent<BaseComponent>());
    }

    [TestMethod]
    public void ComponentTestGeneric1()
    {
        var obj = new Entity(0, "obj");
        Assert.IsTrue(obj.AddComponent<TestComponent1>(out var component));
        Assert.IsNotNull(component);
        Assert.IsTrue(obj.HasComponent<TestComponent1>());
        Assert.IsFalse(obj.AddComponent<TestComponent1>());
        Assert.IsTrue(obj.RemoveComponent<TestComponent1>());
        Assert.IsFalse(obj.HasComponent<TestComponent1>());
    }

    [TestMethod]
    public void ComponentTestGeneric2()
    {
        var obj = new Entity(0, "obj");
        Assert.IsTrue(obj.AddComponent<TestComponent1>());
        Assert.IsTrue(obj.HasComponent<TestComponent1>());
        Assert.IsFalse(obj.AddComponent<TestComponent1>());
        Assert.IsTrue(obj.RemoveComponent<TestComponent1>());
        Assert.IsFalse(obj.HasComponent<TestComponent1>());
    }
    #endregion
}