using cpGames.core;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class LinkTests
{
    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestLink1()
    {
        var entity = new Entity(Id.INVALID);
        FloatProperty? f1, f2 = null;
        var addOutcome =
            entity.AddProperty("f1", out f1) &&
            entity.AddProperty("f2", out f2);
        Assert.Multiple(() =>
        {
            Assert.That(addOutcome, addOutcome.ErrorMessage);
            Assert.That(f1, Is.Not.Null);
            Assert.That(f2, Is.Not.Null);
        });

        // try to add f2 and have a failure
        addOutcome = entity.AddProperty("f2", out IntProperty? i1);
        Assert.Multiple(() =>
        {
            Assert.That(addOutcome.IsSuccess, Is.False);
            Assert.That(i1, Is.Null);
        });

        var linkOutcome = f2!.Link(f1!);
        Assert.Multiple(() =>
        {
            Assert.That(linkOutcome, linkOutcome.ErrorMessage);
            Assert.That(f2.IsLinked(), Is.True);
        });
        var additionOutcome = f1!.Add(1);
        Assert.Multiple(() =>
        {
            Assert.That(additionOutcome, additionOutcome.ErrorMessage);
            Assert.That(f1!.Value, Is.EqualTo(1));
            Assert.That(f2!.Value, Is.EqualTo(1));
            Assert.That(f2.IsLinked(), Is.True);
        });
        var subtractionOutcome = f2!.Subtract(1);
        Assert.Multiple(() =>
        {
            Assert.That(subtractionOutcome, subtractionOutcome.ErrorMessage);
            Assert.That(f1!.Value, Is.EqualTo(1));
            Assert.That(f2!.Value, Is.EqualTo(0));
            Assert.That(f2.IsLinked(), Is.False);
        });
    }
    #endregion
}