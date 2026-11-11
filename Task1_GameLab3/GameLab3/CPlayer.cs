using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace GameLab3
{
    public class CPlayer
    {
        private bool canClick;
        private double timeBeforeClick; 
        private CCountdownTimer countdownTimer;

        public CPlayer(double timeBeforeClick)
        {
            this.timeBeforeClick = timeBeforeClick;
            this.countdownTimer = new CCountdownTimer(timeBeforeClick);
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
    }
}
