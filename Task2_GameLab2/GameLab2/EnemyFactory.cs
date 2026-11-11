using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameLab2
{
    public static class EnemyFactory
    {
        public static IEnemy CreateEnemy(string typeName, params object[] args)
        {
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName);

            if (type == null)
            {
                throw new TypeLoadException($"Класс '{typeName}' не найден в проекте кликера.");
            }

            return (IEnemy)Activator.CreateInstance(type, args);
        }
    }
}
