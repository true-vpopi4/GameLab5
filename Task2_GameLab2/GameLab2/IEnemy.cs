using System;
using System.Collections.Generic;
using System.Text;

namespace GameLab2
{
    public interface IEnemy
    {
        string Name { get; }
        BigNumber MaxHitPoints { get; }
        BigNumber CurrentHitPoints { get; }
        BigNumber GoldReward { get; }
        bool IsDead { get; }
        string Icon { get; }
        void TakeDamage(BigNumber dmg);
    }

    public abstract class CEnemy : IEnemy
    {
        public string Name { get; private set; }
        public BigNumber MaxHitPoints { get; private set; }
        public BigNumber CurrentHitPoints { get; protected set; }
        public BigNumber GoldReward { get; private set; }
        public bool IsDead => !(CurrentHitPoints > new BigNumber("0"));
        public string Icon { get; private set; }

        protected CEnemy(string name, BigNumber maxHitPoints, BigNumber goldReward, string icon)
        {
            Name = name;
            MaxHitPoints = maxHitPoints;
            CurrentHitPoints = maxHitPoints.Clone();
            GoldReward = goldReward;
            Icon = icon;
        }

        public virtual void TakeDamage(BigNumber dmg)
        {
            if (IsDead) return;

            if (!(CurrentHitPoints > dmg))
            {
                CurrentHitPoints = new BigNumber("0");
            }
            else
            {
                CurrentHitPoints = CurrentHitPoints - dmg;
            }
        }
    }

    public class CNormalEnemy : CEnemy
    {
        public CNormalEnemy(string name, BigNumber maxHitPoints, BigNumber goldReward, string icon)
            : base(name, maxHitPoints, goldReward, icon) { }
    }

    public class CArmoredEnemy : CEnemy
    {
        private readonly BigNumber _armor;

        public CArmoredEnemy(string name, BigNumber maxHitPoints, BigNumber goldReward, string icon, double armorValue)
            : base(name, maxHitPoints, goldReward, icon)
        {
            _armor = new BigNumber(((int)armorValue).ToString());
        }

        public override void TakeDamage(BigNumber dmg)
        {
            if (IsDead) return;

            BigNumber finalDamage = new BigNumber("1");
            if (dmg > _armor)
            {
                finalDamage = dmg - _armor;
            }

            if (!(CurrentHitPoints > finalDamage))
            {
                CurrentHitPoints = new BigNumber("0");
            }
            else
            {
                CurrentHitPoints = CurrentHitPoints - finalDamage;
            }
        }
    }

    public class CHealingEnemy : CEnemy
    {
        private readonly double _healChance;
        private readonly Random _rng = new Random();

        public CHealingEnemy(string name, BigNumber maxHitPoints, BigNumber goldReward, string icon, double healChance)
            : base(name, maxHitPoints, goldReward, icon)
        {
            _healChance = healChance;
        }

        public override void TakeDamage(BigNumber dmg)
        {
            if (IsDead) return;

            // Срабатывание шанса исцеления вместо получения урона
            if (_rng.NextDouble() < _healChance)
            {

                BigNumber healAmount = MaxHitPoints / 6.66;
                BigNumber healedHP = CurrentHitPoints + healAmount;

                if (healedHP > MaxHitPoints)
                    CurrentHitPoints = MaxHitPoints.Clone();
                else
                    CurrentHitPoints = healedHP;
            }
            else
            {
                if (!(CurrentHitPoints > dmg))
                    CurrentHitPoints = new BigNumber("0");
                else
                    CurrentHitPoints = CurrentHitPoints - dmg;
            }
        }
    }
}
