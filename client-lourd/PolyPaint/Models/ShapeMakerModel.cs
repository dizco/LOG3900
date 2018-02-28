using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.Models
{
    public class ShapeMaker
    {
        public ShapeMaker(Point mouseStart, Point mouseEnd)
        {
            Start = mouseStart;
            End = mouseEnd;
            Width = End.X - Start.X;
            Height = -(End.Y - Start.Y);
        }

        private Point Start { get; }
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
            Stroke strokeLine = new Stroke(linePoints);
            return strokeLine;
        }

        public Stroke DrawArrow()
        {
            StylusPointCollection arrowPoints = new StylusPointCollection();
            StylusPoint arrowEdge1 = new StylusPoint(Start.X, Start.Y - Height * 3 / 4);
            StylusPoint arrowEdge2 = new StylusPoint(arrowEdge1.X + Width / 2, arrowEdge1.Y);
            StylusPoint arrowEdge3 = new StylusPoint(arrowEdge2.X, arrowEdge2.Y - Height / 4);
            StylusPoint arrowEdge4 = new StylusPoint(arrowEdge3.X + Width / 2, arrowEdge3.Y + Height / 2);
            StylusPoint arrowEdge5 = new StylusPoint(arrowEdge4.X - Width / 2, arrowEdge4.Y + Height / 2);
            StylusPoint arrowEdge6 = new StylusPoint(arrowEdge5.X, arrowEdge5.Y - Height / 4);
            StylusPoint arrowEdge7 = new StylusPoint(arrowEdge6.X - Width / 2, arrowEdge6.Y);
            StylusPoint arrowEdge8 = new StylusPoint(arrowEdge7.X, arrowEdge7.Y - Height / 2);

            arrowPoints.Add(arrowEdge1);
            arrowPoints.Add(arrowEdge2);
            arrowPoints.Add(arrowEdge3);
            arrowPoints.Add(arrowEdge4);
            arrowPoints.Add(arrowEdge5);
            arrowPoints.Add(arrowEdge6);
            arrowPoints.Add(arrowEdge7);
            arrowPoints.Add(arrowEdge8);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(arrowPoints);
            return strokeLine;
        }

        public Stroke DrawTriangle()
        {
            StylusPointCollection trianglePoints = new StylusPointCollection();
            StylusPoint triangleEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint triangleEdge2 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint triangleEdge3 = new StylusPoint(Start.X + Width / 2, Start.Y - Height);

            trianglePoints.Add(triangleEdge1);
            trianglePoints.Add(triangleEdge2);
            trianglePoints.Add(triangleEdge3);
            trianglePoints.Add(triangleEdge1);

            //With the Point variables we create a Stroke
            //CustomStroke strokeLine = new CustomStroke(TrianglePoints, start, end);
            Stroke stroke = new Stroke(trianglePoints);
            return stroke;
        }

        public Stroke DrawDiamond()
        {
            StylusPointCollection diamondPoints = new StylusPointCollection();
            StylusPoint diamondEdge1 = new StylusPoint(Start.X + Width / 2, Start.Y);
            StylusPoint diamondEdge2 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint diamondEdge3 = new StylusPoint(Start.X + Width / 2, Start.Y - Height);
            StylusPoint diamondEdge4 = new StylusPoint(Start.X, Start.Y - Height / 2);

            diamondPoints.Add(diamondEdge1);
            diamondPoints.Add(diamondEdge2);
            diamondPoints.Add(diamondEdge3);
            diamondPoints.Add(diamondEdge4);
            diamondPoints.Add(diamondEdge1);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(diamondPoints);
            return strokeLine;
        }

        public Stroke DrawLightningBolt()
        {
            StylusPointCollection lightningBoltPoints = new StylusPointCollection();
            StylusPoint lightningBoltEdge1 = new StylusPoint(Start.X, Start.Y - Height * 3 / 4);
            StylusPoint lightningBoltEdge2 = new StylusPoint(Start.X + Width / 3, Start.Y - Height * 3 / 5);
            StylusPoint lightningBoltEdge3 = new StylusPoint(Start.X + Width / 4, Start.Y - Height / 2);
            StylusPoint lightningBoltEdge4 = new StylusPoint(Start.X + Width * 3 / 5, Start.Y - Height / 3);
            StylusPoint lightningBoltEdge5 = new StylusPoint(Start.X + Width / 2, Start.Y - Height * 1 / 4);
            StylusPoint lightningBoltEdge6 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint lightningBoltEdge7 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 3);
            StylusPoint lightningBoltEdge8 = new StylusPoint(Start.X + Width * 3 / 4, Start.Y - Height * 2 / 5);
            StylusPoint lightningBoltEdge9 = new StylusPoint(Start.X + Width / 2, Start.Y - Height * 2 / 3);
            StylusPoint lightningBoltEdge10 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height * 7 / 10);
            StylusPoint lightningBoltEdge11 = new StylusPoint(Start.X + Width / 3, Start.Y - Height);

            lightningBoltPoints.Add(lightningBoltEdge1);
            lightningBoltPoints.Add(lightningBoltEdge2);
            lightningBoltPoints.Add(lightningBoltEdge3);
            lightningBoltPoints.Add(lightningBoltEdge4);
            lightningBoltPoints.Add(lightningBoltEdge5);
            lightningBoltPoints.Add(lightningBoltEdge6);
            lightningBoltPoints.Add(lightningBoltEdge7);
            lightningBoltPoints.Add(lightningBoltEdge8);
            lightningBoltPoints.Add(lightningBoltEdge9);
            lightningBoltPoints.Add(lightningBoltEdge10);
            lightningBoltPoints.Add(lightningBoltEdge11);
            lightningBoltPoints.Add(lightningBoltEdge1);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(lightningBoltPoints);
            return strokeLine;
        }

        public Stroke DrawITetromino()
        {
            StylusPointCollection iTetrominoPoints = new StylusPointCollection();
            StylusPoint iTetrominoEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint iTetrominoEdge2 = new StylusPoint(Start.X, Start.Y - Height);
            StylusPoint iTetrominoEdge3 = new StylusPoint(Start.X + Width / 4, Start.Y - Height);
            StylusPoint iTetrominoEdge4 = new StylusPoint(Start.X + Width / 4, Start.Y);
            StylusPoint iTetrominoEdge5 = new StylusPoint(Start.X + Width / 2, Start.Y);
            StylusPoint iTetrominoEdge6 = new StylusPoint(Start.X + Width / 2, Start.Y - Height);
            StylusPoint iTetrominoEdge7 = new StylusPoint(Start.X + Width * 3 / 4, Start.Y - Height);
            StylusPoint iTetrominoEdge8 = new StylusPoint(Start.X + Width * 3 / 4, Start.Y);
            StylusPoint iTetrominoEdge9 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint iTetrominoEdge10 = new StylusPoint(Start.X + Width, Start.Y - Height);

            iTetrominoPoints.Add(iTetrominoEdge1);
            iTetrominoPoints.Add(iTetrominoEdge2);
            iTetrominoPoints.Add(iTetrominoEdge3);
            iTetrominoPoints.Add(iTetrominoEdge4);
            iTetrominoPoints.Add(iTetrominoEdge5);
            iTetrominoPoints.Add(iTetrominoEdge6);
            iTetrominoPoints.Add(iTetrominoEdge7);
            iTetrominoPoints.Add(iTetrominoEdge8);
            iTetrominoPoints.Add(iTetrominoEdge9);
            iTetrominoPoints.Add(iTetrominoEdge10);
            iTetrominoPoints.Add(iTetrominoEdge7);
            iTetrominoPoints.Add(iTetrominoEdge8);
            iTetrominoPoints.Add(iTetrominoEdge5);
            iTetrominoPoints.Add(iTetrominoEdge6);
            iTetrominoPoints.Add(iTetrominoEdge3);
            iTetrominoPoints.Add(iTetrominoEdge4);
            iTetrominoPoints.Add(iTetrominoEdge1);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(iTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawOTetromino()
        {
            StylusPointCollection oTetrominoPoints = new StylusPointCollection();
            StylusPoint oTetrominoEdge1 = new StylusPoint(Start.X + Width / 2, Start.Y - Height / 2);
            StylusPoint oTetrominoEdge2 = new StylusPoint(Start.X + Width / 2, Start.Y);
            StylusPoint oTetrominoEdge3 = new StylusPoint(Start.X, Start.Y);
            StylusPoint oTetrominoEdge4 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint oTetrominoEdge5 = new StylusPoint(Start.X + Width / 2, Start.Y - Height);
            StylusPoint oTetrominoEdge6 = new StylusPoint(Start.X, Start.Y - Height);
            StylusPoint oTetrominoEdge7 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint oTetrominoEdge8 = new StylusPoint(Start.X + Width, Start.Y - Height);
            StylusPoint oTetrominoEdge9 = new StylusPoint(Start.X + Width, Start.Y);

            oTetrominoPoints.Add(oTetrominoEdge1);
            oTetrominoPoints.Add(oTetrominoEdge2);
            oTetrominoPoints.Add(oTetrominoEdge3);
            oTetrominoPoints.Add(oTetrominoEdge4);
            oTetrominoPoints.Add(oTetrominoEdge1);
            oTetrominoPoints.Add(oTetrominoEdge5);
            oTetrominoPoints.Add(oTetrominoEdge6);
            oTetrominoPoints.Add(oTetrominoEdge4);
            oTetrominoPoints.Add(oTetrominoEdge1);
            oTetrominoPoints.Add(oTetrominoEdge7);
            oTetrominoPoints.Add(oTetrominoEdge8);
            oTetrominoPoints.Add(oTetrominoEdge5);
            oTetrominoPoints.Add(oTetrominoEdge1);
            oTetrominoPoints.Add(oTetrominoEdge2);
            oTetrominoPoints.Add(oTetrominoEdge9);
            oTetrominoPoints.Add(oTetrominoEdge7);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(oTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawLTetromino()
        {
            StylusPointCollection lTetrominoPoints = new StylusPointCollection();
            StylusPoint lTetrominoEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint lTetrominoEdge2 = new StylusPoint(Start.X + Width / 3, Start.Y);
            StylusPoint lTetrominoEdge3 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            StylusPoint lTetrominoEdge4 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint lTetrominoEdge5 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint lTetrominoEdge6 = new StylusPoint(Start.X + Width / 3, Start.Y - Height / 2);
            StylusPoint lTetrominoEdge7 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 2);
            StylusPoint lTetrominoEdge8 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint lTetrominoEdge9 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height);
            StylusPoint lTetrominoEdge10 = new StylusPoint(Start.X + Width, Start.Y - Height);

            lTetrominoPoints.Add(lTetrominoEdge1);
            lTetrominoPoints.Add(lTetrominoEdge5);
            lTetrominoPoints.Add(lTetrominoEdge6);
            lTetrominoPoints.Add(lTetrominoEdge2);
            lTetrominoPoints.Add(lTetrominoEdge3);
            lTetrominoPoints.Add(lTetrominoEdge7);
            lTetrominoPoints.Add(lTetrominoEdge8);
            lTetrominoPoints.Add(lTetrominoEdge10);
            lTetrominoPoints.Add(lTetrominoEdge9);
            lTetrominoPoints.Add(lTetrominoEdge7);
            lTetrominoPoints.Add(lTetrominoEdge8);
            lTetrominoPoints.Add(lTetrominoEdge4);
            lTetrominoPoints.Add(lTetrominoEdge3);
            lTetrominoPoints.Add(lTetrominoEdge7);
            lTetrominoPoints.Add(lTetrominoEdge6);
            lTetrominoPoints.Add(lTetrominoEdge2);
            lTetrominoPoints.Add(lTetrominoEdge1);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(lTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawJTetromino()
        {
            StylusPointCollection jTetrominoPoints = new StylusPointCollection();
            StylusPoint jTetrominoEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint jTetrominoEdge2 = new StylusPoint(Start.X + Width / 3, Start.Y);
            StylusPoint jTetrominoEdge3 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            StylusPoint jTetrominoEdge4 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint jTetrominoEdge5 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint jTetrominoEdge6 = new StylusPoint(Start.X + Width / 3, Start.Y - Height / 2);
            StylusPoint jTetrominoEdge7 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 2);
            StylusPoint jTetrominoEdge8 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint jTetrominoEdge9 = new StylusPoint(Start.X, Start.Y - Height);
            StylusPoint jTetrominoEdge10 = new StylusPoint(Start.X + Width / 3, Start.Y - Height);

            jTetrominoPoints.Add(jTetrominoEdge6);
            jTetrominoPoints.Add(jTetrominoEdge2);
            jTetrominoPoints.Add(jTetrominoEdge3);
            jTetrominoPoints.Add(jTetrominoEdge7);
            jTetrominoPoints.Add(jTetrominoEdge8);
            jTetrominoPoints.Add(jTetrominoEdge4);
            jTetrominoPoints.Add(jTetrominoEdge3);
            jTetrominoPoints.Add(jTetrominoEdge7);
            jTetrominoPoints.Add(jTetrominoEdge6);
            jTetrominoPoints.Add(jTetrominoEdge5);
            jTetrominoPoints.Add(jTetrominoEdge9);
            jTetrominoPoints.Add(jTetrominoEdge10);
            jTetrominoPoints.Add(jTetrominoEdge6);
            jTetrominoPoints.Add(jTetrominoEdge5);
            jTetrominoPoints.Add(jTetrominoEdge1);
            jTetrominoPoints.Add(jTetrominoEdge2);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(jTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawTTetromino()
        {
            StylusPointCollection tTetrominoPoints = new StylusPointCollection();
            StylusPoint tTetrominoEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint tTetrominoEdge2 = new StylusPoint(Start.X + Width / 3, Start.Y);
            StylusPoint tTetrominoEdge3 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            StylusPoint tTetrominoEdge4 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint tTetrominoEdge5 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint tTetrominoEdge6 = new StylusPoint(Start.X + Width / 3, Start.Y - Height / 2);
            StylusPoint tTetrominoEdge7 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 2);
            StylusPoint tTetrominoEdge8 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint tTetrominoEdge9 = new StylusPoint(Start.X + Width / 3, Start.Y - Height);
            StylusPoint tTetrominoEdge10 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height);

            tTetrominoPoints.Add(tTetrominoEdge2);
            tTetrominoPoints.Add(tTetrominoEdge6);
            tTetrominoPoints.Add(tTetrominoEdge5);
            tTetrominoPoints.Add(tTetrominoEdge1);
            tTetrominoPoints.Add(tTetrominoEdge2);
            tTetrominoPoints.Add(tTetrominoEdge3);
            tTetrominoPoints.Add(tTetrominoEdge7);
            tTetrominoPoints.Add(tTetrominoEdge6);
            tTetrominoPoints.Add(tTetrominoEdge9);
            tTetrominoPoints.Add(tTetrominoEdge10);
            tTetrominoPoints.Add(tTetrominoEdge7);
            tTetrominoPoints.Add(tTetrominoEdge8);
            tTetrominoPoints.Add(tTetrominoEdge4);
            tTetrominoPoints.Add(tTetrominoEdge3);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(tTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawSTetromino()
        {
            StylusPointCollection sTetrominoPoints = new StylusPointCollection();
            StylusPoint sTetrominoEdge1 = new StylusPoint(Start.X, Start.Y);
            StylusPoint sTetrominoEdge2 = new StylusPoint(Start.X + Width / 3, Start.Y);
            StylusPoint sTetrominoEdge3 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            StylusPoint sTetrominoEdge4 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint sTetrominoEdge5 = new StylusPoint(Start.X + Width / 3, Start.Y - Height / 2);
            StylusPoint sTetrominoEdge6 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 2);
            StylusPoint sTetrominoEdge7 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint sTetrominoEdge8 = new StylusPoint(Start.X + Width / 3, Start.Y - Height);
            StylusPoint sTetrominoEdge9 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height);
            StylusPoint sTetrominoEdge10 = new StylusPoint(Start.X + Width, Start.Y - Height);

            sTetrominoPoints.Add(sTetrominoEdge5);
            sTetrominoPoints.Add(sTetrominoEdge2);
            sTetrominoPoints.Add(sTetrominoEdge1);
            sTetrominoPoints.Add(sTetrominoEdge4);
            sTetrominoPoints.Add(sTetrominoEdge5);
            sTetrominoPoints.Add(sTetrominoEdge8);
            sTetrominoPoints.Add(sTetrominoEdge9);
            sTetrominoPoints.Add(sTetrominoEdge6);
            sTetrominoPoints.Add(sTetrominoEdge5);
            sTetrominoPoints.Add(sTetrominoEdge2);
            sTetrominoPoints.Add(sTetrominoEdge3);
            sTetrominoPoints.Add(sTetrominoEdge6);
            sTetrominoPoints.Add(sTetrominoEdge7);
            sTetrominoPoints.Add(sTetrominoEdge10);
            sTetrominoPoints.Add(sTetrominoEdge9);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(sTetrominoPoints);
            return strokeLine;
        }

        public Stroke DrawZTetromino()
        {
            StylusPointCollection zTetrominoPoints = new StylusPointCollection();
            StylusPoint zTetrominoEdge1 = new StylusPoint(Start.X + Width / 3, Start.Y);
            StylusPoint zTetrominoEdge2 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y);
            StylusPoint zTetrominoEdge3 = new StylusPoint(Start.X + Width, Start.Y);
            StylusPoint zTetrominoEdge4 = new StylusPoint(Start.X, Start.Y - Height / 2);
            StylusPoint zTetrominoEdge5 = new StylusPoint(Start.X + Width / 3, Start.Y - Height / 2);
            StylusPoint zTetrominoEdge6 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height / 2);
            StylusPoint zTetrominoEdge7 = new StylusPoint(Start.X + Width, Start.Y - Height / 2);
            StylusPoint zTetrominoEdge8 = new StylusPoint(Start.X, Start.Y - Height);
            StylusPoint zTetrominoEdge9 = new StylusPoint(Start.X + Width / 3, Start.Y - Height);
            StylusPoint zTetrominoEdge10 = new StylusPoint(Start.X + Width * 2 / 3, Start.Y - Height);

            zTetrominoPoints.Add(zTetrominoEdge5);
            zTetrominoPoints.Add(zTetrominoEdge9);
            zTetrominoPoints.Add(zTetrominoEdge8);
            zTetrominoPoints.Add(zTetrominoEdge4);
            zTetrominoPoints.Add(zTetrominoEdge5);
            zTetrominoPoints.Add(zTetrominoEdge1);
            zTetrominoPoints.Add(zTetrominoEdge2);
            zTetrominoPoints.Add(zTetrominoEdge6);
            zTetrominoPoints.Add(zTetrominoEdge5);
            zTetrominoPoints.Add(zTetrominoEdge9);
            zTetrominoPoints.Add(zTetrominoEdge10);
            zTetrominoPoints.Add(zTetrominoEdge6);
            zTetrominoPoints.Add(zTetrominoEdge7);
            zTetrominoPoints.Add(zTetrominoEdge3);
            zTetrominoPoints.Add(zTetrominoEdge2);

            //With the Point variables we create a Stroke
            Stroke strokeLine = new Stroke(zTetrominoPoints);
            return strokeLine;
        }
    }
}
