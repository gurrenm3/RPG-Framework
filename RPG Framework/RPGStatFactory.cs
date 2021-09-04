using System;
using System.Collections.Generic;

namespace RPG_Framework
{
    /// <summary>
    /// Factory that creates all RPGStats
    /// </summary>
    public class RPGStatFactory
    {
        /// <summary>
        /// A list of every RPGStat created with the Factory
        /// </summary>
        public static List<RPGStat> AllStats { get; private set; } = new List<RPGStat>();

        /// <summary>
        /// Create a default RPGStat
        /// </summary>
        /// <returns>A new RPGStat</returns>
        public static T Create<T>() where T : RPGStat, new()
        {
            var stat = new T();
            Init(stat);
            AllStats.Add(stat);
            return stat;
        }

        /// <summary>
        /// Create a default RPGStat
        /// </summary>
        /// <returns>A new RPGStat</returns>
        public static RPGStat Create()
        {
            var stat = new RPGStat();
            Init(stat);
            AllStats.Add(stat);
            return stat;
        }

        /// <summary>
        /// Creates a RPGStat with a name and max level
        /// </summary>
        /// <param name="statName">Name of the Stat. By default it will also be applied to DisplayName</param>
        /// <param name="maxLevel">Max level for the stat</param>
        /// <returns>A new RPGStat</returns>
        public static RPGStat Create(string statName, int maxLevel) 
        {
            var stat = new RPGStat(statName, maxLevel);
            Init(stat);
            AllStats.Add(stat);
            return stat;
        }

        /// <summary>
        /// Creates a RPGStat with a name, max level, and ExpTable
        /// </summary>
        /// <param name="statName">Name of the Stat. By default it will also be applied to DisplayName</param>
        /// <param name="maxLevel">Max level for the stat</param>
        /// <param name="baseExp">Used for creating the ExpTable. The amount of Exp that each level should be based off of. Also the
        /// amount of Exp for level 1 to level up</param>
        /// <param name="expMultiplier">Used for creating the ExpTable. The amount to multiply the
        /// Exp required for each level to level up</param>
        /// <returns>A new RPGStat</returns>
        public static RPGStat Create(string statName, int maxLevel, double baseExp, double expMultiplier)
        {
            var stat = new RPGStat(statName, maxLevel, baseExp, expMultiplier);
            Init(stat);
            AllStats.Add(stat);
            return stat;
        }

        private static void Init(RPGStat stat)
        {
            
        }
    }
}
