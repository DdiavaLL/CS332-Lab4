using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS332_Lab4
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private bool isLine, isPolygon, isAroundCenter, isScaleAroundCenter;    //переменные для событий
        private PointF startPoint, endPoint = PointF.Empty;     //отрисовка линий
        private PointF mainPoint = new PointF(0, 0);
        private PointF minPolyPoint, maxPolyPoint, minEdgePoint, maxEdgePoint;

        private PointF[] edge = new PointF[2];
        private PointF[] polygon = new PointF[0];

        private int offsetX = 0, offsetY = 0;   //для поворотов и перемещений
        double rotateAngle = 0, scaleX, scaleY;

        PointF intersection = new PointF(-1, -1);

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isLine = false;
            isPolygon = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isLine)
                {
                    startPoint = e.Location;
                }
                else if (isPolygon && polygon.Length == 0)
                {
                    minPolyPoint = maxPolyPoint = e.Location;
                    startPoint = e.Location;
                    Array.Resize(ref polygon, 1);
                    polygon[0] = startPoint;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isLine)
                {
                    edge[0] = new PointF(startPoint.X, startPoint.Y);
                    edge[1] = new PointF(endPoint.X, endPoint.Y);
                    Array.Resize(ref edge, edge.Length + 2);

                    FindMin(ref minEdgePoint, endPoint);
                    FindMax(ref maxEdgePoint, endPoint);

                    FindMin(ref minEdgePoint, startPoint);
                    FindMax(ref maxEdgePoint, startPoint);                
                }
                else if (isPolygon)
                {
                    Array.Resize(ref polygon, polygon.Length + 1);

                    if (endPoint.X < minPolyPoint.X)
                    {
                        minPolyPoint.X = endPoint.X;
                    }
                    if (endPoint.Y < minPolyPoint.Y)
                    {
                        minPolyPoint.Y = endPoint.Y;
                    }
                    if (endPoint.X > maxPolyPoint.X)
                    {
                        maxPolyPoint.X = endPoint.X;
                    }
                    if (endPoint.Y > maxPolyPoint.Y)
                    {
                        maxPolyPoint.Y = endPoint.Y;
                    }

                    polygon[polygon.Length - 1] = endPoint;
                    startPoint = endPoint;
                }
            }
            else
            {
                mainPoint.X = e.Location.X;
                mainPoint.Y = e.Location.Y;
            }


            pictureBox1.Invalidate();
        }

        void FindMin(ref PointF myPoint, PointF point)
        {
            if (point.X < myPoint.X)
            {
                myPoint.X = point.X;
            }
            if (point.Y < myPoint.Y)
            {
                myPoint.Y = point.Y;
            }
        }

        void FindMax(ref PointF myPoint, PointF point)
        {
            if (point.X > myPoint.X)
            {
                myPoint.X = point.X;
            }
            if (point.Y > myPoint.Y)
            {
                myPoint.Y = point.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isLine)
                {
                    endPoint = e.Location;
                    pictureBox1.Invalidate();
                }
                else if (isPolygon)
                {
                    endPoint = e.Location;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < edge.Length; i += 2)
            {
                e.Graphics.DrawLine(Pens.Red, edge[i], edge[i + 1]);
            }

            if (polygon.Length > 1)
                e.Graphics.DrawPolygon(Pens.Red, polygon);

            e.Graphics.DrawLine(Pens.Red, startPoint, endPoint);
        }

        //Очистка picturebox'a.
        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            startPoint = endPoint = Point.Empty;
            Array.Clear(edge, 0, edge.Length);
            Array.Clear(polygon, 0, polygon.Length);
            Array.Resize(ref edge, 2);
            Array.Resize(ref polygon, 0);
            intersection = new PointF(-1, -1);

            pictureBox1.Invalidate();
            minEdgePoint = new Point(9999999, 9999999);
            maxEdgePoint = new Point(-1, -1);
        }
    }
}
