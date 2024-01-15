using System.Text;
using System.Windows.Forms;

namespace BP
{
    public partial class Form1 : Form
    {

        public Node? firstNode;
        public Node? secondNode;
        public List<Node> nodes = new List<Node>();
        public List<NodeConnector> nodeConnectors = new List<NodeConnector>();
        public List<NodeArgumentsConnector> nodeArgsConnectors = new List<NodeArgumentsConnector>();
        public NodeArgumentsConnector connectorArg;

        public NodeArguments? inputNodeArg;
        public NodeArguments? outputNodeArg;
        public Form1()
        {
            InitializeComponent();
        }

        public void SelectedNode(object? sender, NodeSelectedEventArgs e)
        {
            if (firstNode == null)
            {
                firstNode = e.SourceNode;
            }
            else
            {
                secondNode = e.SourceNode;
                if (firstNode != null && firstNode != secondNode)
                {
                    foreach(Node node in nodes)
                    {
                        if(node == firstNode)
                        {
                            node.ConnecteNode = secondNode;
                        }
                    }
                    nodeConnectors.Add(new NodeConnector(firstNode, secondNode, bpp));
                    bpp.Invalidate();
                    firstNode = null;
                    secondNode = null;
                }
            };
        }

        public void SelectedArgNode(object? sender, NodeArgumentSelectedEventArgs e)
        {
            if(e.NodeArgument != null )
            {
                if (outputNodeArg == null && e.NodeArgument.ArgumentTypes == ArgumentTypes.Get)
                {
                    outputNodeArg = e.NodeArgument;
                    console.Items.Add($"[{outputNodeArg.Title}] argument was selected as getter !");
                }
                else
                {
                    if (e.NodeArgument.ArgumentTypes == ArgumentTypes.Set && outputNodeArg != null && outputNodeArg.Type == e.NodeArgument.Type)
                    {
                        console.Items.Add($"[{e.NodeArgument.Title}] argument was selected as setter !");
                        inputNodeArg = e.NodeArgument;
                        e.NodeArgument.ConnectedArgument = outputNodeArg;
                        if (outputNodeArg != null && outputNodeArg != inputNodeArg)
                        {
                            if (outputNodeArg.Selector != null)
                            {
                                outputNodeArg.Selector.BackColor = Color.White;
                            }
                            if (inputNodeArg.Selector != null)
                            {
                                inputNodeArg.Selector.BackColor = Color.White;
                            }
                            inputNodeArg.ParentNode.Selected = null;
                            inputNodeArg.ParentNode.CurrentSelectedArg = null;
                            outputNodeArg.ParentNode.Selected = null;
                            outputNodeArg.ParentNode.CurrentSelectedArg = null;
                            outputNodeArg = null;
                            inputNodeArg = null;

                            bpp.Invalidate();
                        }
                    } else
                    {
                        console.Items.Add($"[{outputNodeArg?.Title ?? ""} => {e.NodeArgument.Title}] don't have same type or are not setter !");
                    }
                    
                };
            }
        }

        public void DrawAllConnectors()
        {
            foreach (Node node in nodes)
            {
                if (node.ConnecteNode != null)
                {
                    new NodeConnector(node, node.ConnecteNode, bpp).Draw(bpp.CreateGraphics());
                }
                foreach (NodeArguments nodeArg in node.nodeArgs)
                {
                    if (nodeArg.ConnectedArgument != null)
                    {
                        new NodeArgumentsConnector(nodeArg.ConnectedArgument, nodeArg, bpp).Draw(bpp.CreateGraphics());
                    }
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            //DrawAllConnectors();
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NodeMaker NodeMaker = new NodeMaker();
            NodeMaker.Show();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.Text = "Blueprint Editor";
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = Color.White;
            RefreshList();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            string pluginsDirectoryPath = Path.Combine(Application.StartupPath, "plugins");

            if (!Directory.Exists(pluginsDirectoryPath))
            {
                MessageBox.Show("Le dossier 'plugins' n'existe pas.");
                return;
            }
            string[] files = Directory.GetFiles(pluginsDirectoryPath, "*.plug.cs");

            listBox1.Items.Clear();
            foreach (string filePath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                listBox1.Items.Add(fileName);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string pluginsDirectoryPath = Path.Combine(Application.StartupPath, "plugins");
                string[] files = Directory.GetFiles(pluginsDirectoryPath, $"{listBox1.SelectedItem}.cs");
                CodeAnalysis codeAnalysis = new CodeAnalysis(files[0]);
                foreach(String method in codeAnalysis.MethodName)
                {
                    Node fNode = new FunctionNode();
                    fNode.Location = new Point(50, 50);
                    fNode.Title = method;
                    fNode.PluginCode = codeAnalysis.Code;
                    bpp.Controls.Add(fNode);
                    fNode.NodeSelected += SelectedNode;
                    fNode.NodeArgumentSelected += SelectedArgNode;

                    for (int i = 0; i < codeAnalysis.ReturnType.Count; i++)
                    {
                        if (codeAnalysis.ReturnType[i] != "void")
                        {
                            NodeArguments returnA = new NodeArguments();
                            returnA.Title = $"return({codeAnalysis.ReturnType[i]})";
                            returnA.ArgumentTypes = ArgumentTypes.Get;
                            returnA.Type = codeAnalysis.ReturnType[i];
                            fNode.AddArg(returnA);
                        }
                    }

                    for (int i = 0; i < codeAnalysis.Arguments.Count; i++)
                    {
                        NodeArguments text = new NodeArguments();
                        text.Title = codeAnalysis.Arguments[i];
                        text.ArgumentTypes = ArgumentTypes.Set;
                        text.Type = codeAnalysis.Arguments[i].Split(" ")[0];
                        fNode.AddArg(text);
                    }

                    nodes.Add(fNode);
                }
                

            }
        }

        private void bpp_Paint_1(object sender, PaintEventArgs e)
        {
            DrawAllConnectors();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder classCode = new StringBuilder();
            classCode.AppendLine("using GTA;");
            classCode.AppendLine("using GTA.Native;");
            classCode.AppendLine();
            classCode.AppendLine("class GTAMOD : Script");
            classCode.AppendLine("{");
            classCode.AppendLine("  public GTAMOD() {");
            classCode.AppendLine("      this.Tick += onUpdate;");
            classCode.AppendLine("  }");

            classCode.AppendLine("  public " + nodes[0].ReturnType + " " + nodes[0].Title + "() {");
            GenerateCode(classCode, nodes[0]);
            classCode.AppendLine("  }");

            for(int i  = 0;  i < nodes.Count; i++)
            {
                if(i != 0) { classCode.AppendLine(nodes[i].PluginCode); }
            }
            

            classCode.AppendLine("}");
            richTextBox1.Text = classCode.ToString();
        }

        static void GenerateCode(StringBuilder classCode, Node currentNode)
        {
            if (currentNode.ConnecteNode != null)
            {
                classCode.Append($"        {currentNode.ConnecteNode.Title}(");
                int i = 0;
                foreach (NodeArguments nodeArg in currentNode.ConnecteNode.nodeArgs)
                {
                    if (nodeArg.ConnectedArgument != null) {
                        classCode.Append($"{nodeArg.ConnectedArgument.ParentNode.Title}()");
                        if (i != currentNode.ConnecteNode.nodeArgs.Count - 1) {  classCode.Append(", "); }
                    }
                    i++;
                }
                classCode.AppendLine(");");
                GenerateCode(classCode, currentNode.ConnecteNode);
            }
        }
    }

}