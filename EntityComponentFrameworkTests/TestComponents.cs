using cpGames.core;
using cpGames.core.EntityComponentFramework;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class TestComponents
{
    #region Nested type: Component1
    private class Component1 : Component
    {
        #region Properties
        [cpGames.core.EntityComponentFramework.Property("TestProperty", typeof(FloatProperty))]
        public IFloatProperty TestProperty { get; private set; } = null!;
        #endregion
    }
    #endregion

    #region Nested type: Component2
    private class Component2 : Component
    {
        #region Properties
        [cpGames.core.EntityComponentFramework.Property("TestProperty", typeof(FloatProperty))]
        public IFloatProperty TestProperty { get; private set; } = null!;
        #endregion
    }
    #endregion

    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestComponents1()
    {
        var entity = new Entity(Id.INVALID);
        Component1? component1 = null;
        Component2? component2 = null;
        var addComponentsOutcome =
            entity.AddComponent(out component1) &&
            entity.AddComponent(out component2);
        Assert.Multiple(() =>
        {
            Assert.That(addComponentsOutcome, addComponentsOutcome.ErrorMessage);
            Assert.That(component1, Is.Not.Null);
            Assert.That(component2, Is.Not.Null);
            Assert.That(component1!.TestProperty, Is.Not.Null);
            Assert.That(component2!.TestProperty, Is.Not.Null);
            Assert.That(component2.TestProperty, Is.EqualTo(component1.TestProperty));
        });
    }
    #endregion
}