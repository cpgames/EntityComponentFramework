using cpGames.core;
using cpGames.core.EntityComponentFramework;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class PropertyConverterTests
{
    #region Nested type: BoolToStringConverter
    private class BoolToStringConverter : IPropertyConverter<string>
    {
        #region IPropertyConverter<string> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(bool);
        }

        public Outcome Convert(object? data, out string value)
        {
            value = string.Empty;
            if (data is bool b)
            {
                value = b ? "true" : "false";
                return Outcome.Success();
            }
            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to string");
        }
        #endregion
    }
    #endregion

    #region Nested type: StringToBoolConverter
    private class StringToBoolConverter : IPropertyConverter<bool>
    {
        #region IPropertyConverter<bool> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out bool value)
        {
            value = false;
            if (data is string str)
            {
                value = str.ToLower() == "true" || str == "1" || str.ToLower() == "yes";
                return Outcome.Success();
            }
            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to bool");
        }
        #endregion
    }
    #endregion

    #region Nested type: TestClassA
    private class TestClassA
    {
        #region Fields
        public string name = string.Empty;
        public int value;
        #endregion
    }
    #endregion

    #region Nested type: TestClassB
    private class TestClassB
    {
        #region Fields
        public string title = string.Empty;
        public int count;
        #endregion
    }
    #endregion

    #region Nested type: TestJsonPropertyA
    private class TestJsonPropertyA : JsonProperty<TestClassA>
    {
        #region Constructors
        public TestJsonPropertyA(Entity owner, string name) : base(owner, name, new TestClassA()) { }
        #endregion
    }
    #endregion

    #region Nested type: TestJsonPropertyB
    private class TestJsonPropertyB : JsonProperty<TestClassB>
    {
        #region Constructors
        public TestJsonPropertyB(Entity owner, string name) : base(owner, name, new TestClassB()) { }
        #endregion
    }
    #endregion

    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestBoolToStringLink_WithoutConverter_ShouldFail()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var boolProp = new BoolProperty(entityA, "boolProperty");
        var stringProp = new StringProperty(entityB, "stringProperty");

        var connectBoolOutcome = boolProp.Connect();
        Assert.That(connectBoolOutcome, connectBoolOutcome.ErrorMessage);
        var connectStringOutcome = stringProp.Connect();
        Assert.That(connectStringOutcome, connectStringOutcome.ErrorMessage);

        var setOutcome = boolProp.Set(true);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should fail because there's no converter between bool and string
        var linkOutcome = stringProp.Link(boolProp);
        Assert.That(linkOutcome.IsSuccess, Is.False);
        Assert.That(linkOutcome.ErrorMessage, Does.Contain("Unsupported link between properties of different types"));
    }

    [Test]
    public void TestBoolToStringLink_WithCustomConverter_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var boolProp = new BoolProperty(entityA, "boolProperty");
        var stringProp = new StringProperty(entityB, "stringProperty");

        // Add custom converter to string property
        var addConverterOutcome = stringProp.AddConverter(new BoolToStringConverter());
        Assert.That(addConverterOutcome, addConverterOutcome.ErrorMessage);

        var connectBoolOutcome = boolProp.Connect();
        Assert.That(connectBoolOutcome, connectBoolOutcome.ErrorMessage);
        var connectStringOutcome = stringProp.Connect();
        Assert.That(connectStringOutcome, connectStringOutcome.ErrorMessage);

        var setOutcome = boolProp.Set(true);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because we added a converter
        var linkOutcome = stringProp.Link(boolProp);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getStringOutcome = stringProp.Get(out var stringValue);
        Assert.That(getStringOutcome, getStringOutcome.ErrorMessage);
        Assert.That(stringValue, Is.EqualTo("true"));

        // Test changing the bool value
        var updateOutcome = boolProp.Set(false);
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getUpdatedStringOutcome = stringProp.Get(out var updatedStringValue);
        Assert.That(getUpdatedStringOutcome, getUpdatedStringOutcome.ErrorMessage);
        Assert.That(updatedStringValue, Is.EqualTo("false"));
    }

    [Test]
    public void TestStringToBoolLink_WithCustomConverter_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var stringProp = new StringProperty(entityA, "stringProperty");
        var boolProp = new BoolProperty(entityB, "boolProperty");

        // Add custom converter to bool property
        var addConverterOutcome = boolProp.AddConverter(new StringToBoolConverter());
        Assert.That(addConverterOutcome, addConverterOutcome.ErrorMessage);

        var connectStringOutcome = stringProp.Connect();
        Assert.That(connectStringOutcome, connectStringOutcome.ErrorMessage);
        var connectBoolOutcome = boolProp.Connect();
        Assert.That(connectBoolOutcome, connectBoolOutcome.ErrorMessage);

        var setOutcome = stringProp.Set("true");
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because we added a converter
        var linkOutcome = boolProp.Link(stringProp);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getBoolOutcome = boolProp.Get(out var boolValue);
        Assert.That(getBoolOutcome, getBoolOutcome.ErrorMessage);
        Assert.That(boolValue, Is.True);

        // Test changing the string value
        var updateOutcome = stringProp.Set("false");
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getUpdatedBoolOutcome = boolProp.Get(out var updatedBoolValue);
        Assert.That(getUpdatedBoolOutcome, getUpdatedBoolOutcome.ErrorMessage);
        Assert.That(updatedBoolValue, Is.False);
    }

    [Test]
    public void TestListToDifferentElementTypeLink_ShouldFail()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var intListProp = new ListProperty<int>(entityA, "intListProperty");
        var stringListProp = new ListProperty<string>(entityB, "stringListProperty");

        var connectIntOutcome = intListProp.Connect();
        Assert.That(connectIntOutcome, connectIntOutcome.ErrorMessage);
        var connectStringOutcome = stringListProp.Connect();
        Assert.That(connectStringOutcome, connectStringOutcome.ErrorMessage);

        var addOutcome = intListProp.AddEntry(42);
        Assert.That(addOutcome, addOutcome.ErrorMessage);

        // Linking should fail because element types are different
        var linkOutcome = stringListProp.Link(intListProp);
        Assert.That(linkOutcome.IsSuccess, Is.False);
    }

    [Test]
    public void TestListToSameElementTypeLink_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var listPropA = new ListProperty<int>(entityA, "listPropertyA");
        var listPropB = new ListProperty<int>(entityB, "listPropertyB");

        var connectAOutcome = listPropA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = listPropB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var addOutcome = listPropA.AddEntry(42);
        Assert.That(addOutcome, addOutcome.ErrorMessage);

        // Linking should succeed because element types are the same
        var linkOutcome = listPropB.Link(listPropA);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getBOutcome = listPropB.Get(out var listB);
        Assert.That(getBOutcome, getBOutcome.ErrorMessage);
        Assert.That(listB, Is.Not.Null);
        Assert.That(listB!.Count, Is.EqualTo(1));
        Assert.That(listB[0], Is.EqualTo(42));

        // Test adding another entry
        var addAnotherOutcome = listPropA.AddEntry(100);
        Assert.That(addAnotherOutcome, addAnotherOutcome.ErrorMessage);

        var getBUpdatedOutcome = listPropB.Get(out var updatedListB);
        Assert.That(getBUpdatedOutcome, getBUpdatedOutcome.ErrorMessage);
        Assert.That(updatedListB, Is.Not.Null);
        Assert.That(updatedListB!.Count, Is.EqualTo(2));
        Assert.That(updatedListB[0], Is.EqualTo(42));
        Assert.That(updatedListB[1], Is.EqualTo(100));
    }

    [Test]
    public void TestJsonToDifferentModelLink_ShouldFail()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var jsonPropA = new TestJsonPropertyA(entityA, "jsonPropertyA");
        var jsonPropB = new TestJsonPropertyB(entityB, "jsonPropertyB");

        var connectAOutcome = jsonPropA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = jsonPropB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var testObjectA = new TestClassA { name = "TestA", value = 42 };
        var setOutcome = jsonPropA.Set(testObjectA);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should fail because the JSON models are different types
        var linkOutcome = jsonPropB.Link(jsonPropA);
        Assert.That(linkOutcome.IsSuccess, Is.False);
    }

    [Test]
    public void TestJsonToDifferentModelLink_WithDifferentFields_ShouldFail()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var jsonPropA = new TestJsonPropertyA(entityA, "jsonPropertyA");
        var jsonPropB = new TestJsonPropertyB(entityB, "jsonPropertyB");

        var connectAOutcome = jsonPropA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = jsonPropB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var testObjectA = new TestClassA { name = "TestA", value = 42 };
        var testObjectB = new TestClassB { title = "TestB", count = 100 };

        var setAOutcome = jsonPropA.Set(testObjectA);
        Assert.That(setAOutcome, setAOutcome.ErrorMessage);
        var setBOutcome = jsonPropB.Set(testObjectB);
        Assert.That(setBOutcome, setBOutcome.ErrorMessage);

        // Verify the objects have different field structures
        Assert.That(testObjectA.name, Is.EqualTo("TestA"));
        Assert.That(testObjectA.value, Is.EqualTo(42));
        Assert.That(testObjectB.title, Is.EqualTo("TestB"));
        Assert.That(testObjectB.count, Is.EqualTo(100));

        // Linking should fail because the JSON models are different types
        var linkOutcome = jsonPropB.Link(jsonPropA);
        Assert.That(linkOutcome.IsSuccess, Is.False);
    }

    [Test]
    public void TestJsonToSameModelLink_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var jsonPropA = new TestJsonPropertyA(entityA, "jsonPropertyA");
        var jsonPropB = new TestJsonPropertyA(entityB, "jsonPropertyB");

        var connectAOutcome = jsonPropA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);
        var connectBOutcome = jsonPropB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);

        var testObject = new TestClassA { name = "TestName", value = 42 };
        var setOutcome = jsonPropA.Set(testObject);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because the JSON models are the same type
        var linkOutcome = jsonPropB.Link(jsonPropA);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getBOutcome = jsonPropB.Get(out var valueB);
        Assert.That(getBOutcome, getBOutcome.ErrorMessage);
        Assert.That(valueB, Is.Not.Null);
        Assert.That(valueB!.name, Is.EqualTo("TestName"));
        Assert.That(valueB.value, Is.EqualTo(42));

        // Test modifying the object in propA (the source)
        var getAOutcome = jsonPropA.Get(out var valueA);
        Assert.That(getAOutcome, getAOutcome.ErrorMessage);
        Assert.That(valueA, Is.Not.Null);

        valueA!.name = "ModifiedName";
        valueA.value = 100;

        var updateOutcome = jsonPropA.UpdateValue();
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        // Now check that propB (the linked property) reflects the changes
        var getBUpdatedOutcome = jsonPropB.Get(out var updatedValueB);
        Assert.That(getBUpdatedOutcome, getBUpdatedOutcome.ErrorMessage);
        Assert.That(updatedValueB, Is.Not.Null);
        Assert.That(updatedValueB!.name, Is.EqualTo("ModifiedName"));
        Assert.That(updatedValueB.value, Is.EqualTo(100));
    }

    [Test]
    public void TestIntToFloatLink_WithBuiltInConverter_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var intProp = new IntProperty(entityA, "intProperty");
        var floatProp = new FloatProperty(entityB, "floatProperty");

        var connectIntOutcome = intProp.Connect();
        Assert.That(connectIntOutcome, connectIntOutcome.ErrorMessage);
        var connectFloatOutcome = floatProp.Connect();
        Assert.That(connectFloatOutcome, connectFloatOutcome.ErrorMessage);

        var setOutcome = intProp.Set(42);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because FloatProperty has IntToFloatConverter
        var linkOutcome = floatProp.Link(intProp);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getFloatOutcome = floatProp.Get(out var floatValue);
        Assert.That(getFloatOutcome, getFloatOutcome.ErrorMessage);
        Assert.That(floatValue, Is.EqualTo(42.0f));

        // Test changing the int value
        var updateOutcome = intProp.Set(100);
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getUpdatedFloatOutcome = floatProp.Get(out var updatedFloatValue);
        Assert.That(getUpdatedFloatOutcome, getUpdatedFloatOutcome.ErrorMessage);
        Assert.That(updatedFloatValue, Is.EqualTo(100.0f));
    }

    [Test]
    public void TestLongToIntLink_WithBuiltInConverter_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var longProp = new LongProperty(entityA, "longProperty");
        var intProp = new IntProperty(entityB, "intProperty");

        var connectLongOutcome = longProp.Connect();
        Assert.That(connectLongOutcome, connectLongOutcome.ErrorMessage);
        var connectIntOutcome = intProp.Connect();
        Assert.That(connectIntOutcome, connectIntOutcome.ErrorMessage);

        var setOutcome = longProp.Set(42L);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because IntProperty has LongToIntConverter
        var linkOutcome = intProp.Link(longProp);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getIntOutcome = intProp.Get(out var intValue);
        Assert.That(getIntOutcome, getIntOutcome.ErrorMessage);
        Assert.That(intValue, Is.EqualTo(42));

        // Test changing the long value
        var updateOutcome = longProp.Set(100L);
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getUpdatedIntOutcome = intProp.Get(out var updatedIntValue);
        Assert.That(getUpdatedIntOutcome, getUpdatedIntOutcome.ErrorMessage);
        Assert.That(updatedIntValue, Is.EqualTo(100));
    }

    [Test]
    public void TestStringToIdLink_WithBuiltInConverter_ShouldSucceed()
    {
        var entityA = new Entity(new Id(1));
        var entityB = new Entity(new Id(2));

        var stringProp = new StringProperty(entityA, "stringProperty");
        var idProp = new IdProperty(entityB, "idProperty");

        var connectStringOutcome = stringProp.Connect();
        Assert.That(connectStringOutcome, connectStringOutcome.ErrorMessage);
        var connectIdOutcome = idProp.Connect();
        Assert.That(connectIdOutcome, connectIdOutcome.ErrorMessage);

        var setOutcome = stringProp.Set("7B");
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        // Linking should succeed because IdProperty has StringToIdConverter
        var linkOutcome = idProp.Link(stringProp);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        var getIdOutcome = idProp.Get(out var idValue);
        Assert.That(getIdOutcome, getIdOutcome.ErrorMessage);
        Assert.That(idValue, Is.EqualTo(new Id(123)));

        // Test changing the string value
        var updateOutcome = stringProp.Set("C8");
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var getUpdatedIdOutcome = idProp.Get(out var updatedIdValue);
        Assert.That(getUpdatedIdOutcome, getUpdatedIdOutcome.ErrorMessage);
        Assert.That(updatedIdValue, Is.EqualTo(new Id(200)));
    }
    #endregion
}