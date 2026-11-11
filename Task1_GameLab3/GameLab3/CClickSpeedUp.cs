using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameLab3
{
    internal class CClickSpeedUp : CCollectable
    {
        private double speedModifier = 0.15; 

        public CClickSpeedUp(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Gold;
        }

        public override bool onClick(CPlayer player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            player.increaseSpeed(speedModifier); 
            return true;
        }
    }
}
