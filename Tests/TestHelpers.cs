using cpGames.core;

namespace Tests;

public class TestHelpers
{
    #region Methods
    public static Id GenerateId(IdGenerator idGenerator, IIdProvider provider)
    {
        var generateIdOutcome = idGenerator.GenerateId(provider, out var id);
        if (!generateIdOutcome)
        {
            Assert.Fail(generateIdOutcome.ErrorMessage);
        }
        return id;
    }
    #endregion
}