using System;
using System.Collections.Generic;
using System.Text;

namespace GameLab2
{
    public class Player
    {
        private int lvl;
        private BigNumber gold;
        private BigNumber damage;
        private double damageModifier;
        private BigNumber upgradeCost;
        private double upgradeModifier;

        private bool canClick;
        public double timeBeforeClick = 3;
        private CCountdownTimer countdownTimer;

        public int Lvl { get => lvl; private set => lvl = value; }
        public BigNumber Gold { get => gold; private set => gold = value; }
        public BigNumber Damage { get => damage; private set => damage = value; }
        public double DamageModifier { get => damageModifier; private set => damageModifier = value; }
        public BigNumber UpgradeCost { get => upgradeCost; private set => upgradeCost = value; }
        public double UpgradeModifier { get => upgradeModifier; private set => upgradeModifier = value; }

        public Player()
        {
            lvl = 1;
            gold = new BigNumber("0");
            damage = new BigNumber("5");           
            upgradeCost = new BigNumber("10");     
            damageModifier = 1.5;                  
            upgradeModifier = 1.2;
            countdownTimer = new CCountdownTimer(timeBeforeClick);
            RecalculateStats();
        }

        public void AddGold(BigNumber amount)
        {
            if (amount != null)
            {
                gold = gold + amount; 
            }
        }

        public bool TryUpgrade()
        {
            if (TrySpendGold(upgradeCost))
            {
                lvl++;
                RecalculateStats();
                return true;
            }
            return false;
        }

        public BigNumber DealDamage()
        {
            return CalculateTotalDamage();
        }

        private void RecalculateStats()
        {
            damage = CalculateTotalDamage();
            upgradeCost = CalculateNextUpgradeCost();
        }

        private BigNumber CalculateNextUpgradeCost()
        {
            
            BigNumber currentCost = new BigNumber("10");

            for (int i = 1; i < lvl; i++)
            {
                double currentMultiplier = upgradeModifier * i;
                currentCost = currentCost * currentMultiplier; 
            }
            return currentCost;
        }

        private BigNumber CalculateTotalDamage()
        {
            BigNumber baseDamage = new BigNumber("5");

            for (int i = 1; i < lvl; i++)
            {
                baseDamage = baseDamage * damageModifier; 
            }
            return baseDamage;
        }

        private bool TrySpendGold(BigNumber amount)
        {
           
            if (amount > gold)
            {
                return false;
            }

            gold = gold - amount;
            return true;
        }

        public void update(double delta)
        {
            countdownTimer.update(delta);
            canClick = countdownTimer.isReady();
        }

        public bool tryClick()
        {
            if (!canClick) return false;

            countdownTimer.reset();
            return true;
        }

        public void increaseSpeed(double speedModifier)
        {
            timeBeforeClick = Math.Max(0.2, timeBeforeClick - speedModifier);
            countdownTimer.setTargetTime(timeBeforeClick);
        }

        public void increaseGold()
        {
            AddGold(new BigNumber("100"));
        }

        public void LoadLevelAndStats(int savedLevel)
        {
            if (savedLevel > 0)
            {
                this.lvl = savedLevel;
                RecalculateStats();
            }
        }
    }
}

