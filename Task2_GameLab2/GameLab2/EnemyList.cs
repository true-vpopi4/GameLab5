using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GameLab2
{
    class EnemyList
    {
        private List<JsonElement> templates = new List<JsonElement>();
        private Random rng = new Random();

        public void LoadFromJson(string path)
        {
            if (!File.Exists(path)) return;

            try
            {
                string jsonFromFile = File.ReadAllText(path);
                templates.Clear();

                using (JsonDocument doc = JsonDocument.Parse(jsonFromFile))
                {
                    foreach (JsonElement element in doc.RootElement.EnumerateArray())
                    {
                        templates.Add(element.Clone());
                    }
                }
            }
            catch { }
        }

        public IEnemy GetNextEnemy(int defeatedCount)
        {
            if (templates.Count == 0)
            {
                return new CNormalEnemy("Заглушка", new BigNumber("100"), new BigNumber("10"), "default.png");
            }

            JsonElement element = templates[rng.Next(templates.Count)];

            string templateType = element.GetProperty("$type").GetString();
            string name = element.GetProperty("Name").GetString();
            string iconName = element.GetProperty("IconName").GetString();
            int baseLife = element.GetProperty("BaseLife").GetInt32();
            double lifeModifier = element.GetProperty("LifeModifier").GetDouble();
            int baseGold = element.GetProperty("BaseGold").GetInt32();
            double goldModifier = element.GetProperty("GoldModifier").GetDouble();

            BigNumber hpValue = new BigNumber(baseLife.ToString());
            BigNumber goldValue = new BigNumber(baseGold.ToString());

            for (int k = 0; k < defeatedCount; k++)
            {
                hpValue = hpValue * (1.0 + lifeModifier);
                goldValue = goldValue * (1.0 + goldModifier);
            }

            string gameClass = templateType.Replace("Template", "");

            if (gameClass == "CArmoredEnemy")
            {
                double armor = element.GetProperty("Armor").GetDouble();
                return EnemyFactory.CreateEnemy(gameClass, name, hpValue, goldValue, iconName, armor);
            }
            else if (gameClass == "CHealingEnemy")
            {
                double healChance = element.GetProperty("HealChance").GetDouble();
                return EnemyFactory.CreateEnemy(gameClass, name, hpValue, goldValue, iconName, healChance);
            }

            return EnemyFactory.CreateEnemy(gameClass, name, hpValue, goldValue, iconName);
        }
    }
}
