using BP;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class Node : UserControl
{
    private Label titleLabel;
    private TableLayoutPanel parameterPanel;
    private Point lastMousePosition;
    private bool isDragging = false;
    private Boolean isMoving = false;

    public Boolean canBeLinked = true;

    private Node connectedNode;

    private Types returnType;

    public event EventHandler<NodePositionChangedEventArgs> NodePositionChanged;
    public event EventHandler<NodeSelectedEventArgs> NodeSelected;

    public event EventHandler<NodeArgumentSelectedEventArgs> NodeArgumentSelected;

    public List<NodeArguments> nodeArgs = new List<NodeArguments>();

    private NodeArguments? currentArgSelect;
    private Button? previousSelectedButton;

    private string pluginCode = "";

    public Node()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {

        this.BackColor = Color.Black;
        this.Padding = new Padding(0);
        this.Height = 12;

        titleLabel = new Label();
        titleLabel.Text = "";
        titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
        titleLabel.Dock = DockStyle.Top;
        //titleLabel.AutoSize = true;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        titleLabel.BackColor = Color.Red;
        titleLabel.ForeColor = Color.White;
        titleLabel.Padding = new Padding(2, 2, 2, 2);
        this.Controls.Add(titleLabel);
        this.AutoSize = true;
        SubscribeEvents();

    }

    public void AddArg(NodeArguments arg)
    {
        int rdn = new Random().Next(1, 10000);
        arg.Position = new Point(5, (this.nodeArgs.Count + 1) * 33);
        arg.ParentNode = this;
        this.nodeArgs.Add(arg);
        this.Size = new Size(this.Size.Width, this.nodeArgs.Count * 22 );
        Label argLabel = new Label();
        argLabel.Name = arg.Title + $"{rdn}__{this.nodeArgs.Count - 1}";
        argLabel.Text = arg.Title;
        argLabel.AutoSize = true;
        argLabel.TextAlign = ContentAlignment.MiddleRight;
        argLabel.BackColor = Color.Transparent;
        argLabel.ForeColor = Color.White;
        this.Controls.Add(argLabel);

        
        
        Button argSelector = new Button();
        
        
        if ( arg.ArgumentTypes == ArgumentTypes.Set)
        {
            argLabel.Location = new Point(22, this.nodeArgs.Count * 33);
            argSelector.Location = new Point(2, this.nodeArgs.Count * 32);
        } else if (arg.ArgumentTypes == ArgumentTypes.Get)
        {
            argLabel.Location = new Point(0, this.nodeArgs.Count * 33);
            argSelector.Location = new Point(this.Size.Width - 22, this.nodeArgs.Count * 32);
        } else
        {
            Button argsReturn = new Button();
            argLabel.Location = new Point(32, this.nodeArgs.Count * 33);
            argsReturn.Name = arg.Title + $"${rdn}__{this.nodeArgs.Count - 1}";
            argSelector.Location = new Point(2, this.nodeArgs.Count * 32);
            argsReturn.Location = new Point(this.Size.Width - 22, this.nodeArgs.Count * 32);
            argsReturn.FlatStyle = FlatStyle.Flat;
            argsReturn.BackColor = Color.White;
            argsReturn.Size = new Size(20, 20);
            argsReturn.Click += NodeArgument_ButtonClick;
            this.Controls.Add(argsReturn);

        }

        argSelector.Name = arg.Title + $"{rdn}__{this.nodeArgs.Count - 1}";
        argSelector.FlatStyle = FlatStyle.Flat;
        argSelector.BackColor = Color.White;
        argSelector.Size = new Size(20, 20);
        argSelector.Click += NodeArgument_ButtonClick;
        arg.Selector = argSelector;
        this.Controls.Add(argSelector);

        
    }

    public void SubscribeEvents()
    {
        titleLabel.MouseDown += Node_MouseDown;
        titleLabel.MouseMove += Node_MouseMove;
        titleLabel.MouseUp += Node_MouseUp;
        titleLabel.DoubleClick += Node_DoubleClick;


        this.MouseDown += Node_MouseDown;
        this.MouseMove += Node_MouseMove;
        this.MouseUp += Node_MouseUp;
        this.DoubleClick += Node_DoubleClick;
    }


    public new Point Location
    {
        get { return base.Location; }
        set
        {
            base.Location = value;
            OnNodePositionChanged(new NodePositionChangedEventArgs(value));
        }
    }

    protected virtual void OnNodePositionChanged(NodePositionChangedEventArgs e)
    {
        NodePositionChanged?.Invoke(this, e);
    }

    protected virtual void OnSelectedNode(NodeSelectedEventArgs e)
    {
        NodeSelected?.Invoke(this, e);
    }

    protected virtual void OnNodeArgumentSelected(NodeArgumentSelectedEventArgs e)
    {
        NodeArgumentSelected?.Invoke(this, e);
    }

    private void SetRoundedCorners(int radius)
    {
        Rectangle rectangle = new Rectangle(0, 0, this.Width, this.Height);
        GraphicsPath roundedPath = GetRoundedRect(rectangle, radius);
        this.Region = new Region(roundedPath);
    }

    private GraphicsPath GetRoundedRect(Rectangle rectangle, int radius)
    {
        GraphicsPath path = new GraphicsPath();

        path.AddArc(rectangle.X, rectangle.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(rectangle.Right - radius * 2, rectangle.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(rectangle.Right - radius * 2, rectangle.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);

        path.CloseFigure();
        return path;
    }

    private bool IsMouseOverNode(Point mouseLocation)
    {
        return mouseLocation.X >= 0 && mouseLocation.X <= this.Width && mouseLocation.Y >= 0 && mouseLocation.Y <= this.Height;
    }

    private void Node_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e?.Button == MouseButtons.Left)
        {
            if (IsMouseOverNode(e.Location))
            {
                lastMousePosition = e.Location;
                isDragging = true;
            }
        }
    }

    private void Node_DoubleClick(object? sender, EventArgs e)
    {
        OnSelectedNode(new NodeSelectedEventArgs(this));
    }

    private void NodeArgument_ButtonClick(object? sender, EventArgs e)
    {
      
        if (sender is Button clickedButton)
        {
            // MessageBox.Show(clickedButton.Name);
            int index = int.Parse(clickedButton.Name.Split("__")[1]);
            if (previousSelectedButton != null)
            {
                previousSelectedButton.BackColor = Color.White;
            }
            clickedButton.BackColor = Color.Green;
            previousSelectedButton = clickedButton;

            if (currentArgSelect != this.nodeArgs[index])
            {
                currentArgSelect = this.nodeArgs[index];
                OnNodeArgumentSelected(new NodeArgumentSelectedEventArgs(currentArgSelect));
            }
            else
            {
                OnNodeArgumentSelected(new NodeArgumentSelectedEventArgs(null));
                currentArgSelect = null;
                clickedButton.BackColor = Color.White;
                previousSelectedButton = null;
            }

        }
    }

    private void Node_MouseMove(object? sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            this.Left += e.X - lastMousePosition.X;
            this.Top += e.Y - lastMousePosition.Y;
            isMoving = true;
            OnNodePositionChanged(new NodePositionChangedEventArgs(base.Location));
        }
    }

    private void Node_MouseUp(object? sender, MouseEventArgs e)
    {
        if (isDragging && e?.Button == MouseButtons.Left)
        {
            isDragging = false;
            isMoving = false;
        }
    }

    public string Title
    {
        get { return titleLabel.Text; }
        set { 
            titleLabel.Text = value;
            titleLabel.Size = new Size(this.Width, titleLabel.Height);
        }
    }

    public Types ReturnType
    {
        get { return returnType; } set { returnType = value; }
    }

    public Button? Selected
    {
        get { return previousSelectedButton;  }
        set { previousSelectedButton = value; }
    }

    public NodeArguments? CurrentSelectedArg
    {
        get { return currentArgSelect; }
        set { currentArgSelect = value; }
    }

    public Boolean CanBeLink
    {
        get { return canBeLinked; }
        set { canBeLinked = value; }
    }

    public Boolean Moving
    {
        get { return isMoving; }
    }



    public Node ConnecteNode
    {
        get { return connectedNode; }
        set { connectedNode = value; }
    }

    public string PluginCode
    {
        get { return pluginCode; }
        set { pluginCode = value; }
    }
}