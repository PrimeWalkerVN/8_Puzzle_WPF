using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // global variable
        public string ImgSource;
        private string imgSource;
        private int paddingLeft;
        private int paddingTop;

        public int ratioImage;
        public int imageWidth;
        public int imageHeight;

        private Random rng = new Random();

        public Image[,] images = new Image[3, 3];
        public List<int> listGame = new List<int>();
        public Tuple<int, int> lastPos;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images/default.png";

            StartGame();
            geanerateRandomGame();
            LoadImg(ImgSource);
        }

        private void LoadImg(string imgSource)
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
            imageHeight = 120;// (pixelHeight * imageWidth / pixelWidth);

            ResultImage.Source = new CroppedBitmap(bitmap, rect);

            lastPos = new Tuple<int, int>(2, 2);
            for (int i = 0; i < 9; i++)
            {
                if (i != 8)
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
                if (i == 8)
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
            MessageBox.Show("You WIN!\n");
        }

        private void DrawLast()
        {
            var bitmap = new BitmapImage(new Uri(ImgSource));
            var cropped = new CroppedBitmap(bitmap, new Int32Rect(2 * ratioImage + paddingLeft, 2 * ratioImage + paddingTop, ratioImage, ratioImage));
            images[2, 2].Source = cropped;
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down) // The Arrow-Down key
            {
                game_keyLEFT(2,2);
            }
        }

        private void game_keyLEFT(int i, int j)
        {
            if (j + 1 > 2) return;
            var newPos = new Tuple<int, int>(i, j + 1);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            lastPos = newPos;
            //   Step++;
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }

        private void game_keyRIGHT(int i, int j)
        {
            if (j - 1 < 0) return;
            var newPos = new Tuple<int, int>(i, j - 1);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            lastPos = newPos;
            //Step++;
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }
        private void game_keyUP(int i, int j)
        {
            if (i + 1 > 2) return;
            var newPos = new Tuple<int, int>(i + 1, j);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            lastPos = newPos;
            //Step++;
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }
        private void game_keyDOWN(int i, int j)
        {
            if (i - 1 < 0) return;
            var newPos = new Tuple<int, int>(i - 1, j);
            SwapImage(new Tuple<int, int>(i, j), newPos);
            lastPos = newPos;
            //Step++;
            if (CheckWin(listGame))
            {
                GameFinish();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                game_keyLEFT(lastPos.Item1, lastPos.Item2);
            }
            if (e.Key == Key.Right)
            {
                game_keyRIGHT(lastPos.Item1, lastPos.Item2);
            }
            if (e.Key == Key.Up)
            {
                game_keyUP(lastPos.Item1, lastPos.Item2);
            }
            if (e.Key == Key.Down)
            {
                game_keyDOWN(lastPos.Item1, lastPos.Item2);
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

        private void GameFrame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ( e.ClickCount < 2)
            {
                var pos = e.GetPosition(GameFrame);
                var j = (int)(pos.X) / imageWidth;
                var i = (int)(pos.Y) / imageHeight;
                if (i >= 0 && i < 3 && j >= 0 && j < 3)
                {
                    if (CheckMovable(i, j))
                    {
                        int i_empty = lastPos.Item1;
                        int j_empty = lastPos.Item2;
                        if (i == i_empty)
                        {
                            if (j - j_empty == 1)
                            {
                                game_keyLEFT(i_empty, j_empty);
                            }
                            else if (j - j_empty == -1)
                            {
                                game_keyRIGHT(i_empty, j_empty);
                            }
                        }
                        else if (j == j_empty)
                        {
                            if (i - i_empty == 1)
                            {
                                game_keyUP(i_empty, j_empty);
                            }
                            else if (i - i_empty == -1)
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

        private bool CheckMovable(int i, int j)
        {
            var lastPos_i = lastPos.Item1;
            var lastPos_j = lastPos.Item2;
            var distance = Math.Abs(i - lastPos_i) + Math.Abs(j - lastPos_j);
            if (distance == 1)
                return true;
            return false;
        }
    }
}
