using System.Collections.Generic;

namespace RPG_Framework
{
    /// <summary>
    /// Class that manages how much Exp is required for each level to level up
    /// </summary>
    public class ExpTable
    {
        /// <summary>
        /// How much Exp is required for each level in order to level up
        /// </summary>
        public List<double> ExpPerLevel { get; set; } = new List<double>();

        /// <summary>
        /// 
        /// </summary>
        public ExpTable()
        {
            
        }

        /// <summary>
        /// Check if a Stat can level up based on it's current level and current exp
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="currentExp"></param>
        /// <returns></returns>
        public virtual bool CanLevelUp(int currentLevel, double currentExp)
        {
            var expToLevelUp = GetLevelMaxExp(currentLevel);
            if (expToLevelUp == -1)
                return false;

            return currentExp >= expToLevelUp;
        }

        /// <summary>
        /// Returns how much Exp is still needed in order to level up
        /// </summary>
        /// <param name="currentLevel">Stat's current level</param>
        /// <param name="currentExp">Stat's current exp</param>
        /// <returns></returns>
        public virtual double GetRemainingExp(int currentLevel, double currentExp)
        {
            return GetLevelMaxExp(currentLevel) - currentExp;
        }

        /// <summary>
        /// Returns how much Exp is required for this level to Level Up
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <returns>Returns how much Exp is required for this level to Level Up. If it fails
        /// it will return -1</returns>
        public virtual double GetLevelMaxExp(int currentLevel)
        {
            try
            {
                if (ExpPerLevel.Count >= currentLevel)
                    return ExpPerLevel[currentLevel];
            }
            catch {  }

            return -1;
        }

        /// <summary>
        /// Creates a "Max Level Exp" list that is based off of the <paramref name="baseExp"/> and
        /// <paramref name="multiplier"/>. Can be used to quickly make an ExpTable that works up to
        /// <paramref name="levelsToMake"/>
        /// </summary>
        /// <param name="baseExp">How much Exp should Level 1 require to level up</param>
        /// <param name="multiplier">How much</param>
        /// <param name="levelsToMake"></param>
        /// <returns></returns>
        public static ExpTable CreateFromMultiplier(double baseExp, double multiplier, int levelsToMake)
        {
            List<double> expPerLevel = new List<double>();

            expPerLevel.Add(baseExp);
            double lastExp = baseExp;

            for (int i = 1; i < levelsToMake; i++)
            {
                double nextLevel = lastExp * multiplier;
                if (multiplier <= 1)
                    nextLevel += lastExp;

                expPerLevel.Add(nextLevel);
                lastExp = nextLevel;
            }

            var table = new ExpTable();
            table.ExpPerLevel = expPerLevel;

            return table;
        }
    }
}
