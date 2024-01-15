using BP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class NodeArguments
{
   
    private String title;
    private String type;
    private ArgumentTypes argumentTypes;
    private Node parentNode;
    private Point position;
    private NodeArguments connectedArgument;

    public String Title
    {
        get { return title; }
        set { title = value; }
    }

    public Node ParentNode
    {
        get { return parentNode; } 
        set {  parentNode = value; }
    }

    public Point Position
    {
        get { return position; }
        set { position = value; }
    }



    public String Type
    {
        get { return type; }
        set { type = value; }
    }

    public ArgumentTypes ArgumentTypes
    {
        get { return argumentTypes; }
        set { argumentTypes = value; }
    }

    public NodeArguments ConnectedArgument
    {
        get {  return connectedArgument; }
        set { connectedArgument = value; }
    }

}
