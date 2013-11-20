using System;

namespace Vre.Server
{
    /// <summary>
    /// Database settings persistent class. Object IDs must be taken from <see cref="DatabaseSettings.Setting"/> enumeration.
    /// </summary>
    internal class DatabaseSettings
    {
        /// <summary>
        /// Current schema version of database; to be verified against real DB on startup.
        /// </summary>
        public const int CurrentDbVersion = 25;

        /// <summary>
        /// Possible values of <see cref="Id"/> property.
        /// </summary>
        public enum Setting : int
        {
            DbSchemaVersionMin = 1,
            CreateTime = 2,
            DbSchemaVersionMax = 3,
            UpgradeStatus = 4
        }

        /// <summary>
        /// Object ID.  Must be taken from <see cref="DatabaseSettings.Setting"/> enumeration.
        /// </summary>
        public int Id { get; set; }
        public bool? BitValue { get; set; }
        public long? IntValue { get; set; }
        public double? FloatValue { get; set; }
        public DateTime? TimeValue { get; set; }
        public string TextValue { get; set; }
    }
}