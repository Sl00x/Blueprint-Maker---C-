using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BP
{
    public partial class NodeMaker : Form
    {

        public String code;
        public NodeMaker()
        {
            InitializeComponent();
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
            richTextBox1.Font = new Font("Consolas", 12f);
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            HighlightSyntax();
            code = richTextBox1.Text;
        }

        private void HighlightSyntax()
        {
            string keywords = @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while)\b";
            string comments = @"(//.*?$)|(/\*.*?\*/)";

            MatchCollection keywordMatches = Regex.Matches(richTextBox1.Text, keywords, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection commentMatches = Regex.Matches(richTextBox1.Text, comments, RegexOptions.Multiline | RegexOptions.Singleline);

            int originalIndex = richTextBox1.SelectionStart;
            int originalLength = richTextBox1.SelectionLength;
            Color originalColor = Color.Black;

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionColor = originalColor;

            foreach (Match m in keywordMatches)
            {
                richTextBox1.SelectionStart = m.Index;
                richTextBox1.SelectionLength = m.Length;
                richTextBox1.SelectionColor = Color.Blue;
            }

            foreach (Match m in commentMatches)
            {
                richTextBox1.SelectionStart = m.Index;
                richTextBox1.SelectionLength = m.Length;
                richTextBox1.SelectionColor = Color.Green;
            }

            richTextBox1.SelectionStart = originalIndex;
            richTextBox1.SelectionLength = originalLength;
            richTextBox1.SelectionColor = originalColor;
        }

        private void NodeMaker_Load(object sender, EventArgs e)
        {

        }

        public String CodeSyntax
        {
            get { return code; }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToPluginDirectory();
        }

        private void SaveToPluginDirectory()
        {
            string pluginsDirectoryPath = Path.Combine(Application.StartupPath, "plugins");

            if (!Directory.Exists(pluginsDirectoryPath))
            {
                Directory.CreateDirectory(pluginsDirectoryPath);
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = pluginsDirectoryPath;
                saveFileDialog.Filter = "Fichiers plug.cs (*.plug.cs)|*.plug.cs|Tous les fichiers (*.*)|*.*";
                saveFileDialog.DefaultExt = "plug.cs";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    string richTextBoxContent = richTextBox1.Text;

                    if (File.Exists(filePath))
                    {
                        DialogResult result = MessageBox.Show("Le fichier existe déjà. Voulez-vous le remplacer ?", "Confirmation", MessageBoxButtons.YesNo);

                        if (result == DialogResult.No) return;
                    }

                    File.WriteAllText(filePath, richTextBoxContent);

                    MessageBox.Show($"Le fichier a été sauvegardé avec succès dans le dossier 'plugins' avec le nom {Path.GetFileName(filePath)}.");
                }
            }
        }
    }
}
