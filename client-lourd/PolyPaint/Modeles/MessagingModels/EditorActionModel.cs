namespace PolyPaint.Modeles.MessagingModels
{
    public class EditorActionModel : MessageModelBase
    {
        public StrokeActionModel action { get; set; }
        public AuthorModel author { get; set; }
        public DrawingModel drawing { get; set; }
        public StrokeModel stroke { get; set; }
    }

    public class StrokeActionModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public enum ActionIds
    {
        NewStroke = 1,
        PartialEraseStroke,
        FullEraseStroke
    }

    public class DrawingModel
    {
        public string id { get; set; }
    }


    public class StrokeModel
    {
        public DrawingAttributesModel drawingAttributes { get; set; }
        public StylusPointModel[] dots { get; set; }
    }

    public class DrawingAttributesModel
    {
        public string color { get; set; }
        public double height { get; set; }
        public double width { get; set; }
        public string stylusTip { get; set; }
    }

    public class StylusPointModel
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}