using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GameLab2
{
    public class CCountdownTimer
    {
        private double targetTime;
        private double currentTime;

        public CCountdownTimer(double targetTime)
        {
            this.targetTime = targetTime;
            this.currentTime = targetTime; 
        }

        public void setTargetTime(double time) { targetTime = time; }
        public double getTime() { return targetTime - currentTime; }

        public void update(double delta)
        {
            if (currentTime < targetTime)
                currentTime += delta;
        }

        public void reset() { currentTime = 0; } 
        public bool isReady() { return currentTime >= targetTime; } 
    }
}
