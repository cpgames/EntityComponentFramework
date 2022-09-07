using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public static class GameIds
    {
        #region Fields
        public const byte NONE = 0;
        public const byte WORLD = 1;

        public const byte ACCOUNT = 10;
        public const byte CORP = 11;
        public const byte MAP = 12;
        public const byte BEHAVIOR = 13;
        public const byte FLEET = 14;
        public const byte INVENTORY = 15;
        public const byte CELESTIAL = 16;
        public const byte SECTOR = 17;
        public const byte BODY = 18;
        public const byte GALAXY = 19;
        public const byte QUEST = 20;
        public const byte STATION = 21;
        public const byte ADMIRAL = 22;
        public const byte ADMIRAL_TEMPLATE = 23;
        public const byte ENCOUNTER = 24;
        public const byte ACTOR = 25;
        public const byte ITEM = 26;
        public const byte ITEM_TEMPLATE = 27;
        public const byte WALLET = 28;
        public const byte AUTHENTICATION = 29;

        private static readonly Dictionary<byte, string> _idNames = new()
        {
            { NONE, "None" },
            { WORLD, "World" },

            { ACCOUNT, "Account" }, // 50
            { CORP, "Corp" }, // 51
            { MAP, "Map" }, // 52
            { BEHAVIOR, "Behavior" }, // 53
            { FLEET, "Fleet" }, // 54
            { INVENTORY, "Inventory" }, // 55
            { CELESTIAL, "Celestial" }, // 56
            { SECTOR, "Sector" }, // 57
            { BODY, "Body" }, // 58
            { GALAXY, "Galaxy" }, // 59
            { QUEST, "Quest" }, // 60
            { STATION, "Station" }, // 61
            { ADMIRAL, "Admiral" }, // 62
            { ADMIRAL_TEMPLATE, "Admiral Template" }, // 63
            { ENCOUNTER, "Encounter" }, // 64
            { ACTOR, "Actor" }, // 65
            { ITEM, "Item" }, // 66
            { ITEM_TEMPLATE, "Item Template" }, // 67
            { WALLET, "Wallet" }, // 68
            { AUTHENTICATION, "Authentication" } // 69
        };
        #endregion

        #region Properties
        public static IKey WorldKey => World.Key;
        #endregion

        #region Methods
        public static string ToTableName(Id id)
        {
            if (id.Length != 1)
            {
                return "unknown";
            }
            return _idNames.TryGetValue(id.Bytes[0], out var idName) ? idName : "unknown";
        }

        public static Outcome ToTableName(Id id, out string idName)
        {
            if (id.Length != 1)
            {
                idName = string.Empty;
                return Outcome.Fail("Id must have size of 1");
            }
            return _idNames.TryGetValue(id.Bytes[0], out idName) ?
                Outcome.Success() :
                Outcome.Fail($"No id <{id}> is found in GameIds");
        }

        public static string ToTableName(Address address)
        {
            return address.IdCount < 2 ?
                "unknown" :
                ToTableName(address.GetId(1));
        }

        public static Outcome ToTableName(Address address, out string idName)
        {
            if (address.IdCount < 2)
            {
                idName = string.Empty;
                return Outcome.Fail("Address must contain at least 2 ids");
            }
            return ToTableName(address.GetId(1), out idName);
        }
        #endregion
    }
}