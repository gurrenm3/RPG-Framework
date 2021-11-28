namespace RPG_Framework.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var poppingPower = RPGStatFactory.Create("Popping Power", maxLevel:50);
            poppingPower.onLevelRaised.Add(new Action(() => Console.WriteLine("You leveled up! Current level: " + poppingPower.CurrentLevel)));
            poppingPower.RaiseLevel(1);
            poppingPower.RaiseExp(1000);

            Console.WriteLine("======");
            Console.WriteLine("Exp before removing: " + poppingPower.CurrentExp);
            poppingPower.ReduceLevel(1);
            Console.WriteLine("Exp after removing: " + poppingPower.CurrentExp);
            Console.WriteLine("Stat level is: " + poppingPower.CurrentLevel);
        }
    }
}