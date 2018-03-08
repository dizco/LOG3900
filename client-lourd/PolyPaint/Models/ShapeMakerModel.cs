using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using PolyPaint.CustomComponents;

namespace PolyPaint.Models
{
    public class ShapeMaker
    {
        public ShapeMaker(Point mouseStart, Point mouseEnd)
        {
            Start = mouseStart;
            End = mouseEnd;
            Width = End.X - Start.X;
            Height = End.Y - Start.Y;
        }

        public Point Start { get;}
        private Point End { get; }
        private double Width { get; }
        private double Height { get; }

        public Stroke DrawLine()
        {
            //We save the start and the end of the mouse gesture
            StylusPointCollection linePoints = new StylusPointCollection
            {
                new StylusPoint(Start.X, Start.Y),
                new StylusPoint(End.X, End.Y)
            };

            //With the Point variables we create a Stroke
            return new CustomStroke(linePoints);
        }

        public Stroke DrawArrow()
        {
            StylusPoint[] arrowEdges = new StylusPoint[8];

            arrowEdges[0] = new StylusPoint(Start.X, Start.Y + Height * 3 / 4);
            arrowEdges[1] = new StylusPoint(arrowEdges[0].X + Width / 2, arrowEdges[0].Y);
            arrowEdges[2] = new StylusPoint(arrowEdges[1].X, arrowEdges[1].Y + Height / 4);
            arrowEdges[3] = new StylusPoint(arrowEdges[2].X + Width / 2, arrowEdges[2].Y - Height / 2);
            arrowEdges[4] = new StylusPoint(arrowEdges[3].X - Width / 2, arrowEdges[3].Y - Height / 2);
            arrowEdges[5] = new StylusPoint(arrowEdges[4].X, arrowEdges[4].Y + Height / 4);
            arrowEdges[6] = new StylusPoint(arrowEdges[5].X - Width / 2, arrowEdges[5].Y);
            arrowEdges[7] = new StylusPoint(arrowEdges[6].X, arrowEdges[6].Y + Height / 2);

            //With the Point variables we create a Stroke
            StylusPointCollection arrowPoints = new StylusPointCollection(arrowEdges);
            return new CustomStroke(arrowPoints);
        }

        public Stroke DrawTriangle()
        {
            StylusPoint[] triangleEdges = new StylusPoint[4];

            triangleEdges[0] = new StylusPoint(Start.X, Start.Y);
            triangleEdges[1] = new StylusPoint(Start.X + Width, Start.Y);
            triangleEdges[2] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            triangleEdges[3] = new StylusPoint(Start.X, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection trianglePoints = new StylusPointCollection(triangleEdges);
            return new CustomStroke(trianglePoints);
        }

        public Stroke DrawDiamond()
        {
            StylusPoint[] diamondEdges = new StylusPoint[5];

            diamondEdges[0] = new StylusPoint(Start.X + Width / 2, Start.Y);
            diamondEdges[1] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            diamondEdges[2] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            diamondEdges[3] = new StylusPoint(Start.X, Start.Y + Height / 2);
            diamondEdges[4] = new StylusPoint(Start.X + Width / 2, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection diamondPoints = new StylusPointCollection(diamondEdges);
            return new CustomStroke(diamondPoints);
        }

        public Stroke DrawLightningBolt()
        {
            StylusPoint[] lightningBoltEdges = new StylusPoint[12];

            lightningBoltEdges[0] = new StylusPoint(Start.X, Start.Y + Height * 3 / 4);
            lightningBoltEdges[1] = new StylusPoint(Start.X + Width / 3, Start.Y + Height * 3 / 5);
            lightningBoltEdges[2] = new StylusPoint(Start.X + Width / 4, Start.Y + Height / 2);
            lightningBoltEdges[3] = new StylusPoint(Start.X + Width * 3 / 5, Start.Y + Height / 3);
            lightningBoltEdges[4] = new StylusPoint(Start.X + Width / 2, Start.Y + Height * 1 / 4);
            lightningBoltEdges[5] = new StylusPoint(Start.X + Width, Start.Y);
            lightningBoltEdges[6] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 3);
            lightningBoltEdges[7] = new StylusPoint(Start.X + Width * 3 / 4, Start.Y + Height * 2 / 5);
            lightningBoltEdges[8] = new StylusPoint(Start.X + Width / 2, Start.Y + Height * 2 / 3);
            lightningBoltEdges[9] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height * 7 / 10);
            lightningBoltEdges[10] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            lightningBoltEdges[11] = new StylusPoint(Start.X, Start.Y + Height * 3 / 4);

            //With the Point variables we create a Stroke
            StylusPointCollection lightningBoltPoints = new StylusPointCollection(lightningBoltEdges);
            return new CustomStroke(lightningBoltPoints);
        }

        public Stroke DrawITetromino()
        {
            StylusPoint[] iTetrominoEdges = new StylusPoint[17];

            iTetrominoEdges[0] = new StylusPoint(Start.X, Start.Y);
            iTetrominoEdges[1] = new StylusPoint(Start.X, Start.Y + Height);
            iTetrominoEdges[2] = new StylusPoint(Start.X + Width / 4, Start.Y + Height);
            iTetrominoEdges[3] = new StylusPoint(Start.X + Width / 4, Start.Y);
            iTetrominoEdges[4] = new StylusPoint(Start.X + Width / 2, Start.Y);
            iTetrominoEdges[5] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            iTetrominoEdges[6] = new StylusPoint(Start.X + Width * 3 / 4, Start.Y + Height);
            iTetrominoEdges[7] = new StylusPoint(Start.X + Width * 3 / 4, Start.Y);
            iTetrominoEdges[8] = new StylusPoint(Start.X + Width, Start.Y);
            iTetrominoEdges[9] = new StylusPoint(Start.X + Width, Start.Y + Height);
            iTetrominoEdges[10] = new StylusPoint(Start.X + Width * 3 / 4, Start.Y + Height);
            iTetrominoEdges[11] = new StylusPoint(Start.X + Width * 3 / 4, Start.Y);
            iTetrominoEdges[12] = new StylusPoint(Start.X + Width / 2, Start.Y);
            iTetrominoEdges[13] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            iTetrominoEdges[14] = new StylusPoint(Start.X + Width / 4, Start.Y + Height);
            iTetrominoEdges[15] = new StylusPoint(Start.X + Width / 4, Start.Y);
            iTetrominoEdges[16] = new StylusPoint(Start.X, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection iTetrominoPoints = new StylusPointCollection(iTetrominoEdges);
            return new CustomStroke(iTetrominoPoints);
        }

        public Stroke DrawOTetromino()
        {
            StylusPoint[] oTetrominoEdges = new StylusPoint[16];

            oTetrominoEdges[0] = new StylusPoint(Start.X + Width / 2, Start.Y + Height / 2);
            oTetrominoEdges[1] = new StylusPoint(Start.X + Width / 2, Start.Y);
            oTetrominoEdges[2] = new StylusPoint(Start.X, Start.Y);
            oTetrominoEdges[3] = new StylusPoint(Start.X, Start.Y + Height / 2);
            oTetrominoEdges[4] = new StylusPoint(Start.X + Width / 2, Start.Y + Height / 2);
            oTetrominoEdges[5] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            oTetrominoEdges[6] = new StylusPoint(Start.X, Start.Y + Height);
            oTetrominoEdges[7] = new StylusPoint(Start.X, Start.Y + Height / 2);
            oTetrominoEdges[8] = new StylusPoint(Start.X + Width / 2, Start.Y + Height / 2);
            oTetrominoEdges[9] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            oTetrominoEdges[10] = new StylusPoint(Start.X + Width, Start.Y + Height);
            oTetrominoEdges[11] = new StylusPoint(Start.X + Width / 2, Start.Y + Height);
            oTetrominoEdges[12] = new StylusPoint(Start.X + Width / 2, Start.Y + Height / 2);
            oTetrominoEdges[13] = new StylusPoint(Start.X + Width / 2, Start.Y);
            oTetrominoEdges[14] = new StylusPoint(Start.X + Width, Start.Y);
            oTetrominoEdges[15] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);

            //With the Point variables we create a Stroke
            StylusPointCollection oTetrominoPoints = new StylusPointCollection(oTetrominoEdges);
            return new CustomStroke(oTetrominoPoints);
        }

        public Stroke DrawLTetromino()
        {
            StylusPoint[] lTetrominoEdges = new StylusPoint[17];

            lTetrominoEdges[0] = new StylusPoint(Start.X, Start.Y);
            lTetrominoEdges[1] = new StylusPoint(Start.X, Start.Y + Height / 2);
            lTetrominoEdges[2] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            lTetrominoEdges[3] = new StylusPoint(Start.X + Width / 3, Start.Y);
            lTetrominoEdges[4] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            lTetrominoEdges[5] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            lTetrominoEdges[6] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            lTetrominoEdges[7] = new StylusPoint(Start.X + Width, Start.Y + Height);
            lTetrominoEdges[8] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height);
            lTetrominoEdges[9] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            lTetrominoEdges[10] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            lTetrominoEdges[11] = new StylusPoint(Start.X + Width, Start.Y);
            lTetrominoEdges[12] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            lTetrominoEdges[13] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            lTetrominoEdges[14] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            lTetrominoEdges[15] = new StylusPoint(Start.X + Width / 3, Start.Y);
            lTetrominoEdges[16] = new StylusPoint(Start.X, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection lTetrominoPoints = new StylusPointCollection(lTetrominoEdges);
            return new CustomStroke(lTetrominoPoints);
        }

        public Stroke DrawJTetromino()
        {
            StylusPoint[] jTetrominoEdges = new StylusPoint[16];

            jTetrominoEdges[0] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            jTetrominoEdges[1] = new StylusPoint(Start.X + Width / 3, Start.Y);
            jTetrominoEdges[2] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            jTetrominoEdges[3] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            jTetrominoEdges[4] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            jTetrominoEdges[5] = new StylusPoint(Start.X + Width, Start.Y);
            jTetrominoEdges[6] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            jTetrominoEdges[7] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            jTetrominoEdges[8] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            jTetrominoEdges[9] = new StylusPoint(Start.X, Start.Y + Height / 2);
            jTetrominoEdges[10] = new StylusPoint(Start.X, Start.Y + Height);
            jTetrominoEdges[11] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            jTetrominoEdges[12] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            jTetrominoEdges[13] = new StylusPoint(Start.X, Start.Y + Height / 2);
            jTetrominoEdges[14] = new StylusPoint(Start.X, Start.Y);
            jTetrominoEdges[15] = new StylusPoint(Start.X + Width / 3, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection jTetrominoPoints = new StylusPointCollection(jTetrominoEdges);
            return new CustomStroke(jTetrominoPoints);
        }

        public Stroke DrawTTetromino()
        {
            StylusPoint[] tTetrominoEdges = new StylusPoint[14];

            tTetrominoEdges[0] = new StylusPoint(Start.X + Width / 3, Start.Y);
            tTetrominoEdges[1] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            tTetrominoEdges[2] = new StylusPoint(Start.X, Start.Y + Height / 2);
            tTetrominoEdges[3] = new StylusPoint(Start.X, Start.Y);
            tTetrominoEdges[4] = new StylusPoint(Start.X + Width / 3, Start.Y);
            tTetrominoEdges[5] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            tTetrominoEdges[6] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            tTetrominoEdges[7] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            tTetrominoEdges[8] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            tTetrominoEdges[9] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height);
            tTetrominoEdges[10] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            tTetrominoEdges[11] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            tTetrominoEdges[12] = new StylusPoint(Start.X + Width, Start.Y);
            tTetrominoEdges[13] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection tTetrominoPoints = new StylusPointCollection(tTetrominoEdges);
            return new CustomStroke(tTetrominoPoints);
        }

        public Stroke DrawSTetromino()
        {
            StylusPoint[] sTetrominoEdges = new StylusPoint[15];

            sTetrominoEdges[0] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            sTetrominoEdges[1] = new StylusPoint(Start.X + Width / 3, Start.Y);
            sTetrominoEdges[2] = new StylusPoint(Start.X, Start.Y);
            sTetrominoEdges[3] = new StylusPoint(Start.X, Start.Y + Height / 2);
            sTetrominoEdges[4] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            sTetrominoEdges[5] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            sTetrominoEdges[6] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height);
            sTetrominoEdges[7] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            sTetrominoEdges[8] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            sTetrominoEdges[9] = new StylusPoint(Start.X + Width / 3, Start.Y);
            sTetrominoEdges[10] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            sTetrominoEdges[11] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            sTetrominoEdges[12] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            sTetrominoEdges[13] = new StylusPoint(Start.X + Width, Start.Y + Height);
            sTetrominoEdges[14] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height);

            //With the Point variables we create a Stroke
            StylusPointCollection sTetrominoPoints = new StylusPointCollection(sTetrominoEdges);
            return new CustomStroke(sTetrominoPoints);
        }

        public Stroke DrawZTetromino()
        {
            StylusPoint[] zTetrominoEdges = new StylusPoint[15];

            zTetrominoEdges[0] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            zTetrominoEdges[1] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            zTetrominoEdges[2] = new StylusPoint(Start.X, Start.Y + Height);
            zTetrominoEdges[3] = new StylusPoint(Start.X, Start.Y + Height / 2);
            zTetrominoEdges[4] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            zTetrominoEdges[5] = new StylusPoint(Start.X + Width / 3, Start.Y);
            zTetrominoEdges[6] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            zTetrominoEdges[7] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            zTetrominoEdges[8] = new StylusPoint(Start.X + Width / 3, Start.Y + Height / 2);
            zTetrominoEdges[9] = new StylusPoint(Start.X + Width / 3, Start.Y + Height);
            zTetrominoEdges[10] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height);
            zTetrominoEdges[11] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y + Height / 2);
            zTetrominoEdges[12] = new StylusPoint(Start.X + Width, Start.Y + Height / 2);
            zTetrominoEdges[13] = new StylusPoint(Start.X + Width, Start.Y);
            zTetrominoEdges[14] = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);

            //With the Point variables we create a Stroke
            StylusPointCollection zTetrominoPoints = new StylusPointCollection(zTetrominoEdges);
            return new CustomStroke(zTetrominoPoints);
        }
    }
}
