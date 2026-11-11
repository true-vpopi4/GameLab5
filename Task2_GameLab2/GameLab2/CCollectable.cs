using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameLab2
{
    public abstract class CCollectable
    {
        Point position;
        protected Size size;
        double lifetime;
        protected Ellipse sprite;

        public CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = new Size(size, size);
            this.lifetime = lifetime;

            sprite = new Ellipse();
            sprite.Fill = Brushes.BlueViolet;
            sprite.StrokeThickness = 2;
            sprite.Stroke = Brushes.Black;
            sprite.HorizontalAlignment = HorizontalAlignment.Center;
            sprite.VerticalAlignment = VerticalAlignment.Center;
            sprite.Width = this.size.Width;
            sprite.Height = this.size.Height;
            sprite.RenderTransform = new TranslateTransform(position.X, position.Y);
        }

        public abstract bool onClick(Player player, CController controller, Point mousePosition);

        public Ellipse getSprite() { return sprite; }

        public bool updateLifetime(double delta)
        {
            lifetime -= delta;
            return lifetime <= 0;
        }

        public bool isMouseOnObject(Point mousePosition)
        {
            double distance = Math.Sqrt(Math.Pow(mousePosition.X - position.X, 2) +
                                        Math.Pow(mousePosition.Y - position.Y, 2));
            return distance <= (size.Width / 2);
        }
    }
}
