using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameLab3
{
    public delegate void SceneEvent(object sender, CControllerEventArgs e);

    public class CControllerEventArgs : EventArgs
    {
        public Ellipse sprite;
        public string msg = "";

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

            if (rng.Next(0, 100) < 50) 
            {
                newObj = new CPointGiver(randPos, randSize, randLifetime);
            }
            else if  (rng.Next(0, 100) < 60)
            {
                newObj = new CClickSpeedUp(randPos, randSize, randLifetime);
            }
            else
            {
                newObj = new CLifetimeUp(randPos, randSize, randLifetime);
            }
            
            objects.Add(newObj);

            // событие добавления объекта, а не canvas.Children.Add(...)
            addObject?.Invoke(this, new CControllerEventArgs(newObj.getSprite()));
            return newObj;
        }

        private void destroyObject(CCollectable o, string message)
        {
            CControllerEventArgs e = new CControllerEventArgs(o.getSprite());
            e.msg = message;
            removeObject?.Invoke(this, e);
            objects.Remove(o);
        }

        public void clear()
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                destroyObject(objects[i], "");
            }
        }

        public void restart(Size sceneSize)
        {
            clear();
            this.sceneSize = sceneSize;
            time = 0;
            points = 0;
            minLifetime = 1;
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
                    destroyObject(objects[i], "");
                }
            }
        }

        public void mouseClick(Point mousePosition, CPlayer player)
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].onClick(player, this, mousePosition) == true)
                {
                    destroyObject(objects[i], "");
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
