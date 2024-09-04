using cpGames.core;
using cpGames.core.EntityComponentFramework.impl;

namespace EntityComponentFrameworkTests;

public class ListTests
{
    #region Nested type: BaseEntry
    private class BaseEntry
    {
        #region Fields
        public int i;
        #endregion
    }
    #endregion

    #region Nested type: DerivedEntry
    private class DerivedEntry : BaseEntry
    {
        #region Fields
        public string a = string.Empty;
        #endregion
    }
    #endregion

    #region Methods
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestAddRemoveElementsLinked()
    {
        var entity = new Entity(Id.INVALID);
        var addOutcome = entity.AddProperty<ListProperty<int>>("list1", out var list1);
        Assert.That(addOutcome, addOutcome.ErrorMessage);
        var addOutcome2 = entity.AddProperty<ListProperty<int>>("list2", out var list2);
        Assert.That(addOutcome2, addOutcome2.ErrorMessage);
        var linkOutcome = list2!.Link(list1!);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);
        // add value to list1 and verify it is in list2
        var addValueOutcome = list1!.AddEntry(1);
        Assert.That(addValueOutcome, addValueOutcome.ErrorMessage);
        Assert.That(list2.Contains(1), Is.True);
        // remove value from list1 and verify it is not in list2
        var removeValueOutcome = list1.RemoveEntry(1);
        Assert.That(removeValueOutcome, removeValueOutcome.ErrorMessage);
        Assert.That(list2.Contains(1), Is.False);
    }

    [Test]
    public void TestCovariantLink()
    {
        var entity = new Entity(Id.INVALID);
        var addDerivedListOutcome = entity.AddProperty<ListProperty<DerivedEntry>>("derived_list", out var derivedList);
        Assert.That(addDerivedListOutcome, addDerivedListOutcome.ErrorMessage);
        var setOutcome = derivedList!.Set(
            new List<DerivedEntry>
            {
                new() { i = 1, a = "a" },
                new() { i = 2, a = "b" },
                new() { i = 3, a = "c" }
            });
        Assert.That(setOutcome, setOutcome.ErrorMessage);
        var addBaseListOutcome = entity.AddProperty<ListProperty<BaseEntry>>("base_list", out var baseList);
        Assert.That(addBaseListOutcome, addBaseListOutcome.ErrorMessage);
        var linkOutcome = baseList!.Link(derivedList);
        Assert.That(linkOutcome, linkOutcome.ErrorMessage);
        var getOutcome = baseList.Get(out var baseListData);
        Assert.That(getOutcome, getOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(baseListData!.Count, Is.EqualTo(3));

            var entry = baseListData[0];
            Assert.That(entry is DerivedEntry);
            var derivedEntry = entry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(1));
            Assert.That(derivedEntry.a, Is.EqualTo("a"));

            entry = baseListData[1];
            Assert.That(entry is DerivedEntry);
            derivedEntry = entry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(2));
            Assert.That(derivedEntry.a, Is.EqualTo("b"));

            entry = baseListData[2];
            Assert.That(entry is DerivedEntry);
            derivedEntry = entry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(3));
            Assert.That(derivedEntry.a, Is.EqualTo("c"));
        });

        BaseEntry? newBaseEntry = null;
        baseList.EntryAddedSignal.AddCommand(
            val =>
            {
                newBaseEntry = val;
                return Outcome.Success();
            });
        object? objEntry = null;
        baseList.EntryObjAddedSignal.AddCommand(
            val =>
            {
                objEntry = val;
                return Outcome.Success();
            });
        var addOutcome = derivedList.AddEntry(new DerivedEntry { i = 4, a = "d" });
        Assert.That(addOutcome, addOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(newBaseEntry, Is.Not.Null);
            Assert.That(newBaseEntry is DerivedEntry);
            var derivedEntry = newBaseEntry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(4));
            Assert.That(derivedEntry.a, Is.EqualTo("d"));

            Assert.That(objEntry, Is.Not.Null);
            Assert.That(objEntry is DerivedEntry);
            derivedEntry = objEntry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(4));
            Assert.That(derivedEntry.a, Is.EqualTo("d"));
        });
        var unlinkOutcome = baseList.Unlink();
        Assert.That(unlinkOutcome, unlinkOutcome.ErrorMessage);
        addOutcome = derivedList.AddEntry(new DerivedEntry { i = 5, a = "e" });
        Assert.That(addOutcome, addOutcome.ErrorMessage);
        getOutcome = baseList.Get(out baseListData);
        Assert.That(getOutcome, getOutcome.ErrorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(baseList.Empty, Is.True);
            Assert.That(derivedList.Count, Is.EqualTo(5));
            Assert.That(newBaseEntry, Is.Not.Null);
            Assert.That(newBaseEntry is DerivedEntry);
            var derivedEntry = newBaseEntry as DerivedEntry;
            Assert.That(derivedEntry!.i, Is.EqualTo(4));
            Assert.That(derivedEntry.a, Is.EqualTo("d"));
        });
    }
    #endregion
}