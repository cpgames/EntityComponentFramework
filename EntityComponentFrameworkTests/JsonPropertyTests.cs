using cpGames.core;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

[TestFixture]
public class JsonPropertyTests
{
    private const string InitialName = "InitialName";
    private const int InitialValue = 42;

    private TestJsonProperty _propA = null!;
    private TestJsonProperty _propB = null!;

    [SetUp]
    public void SetUp()
    {
        _propA = CreateProperty(1, "testProperty");
        _propB = CreateProperty(2, "linkedProperty");

        var connectAOutcome = _propA.Connect();
        Assert.That(connectAOutcome, connectAOutcome.ErrorMessage);

        var connectBOutcome = _propB.Connect();
        Assert.That(connectBOutcome, connectBOutcome.ErrorMessage);
    }

    [Test]
    public void Link_WhenPropertiesAreConnected_MarksTargetAsLinked()
    {
        // Arrange
        SetInitialValueAndLink();

        // Act
        var isLinked = _propB.IsLinked();

        // Assert
        Assert.That(isLinked, Is.True);
    }

    [Test]
    public void Link_WhenSourceHasValue_CopiesValueToTarget()
    {
        // Arrange
        SetInitialValueAndLink();

        // Act
        var valueB = GetValue(_propB);

        // Assert
        AssertTestValue(valueB, InitialName, InitialValue, true);
    }

    [Test]
    public void UpdateValue_WhenSourceIsModified_UpdatesSourceValue()
    {
        // Arrange
        var sourceValue = SetInitialValueAndLink();

        sourceValue.Name = "ModifiedName";
        sourceValue.Value = 100;

        // Act
        var updateOutcome = _propA.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var updatedValueA = GetValue(_propA);
        AssertTestValue(updatedValueA, "ModifiedName", 100, true);
    }

    [Test]
    public void UpdateValue_WhenSourceIsModified_PropagatesValueToLinkedTarget()
    {
        // Arrange
        var sourceValue = SetInitialValueAndLink();

        sourceValue.Name = "ModifiedName";
        sourceValue.Value = 100;

        // Act
        var updateOutcome = _propA.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var updatedValueB = GetValue(_propB);
        AssertTestValue(updatedValueB, "ModifiedName", 100, true);
    }

    [Test]
    public void UpdateValue_WhenSourceIsModified_KeepsTargetLinked()
    {
        // Arrange
        var sourceValue = SetInitialValueAndLink();

        sourceValue.Name = "ModifiedName";

        // Act
        var updateOutcome = _propA.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);
        Assert.That(_propB.IsLinked(), Is.True);
    }

    [Test]
    public void UpdateValue_WhenTargetIsModified_UnlinksTarget()
    {
        // Arrange
        SetInitialValueAndLink();

        var targetValue = GetValue(_propB);
        targetValue.Name = "ModifiedInB";
        targetValue.Value = 200;
        targetValue.IsActive = false;

        // Act
        var updateOutcome = _propB.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);
        Assert.That(_propB.IsLinked(), Is.False);
    }

    [Test]
    public void UpdateValue_WhenTargetIsModified_PersistsTargetValue()
    {
        // Arrange
        SetInitialValueAndLink();

        var targetValue = GetValue(_propB);
        targetValue.Name = "ModifiedInB";
        targetValue.Value = 200;
        targetValue.IsActive = false;

        // Act
        var updateOutcome = _propB.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var updatedTargetValue = GetValue(_propB);
        AssertTestValue(updatedTargetValue, "ModifiedInB", 200, false);
    }

    [Test]
    public void UpdateValue_WhenTargetIsModified_DoesNotModifySourceValue()
    {
        // Arrange
        SetInitialValueAndLink();

        var targetValue = GetValue(_propB);
        targetValue.Name = "ModifiedInB";
        targetValue.Value = 200;
        targetValue.IsActive = false;

        // Act
        var updateOutcome = _propB.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var sourceValue = GetValue(_propA);
        AssertTestValue(sourceValue, InitialName, InitialValue, true);
    }

    [Test]
    public void UpdateValue_WhenSourceIsModified_SourceAndTargetHaveEquivalentValues()
    {
        // Arrange
        var sourceValue = SetInitialValueAndLink();

        sourceValue.Name = "ModifiedName";
        sourceValue.Value = 100;

        // Act
        var updateOutcome = _propA.UpdateValue();

        // Assert
        Assert.That(updateOutcome, updateOutcome.ErrorMessage);

        var updatedSource = GetValue(_propA);
        var updatedTarget = GetValue(_propB);

        Assert.Multiple(() =>
        {
            Assert.That(updatedTarget.Name, Is.EqualTo(updatedSource.Name));
            Assert.That(updatedTarget.Value, Is.EqualTo(updatedSource.Value));
            Assert.That(updatedTarget.IsActive, Is.EqualTo(updatedSource.IsActive));
        });
    }

    private TestClass SetInitialValueAndLink()
    {
        var initialValue = CreateInitialValue();

        var setOutcome = _propA.Set(initialValue);
        Assert.That(setOutcome, setOutcome.ErrorMessage);

        var linkOutcome = _propB.Link(_propA);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);

        return initialValue;
    }

    private static TestClass GetValue(TestJsonProperty property)
    {
        var getOutcome = property.Get(out var value);

        Assert.That(getOutcome, getOutcome.ErrorMessage);
        Assert.That(value, Is.Not.Null);

        return value!;
    }

    private static void AssertTestValue(
        TestClass actual,
        string expectedName,
        int expectedValue,
        bool expectedIsActive)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Name, Is.EqualTo(expectedName));
            Assert.That(actual.Value, Is.EqualTo(expectedValue));
            Assert.That(actual.IsActive, Is.EqualTo(expectedIsActive));
        });
    }

    private static TestClass CreateInitialValue()
    {
        return new TestClass
        {
            Name = InitialName,
            Value = InitialValue,
            IsActive = true
        };
    }

    private static TestJsonProperty CreateProperty(byte entityId, string propertyName)
    {
        var entity = new Entity(new Id(entityId));
        return new TestJsonProperty(entity, propertyName);
    }

    private sealed class TestClass
    {
        public string Name { get; set; } = string.Empty;

        public int Value { get; set; }

        public bool IsActive { get; set; }
    }

    private sealed class TestJsonProperty : JsonProperty<TestClass>
    {
        public TestJsonProperty(Entity owner, string name)
            : base(owner, name, new TestClass())
        {
        }
    }
}