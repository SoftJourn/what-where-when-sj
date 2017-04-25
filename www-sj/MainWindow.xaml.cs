using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
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
            _roundNumber = 0;
            _timerTicksMax = 60;
        }

        #region Menu

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
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

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void spinningTopButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Spinning top?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (_question != null)
            {
                _question.Asked = true;
            }
            _question = _game.NextQuestion();
            if (_question == null)
            {
                AuthorTextBlock.Text = "GAME OVER";
                QuestionTextBlock.Text = "";
                AnswerTextBlock.Text = "";
                //questionImage.Source = null;
                return;
            }

            _roundNumber++;
            RoundNumberTextBlock.Text = _roundNumber.ToString("00");
            AuthorTextBlock.Text = $"{_question.OrdinalNumber}. {_question.Author}";
            QuestionTextBlock.Text = "";
            AnswerTextBlock.Text = "";
            //questionImage.Source = !string.IsNullOrEmpty(_question.Image) ? new BitmapImage(new Uri(_question.Image)) : null;
            InitTimer();
        }

        private void questionButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Show question?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (_question == null) return;
            QuestionTextBlock.Text = _question.Text;
            AnswerTextBlock.Text = "";
            //questionImage.Source = !string.IsNullOrEmpty(_question.Image) ? new BitmapImage(new Uri(_question.Image)) : null;
            InitTimer();
        }

        private void answerButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Show answer?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (_question == null) return;
            AnswerTextBlock.Text = _question.Answer;
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

        private static void PlaySound(string path)
        {
            var mediaPlayerBeep = new MediaPlayer();
            mediaPlayerBeep.Open(new Uri(path));
            mediaPlayerBeep.Play();
            //do
            //{
            //    Thread.Sleep(10);
            //} while (!mediaPlayerBeep.NaturalDuration.HasTimeSpan);
            //var duration = mediaPlayerBeep.NaturalDuration.TimeSpan;
            //Thread.Sleep(duration);
            //mediaPlayerBeep.Close();
        }
    }
}
