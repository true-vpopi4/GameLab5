using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameLab2
{
    public delegate void SceneEvent(object sender, CControllerEventArgs e);

    public class CControllerEventArgs : EventArgs
    {
        public Ellipse sprite;

        public CControllerEventArgs(Ellipse sprite)
        {
            this.sprite = sprite;
        }
    }

    public class CController
    {
        public event SceneEvent addObject;
        public event SceneEvent removeObject;

        private List<CCollectable> objects;
        private double spawnRate;
        private double time;
        private Random rng;
        private double minLifetime;
        private double maxLifetime;
        private double minSpriteSize;
        private double maxSpriteSize;
        private Size sceneSize;
        private double points;

        public CController(double spawnRate, ulong startTime, Size sceneSize)
        {
            rng = new Random();
            objects = new List<CCollectable>();
            this.spawnRate = spawnRate;
            time = startTime;
            this.sceneSize = sceneSize;
            points = 0;
            minLifetime = 1;
            maxLifetime = 5;
            minSpriteSize = 10;
            maxSpriteSize = 20;
        }

        public double Points => points;
        public List<CCollectable> Objects => objects;

        public CCollectable spawnObject()
        {
            double randSize = rng.NextDouble() * (maxSpriteSize - minSpriteSize) + minSpriteSize;
            double randLifetime = rng.NextDouble() * (maxLifetime - minLifetime) + minLifetime;

            double randX = rng.NextDouble() * (sceneSize.Width - randSize);
            double randY = rng.NextDouble() * (sceneSize.Height - randSize);

            Point randPos = new Point(randX, randY);

            CCollectable newObj ;

            if (rng.Next(1,100) < 70)
            {
                newObj = new CDamageUp(randPos, randSize, randLifetime);
            }
            else
            {
                newObj = new CClickSpeedUp(randPos, randSize, randLifetime);
            }


            objects.Add(newObj);
            addObject?.Invoke(this, new CControllerEventArgs(newObj.getSprite()));
            return newObj;
        }

        private void destroyObject(CCollectable o)
        {
            removeObject?.Invoke(this, new CControllerEventArgs(o.getSprite()));
            objects.Remove(o);
        }

        public void update(double delta)
        {
            time += delta;
            if (time >= spawnRate)
            {
                time = 0;
                spawnObject();
            }

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].updateLifetime(delta))
                {
                    destroyObject(objects[i]);
                }
            }
        }

        public void mouseClick(Point mousePosition, Player player)
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].onClick(player, this, mousePosition) == true)
                {
                    destroyObject(objects[i]);
                    break;
                }
            }
        }

        public void pointsIncrease(double value)
        {
            this.points += value;
        }

        public void lifetimeIncrease(double value)
        {
            this.minLifetime *= value;
        }
    }
}
