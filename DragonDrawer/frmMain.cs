using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragonDrawer
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// 胴体の角度
        /// </summary>
        private double m_currentAngle = 0;

        /// <summary>
        /// 胴体の各部位の座標
        /// </summary>
        private List<PointF> m_points = new List<PointF>();

        /// <summary>
        /// 胴体の各部位の角度
        /// </summary>
        private List<double> m_angles = new List<double>();

        /// <summary>
        /// ペンの太さ(最低限辰と認識できる大きさ)
        /// </summary>
        private int m_penSize = 5;

        /// <summary>
        /// 身体のメインカラー
        /// </summary>
        private Color m_bodyMain = Color.FromArgb(34, 177, 76);

        /// <summary>
        /// 身体の暗い所(背中等)
        /// </summary>
        private Color m_bodyDark = Color.FromArgb(24, 128, 55);

        /// <summary>
        /// 目口鼻
        /// </summary>
        private Color m_eyeAndMouthAndNose = Color.FromArgb(0, 0, 0);

        /// <summary>
        /// 角とひげ
        /// </summary>
        private Color m_antlersAndBeard = Color.FromArgb(239, 228, 176);

        /// <summary>
        /// 尻尾
        /// </summary>
        private Color m_Tail = Color.FromArgb(255, 0, 0);

        /// <summary>
        /// 空
        /// </summary>
        private Color m_sky = Color.LightSkyBlue;

        /// <summary>
        /// 太陽
        /// </summary>
        private Color m_sun = Color.Yellow;

        /// <summary>
        /// 雲
        /// </summary>
        private Color m_cloud = Color.White;

        /// <summary>
        /// 描画タスク
        /// </summary>
        private Thread m_moveTask;

        /// <summary>
        /// タスクの状態
        /// </summary>
        private bool m_taskMoving = false;

        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 読み込みイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 描画メインイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDraw_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_taskMoving == false)
                {
                    InitImage();

                    // 辰の描画
                    MakeDragonBody((int)(nudLevel.Value), picMain.Width);
                    DrawDragonBody(picMain.Image,m_points);
                    DrawDragonHead(picMain.Image, m_points.First());
                    DrawDragonTail(picMain.Image, m_points.Last());

                    MessageBox.Show("あけましておめでとうございます。");
                }
                else
                {
                    m_taskMoving = false;
                    MessageBox.Show("あけましておめでとうございます。一旦停止します。");
                }

            }
            catch (Exception _ex)
            {
                MessageBox.Show("あけましておめでとうございます。：エラーです。" + _ex.Message);
            }
        }

        /// <summary>
        /// 辰が流れるモード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnimation_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_taskMoving == false)
                {
                    InitImage();

                    // 先に座標を計算する
                    MakeDragonBody((int)(nudLevel.Value), picMain.Width);
                    m_taskMoving = true;
                    m_moveTask = new Thread(new ThreadStart(DragonAnimation));
                    m_moveTask.Start();
                }
                else
                {
                    m_taskMoving = false;
                    MessageBox.Show("あけましておめでとうございます。一旦停止します。");
                }

            }
            catch (Exception _ex)
            {
                MessageBox.Show("あけましておめでとうございます。：エラーです。" + _ex.Message);
            }
        }

        /// <summary>
        /// 辰を動かしながら描画する
        /// </summary>
        private void DragonAnimation()
        {
            try
            {
                int _startIndex = m_angles.Count - 1;

                while (m_taskMoving == true)
                {
                    int _currentStart = Math.Max(0, _startIndex);
                    int _currentEnd = Math.Min(m_angles.Count - 1, _startIndex + m_angles.Count / 4);

                    bool _drawHead = (_startIndex >= 0);
                    bool _drawTail = (_startIndex + m_angles.Count / 4 <= m_angles.Count - 1);

                    if ((_drawHead == true && m_angles[_currentStart] == 0) || (m_angles[_currentEnd] == 0))
                    {
                        Image _currentImage = new Bitmap(picMain.Width, picMain.Height);
                        List<PointF> _currentList = m_points.GetRange(_currentStart, _currentEnd - _currentStart + 1);
                        DrawDragonBody(_currentImage, _currentList);
                        if (_drawHead == true)
                        {
                            DrawDragonHead(_currentImage, _currentList.First());
                        }

                        if (_drawTail == true)
                        {
                            DrawDragonTail(_currentImage, _currentList.Last());
                        }

                        Invoke((MethodInvoker)(() =>
                        {
                            if (picMain.Image != null)
                            {
                                picMain.Image.Dispose();
                                picMain.Image = null;
                            }
                            picMain.Image = _currentImage;
                            Thread.Sleep(10);
                        }
                        ));
                    }
                    _startIndex--;

                    if (_currentEnd <= 0)
                    {
                        _startIndex = m_angles.Count - 1;
                    }
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show("あけましておめでとうございます。：エラーです。" + _ex.Message);
                m_taskMoving = false;
            }
        }

        /// <summary>
        /// 辰の顔を描画する
        /// </summary>
        /// <param name="image"></param>
        private void DrawDragonHead(Image image, PointF headPoint)
        {
            using (Graphics _gra = Graphics.FromImage(image))
            {
                _gra.FillRectangle(new SolidBrush(m_bodyMain), new RectangleF(headPoint, new SizeF(m_penSize, m_penSize)));
                _gra.FillRectangle(new SolidBrush(m_eyeAndMouthAndNose), new RectangleF(headPoint.X, headPoint.Y + 1, 1, 1));
                _gra.FillRectangle(new SolidBrush(m_eyeAndMouthAndNose), new RectangleF(headPoint.X + 2, headPoint.Y + 1, 2, 1));
                _gra.FillRectangle(new SolidBrush(m_eyeAndMouthAndNose), new RectangleF(headPoint.X, headPoint.Y + 3, 3, 1));
                _gra.FillRectangle(new SolidBrush(m_antlersAndBeard), new RectangleF(headPoint.X + 2, headPoint.Y, 3, 1));
                _gra.FillRectangle(new SolidBrush(m_antlersAndBeard), new RectangleF(headPoint.X + 4, headPoint.Y + 1, 1, 1));
                _gra.FillRectangle(new SolidBrush(m_antlersAndBeard), new RectangleF(headPoint.X, headPoint.Y + 4, 3, 1));
            }
        }

        /// <summary>
        /// 辰の尻尾を描画する
        /// </summary>
        /// <param name="image"></param>
        private void DrawDragonTail(Image image, PointF tailPoint)
        {
            using (Graphics _gra = Graphics.FromImage(image))
            {
                _gra.FillRectangle(new SolidBrush(m_Tail), new RectangleF(tailPoint.X - 5, tailPoint.Y, 5, 5));
            }
        }

        /// <summary>
        /// 辰の身体を描画する
        /// </summary>
        /// <param name="image"></param>
        private void DrawDragonBody(Image image,List<PointF> bodys)
        {
            if (1 < bodys.Count)
            {
                PointF[] pointsMain = bodys.ToArray();
                PointF[] pointsBack = bodys.ToArray();

                for (int _index = 0; _index < pointsMain.Length; _index++)
                {
                    pointsMain[_index].Y += 2;
                }
                using (Graphics _gra = Graphics.FromImage(image))
                {
                    _gra.DrawLines(new Pen(m_bodyMain, 5), pointsMain);
                    _gra.DrawLines(new Pen(m_bodyDark, 1), pointsBack);
                }
            }
        }

        /// <summary>
        /// 辰の身体の座標を求める
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="length"></param>
        private void MakeDragonBody(int currentLevel, double length)
        {
            if (currentLevel > 0)
            {
                //目標の深さに行くまで再帰します。
                MakeDragonBody(currentLevel - 1, length / 3);//一つ深いdepthを描きながら(この中にもさらに一つ深いdepthを4回描いている)
                Rotate(60);             //60
                MakeDragonBody(currentLevel - 1, length / 3);
                Rotate(-120);           //120
                MakeDragonBody(currentLevel - 1, length / 3);
                Rotate(60);             //60と折り返す
                MakeDragonBody(currentLevel - 1, length / 3);
            }
            else if (currentLevel == 0)
            {//目標の深さに到達したら座標が決まります
                PointF _newPoint = new PointF();

                _newPoint.X = m_points.Last().X + (float)(length * Math.Cos(m_currentAngle * Math.PI / 180.0));
                _newPoint.Y = m_points.Last().Y + (float)(length * Math.Sin(m_currentAngle * Math.PI / 180.0));

                m_points.Add(_newPoint);
                m_angles.Add(m_currentAngle);
            }
        }

        /// <summary>
        /// 背景を描画する
        /// </summary>
        /// <param name="image"></param>
        private void DrawBack(Image image)
        {
            using (Graphics _gra = Graphics.FromImage(image))
            {
                _gra.FillRectangle(new SolidBrush(m_sky), 0, 0, image.Width, image.Height);
                _gra.FillPie(new SolidBrush(m_sun), image.Width / 2 - image.Height / 2, image.Height / 2, image.Height, image.Height, 180, 360);
                _gra.FillPie(new SolidBrush(m_cloud), 0, image.Height / 2, image.Width / 5, image.Height / 10, 0, 360);
                _gra.FillPie(new SolidBrush(m_cloud), image.Width - image.Width / 4, image.Height / 4, image.Width / 5, image.Height / 8, 0, 360);
            }
        }

        /// <summary>
        /// 胴体を回転させる
        /// </summary>
        /// <param name="rotateAngle"></param>
        private void Rotate(double rotateAngle)
        {
            m_currentAngle = m_currentAngle + rotateAngle;
            if (m_currentAngle > 360)
            {
                m_currentAngle = m_currentAngle - 360;
            }
            else if (m_currentAngle < 0)
            {
                m_currentAngle = m_currentAngle + 360;
            }
        }

        /// <summary>
        /// 画像の初期化
        /// </summary>
        private void InitImage()
        {
            // 初期化
            if (picMain.Image != null)
            {
                picMain.Image.Dispose();
                picMain.Image = null;
            }
            picMain.Image = new Bitmap(picMain.Width, picMain.Height);

            // 初期化
            if (picMain.BackgroundImage != null)
            {
                picMain.BackgroundImage.Dispose();
                picMain.BackgroundImage = null;
            }
            picMain.BackgroundImage = new Bitmap(picMain.Width, picMain.Height);
            DrawBack(picMain.BackgroundImage);
            m_points = new List<PointF>();
            m_points.Add(new PointF(0, 0));
            m_angles = new List<double>();
            m_angles.Add(0);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(m_taskMoving == true) 
            {
                m_taskMoving = false;
                MessageBox.Show("あけましておめでとうございます。辰を停止させるので、もう一回終了を試行してください。");
                e.Cancel = true;
            }
        }
    }
}
