using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NodeSelectedEventArgs : EventArgs
    {
        public Node SourceNode { get; }

        public NodeSelectedEventArgs(Node sourceNode)
        {
            SourceNode = sourceNode;
        }
}

public class NodePositionChangedEventArgs : EventArgs
{
    public Point NewPosition { get; }

    public NodePositionChangedEventArgs(Point newPosition)
    {
        NewPosition = newPosition;
    }
}


public class NodeArgumentSelectedEventArgs : EventArgs
{
    public NodeArguments NodeArgument { get; }

    public NodeArgumentSelectedEventArgs(NodeArguments nodeArgument)
    {
        NodeArgument = nodeArgument;
    }
}
