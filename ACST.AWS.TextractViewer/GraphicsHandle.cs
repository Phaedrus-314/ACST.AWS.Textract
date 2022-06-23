using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACST.AWS.TextractViewer
{
    public struct GraphicsHandle
    {
        public int Index { get; set; }

        public PointF Origin { get; set; }

        public bool Selected { get; private set; }

        public GraphicsHandle(int index, Point origin, bool selected = false)
        {
            this.Index = index;
            this.Origin = origin;
            this.Selected = selected;
        }

        public GraphicsHandle(int index, PointF origin, bool selected = false)
        {
            this.Index = index;
            this.Origin = origin;
            this.Selected = selected;
        }

        public void Release()
        {
            this.Selected = false;
        }

        public PointF Offset(PointF point)
        {
            return new PointF { X = point.X - this.Origin.X, Y = point.Y - this.Origin.Y };
        }

        //public Point Offset(Point point)
        //{
        //    return new Point { X = point.X - this.Origin.X, Y = point.Y - this.Origin.Y };
        //}

        public static GraphicsHandle Default()
        {
            return new GraphicsHandle(-1, new Point() { X = 0, Y = 0 }, false);
        }
    }

    //Todo: remove me
    public class GraphicsLine
    {
        public PointF Start { get; set; }

        public PointF End { get; set; }

        public Color Color { get; set; }
    }

    public class GraphicsText
    {
        public PointF Location { get; set; }

        public string Text { get; set; }

        public Brush Brush { get; set; } = Brushes.Black;
    }

    public class GraphicsTextDictionary
        : Dictionary<string, GraphicsText>
    {
        public void Add(string text, PointF location, Brush brush)
        {
            Add(new GraphicsText { Text = text, Location = location, Brush = brush });
        }

        public void Add(GraphicsText graphicsText)
        {

            if(!base.ContainsKey(graphicsText.Text))
                base.Add(graphicsText.Text, graphicsText);
        }
    }

    
}
