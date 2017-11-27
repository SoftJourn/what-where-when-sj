using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;
using www_sj.Extensions;
using www_sj.Models;

namespace www_sj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string AssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private DispatcherTimer _timer;
        private int _timerTicks;
        private int _timerTicksMax;
        private int _roundNumber;

        private Question[] _game;
        private Question _question;

        public MainWindow()
        {
            InitializeComponent();
            AuthorTextBlock.Text = "";
            QuestionTextBlock.Text = "";
            AnswerTextBlock.Text = "";
            Grid.SetColumnSpan(AnswerTextBlock, 2);
            QuestionImage.Source = null;
            AnswerImage.Source = null;
            _roundNumber = 0;
            _timerTicksMax = 60;
            OpenGame();
        }

        private void OpenGame()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "Files|*.json"
            };

            // Open the dialog box modally
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                _game = JsonConvert.DeserializeObject<Question[]>(File.ReadAllText(dlg.FileName));
                _roundNumber = 0;
            }
        }

        private void spinningTopButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Spinning top?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (_question != null)
            {
                _question.Asked = true;
            }
            Volchok();
            _question = _game.NextQuestion();
            if (_question == null)
            {
                AuthorTextBlock.Text = "GAME OVER";
                QuestionTextBlock.Text = "";
                AnswerTextBlock.Text = "";
                Grid.SetColumnSpan(AnswerTextBlock, 2);
                QuestionImage.Source = null;
                AnswerImage.Source = null;
                return;
            }

            _roundNumber++;
            RoundNumberTextBlock.Text = _roundNumber.ToString("00");
            AuthorTextBlock.Text = $"{_question.OrdinalNumber}. {_question.Author}";
            QuestionTextBlock.Text = "";
            AnswerTextBlock.Text = "";
            Grid.SetColumnSpan(AnswerTextBlock, 2);
            QuestionImage.Source = null;
            AnswerImage.Source = null;
            InitTimer();
        }

        private void questionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_question == null) return;
            QuestionTextBlock.Text = _question.Text;
            AnswerTextBlock.Text = "";
            if (!string.IsNullOrEmpty(_question.Image))
            {
                var imageUri = new Uri(_question.Image);
                QuestionImage.Source = new BitmapImage(imageUri);
                Grid.SetColumnSpan(QuestionTextBlock, 1);
            }
            else
            {
                Grid.SetColumnSpan(QuestionTextBlock, 2);
                QuestionImage.Source = null;
            }
            Grid.SetColumnSpan(AnswerTextBlock, 2);
            AnswerImage.Source = null;
            InitTimer();
        }

        private void answerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QuestionTextBlock.Text.Trim()))
            {
                MessageBox.Show("Question should go first!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Task.Run(() => Gong());
            var result = MessageBox.Show("Show answer?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (_question == null) return;
            AnswerTextBlock.Text = _question.Answer;
            if (!string.IsNullOrEmpty(_question.AnswerImage))
            {
                var imageUri = new Uri(_question.AnswerImage);
                AnswerImage.Source = new BitmapImage(imageUri);
                Grid.SetColumnSpan(AnswerTextBlock, 1);
            }
            else
            {
                Grid.SetColumnSpan(AnswerTextBlock, 2);
                AnswerImage.Source = null;
            }
        }

        private void startTimer60secButton_Click(object sender, RoutedEventArgs e)
        {
            InitTimer();
            _timerTicksMax = 60;
            Beep1();
            _timer.Start();
        }

        private void startTimer30secButton_Click(object sender, RoutedEventArgs e)
        {
            InitTimer();
            _timerTicksMax = 30;
            Beep1();
            _timer.Start();
        }

        private void InitTimer()
        {
            _timer?.Stop();
            TimerTextBlock.Text = "00";
            _timerTicks = 0;
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timerTicks++;
            TimerTextBlock.Text = _timerTicks.ToString("00");
            if (_timerTicks == _timerTicksMax - 10)
            {
                Beep1();
            }
            if (_timerTicks == _timerTicksMax)
            {
                _timer.Stop();
                Beep2();
            }
        }

        private static void Beep1()
        {
            var path = Path.Combine(AssemblyLocation, @"resources\mat1.mp3");
            PlaySound(path);
        }

        private static void Beep2()
        {
            var path = Path.Combine(AssemblyLocation, @"resources\mat2.mp3");
            PlaySound(path);
        }

        private static void Gong()
        {
            var path = Path.Combine(AssemblyLocation, @"resources\gong.mp3");
            PlaySound(path);
        }

        private static void Volchok()
        {
            var path = Path.Combine(AssemblyLocation, @"resources\volchok.mp3");
            PlaySound(path);
        }

        private static void PlaySound(string path)
        {
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(path));
            do
            {
                Thread.Sleep(10);
            } while (!mediaPlayer.NaturalDuration.HasTimeSpan);
            var duration = mediaPlayer.NaturalDuration.TimeSpan;
            mediaPlayer.Play();
            Thread.Sleep(duration);
            mediaPlayer.Close();
        }
    }
}
