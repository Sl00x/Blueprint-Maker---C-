using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NodeArgumentsConnector
{
    private NodeArguments firstArg;
    private NodeArguments secondArg;
    private Control containerControl;

    public NodeArgumentsConnector(NodeArguments firstArg, NodeArguments secondArg, Control containerControl)
    {
        this.firstArg = firstArg;
        this.secondArg = secondArg;
        this.containerControl = containerControl;

        SubscribeToNodeEvents();
    }

    private void SubscribeToNodeEvents()
    {
        firstArg.ParentNode.Move += NodeMoved;
        secondArg.ParentNode.Move += NodeMoved;
    }


    private void NodeMoved(object? sender, EventArgs e)
    {

        containerControl.Invalidate();
    }

    public void Draw(Graphics g)
    {

        Point newFirstArgPosition = new Point(firstArg.ParentNode.Location.X, firstArg.ParentNode.Location.Y + firstArg.Position.Y);
        Point newSecondeArgPosition = new Point(secondArg.ParentNode.Location.X, secondArg.ParentNode.Location.Y + secondArg.Position.Y);
        DrawBezierCurve(g, newFirstArgPosition, newSecondeArgPosition);

    }

    private void DrawBezierCurve(Graphics g, Point startPoint, Point endPoint)
    {

        int startNodePointX = startPoint.X + firstArg.ParentNode.Width + 2;
        int startNodePointY = startPoint.Y + 10;
        int endNodePointX = endPoint.X - 2;
        int endNodePointY = endPoint.Y + 10;
        Point controlPoint1 = new Point(startNodePointX + (endNodePointX - startNodePointX) / 2, startNodePointY);
        Point controlPoint2 = new Point(endNodePointX - (endNodePointX - startNodePointX) / 2, endNodePointY);
        Point startControlPoint = new Point(startNodePointX, startNodePointY);
        Point endControlPoint = new Point(endNodePointX, endNodePointY);

        using (Pen pen = new Pen(Color.Red, 1))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawBezier(pen, startControlPoint, controlPoint1, controlPoint2, endControlPoint);
        }
    }
}

