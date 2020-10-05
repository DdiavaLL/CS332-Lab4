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
        private bool isLine, isPolygon, isDot, isAroundCenter, isAroundDot, isDraw, isScaleAroundCenter;    //переменные для событий
        private PointF startPoint, endPoint, rotatePoint = PointF.Empty;     //отрисовка линий
        private PointF mainPoint, rotatePointt = new PointF(0, 0);
        private PointF minPolyPoint, maxPolyPoint, minEdgePoint, maxEdgePoint;
        private int countrotate = 0;

        private PointF[] edge = new PointF[2];
        private PointF[] polygon = new PointF[0];
        private PointF[] dot = new PointF[0];

        private int offsetX = 0, offsetY = 0;   //для поворотов и перемещений
        double rotateAngle = 0, scaleX, scaleY;

        PointF intersection = new PointF(-13, -13);

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            offsetX = (int)numericUpDown1.Value;
            offsetY = (int)numericUpDown2.Value;
            rotateAngle = (double)numericUpDown3.Value;
            isAroundCenter = checkBox1.Checked;
            isScaleAroundCenter = checkBox2.Checked;
            scaleX = (double)numericUpDown4.Value / 100d;
            scaleY = (double)numericUpDown5.Value / 100d;

            if (isLine)
            {
                for (int i = 0; i < edge.Length; i++)
                {
                    Translate(ref edge[i]);
                    Rotate(ref edge[i], 0, 0);
                }
            }
            else if (isPolygon)
            {
                for (int i = 0; i < polygon.Length; i++)
                {
                    Translate(ref polygon[i]);
                    Scale(ref polygon[i]);
                    var angle = (rotateAngle / 180 * Math.PI);                  
                    if (!isAroundCenter )
                    {
                        //var pointA = -mainPoint.X * Math.Cos(angle) + mainPoint.Y * Math.Sin(angle) + mainPoint.X;
                        //var pointB = -mainPoint.X * Math.Sin(angle) - mainPoint.Y * Math.Cos(angle) + mainPoint.Y;
                        var pointA = -rotatePointt.X * Math.Cos(angle) + rotatePointt.Y * Math.Sin(angle) + rotatePointt.X;
                        var pointB = -rotatePointt.X * Math.Sin(angle) - rotatePointt.Y * Math.Cos(angle) + rotatePointt.Y;
                        Rotate(ref polygon[i], pointA, pointB);
                    }
                    else
                    {
                        if (countrotate < 1)
                        {
                            rotatePoint = ariphHelp();
                            var pointA = -rotatePoint.X * Math.Cos(angle) + rotatePoint.Y * Math.Sin(angle) + rotatePoint.X;
                            var pointB = -rotatePoint.X * Math.Sin(angle) - rotatePoint.Y * Math.Cos(angle) + rotatePoint.Y;
                            countrotate = 1;
                            Rotate(ref polygon[i], pointA, pointB);
                        }
                        else
                        {
                            var pointA = -rotatePoint.X * Math.Cos(angle) + rotatePoint.Y * Math.Sin(angle) + rotatePoint.X;
                            var pointB = -rotatePoint.X * Math.Sin(angle) - rotatePoint.Y * Math.Cos(angle) + rotatePoint.Y;
                            Rotate(ref polygon[i], pointA, pointB);
                        }
                    }
                }
            }

            startPoint = edge[0];
            endPoint = edge[1];
            pictureBox1.Invalidate();
        }

        //Смещение
        private void Translate(ref PointF Point)
        {
            double[] offsetVector = new double[3] { Point.X, Point.Y, 1 };
            double[,] Matrix = new double[3, 3];

            double[] resultVector = new double[3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                        Matrix[i, j] = 1;
                    else if (i == 0 && j == 2)
                        Matrix[i, j] = offsetX;
                    else if (i == 1 && j == 2)
                        Matrix[i, j] = offsetY;
                    else
                        Matrix[i, j] = 0;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    resultVector[i] += Matrix[i, j] * offsetVector[j];
            }

            Point.X = (float)resultVector[0];
            Point.Y = (float)resultVector[1];
        }

        //Поворот
        private void Rotate(ref PointF Point, double help1, double help2)
        {
            var angle = (rotateAngle / 180 * Math.PI);

            if (isLine)
            {
                if (isAroundCenter)
                {
                    var rotatePoint = new PointF((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
                    help1 = -rotatePoint.X * (Math.Cos(angle) - 1) + rotatePoint.Y * Math.Sin(angle);
                    help2 = -rotatePoint.X * Math.Sin(angle) - rotatePoint.Y * (Math.Cos(angle) - 1);
                }
                else
                {
                    var pointA = -mainPoint.X * Math.Cos(angle) + mainPoint.Y * Math.Sin(angle) + mainPoint.X;
                    var pointB = -mainPoint.X * Math.Sin(angle) - mainPoint.Y * Math.Cos(angle) + mainPoint.Y;
                }
            }

            double[] offsetVector = new double[3] { Point.X, Point.Y, 1 };
            double[,] Matrix = new double[3, 3];
            double[] resultVector = new double[3];

            Matrix[0, 0] = Math.Cos(angle);
            Matrix[0, 1] = -Math.Sin(angle);
            Matrix[0, 2] = help1;
            Matrix[1, 0] = Math.Sin(angle);
            Matrix[1, 1] = Math.Cos(angle);
            Matrix[1, 2] = help2;
            Matrix[2, 0] = 0;
            Matrix[2, 1] = 0;
            Matrix[2, 2] = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    resultVector[i] += offsetVector[j] * Matrix[i, j];
            }


            Point.X = (float)resultVector[0];
            Point.Y = (float)resultVector[1];
        }

        private void button7_Click(object sender, EventArgs e)
        {
            isAroundDot = false;
            isDraw = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (edge.Length > 4)
            {
                int n = edge.Length - 3;
                intersection = Intersection(edge[n - 3], edge[n - 2], edge[n - 1], edge[n]);
            }
            else
                intersection = new PointF(-13, -13);

            if (intersection.X == -13 && intersection.Y == -13)
                label8.Text = "Не сущ-ет";
            else
            {
                label8.Text = string.Format("X: {0:N2} Y: {1:N2}", intersection.X, intersection.Y);
                pictureBox1.Invalidate();
            }
        }

        private PointF Intersection(PointF p0, PointF p1, PointF p2, PointF p3)
        {
            PointF i = new PointF(-1, -1);
            PointF s1 = new PointF();
            PointF s2 = new PointF();
            s1.X = p1.X - p0.X;
            s1.Y = p1.Y - p0.Y;
            s2.X = p3.X - p2.X;
            s2.Y = p3.Y - p2.Y;
            float s, t;
            s = (-s1.Y * (p0.X - p2.X) + s1.X * (p0.Y - p2.Y)) / (-s2.X * s1.Y + s1.X * s2.Y);
            t = (s2.X * (p0.Y - p2.Y) - s2.Y * (p0.X - p2.X)) / (-s2.X * s1.Y + s1.X * s2.Y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                i.X = p0.X + (t * s1.X);
                i.Y = p0.Y + (t * s1.Y);

            }
            if (i == p0)
                i = new PointF(-1, -1);
            return i;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            isAroundDot = true;
            isDraw = false;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            rotatePointt = e.Location;
        }

        private PointF ariphHelp()
        {
            float allX = 0, allY = 0;
            foreach (var p in polygon)
            {
                allX += p.X;
                allY += p.Y;
            }
            return new PointF(allX / (polygon.Length), allY / (polygon.Length));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isDraw)
            {
                isDot = false;
                isLine = false;
                isPolygon = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isDraw)
            {
                isDot = false;
                isLine = true;
                isPolygon = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (isDraw)
            {
                isDot = true;
                isLine = false;
                isPolygon = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDraw)
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
                    else if (isDot)
                    {
                        startPoint = e.Location;
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDraw)
                {
                    if (isLine)
                    {
                        edge[edge.Length - 2] = new PointF(startPoint.X, startPoint.Y);
                        edge[edge.Length - 1] = new PointF(endPoint.X, endPoint.Y);
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
                    else if (isDot)
                    {
                        Array.Resize(ref dot, dot.Length + 1);
                        dot[dot.Length - 1] = startPoint;
                    }
                }             
            }
            else
            {
                mainPoint.X = e.Location.X;
                mainPoint.Y = e.Location.Y;
            }
            rotatePointt = e.Location;

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
                if (isDraw)
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
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < edge.Length; i += 2)
            {
                e.Graphics.DrawLine(Pens.Red, edge[i], edge[i + 1]);
            }

            Brush aBrush = (Brush)Brushes.Red;
            for (int i = 0; i < dot.Length; i ++)
            {
                e.Graphics.FillRectangle(aBrush, dot[i].X, dot[i].Y, 1, 1);
            }

            if (polygon.Length > 1)
                e.Graphics.DrawPolygon(Pens.Red, polygon);
            e.Graphics.DrawEllipse(Pens.Green, intersection.X - 2, intersection.Y - 2, 5, 5);
        }

        //Очистка picturebox'a.
        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            startPoint = endPoint = Point.Empty;
            Array.Clear(edge, 0, edge.Length);
            Array.Clear(polygon, 0, polygon.Length);
            Array.Clear(dot, 0, dot.Length);
            Array.Resize(ref dot, 0);
            Array.Resize(ref edge, 2);
            Array.Resize(ref polygon, 0);
            intersection = new PointF(-1, -1);

            pictureBox1.Invalidate();
            minEdgePoint = new Point(9999999, 9999999);
            maxEdgePoint = new Point(-1, -1);
        }

        private void Scale(ref PointF Point)
        {
            double[] offsetVector = new double[3] { Point.X, Point.Y, 1 };
            double[,] Matrix = new double[3, 3];
            double[] resultVector = new double[3];

            PointF scalePoint;
            if (isScaleAroundCenter)
            {
                scalePoint = new PointF((minPolyPoint.X + maxPolyPoint.X) / 2, (minPolyPoint.Y + maxPolyPoint.Y) / 2);
            }
            else
            {
                scalePoint = mainPoint;
            }

            Matrix[0, 0] = scaleX;
            Matrix[0, 1] = 0;
            Matrix[0, 2] = (1 - scaleX) * scalePoint.X;
            Matrix[1, 0] = 0;
            Matrix[1, 1] = scaleY;
            Matrix[1, 2] = (1 - scaleY) * scalePoint.Y;
            Matrix[2, 0] = 0;
            Matrix[2, 1] = 0;
            Matrix[2, 2] = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    resultVector[i] += Matrix[i, j] * offsetVector[j];
            }

            Point.X = (float)resultVector[0];
            Point.Y = (float)resultVector[1];
        }

    }
}
