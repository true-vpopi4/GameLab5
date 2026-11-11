using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameLab3
{
    internal class CObject
    {
        private Point position;
        private Size size;
        private double lifetime;
        private double pointsValue;
        private Ellipse sprite;

        public CObject(Point position, double size, double lifetime)
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
            pointsValue = ((1 / this.size.Width) / lifetime) * 1000;
        }
        public Ellipse getSprite() { return sprite; }
        public double getPointsValue() { return pointsValue; }

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
