namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Control that traverses an object returned from a protocol using reflection and shows it in tree view with
    /// detailed information shown separately in two-column list view.
    /// </summary>
    public sealed partial class ResultsViewer : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsViewer"/> class.
        /// </summary>
        public ResultsViewer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Displays a list of objects recursively traversing each one using reflection and adding information into tree view.
        /// </summary>
        /// <param name="results">Objects that are to be displayed.</param>
        public void DisplayResults(List<object> results) =>
            this.resultsView.Nodes.AddRange(this.DisplayResultsRecursive(results).ToArray());

        /// <summary>
        /// Clears a view.
        /// </summary>
        public void Clear()
        {
            this.resultsView.Nodes.Clear();
            this.detailedInfoView.Items.Clear();
        }

        /// <summary>
        /// Shows human-readable string representation for some known types that are not properly processed
        /// by DisplayResultsRecursive.
        /// </summary>
        /// <param name="element">An object to be displayed.</param>
        /// <returns>Human-readable string for that object.</returns>
        private static string GetDisplayedName(object element)
        {
            Func<string, string, string> displayedNameFor =
                (typeName, mainPropertyName) =>
                element.GetType().Name == typeName
                    ? element.GetType()
                          .GetProperties()
                          .FirstOrDefault(p => p.Name == mainPropertyName)?.GetValue(element, null)
                          .ToString()
                    : null;

            return displayedNameFor("File", "Name") ?? displayedNameFor("Comment", "Content") ?? element.GetType().Name;
        }

        /// <summary>
        /// Adds tree nodes as children for given parent, or, when possible, to a Tag of a parent to be shown in detailed view later.
        /// </summary>
        /// <param name="children">Children tree nodes to be sorted.</param>
        /// <param name="node">Parent node.</param>
        private static void SortOutChildren(IEnumerable<TreeNode> children, TreeNode node)
        {
            var treeNodes = children as TreeNode[] ?? children.ToArray();
            var forDetailedInfo = treeNodes.Where(ResultsViewer.IsSuitableForDetailedInfo);
            node.Nodes.AddRange(treeNodes.Where(c => !ResultsViewer.IsSuitableForDetailedInfo(c)).ToArray());
            if (forDetailedInfo.Any())
            {
                node.Tag = forDetailedInfo;
            }
        }

        /// <summary>
        /// Adds tree nodes as children for given dictionary, or, when possible, to a Tag of a dictionary to be shown in detailed view.
        /// Dictionary is a special case because we want to show all children as tree items or all children in detailed view
        /// simultaneously, not half of them there and half there.
        /// </summary>
        /// <param name="children">Children tree nodes to be sorted.</param>
        /// <param name="node">Parent node.</param>
        private static void SortOutChildrenForDictionary(IEnumerable<TreeNode> children, TreeNode node)
        {
            var treeNodes = children as TreeNode[] ?? children.ToArray();
            if (treeNodes.Any() && treeNodes.All(ResultsViewer.IsSuitableForDetailedInfo))
            {
                node.Tag = children;
            }
            else
            {
                node.Nodes.AddRange(treeNodes.ToArray());
            }
        }

        /// <summary>
        /// Predicate that tells if a node can be shown as detailed info - i.e. it is a simple key-value pair without detailed
        /// info itself.
        /// </summary>
        /// <param name="child">A node that needs to be checked.</param>
        /// <returns>True, if this node is good for detailed info view.</returns>
        private static bool IsSuitableForDetailedInfo(TreeNode child)
            => (child.Nodes.Count == 1) && (child.Nodes[0].Nodes.Count == 0) && (child.Tag == null) && (child.Nodes[0].Tag == null);

        /// <summary>
        /// Returns a sequence of tree nodes that show contents of given object.
        /// </summary>
        /// <param name="nodeToDisplay">Object that needs to be displayed.</param>
        /// <returns>A sequence of tree nodes corresponding to an object.</returns>
        private IList<TreeNode> DisplayResultsRecursive(object nodeToDisplay)
        {
            if (nodeToDisplay == null)
            {
                return new List<TreeNode>();
            }

            if (nodeToDisplay is string || nodeToDisplay is DateTime)
            {
                return new List<TreeNode> { new TreeNode(nodeToDisplay.ToString()) };
            }

            var jObject = nodeToDisplay as JObject;
            if (jObject != null)
            {
                var result = new List<TreeNode>();
                foreach (var children in jObject.Properties().Select(this.DisplayResultsRecursive))
                {
                    result.AddRange(children);
                }

                return result;
            }

            var jProperty = nodeToDisplay as JProperty;
            if (jProperty != null)
            {
                var node = new TreeNode(jProperty.Name);
                var children = this.DisplayResultsRecursive(jProperty.Value);
                ResultsViewer.SortOutChildren(children, node);
                return new List<TreeNode> { node };
            }

            var jValue = nodeToDisplay as JValue;
            if (jValue != null)
            {
                return jValue.Value != null ? new List<TreeNode> { new TreeNode(jValue.Value.ToString()) } : new List<TreeNode>();
            }

            var iDictionary = nodeToDisplay as IDictionary;
            if (iDictionary != null)
            {
                var result = new List<TreeNode>();
                foreach (var element in iDictionary.Keys)
                {
                    var children = this.DisplayResultsRecursive(iDictionary[element]);
                    var node = new TreeNode(element.ToString());
                    ResultsViewer.SortOutChildren(children, node);
                    result.Add(node);
                }

                return result;
            }

            var iEnumerable = nodeToDisplay as IEnumerable;
            if (iEnumerable != null)
            {
                var result = new List<TreeNode>();
                foreach (var element in iEnumerable)
                {
                    var children = this.DisplayResultsRecursive(element);
                    if (children.Count > 1)
                    {
                        var node = new TreeNode(ResultsViewer.GetDisplayedName(element));
                        ResultsViewer.SortOutChildren(children, node);
                        result.Add(node);
                    }
                    else if (children.Count == 1)
                    {
                        result.Add(children.First());
                    }
                }

                return result;
            }

            if (nodeToDisplay.GetType().GetProperties().Any())
            {
                var result = new List<TreeNode>();

                foreach (var property in nodeToDisplay.GetType().GetProperties())
                {
                    var propertyValue = nodeToDisplay.GetType().GetProperty(property.Name).GetValue(nodeToDisplay, null);
                    var children = this.DisplayResultsRecursive(propertyValue);
                    if (!children.Any())
                    {
                        // Property is not filled, just dropping it
                        continue;
                    }

                    var node = new TreeNode(property.Name);
                    if (propertyValue is IDictionary)
                    {
                        ResultsViewer.SortOutChildrenForDictionary(children, node);
                    }
                    else
                    {
                        ResultsViewer.SortOutChildren(children, node);
                    }

                    result.Add(node);
                }

                return result;
            }

            return new List<TreeNode> { new TreeNode(nodeToDisplay.ToString()) };
        }

        /// <summary>
        /// Handler for click on an item in results view. Populates detailed view.
        /// </summary>
        /// <param name="sender">Control that sent the signal.</param>
        /// <param name="e">Event arguments.</param>
        private void OnResultsViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.detailedInfoView.Items.Clear();
            if (this.resultsView.SelectedNode?.Tag == null)
            {
                return;
            }

            Action<string, string> item = (name, text) => this.detailedInfoView.Items.Add(new ListViewItem(new[] { name, text }));
            var tag = this.resultsView.SelectedNode.Tag as IEnumerable<TreeNode>;

            if (tag == null)
            {
                return;
            }

            foreach (var node in tag)
            {
                item(node.Text, node.Nodes[0].Text);
            }
        }
    }
}
