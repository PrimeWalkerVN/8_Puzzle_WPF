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
    }
}
