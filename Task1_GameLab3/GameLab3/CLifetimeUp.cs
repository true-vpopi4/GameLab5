using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameLab3
{
    internal class CLifetimeUp : CCollectable
    {
        private double speedModifier = 0.1;

        public CLifetimeUp(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Red;
        }

        public override bool onClick(CPlayer player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            controller.lifetimeIncrease(speedModifier);
            return true;
        }
    }
}
