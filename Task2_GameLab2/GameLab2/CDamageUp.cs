using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameLab2
{
    internal class CDamageUp : CCollectable
    {
        private double speedModifier = 0.15;

        public CDamageUp(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Red;
        }

        public override bool onClick(Player player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            player.increaseGold();
            return true;
        }
    }
}
