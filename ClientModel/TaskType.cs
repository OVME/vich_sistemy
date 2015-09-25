using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientModel
{
    public enum TaskType
    {
        Default = 0,
        Factorial = 1,
        LifeGame = 2,
    }

    public static class TaskTypeExt
    {
        public static Dictionary<TaskType, string> strings = new Dictionary<TaskType, string>()
        {
            {TaskType.Factorial,"Факториал"},
            {TaskType.LifeGame, "Игра жизнь"},
            {TaskType.Default, "Ничего"}

        };
        public static string GetTaskName(this TaskType tt)
        {
            return strings[tt];
        }
    }
}
