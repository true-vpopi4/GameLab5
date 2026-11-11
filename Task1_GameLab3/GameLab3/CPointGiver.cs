using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace GameLab3
{
    internal class CPointGiver : CCollectable
    {
        double pointsValue;

        public CPointGiver(Point position, double size, double lifetime) : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.BlueViolet;
            pointsValue = ((1 / this.size.Width) / lifetime) * 1000;
        }

        public override bool onClick(CPlayer player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            controller.pointsIncrease(pointsValue);
            return true;
        }
    }
}
