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
        private IEnemy _currentEnemy; 
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

            SpawnNewEnemy();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;

            Size sceneSize = new Size(GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            if (sceneSize.Width == 0 || sceneSize.Height == 0) sceneSize = new Size(600, 400);

            controller = new CController(1.0, 0, sceneSize);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            controller.update(0.1, GameCanvas);
            _player.update(0.1);
        }

        private void SpawnNewEnemy()
        {
            _currentEnemy = _enemyStorage.GetNextEnemy(_defeatedCount);
            UpdateUI();
        }

        private void DealDamage(object sender, RoutedEventArgs e)
        {
            if (_currentEnemy == null || _currentEnemy.IsDead) return;
            if (!_player.tryClick()) return;

            BigNumber damage = _player.DealDamage();

            // Наносим урон через полиморфную логику
            _currentEnemy.TakeDamage(damage);

            if (_currentEnemy.IsDead)
            {
                _player.AddGold(_currentEnemy.GoldReward);
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
            if (_player == null || _currentEnemy == null) return;

            PlayerDamage.Text = _player.Damage.ToString();
            PlayerGold.Text = _player.Gold.ToString();
            PlayerLevel.Text = _player.Lvl.ToString();

            EnemyHitPoint.Text = _currentEnemy.CurrentHitPoints.ToString();
            EnemyName.Text = _currentEnemy.Name;
            GoldFromenemy.Text = _currentEnemy.GoldReward.ToString();
            UpgradeCost.Text = _player.UpgradeCost.ToString();
            PlayerCD.Text = _player.timeBeforeClick.ToString();

            if (!string.IsNullOrEmpty(_currentEnemy.Icon))
            {
                try
                {
                    string cleanPath = _currentEnemy.Icon.Replace("\"", "").Trim();

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
                        EnemyName.Text = $"{_currentEnemy.Name} (Иконка отсутствует)";
                        EnemyImage.Source = null;
                    }
                }
                catch
                {
                    EnemyImage.Source = null;
                }
            }
        }

        private void NextEnemy(object sender, RoutedEventArgs e) => SpawnNewEnemy();

        private void RepeatEnemy(object sender, RoutedEventArgs e)
        {
            if (_currentEnemy == null) return;
            SpawnNewEnemy();
        }

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(GameCanvas);
            controller.mouseClick(mousePosition, GameCanvas, _player);
            UpdateUI();
        }
    }
}
