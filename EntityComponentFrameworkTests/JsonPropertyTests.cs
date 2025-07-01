using cpGames.core;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class JsonPropertyTests
{
    #region Nested type: TestClass
    private class TestClass
    {
        #region Fields
        public string name = string.Empty;
        public int value;
        public bool isActive;
        #endregion
    }
    #endregion

    #region Nested type: TestJsonProperty
    private class TestJsonProperty : JsonProperty<TestClass>
    {
        #region Constructors
        public TestJsonProperty(Entity owner, string name) : base(owner, name, new TestClass()) { }
        #endregion
    }
    #endregion

    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestJsonLink()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var propA = new TestJsonProperty(entityA, "testProperty");
        var propB = new TestJsonProperty(entityB, "linkedProperty");

        var connectAOutcome = propA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = propB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var testObject = new TestClass
        {
            name = "InitialName",
            value = 42,
            isActive = true
        };

        var setOutcome = propA.Set(testObject);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        var linkOutcome = propB.Link(propA);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getAOutcome = propA.Get(out var valueA);
        Assert.That(getAOutcome, getAOutcome.ErrorMessage);
        var getBOutcome = propB.Get(out var valueB);
        Assert.That(getBOutcome, getBOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(valueA, Is.Not.Null);
            Assert.That(valueA!.name, Is.EqualTo("InitialName"));
            Assert.That(valueA.value, Is.EqualTo(42));
            Assert.That(valueA.isActive, Is.True);
            Assert.That(valueB, Is.Not.Null);
            Assert.That(valueB!.name, Is.EqualTo("InitialName"));
            Assert.That(valueB.value, Is.EqualTo(42));
            Assert.That(valueB.isActive, Is.True);
            Assert.That(propB.IsLinked(), Is.True);
        });

        testObject.name = "ModifiedName";
        testObject.value = 100;

        var updateOutcome = propA.UpdateValue();
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getAUpdatedOutcome = propA.Get(out var updatedValueA);
        Assert.That(getAUpdatedOutcome, getAUpdatedOutcome.ErrorMessage);
        var getBUpdatedOutcome = propB.Get(out var updatedValueB);
        Assert.That(getBUpdatedOutcome, getBUpdatedOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(updatedValueA, Is.Not.Null);
            Assert.That(updatedValueA!.name, Is.EqualTo("ModifiedName"));
            Assert.That(updatedValueA.value, Is.EqualTo(100));
            Assert.That(updatedValueA.isActive, Is.True);
            Assert.That(updatedValueB, Is.Not.Null);
            Assert.That(updatedValueB!.name, Is.EqualTo("ModifiedName"));
            Assert.That(updatedValueB.value, Is.EqualTo(100));
            Assert.That(updatedValueB.isActive, Is.True);
            Assert.That(updatedValueA.name, Is.EqualTo(updatedValueB.name));
            Assert.That(updatedValueA.value, Is.EqualTo(updatedValueB.value));
            Assert.That(updatedValueA.isActive, Is.EqualTo(updatedValueB.isActive));
        });
    }

    [Test]
    public void TestJsonLinkModifyPropB()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var propA = new TestJsonProperty(entityA, "testProperty");
        var propB = new TestJsonProperty(entityB, "linkedProperty");

        var connectAOutcome = propA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = propB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var testObject = new TestClass
        {
            name = "InitialName",
            value = 42,
            isActive = true
        };

        var setOutcome = propA.Set(testObject);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        var linkOutcome = propB.Link(propA);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getAOutcome = propA.Get(out var valueA);
        Assert.That(getAOutcome, getAOutcome.ErrorMessage);
        var getBOutcome = propB.Get(out var valueB);
        Assert.That(getBOutcome, getBOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(valueA, Is.Not.Null);
            Assert.That(valueA!.name, Is.EqualTo("InitialName"));
            Assert.That(valueA.value, Is.EqualTo(42));
            Assert.That(valueA.isActive, Is.True);
            Assert.That(valueB, Is.Not.Null);
            Assert.That(valueB!.name, Is.EqualTo("InitialName"));
            Assert.That(valueB.value, Is.EqualTo(42));
            Assert.That(valueB.isActive, Is.True);
            Assert.That(propB.IsLinked(), Is.True);
        });

        // Modify value in propB
        valueB!.name = "ModifiedInB";
        valueB.value = 200;
        valueB.isActive = false;

        var updateBOutcome = propB.UpdateValue();
        Assert.That(updateBOutcome, updateBOutcome.ErrorMessage);

        var getAFinalOutcome = propA.Get(out var finalValueA);
        Assert.That(getAFinalOutcome, getAFinalOutcome.ErrorMessage);
        var getBFinalOutcome = propB.Get(out var finalValueB);
        Assert.That(getBFinalOutcome, getBFinalOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            // propA should still have original value
            Assert.That(finalValueA, Is.Not.Null);
            Assert.That(finalValueA!.name, Is.EqualTo("InitialName"));
            Assert.That(finalValueA.value, Is.EqualTo(42));
            Assert.That(finalValueA.isActive, Is.True);
            
            // propB should have modified value
            Assert.That(finalValueB, Is.Not.Null);
            Assert.That(finalValueB!.name, Is.EqualTo("ModifiedInB"));
            Assert.That(finalValueB.value, Is.EqualTo(200));
            Assert.That(finalValueB.isActive, Is.False);
            
            // Values should be different (no longer linked)
            Assert.That(finalValueA.name, Is.Not.EqualTo(finalValueB.name));
            Assert.That(finalValueA.value, Is.Not.EqualTo(finalValueB.value));
            Assert.That(finalValueA.isActive, Is.Not.EqualTo(finalValueB.isActive));
            
            // propB should no longer be linked
            Assert.That(propB.IsLinked(), Is.False);
        });
    }
    #endregion
}