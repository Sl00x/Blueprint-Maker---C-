using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class NodeConnector
{
    private Node startNode;
    private Node endNode;
    private Control containerControl;

    public NodeConnector(Node startNode, Node endNode, Control containerControl)
    {
        this.startNode = startNode;
        this.endNode = endNode;
        this.containerControl = containerControl;

        SubscribeToNodeEvents();
    }

    private void SubscribeToNodeEvents()
    {
        startNode.Move += NodeMoved;
        endNode.Move += NodeMoved;
    }


    private void NodeMoved(object? sender, EventArgs e)
    {
   
        containerControl.Invalidate();
    }

    public void Draw(Graphics g)
    {
        DrawBezierCurve(g, startNode.Location, endNode.Location);

    }

    private void DrawBezierCurve(Graphics g, Point startPoint, Point endPoint)
    {

        int startNodePointX = startPoint.X + startNode.Width + 2;
        int startNodePointY = startPoint.Y + 10;
        int endNodePointX = endPoint.X - 2;
        int endNodePointY = endPoint.Y + 10;
        Point controlPoint1 = new Point(startNodePointX + (endNodePointX - startNodePointX) / 2, startNodePointY);
        Point controlPoint2 = new Point(endNodePointX - (endNodePointX - startNodePointX) / 2, endNodePointY);
        Point startControlPoint = new Point(startNodePointX, startNodePointY);
        Point endControlPoint = new Point(endNodePointX, endNodePointY);

        using (Pen pen = new Pen(Color.Black, 2))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawBezier(pen, startControlPoint, controlPoint1, controlPoint2, endControlPoint);
        }
    }
}