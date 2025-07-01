using cpGames.core;
using cpGames.core.EntityComponentFramework;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class ReferenceTests
{
    #region Nested type: Component1
    private class Component1 : Component
    {
        #region Properties
        [cpGames.core.EntityComponentFramework.Property("TestFloat", typeof(FloatProperty))]
        public IFloatProperty TestFloat { get; private set; } = null!;
        #endregion
    }
    #endregion

    #region Nested type: Component2
    private class Component2 : Component
    {
        #region Properties
        [cpGames.core.EntityComponentFramework.Property("TestReference", typeof(ReferenceProperty<Component1>))]
        public IReferenceProperty<Component1> TestReference { get; private set; } = null!;
        #endregion
    }
    #endregion

    #region Nested type: Component3
    private class Component3 : Component
    {
        #region Properties
        [cpGames.core.EntityComponentFramework.Property("TestList", typeof(ListProperty<Component1>))]
        public IListProperty<Component1> TestList { get; private set; } = null!;
        #endregion
    }
    #endregion

    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestReferenceDisconnect()
    {
        var entity1 = new Entity(Id.INVALID);
        var entity2 = new Entity(Id.INVALID);

        var outcome =
            entity1.AddComponent<Component1>(out var component1) &&
            component1!.TestFloat.Set(1.0f);
        Assert.That(outcome);

        outcome =
            entity2.AddComponent<Component2>(out var component2) &&
            component2!.TestReference.Set(component1);

        Assert.That(outcome);

        outcome = component2!.TestReference.IsDefault(out var result);
        Assert.That(outcome);
        Assert.That(!result);

        outcome = entity1.Dispose();
        Assert.That(outcome);

        outcome = component2.TestReference.IsDefault(out result);
        Assert.That(outcome);
        Assert.That(result);
    }

    [Test]
    public void TestListDisconnect()
    {
        var entity1 = new Entity(Id.INVALID);
        var entity2 = new Entity(Id.INVALID);

        var outcome =
            entity1.AddComponent<Component1>(out var component1) &&
            component1!.TestFloat.Set(1.0f);
        Assert.That(outcome);

        outcome =
            entity2.AddComponent<Component3>(out var component3) &&
            component3!.TestList.AddEntry(component1!);

        Assert.That(outcome);

        var count = component3!.TestList.Count;
        Assert.That(count == 1);

        outcome = entity1.Dispose() && entity2.Dispose();
        Assert.That(outcome);

        count = component3.TestList.Count;
        Assert.That(count == 0);
    }

    [Test]
    public void TestSet()
    {
        var entity1 = new Entity(Id.INVALID);
        var entity2 = new Entity(Id.INVALID);

        var outcome =
            entity1.AddComponent<Component1>(out var component1) &&
            component1!.TestFloat.Set(1.0f);
        Assert.That(outcome);

        outcome =
            entity2.AddComponent<Component3>(out var component3) &&
            component3!.TestList.AddEntry(component1!);

        Assert.That(outcome);

        var count = component3!.TestList.Count;
        Assert.That(count == 1);

        outcome = entity1.Dispose();
        Assert.That(outcome);

        count = component3.TestList.Count;
        Assert.That(count == 0);
    }
    #endregion
}