using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameLab2
{
    public interface ISaveList<T>
    {
        T Load(string path);
        void Save(T data, string path);
    }

    public class PlayerProgressData
    {
        public int Level { get; set; }
        public string GoldString { get; set; }
        public int DefeatedCount { get; set; }
    }

    public class PlayerProgressSaver : ISaveList<PlayerProgressData>
    {
        public PlayerProgressData Load(string path)
        {
            if (!File.Exists(path)) return new PlayerProgressData { Level = 1, GoldString = "0", DefeatedCount = 0 };
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<PlayerProgressData>(json) ?? new PlayerProgressData();
        }

        public void Save(PlayerProgressData data, string path)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }

    public partial class MainWindow : Window
    {
        private CController controller;
        private DispatcherTimer timer;
        private Player _player;
        private EnemyList _enemyStorage;
        private int _defeatedCount = 0;

        private readonly ISaveList<PlayerProgressData> _progressSaver = new PlayerProgressSaver();

        string jsonPath = "C:/Users/lich/Desktop/labs/2 kyrs/code/lab4/New Lab1/GameLab4/GameLab4/bin/Debug/net10.0-windows/enemies.json";
        string progressPath = "player_progress.json";

        public MainWindow()
        {
            InitializeComponent();

            _player = new Player();
            _enemyStorage = new EnemyList();
            _enemyStorage.LoadFromJson(jsonPath);
            PlayerProgressData savedData = _progressSaver.Load(progressPath);

            _defeatedCount = savedData.DefeatedCount;

            _player.AddGold(new BigNumber(savedData.GoldString));

            _player.LoadLevelAndStats(savedData.Level);

            Size sceneSize = new Size(GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            if (sceneSize.Width == 0 || sceneSize.Height == 0) sceneSize = new Size(600, 400);

            controller = new CController(1.0, 0, sceneSize);

            controller.addObject += addObjectInScene;
            controller.removeObject += removeObjectFromScene;
            controller.enemyChanged += showEnemy;
            controller.enemyDamaged += showEnemyDamage;

            SpawnNewEnemy();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void addObjectInScene(object sender, CControllerEventArgs e)
        {
            GameCanvas.Children.Add(e.sprite);
        }

        public void removeObjectFromScene(object sender, CControllerEventArgs e)
        {
            GameCanvas.Children.Remove(e.sprite);
        }

        public void showEnemy(object sender, CEnemyEventArgs e)
        {
            IEnemy enemy = e.enemy;

            EnemyHitPoint.Text = enemy.CurrentHitPoints.ToString();
            EnemyName.Text = enemy.Name;
            GoldFromenemy.Text = enemy.GoldReward.ToString();

            if (!string.IsNullOrEmpty(enemy.Icon))
            {
                try
                {
                    string cleanPath = enemy.Icon.Replace("\"", "").Trim();

                    if (System.IO.File.Exists(cleanPath))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(cleanPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        EnemyImage.Source = bitmap;
                    }
                    else
                    {
                        EnemyName.Text = $"{enemy.Name} (Иконка отсутствует)";
                        EnemyImage.Source = null;
                    }
                }
                catch
                {
                    EnemyImage.Source = null;
                }
            }
        }

        public void showEnemyDamage(object sender, CEnemyEventArgs e)
        {
            EnemyHitPoint.Text = e.enemy.CurrentHitPoints.ToString();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            controller.update(0.1);
            _player.update(0.1);
        }

        private void SpawnNewEnemy()
        {
            controller.setEnemy(_enemyStorage.GetNextEnemy(_defeatedCount));
            UpdateUI();
        }

        private void DealDamage(object sender, RoutedEventArgs e)
        {
            IEnemy enemy = controller.CurrentEnemy;
            if (enemy == null || enemy.IsDead) return;
            if (!_player.tryClick()) return;

            BigNumber damage = _player.DealDamage();

            // Наносим урон через контроллер, он вызывает событие enemyDamaged
            controller.damageEnemy(damage);

            if (enemy.IsDead)
            {
                _player.AddGold(enemy.GoldReward);
                _defeatedCount++;

                _progressSaver.Save(new PlayerProgressData
                {
                    Level = _player.Lvl,
                    GoldString = _player.Gold.ToString(),
                    DefeatedCount = _defeatedCount
                }, progressPath);

                SpawnNewEnemy();
            }

            UpdateUI();
        }

        private void UpgradeClick(object sender, RoutedEventArgs e)
        {
            _player.TryUpgrade();

            _progressSaver.Save(new PlayerProgressData
            {
                Level = _player.Lvl,
                GoldString = _player.Gold.ToString(),
                DefeatedCount = _defeatedCount
            }, progressPath);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_player == null) return;

            PlayerDamage.Text = _player.Damage.ToString();
            PlayerGold.Text = _player.Gold.ToString();
            PlayerLevel.Text = _player.Lvl.ToString();
            UpgradeCost.Text = _player.UpgradeCost.ToString();
            PlayerCD.Text = _player.timeBeforeClick.ToString();
        }

        private void NextEnemy(object sender, RoutedEventArgs e) => SpawnNewEnemy();

        private void RepeatEnemy(object sender, RoutedEventArgs e)
        {
            if (controller.CurrentEnemy == null) return;
            SpawnNewEnemy();
        }

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(GameCanvas);
            controller.mouseClick(mousePosition, _player);
            UpdateUI();
        }
    }
}
