using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Model = ACST.AWS.Textract.Model;
using Newtonsoft.Json.Linq;

namespace ACST.AWS.TextractViewer
{
    public partial class ContextViewer : Form
    {

        string JSON { get; set; }

        string RootName { get; set; }

        // this formulation works in .Net 4.72 but not 4.5, remove HighlightKeyWord class when going back to 4.72
        //public List<(string word, Color color, bool bold)> HighlightKeyWords { get; set; }
        public List<HighlightKeyWord> HighlightKeyWords { get; set; }

        Model.TextractDocument TextractDocument { get; }

        public ContextViewer()
        {
            InitializeComponent();
            splitContainer1.Panel1MinSize = 150;
            splitContainer1.SplitterDistance = 200;
        }

        public ContextViewer(Model.TextractDocument textractDocument, string json, string rootName = null)
            : this()
        {
            this.JSON = json;
            this.RootName = rootName;
            this.TextractDocument = textractDocument;
        }

        void ContextViewer_Load(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = true;
            splitContainer1.SplitterDistance = 500;

            PopulateTreeView();

            RichtextboxHighlights();

            tsslStatus.Text = "Ready";
        }

        void t()
        {
            string[] lines = this.JSON.Split(new string[] { Environment.NewLine }, StringSplitOptions.None );
            
            foreach (string line in lines)
            {
                bool highlighted = false;
                foreach (var coloredWord in this.HighlightKeyWords)
                {
                    //int wordstartIndex = richTextBox1.Find(coloredWord.word, 0, RichTextBoxFinds.None);
                    if (line.IndexOf(coloredWord.Word) != -1)
                    {
                        highlighted = true;
                        richTextBox1.SelectionColor = coloredWord.Color;
                        richTextBox1.SelectionFont = new Font("Arial", 10, coloredWord.Bold ? FontStyle.Bold : FontStyle.Regular);
                        richTextBox1.SelectedText = $"{line}{Environment.NewLine}";
                        break;
                    }
                }

                if (!highlighted)
                {
                    //richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.SelectionFont = new Font("Arial", 10);
                    richTextBox1.SelectedText = $"{line}{Environment.NewLine}";
                }

            }
        }

        void RichtextboxHighlights()
        {
            t();
            return;
            if (this.HighlightKeyWords.Any())
            {
                //foreach (var line in richTextBox1.Lines)
                //{

                //}
                foreach (var coloredWord in this.HighlightKeyWords)
                {
                    var matches = richTextBox1.Lines.Where(l => l.Contains(coloredWord.Word)).ToList();


                    matches.ForEach(m => 
                        {
                        int wordstartIndex = richTextBox1.Find(m, 0, RichTextBoxFinds.None);
                            if (wordstartIndex != -1)
                            {
                                richTextBox1.SelectionBackColor = Color.Red;
                            }
                            //richTextBox1.SelectedText = m; 
                            // 
                        });

                    
                }

            }
        }

        void RichtextboxHighlights2()
        {
            if (this.HighlightKeyWords.Any())
            {
                foreach (var coloredWord in this.HighlightKeyWords)
                {
                    int startindex = 0;
                    while (startindex < richTextBox1.TextLength)
                    {
                        int wordstartIndex = richTextBox1.Find(coloredWord.Word, startindex, RichTextBoxFinds.None);
                        if (wordstartIndex != -1)
                        {
                            var crlfPos = richTextBox1.Find("\n", startindex, RichTextBoxFinds.None);

                            richTextBox1.SelectionStart = wordstartIndex;
                            richTextBox1.SelectionLength = coloredWord.Word.Length;
                            richTextBox1.SelectionBackColor = coloredWord.Color;
                            richTextBox1.SelectionColor = Color.White;
                            
                        }
                        else
                            break;
                        startindex += wordstartIndex + coloredWord.Word.Length;
                    }
                }
            }
        }

        void PopulateTreeView()
        {
            tvJsonExplorer.Nodes.Clear();

            try
            {
                LoadJsonToTreeView(tvJsonExplorer, this.JSON);
            }
            catch (ArgumentException argE)
            {
                tsslStatus.Text = "Error parsing JSON";
            }
        }

        void LoadJsonToTreeView(TreeView treeView, string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "[]" || json == "null") return;

            //var arr = JArray.Parse(json);
            //AddArrayNodes(arr, RootName ?? "JSON", treeView.Nodes);

            var @object = JObject.Parse(json);
            AddObjectNodes(@object, "JSON", treeView.Nodes);

            treeView?.Nodes[0]?.Expand();
            treeView?.Nodes[0]?.Nodes[0]?.Expand();
        }

        void AddObjectNodes(JObject @object, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            foreach (var property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, node.Nodes);
            }
        }

        void AddArrayNodes(JArray array, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            for (var i = 0; i < array.Count; i++)
            {
                AddTokenNodes(array[i], $"[{i}]", node.Nodes);
            }
        }

        void AddTokenNodes(JToken token, string name, TreeNodeCollection parent)
        {
            if (token is JValue)
            {
                parent.Add(new TreeNode($"{name}: {((JValue)token).Value}"));
            }
            else if (token is JArray)
            {
                AddArrayNodes((JArray)token, name, parent);
            }
            else if (token is JObject)
            {
                AddObjectNodes((JObject)token, name, parent);
            }
            
        }

        private void btnSearchByID_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    // Can be removed if going back to .Net 4.72
    public class HighlightKeyWord
    {
        public string Word { get; set; }
        public Color Color { get; set; }
        public bool Bold { get; set; }

        public HighlightKeyWord(string word, Color color, bool bold)
        {
            this.Word = word;
            this.Color = color;
            this.Bold = bold;
        }
    }

}
