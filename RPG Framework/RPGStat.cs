using System;
using System.Collections.Generic;
using RPG_Framework.Extensions;

namespace RPG_Framework
{
    /// <summary>
    /// TODO: Make sure RPG_Stats get loaded from a directory relative to the current save.
    /// This allows for different saves to have different stat levels
    /// </summary>
    public class RPGStat
    {
        /// <summary>
        /// Called when a level is gained. If multiple levels are gained it will fire for
        /// each level gained
        /// </summary>
        public List<Action> OnLevelRaised { get; set; } = new List<Action>();

        /// <summary>
        /// Called when a level is lost. If multiple levels are lost it will fire for
        /// each level lost
        /// </summary>
        public List<Action> OnLevelReduced { get; set; } = new List<Action>();

        /// <summary>
        /// Name of the stat
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name for the stat. Can be used if you want the Stat Name to be different from
        /// what gets displayed in game. By default it will be the same as Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Current Exp for the stat
        /// </summary>
        public double CurrentExp { get; private set; }

        /// <summary>
        /// Current level of the stat. Starts at level 0
        /// </summary>
        public int CurrentLevel { get; private set; } = 0;

        /// <summary>
        /// Minimum level the stat can be
        /// </summary>
        public int MinLevel { get; set; } = 0;

        /// <summary>
        /// Max level of the stat
        /// </summary>
        public int MaxLevel { get; private set; }

        /// <summary>
        /// An ExpTable containing a list of how much Exp is required to level up for each level
        /// </summary>
        public ExpTable ExpTable { get; set; }

        /// <summary>
        /// Create a default RPGStat
        /// </summary>
        public RPGStat()
        {

        }

        /// <summary>
        /// Creates a RPGStat with a name and max level
        /// </summary>
        /// <param name="name">Name of the Stat. By default it will also be applied to DisplayName</param>
        /// <param name="maxLevel">Max level for the stat</param>
        public RPGStat(string name, int maxLevel)
        {
            Name = name;
            DisplayName = name;
            MaxLevel = maxLevel;
        }

        /// <summary>
        /// Creates a RPGStat with a name, max level, and ExpTable
        /// </summary>
        /// <param name="name">Name of the Stat. By default it will also be applied to DisplayName</param>
        /// <param name="maxLevel">Max level for the stat</param>
        /// <param name="baseExp">Used for creating the ExpTable. The amount of Exp that each level should be based off of. Also the
        /// amount of Exp for level 1 to level up</param>
        /// <param name="expMultiplier">Used for creating the ExpTable. The amount to multiply the
        /// Exp required for each level to level up</param>
        public RPGStat(string name, int maxLevel, double baseExp, double expMultiplier) : this(name, maxLevel)
        {
            ExpTable = ExpTable.CreateFromMultiplier(baseExp, expMultiplier, maxLevel);
        }

        /// <summary>
        /// Add Exp to this <see cref="RPGStat"/> and automatically raise 
        /// <see cref="CurrentLevel"/> up it's able to Level Up </summary>
        /// <param name="amountToAdd">Amount Exp to add</param>
        public virtual void RaiseExp(double amountToAdd)
        {
            if (amountToAdd <= 0)
                return;

            while (CurrentLevel <= MaxLevel && amountToAdd > 0)// && we still have EXP to level up)
            {
                double expToLevelUp = ExpTable.GetRemainingExp(CurrentLevel, CurrentExp);
                if (expToLevelUp < 0)
                    break;

                if (amountToAdd < expToLevelUp) // we can't level up
                {
                    CurrentExp += amountToAdd;
                    amountToAdd = 0;
                }
                else // we can level up
                {
                    CurrentExp += expToLevelUp;
                    amountToAdd -= expToLevelUp; // remove the exp we're consuming to level up
                    RaiseLevel();
                }
            }
        }

        /// <summary>
        /// Remove Exp from this <see cref="RPGStat"/> and automatically reduce 
        /// the level if necessary </summary>
        /// <param name="amountToRemove">Amount of Exp to remove</param>
        public virtual void ReduceExp(double amountToRemove)
        {
            while (amountToRemove > 0)
            {
                if (amountToRemove <= CurrentExp) // our level won't decrease
                {
                    CurrentExp -= amountToRemove;
                    amountToRemove = 0;
                }
                else if (CurrentLevel == MinLevel) // level can't decrease since it's already MinLevl
                {
                    CurrentExp = 0;
                    amountToRemove = 0;
                }
                else // our level will decrease
                {
                    amountToRemove -= CurrentExp; // remove the exp we're consuming to reduce the level
                    ReduceLevel();
                    CurrentExp = ExpTable.GetLevelMaxExp(CurrentLevel);
                }
            }

            if (CurrentExp < 0) CurrentExp = 0;
        }

        /// <summary>
        /// Sets the value of <see cref="MinLevel"/>. Will raise <see cref="CurrentLevel"/> if
        /// it is below <paramref name="newMinLevel"/>
        /// </summary>
        /// <param name="newMinLevel"></param>
        /// <param name="useLevelingLogic">If set to true this method will use <see cref="RaiseLevel(int)"/>
        /// to raise the level, which would also perform other Levelup logic. Setting it to false
        /// will just raise <see cref="CurrentLevel"/> to <paramref name="newMinLevel"/> and do nothing else.<br/><br/>
        /// This will only be used if <see cref="CurrentLevel"/> is below <paramref name="newMinLevel"/></param>
        public virtual void SetMinLevel(int newMinLevel, bool useLevelingLogic = true)
        {
            if (newMinLevel == MinLevel) return;
            if (CurrentLevel < MinLevel)
                SetLevel(newMinLevel, useLevelingLogic);

            MinLevel = newMinLevel;
        }

        /// <summary>
        /// Set the max level of this <see cref="RPGStat"/>. Will also reduce 
        /// <see cref="CurrentLevel"/> if <paramref name="newMaxLevel"/> is less than 
        /// the current level</summary>
        /// <param name="newMaxLevel"></param>
        /// <param name="useLevelingLogic">If set to true this method will use <see cref="ReduceLevel(int)"/>
        /// to lower the level, which will also perform other ReduceLevel logic. Setting it to false
        /// will just lower <see cref="CurrentLevel"/> to <paramref name="newMaxLevel"/> and do nothing else.<br/><br/>
        /// This will only be used if <see cref="CurrentLevel"/> is above <paramref name="newMaxLevel"/></param>
        public virtual void SetMaxLevel(int newMaxLevel, bool useLevelingLogic = true)
        {
            if (newMaxLevel == MaxLevel) return;
            if (CurrentLevel > newMaxLevel)
                SetLevel(newMaxLevel, useLevelingLogic);

            MaxLevel = newMaxLevel;
        }

        /// <summary>
        /// Set the level of this <see cref="RPGStat"/>. Will use <see cref="RaiseLevel(int)"/>
        /// or <see cref="ReduceLevel(int)"/> to match the <paramref name="newLevel"/>
        /// </summary>
        /// <param name="newLevel">New value to set <see cref="CurrentLevel"/> to</param>
        /// <param name="useLevelingLogic">If set to true this method will use <see cref="RaiseLevel(int)"/>
        /// or <see cref="ReduceLevel(int)"/> to modify the level. If set to false it will
        /// just set <see cref="CurrentLevel"/> and do nothing else</param>
        public virtual void SetLevel(int newLevel, bool useLevelingLogic = true)
        {
            if (!useLevelingLogic)
            {
                CurrentLevel = newLevel;
                return;
            }

            if (CurrentLevel < newLevel)
                RaiseLevel(newLevel - CurrentLevel);

            if (CurrentLevel > newLevel)
                ReduceLevel(CurrentLevel - newLevel);
        }

        /// <summary>
        /// Raise the level of this <see cref="RPGStat"/>
        /// </summary>
        /// <param name="numLevels">Number of levels to raise by. If greater than 1 it
        /// raises by 1 level at a time</param>
        public virtual void RaiseLevel(int numLevels = 1)
        {
            while (numLevels > 0 && CurrentLevel < MaxLevel)
            {
                CurrentLevel++;
                CurrentExp = 0;
                OnLevelRaised?.InvokeAll();
                numLevels--;
            }
        }

        /// <summary>
        /// Reduce the level of this <see cref="RPGStat"/>
        /// </summary>
        /// <param name="numLevels">Number of levels to reduce by. If greater than 1 it
        /// reduces by 1 level at a time</param>
        public virtual void ReduceLevel(int numLevels = 1)
        {
            while (numLevels > 0 && CurrentLevel > MinLevel)
            {
                CurrentLevel--;
                CurrentExp = 0;
                OnLevelReduced?.InvokeAll();
                numLevels--;
            }
        }
    }
}
