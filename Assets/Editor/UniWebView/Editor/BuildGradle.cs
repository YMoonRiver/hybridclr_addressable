using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class UniWebViewGradleConfig
{
    private UniWebViewGradleNode m_root;
    private String m_filePath;
    private UniWebViewGradleNode m_curNode;

    public UniWebViewGradleNode ROOT
    {
        get { return m_root; }
    }

    public UniWebViewGradleConfig(string filePath)
    {
        string file = File.ReadAllText(filePath);
        TextReader reader = new StringReader(file);

        m_filePath = filePath;
        m_root = new UniWebViewGradleNode("root");
        m_curNode = m_root;

        StringBuilder str = new StringBuilder();
        bool inDoubleQuote = false;
        bool inSingleQuote = false;

        while (reader.Peek() > 0)
        {
            char c = (char)reader.Read();
            switch (c)
            {
                // case '/':
                //     if (reader.Peek() == '/')
                //     {
                //         reader.Read();
                //         string comment = reader.ReadLine();
                //         Debug.Log("Comment line: " + comment);
                //         m_curNode.AppendChildNode(new UniWebViewGradleCommentNode(comment, m_curNode));
                //     }
                //     else
                //     {
                //         str.Append('/');
                //     }
                //     break;
                case '\n':
                    {
                        var strf = FormatStr(str);
                        if (!string.IsNullOrEmpty(strf))
                        {
                            m_curNode.AppendChildNode(new UniWebViewGradleContentNode(strf, m_curNode));
                        }
                    }
                    str = new StringBuilder();
                    break;
                case '\r':
                    break;
                case '\t':
                    break;
                case '{':
                    {
                        if (inDoubleQuote || inSingleQuote) {
                            break;
                        }
                        var n = FormatStr(str);
                        if (!string.IsNullOrEmpty(n))
                        {
                            UniWebViewGradleNode node = new UniWebViewGradleNode(n, m_curNode);
                            m_curNode.AppendChildNode(node);
                            m_curNode = node;
                        }
                    }
                    str = new StringBuilder();
                    break;
                case '}':
                    {
                        if (inDoubleQuote || inSingleQuote) {
                            break;
                        }
                        var strf = FormatStr(str);
                        if (!string.IsNullOrEmpty(strf))
                        {
                            m_curNode.AppendChildNode(new UniWebViewGradleContentNode(strf, m_curNode));
                        }
                        m_curNode = m_curNode.PARENT;
                    }
                    str = new StringBuilder();
                    break;
                case '\"':
                    inDoubleQuote = !inDoubleQuote;
                    str.Append(c);
                    break;
                case '\'':
                    inSingleQuote = !inSingleQuote;
                    str.Append(c);
                    break;
                default:
                    str.Append(c);
                    break;
            }
        }

        //Debug.Log("Gradle parse done!");
    }

    public void Save(string path = null)
    {
        if (path == null)
            path = m_filePath;
        File.WriteAllText(path, Print());
        //Debug.Log("Gradle parse done! " + path);
    }

    private string FormatStr(StringBuilder sb)
    {
        string str = sb.ToString();
        str = str.Trim();
        return str;
    }
    public string Print()
    {
        StringBuilder sb = new StringBuilder();
        printNode(sb, m_root, -1);
        return sb.ToString();
    }
    private string GetLevelIndent(int level)
    {
        if (level <= 0) return "";
        StringBuilder sb = new StringBuilder("");
        for (int i = 0; i < level; i++)
        {
            sb.Append('\t');
        }
        return sb.ToString();
    }
    private void printNode(StringBuilder stringBuilder, UniWebViewGradleNode node, int level)
    {
        if (node.PARENT != null)
        {
            if (node is UniWebViewGradleCommentNode)
            {
                stringBuilder.Append("\n" + GetLevelIndent(level) + @"//" + node.NAME);
            }
            else
            {
                stringBuilder.Append("\n" + GetLevelIndent(level) + node.NAME);
            }

        }

        if (!(node is UniWebViewGradleContentNode) && !(node is UniWebViewGradleCommentNode))
        {
            if (node.PARENT != null)
            {
                stringBuilder.Append(" {");
            }
            foreach (var c in node.CHILDREN)
            {
                printNode(stringBuilder, c, level + 1);
            }
            if (node.PARENT != null)
            {
                stringBuilder.Append("\n" + GetLevelIndent(level) + "}");
            }
        }
    }
}

public class UniWebViewGradleNode
{
    protected List<UniWebViewGradleNode> m_children = new List<UniWebViewGradleNode>();
    protected UniWebViewGradleNode m_parent;
    protected String m_name;
    public UniWebViewGradleNode PARENT
    {
        get { return m_parent; }
    }

    public string NAME
    {
        get { return m_name; }
    }

    public List<UniWebViewGradleNode> CHILDREN
    {
        get { return m_children; }
    }

    public UniWebViewGradleNode(string name, UniWebViewGradleNode parent = null)
    {
        m_parent = parent;
        m_name = name;
    }

    public void Each(Action<UniWebViewGradleNode> f)
    {
        f(this);
        foreach (var n in m_children)
        {
            n.Each(f);
        }
    }

    public void AppendChildNode(UniWebViewGradleNode node)
    {
        if (m_children == null) m_children = new List<UniWebViewGradleNode>();
        m_children.Add(node);
        node.m_parent = this;
    }

    public UniWebViewGradleNode TryGetNode(string path)
    {
        string[] subpath = path.Split('/');
        UniWebViewGradleNode cnode = this;
        for (int i = 0; i < subpath.Length; i++)
        {
            var p = subpath[i];
            if (string.IsNullOrEmpty(p)) continue;
            UniWebViewGradleNode tnode = cnode.FindChildNodeByName(p);
            if (tnode == null)
            {
                Debug.Log("Can't find Node:" + p);
                return null;
            }

            cnode = tnode;
            tnode = null;
        }

        return cnode;
    }

    public UniWebViewGradleNode FindChildNodeByName(string name)
    {
        foreach (var n in m_children)
        {
            if (n is UniWebViewGradleCommentNode || n is UniWebViewGradleContentNode)
                continue;
            if (n.NAME == name)
                return n;
        }
        return null;
    }

    public bool ReplaceContenStartsWith(string patten, string value)
    {
        foreach (var n in m_children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name.StartsWith(patten))
            {
                n.m_name = value;
                return true;
            }
        }
        return false;
    }

    public UniWebViewGradleContentNode ReplaceContenOrAddStartsWith(string patten, string value)
    {
        foreach (var n in m_children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name.StartsWith(patten))
            {
                n.m_name = value;
                return (UniWebViewGradleContentNode)n;
            }
        }
        return AppendContentNode(value);
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public UniWebViewGradleContentNode AppendContentNode(string content)
    {
        foreach (var n in m_children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name == content)
            {
                Debug.Log("UniWebViewGradleContentNode with " + content + " already exists!");
                return null;
            }
        }
        UniWebViewGradleContentNode cnode = new UniWebViewGradleContentNode(content, this);
        AppendChildNode(cnode);
        return cnode;
    }


    public bool RemoveContentNode(string contentPattern)
    {
        for(int i=0;i<m_children.Count;i++)
        {
            if (!(m_children[i] is UniWebViewGradleContentNode)) continue;
            if(m_children[i].m_name.Contains(contentPattern))
            {
                m_children.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}

public sealed class UniWebViewGradleContentNode : UniWebViewGradleNode
{
    public UniWebViewGradleContentNode(String content, UniWebViewGradleNode parent) : base(content, parent)
    {

    }

    public void SetContent(string content)
    {
        m_name = content;
    }
}

public sealed class UniWebViewGradleCommentNode : UniWebViewGradleNode
{
    public UniWebViewGradleCommentNode(String content, UniWebViewGradleNode parent) : base(content, parent)
    {

    }

    public string GetComment()
    {
        return m_name;
    }
}

