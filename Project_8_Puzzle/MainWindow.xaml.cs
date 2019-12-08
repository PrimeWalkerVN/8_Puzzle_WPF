using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace Project_8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // global variable
        public string ImgSource;
        private int paddingLeft;
        private int paddingTop;

        public int ratioImage;
        public int imageWidth;
        public int imageHeight;


        private Random rng = new Random();

        public Image[,] images = new Image[3, 3];
        public List<int> listGame = new List<int>();
        public Tuple<int, int> whitePos;

        public Tuple<int, int> currentClickPos;

        //============Timer Area====================
        Timer _timer = new Timer(1000);
        public int CountTime;
        public string timerZone = "00:04:00";
        public string TimeZone
        {
            get => timerZone; set
            {
                timerZone = value;
                OnPropertyChanged("TimeZone");
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountTime--;
            TimeZone = TimeSpan.FromSeconds(CountTime).ToString();
            
            if (CountTime == 0)
            {
                _timer.Stop();
                MessageBox.Show("Time Out! You Lose");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
        //============ End Timer Area====================

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Binding clock 
            this.DataContext = this;
            _timer.Elapsed += Timer_Elapsed;
            CountTime = 240;
            TimeZone = TimeSpan.FromSeconds(CountTime).ToString();
            _timer.Enabled = true;

            //Play Game
            ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images/default.png";
            StartGame();
            geanerateRandomGame();
            whitePos = new Tuple<int, int>(2, 2);
            LoadImg(ImgSource,whitePos);
        }

        private void LoadImg(string imgSource, Tuple<int, int> whitePos)
        {
            var bitmap = new BitmapImage(new Uri(imgSource));
            var pixelWidth = bitmap.PixelWidth / 3;
            var pixelHeight = bitmap.PixelHeight / 3;
            paddingLeft = 0;
            paddingTop = 0;
            var rect = new Int32Rect();

            if (pixelWidth < pixelHeight)
            {
                ratioImage = pixelWidth;
                paddingTop = (bitmap.PixelHeight - bitmap.PixelWidth) / 2;
                rect.X = 0;
                rect.Y = paddingTop;
                rect.Width = bitmap.PixelWidth;
                rect.Height = bitmap.PixelHeight - 2 * paddingTop;
            }
            else
            {
                ratioImage = pixelHeight;
                paddingLeft = (bitmap.PixelWidth - bitmap.PixelHeight) / 2;
                rect.X = paddingLeft;
                rect.Y = 0;
                rect.Height = bitmap.PixelHeight;
                rect.Width = bitmap.PixelWidth - 2 * paddingLeft;
            }
            imageWidth = 120;
            imageHeight = 120;

            ResultImage.Source = new CroppedBitmap(bitmap, rect);

            var whitePosition = 8;
            if (whitePos.Item1 == 0)
                whitePosition = whitePos.Item2;
            if (whitePos.Item1 == 1)
                whitePosition = whitePos.Item2 + 3;
            if (whitePos.Item1 == 2)
                whitePosition = whitePos.Item2 + 6;


            for (int i = 0; i < 9; i++)
            {
                if (i != whitePosition)
                {
                    var index = listGame[i];
                    var index_i = index / 3;
                    var index_j = index % 3;
                    int axisY = (int)(index_i * ratioImage + paddingTop);
                    int axisX = (int)(index_j * ratioImage + paddingLeft);

                    var cropped = new CroppedBitmap(bitmap, new Int32Rect(axisX, axisY, ratioImage, ratioImage));

                    var imageView = new Image();

                    imageView.Source = cropped;
                    imageView.Width = imageWidth;
                    imageView.Height = imageHeight;
                    GameFrame.Children.Add(imageView);

                    Canvas.SetTop(imageView, (i / 3) * (imageHeight));
                    Canvas.SetLeft(imageView, (i % 3) * (imageWidth));

                    images[i / 3, i % 3] = imageView;
                }
                if (i == whitePosition)
                {
                    int axisX = 2 * ratioImage;
                    int axisY = 2 * ratioImage;

                    var imageView = new Image();

                    imageView.Width = imageWidth;
                    imageView.Height = imageHeight;
                    GameFrame.Children.Add(imageView);

                    Canvas.SetLeft(imageView, 2 * imageWidth);
                    Canvas.SetTop(imageView, 2 * imageHeight);

                    images[2, 2] = imageView;
                }
            }
        }

        private void geanerateRandomGame()
        {
            listGame = GenerateRandomGame();
            while (!CanSolvePuzzle(listGame))
            {
                listGame = GenerateRandomGame();
            }
        }
        private List<int> GenerateRandomGame()
        {
            List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            List<int> resList = new List<int>();
            var rd = new Random();

            while (list.Count > 0)
            {
                var num = rng.Next(list.Count);
                resList.Add(list[num]);
                list.Remove(list[num]);
            }
            resList.Add(8);
            return resList;
        }

        private bool CanSolvePuzzle(List<int> listGame)
        {
            int inv = 0;
            for (int i = 0; i < listGame.Count - 1; ++i)
            {
                for (int j = i + 1; j < listGame.Count; ++j)
                {
                    if (listGame[i] > listGame[j])
                        inv++;
                }
            }

            return inv % 2 == 0;
        }

        private void StartGame()
        {
            ClearGameFrame();
        }

        private void ClearGameFrame()
        {
            while (GameFrame.Children.Count > 0)
            {
                GameFrame.Children.RemoveAt(0);
            }
        }

        private void SwapImage(Tuple<int, int> src, Tuple<int, int> des)
        {
            var temp1 = listGame[src.Item1 * 3 + src.Item2];
            listGame[src.Item1 * 3 + src.Item2] = listGame[des.Item1 * 3 + des.Item2];
            listGame[des.Item1 * 3 + des.Item2] = temp1;

            Canvas.SetLeft(images[src.Item1, src.Item2], des.Item2 * (imageWidth));
            Canvas.SetTop(images[src.Item1, src.Item2], des.Item1 * (imageHeight));

            Canvas.SetLeft(images[des.Item1, des.Item2], src.Item2 * (imageWidth));
            Canvas.SetTop(images[des.Item1, des.Item2], src.Item1 * (imageHeight));

            var temp2 = images[src.Item1, src.Item2];
            images[src.Item1, src.Item2] = images[des.Item1, des.Item2];
            images[des.Item1, des.Item2] = temp2;
        }

        private void GameFinish()
        {
            DrawLast();
            MessageBox.Show("You WIN! Refresh Game.....\n");
            RefreshGame();
        }

        private void DrawLast()
        {
            var bitmap = new BitmapImage(new Uri(ImgSource));
            var cropped = new CroppedBitmap(bitmap, new Int32Rect(2 * ratioImage + paddingLeft, 2 * ratioImage + paddingTop, ratioImage, ratioImage));
            images[2, 2].Source = cropped;
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down) 
            {
                game_keyLEFT(2, 2);
            }
        }

        private void game_keyLEFT(int i, int j)
        {
            if (j + 1 > 2)
                return;

            var newPos = new Tuple<int, int>(i, j + 1);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            whitePos = newPos;

            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }

        private void game_keyRIGHT(int i, int j)
        {
            if (j - 1 < 0)
                return;

            var newPos = new Tuple<int, int>(i, j - 1);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            whitePos = newPos;

            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }
        private void game_keyUP(int i, int j)
        {
            if (i + 1 > 2)
                return;

            var newPos = new Tuple<int, int>(i + 1, j);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            whitePos = newPos;
             
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }
        private void game_keyDOWN(int i, int j)
        {
            if (i - 1 < 0)
                return;

            var newPos = new Tuple<int, int>(i - 1, j);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            whitePos = newPos;
             
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                game_keyLEFT(whitePos.Item1, whitePos.Item2);
            }
            if (e.Key == Key.Right)
            {
                game_keyRIGHT(whitePos.Item1, whitePos.Item2);
            }
            if (e.Key == Key.Up)
            {
                game_keyUP(whitePos.Item1, whitePos.Item2);
            }
            if (e.Key == Key.Down)
            {
                game_keyDOWN(whitePos.Item1, whitePos.Item2);
            }
        }


        private bool CheckWin(List<int> list)
        {
            for (int i = 0; i < 9; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }
            return true;
        }

        //drag variable
        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _whitePosition;
        private bool mouseDown = false;
        private Point mouseDownPos;
        private void GameFrame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            mouseDownPos = e.GetPosition(GameFrame);

            var position = e.GetPosition(GameFrame);
            int i = ((int)position.Y) / imageHeight;
            int j = ((int)position.X) / imageWidth;
            _selectedBitmap = images[i, j];
            _whitePosition = e.GetPosition(GameFrame);

            currentClickPos = new Tuple<int, int>(i, j);

        }

        private bool CheckMovable(int i, int j)
        {
            var whitePos_i = whitePos.Item1;
            var whitePos_j = whitePos.Item2;
            var distance = Math.Abs(i - whitePos_i) + Math.Abs(j - whitePos_j);
            if (distance == 1)
                return true;
            return false;
        }


       

        private void GameFrame_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(GameFrame);
            if ((position.X < 0 || position.X > 360 || position.Y > 360 || position.Y < 0) && mouseDown)
            {
                var tempwhitePos = mouseDownPos;

                int a = ((int)tempwhitePos.Y) / imageHeight;
                int b = ((int)tempwhitePos.X) / imageWidth;
                Tuple<int, int> temp = new Tuple<int, int>(a, b);
                SwapImage(temp, temp);
                _selectedBitmap = null;
                _isDragging = false;
                mouseDown = false;
            }
            else
            {
                if (mouseDown)
                {
                    bool check1 = mouseDownPos.X >= position.X - 2 && mouseDownPos.X <= position.X + 2;
                    bool check2 = mouseDownPos.Y >= position.Y - 2 && mouseDownPos.Y <= position.Y + 2;

                    if (!check1 || !check2)
                    {
                        _isDragging = true;
                    }
                }

                int i = ((int)position.Y) / imageHeight;
                int j = ((int)position.X) / imageWidth;

                if (_isDragging)
                {
                    var dx = position.X - _whitePosition.X;
                    var dy = position.Y - _whitePosition.Y;

                    var lastLeft = Canvas.GetLeft(_selectedBitmap);
                    var lastTop = Canvas.GetTop(_selectedBitmap);
                    Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                    Canvas.SetTop(_selectedBitmap, lastTop + dy);

                    _whitePosition = position;
                }
            }
        }


        private void GameFrame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var positioncheck = e.GetPosition(GameFrame);
            mouseDown = false;

                if (_isDragging == false)
                {
                    if (e.ClickCount < 2)
                    {
                        var pos = e.GetPosition(GameFrame);
                        var jpos = (int)(pos.X) / imageWidth;
                        var ipos = (int)(pos.Y) / imageHeight;
                        if (ipos >= 0 && ipos < 3 && jpos >= 0 && jpos < 3)
                        {
                            if (CheckMovable(ipos, jpos))
                            {
                                int i_empty = whitePos.Item1;
                                int j_empty = whitePos.Item2;
                                if (ipos == i_empty)
                                {
                                    if (jpos - j_empty == 1)
                                    {
                                        game_keyLEFT(i_empty, j_empty);
                                    }
                                    else if (jpos - j_empty == -1)
                                    {
                                        game_keyRIGHT(i_empty, j_empty);
                                    }
                                }
                                else if (jpos == j_empty)
                                {
                                    if (ipos - i_empty == 1)
                                    {
                                        game_keyUP(i_empty, j_empty);
                                    }
                                    else if (ipos - i_empty == -1)
                                    {
                                        game_keyDOWN(i_empty, j_empty);
                                    }
                                }
                                if (CheckWin(listGame))
                                {
                                    GameFinish();
                                }
                            }
                        }
                    }
                }
                else
                {

                    var jcheck = (int)(positioncheck.X) / imageWidth;
                    var icheck = (int)(positioncheck.Y) / imageHeight;
                    var check = new Tuple<int, int>(icheck, jcheck);
                    
                    var position = mouseDownPos;

                    int i = ((int)position.Y) / imageHeight;
                    int j = ((int)position.X) / imageWidth;

                    if (CheckMovable(i, j) && !currentClickPos.Equals(check) && check.Equals(whitePos))
                    {
                        SwapImage(new Tuple<int, int>(i, j), new Tuple<int, int>(whitePos.Item1, whitePos.Item2));
                        whitePos = new Tuple<int, int>(i, j);
                        if (CheckWin(listGame))
                        {
                            GameFinish();
                        }
                    }
                    else
                    {
                        Tuple<int, int> temp = new Tuple<int, int>(i, j);
                        SwapImage(temp, temp);
                    }
                    _isDragging = false;
                    _selectedBitmap = null;
                }
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                ImgSource = op.FileName;
                StartGame();
                geanerateRandomGame();
                whitePos = new Tuple<int, int>(2, 2);
                LoadImg(ImgSource,whitePos);
                MessageBox.Show("Load successful! Let's start\n");
                CountTime = 240;
                TimeZone = TimeSpan.FromSeconds(CountTime).ToString();
                return;
            }
            
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            open.Title = "Select Game saved";

            if (open.ShowDialog() == true)
            {
                var doc = new XmlDocument();
                doc.Load(open.FileName);

                var root = doc.DocumentElement;

                var img = root.ChildNodes[1];
                ImgSource = img.Attributes["Source"].Value;
                if (!File.Exists(ImgSource))
                {
                    MessageBox.Show("Not exists image, check again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _timer.Start();
                    return;
                }
                ratioImage = int.Parse(img.Attributes["Ratio"].Value);
                imageWidth = int.Parse(img.Attributes["Width"].Value);
                imageHeight = int.Parse(img.Attributes["Height"].Value);
                paddingLeft = int.Parse(img.Attributes["PaddingLeft"].Value);
                paddingTop = int.Parse(img.Attributes["PaddingTop"].Value);

                var time_counter = root.FirstChild.ChildNodes[0];

                CountTime = int.Parse(time_counter.Attributes["CountTime"].Value);

                var state = root.ChildNodes[2];
                var empty = state.Attributes["WhitePos"].Value;
                var tokens = empty.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                whitePos = new Tuple<int, int>(int.Parse(tokens[0]), int.Parse(tokens[1]));
                for (int i = 0; i < 3; i++)
                {
                    var line = state.ChildNodes[i].Attributes["Value"].Value;
                    tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    listGame[i * 3] = int.Parse(tokens[0]);
                    listGame[i * 3 + 1] = int.Parse(tokens[1]);
                    listGame[i * 3 + 2] = int.Parse(tokens[2]);
                }
                ClearGameFrame();
                LoadImg(ImgSource,whitePos);
                TimeZone = TimeSpan.FromSeconds(CountTime).ToString();
                _timer.Start();

                if (CheckWin(listGame))
                {
                    GameFinish();
                }
            }
            else _timer.Start();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (save.ShowDialog() == true)
            {
                XmlDocument doc = new XmlDocument();
                var root = doc.CreateElement("Game");

                var counter = doc.CreateElement("Counter");

                var time_counter = doc.CreateElement("TimeZone");
                time_counter.SetAttribute("CountTime", CountTime.ToString());

                counter.AppendChild(time_counter);
                root.AppendChild(counter);

                var img = doc.CreateElement("Image");
                img.SetAttribute("Source", ImgSource.ToString());
                img.SetAttribute("Ratio", ratioImage.ToString());
                img.SetAttribute("Width", imageWidth.ToString());
                img.SetAttribute("Height", imageHeight.ToString());
                img.SetAttribute("PaddingLeft", paddingLeft.ToString());
                img.SetAttribute("PaddingTop", paddingTop.ToString());
                root.AppendChild(img);

                var state = doc.CreateElement("State");
                state.SetAttribute("WhitePos", $"{whitePos.Item1} {whitePos.Item2}");

                for (int i = 0; i < 3; i++)
                {
                    var line = doc.CreateElement("Line");
                    line.SetAttribute("Value", $"{listGame[i * 3]} {listGame[i * 3 + 1]} {listGame[i * 3 + 2]}");
                    state.AppendChild(line);
                }
                root.AppendChild(state);

                doc.AppendChild(root);
                doc.Save(save.FileName);
                MessageBox.Show("Saved game!");
            }
            else MessageBox.Show("Error....Can not save!");
          _timer.Start();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshGame();
        }
        private void RefreshGame() {
            _timer.Stop();
            StartGame();
            geanerateRandomGame();
            whitePos = new Tuple<int, int>(2, 2);
            LoadImg(ImgSource,whitePos);
            CountTime = 240;
            TimeZone = TimeSpan.FromSeconds(CountTime).ToString();
            _timer.Start();
        }
    }
}
