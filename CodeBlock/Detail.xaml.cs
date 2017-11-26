using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeBlock
{
    /// <summary>
    /// Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Detail : Window
    {
        class ImportantPos : IComparable
        {
            public int NodeId { get; set; }
            public int Pos { get; set; }
            public bool IsStart { get; set; }
            public int CompareTo(object obj)
            {
                ImportantPos ip = obj as ImportantPos;
                return Pos.CompareTo(ip.Pos);
            }
        }
        class OneNode
        {
            public int Father { get; set; }
            public string FatherLinkType { get; set; }
            public int LineNumber { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
            public int OneLineLeft { get; set; }
            public int OneLineRight { get; set; }
            public int TypeMacro { get; set; }
            public string TypeName { get; set; }
        };

        class RenderLayer
        {
            public Brush Color { get; private set; }
            public string Text { get; private set; }
            public RenderLayer(Color inputColor, string inputText)
            {
                Color = new SolidColorBrush(inputColor);
                Text = inputText;
            }
        }
        ObservableCollection<RenderLayer> currentRenderLayers = new ObservableCollection<RenderLayer>();
        List<OneNode> nodes;

        List<StackPanel> smallstackpanels = new List<StackPanel>();
        List<List<TextBlock>> textblocks = new List<List<TextBlock>>();
        int TotalNodeNum { get; set; }
        public Detail(string code)
        {
            InitializeComponent();
            resultView.ItemsSource = currentRenderLayers;
            colorlist = new List<Brush>
            {
                Brushes.Green,
                Brushes.Orange,
                Brushes.Pink,
                Brushes.Purple,
                Brushes.Blue
            };
            Random rnd = new Random();
            int rand = 0;

            /*
            使用tempstackpanellist处理临时stackpanel的回收问题
            节点范围跨行，即当做方框型
            树节点数据结构：father, linenum, left, right
                linenum为-1，此为跨行节点，left到right行
                否则不跨行，linenum行第left到right的token
            */
            var input = File.ReadAllLines("test17-space.out");
            TotalNodeNum = Convert.ToInt32(input[0]);
            nodes = new List<OneNode>();
            for (int i = TotalNodeNum; i > 0; i--)
                nodes.Add(new OneNode() { Father = -1 });
            List<ImportantPos> poslist = new List<ImportantPos>();
            for (int i = 1; i < input.Length; i++)
            {
                var part = Regex.Split(input[i], @"\s");
                int nowpartpos = 6;
                int nowid = Convert.ToInt32(part[0]);
                nodes[nowid].TypeMacro = Convert.ToInt32(part[1]);
                nodes[nowid].TypeName = part[2];
                int startpos = Convert.ToInt32(part[3]);
                int endpos = startpos + Convert.ToInt32(part[4]);
                ImportantPos ipstart = new ImportantPos()
                {
                    IsStart = true,
                    NodeId = nowid,
                    Pos = startpos
                };
                ImportantPos ipend = new ImportantPos()
                {
                    IsStart = false,
                    NodeId = nowid,
                    Pos = endpos
                };
                poslist.Add(ipstart);
                poslist.Add(ipend);
                int childnum = Convert.ToInt32(part[5]);
                for (; childnum > 0; childnum--)
                {
                    int childid = Convert.ToInt32(part[nowpartpos++]);
                    string childlinestring = part[nowpartpos++];
                    nodes[childid].Father = nowid;
                    nodes[childid].FatherLinkType = childlinestring;
                }
            }
            //TODO: poslist sort
            poslist.Sort();
            int codeposoffset = 0, nowpos = 0, nowcodeline = 0;
            var codeslinesplit = Regex.Split(code, @"\n");
            int[] codebelong = new int[code.Length];
            int[] linestartpos = new int[codeslinesplit.Length + 1];
            linestartpos[codeslinesplit.Length] = code.Length;
            for (int i = 0; i < codebelong.Length; i++)
                codebelong[i] = -1;
            for (;;)
            {
                linestartpos[nowcodeline] = codeposoffset;
                if (nowpos >= poslist.Count)
                    break;
                int start = codeposoffset, end = codeposoffset + codeslinesplit[nowcodeline].Length + 1;
                for (; nowpos < poslist.Count && poslist[nowpos].Pos <= end; nowpos++)
                {
                    var pos = poslist[nowpos];
                    if (pos.IsStart)
                    {
                        nodes[pos.NodeId].LineNumber = nowcodeline;
                        nodes[pos.NodeId].Left = pos.Pos - codeposoffset;
                        nodes[pos.NodeId].OneLineLeft = pos.Pos;
                    }
                    else
                    {
                        nodes[pos.NodeId].OneLineRight = pos.Pos;
                        for (int i = nodes[pos.NodeId].OneLineLeft; i < nodes[pos.NodeId].OneLineRight; i++)
                            if (codebelong[i] == -1)
                                codebelong[i] = pos.NodeId;
                        if (nowcodeline == nodes[pos.NodeId].LineNumber)
                            nodes[pos.NodeId].Right = pos.Pos - codeposoffset;
                        else
                        {
                            nodes[pos.NodeId].Left = nodes[pos.NodeId].LineNumber;
                            nodes[pos.NodeId].LineNumber = -1;
                            nodes[pos.NodeId].Right = nowcodeline + 1;
                        }
                    }
                }
                codeposoffset = end;
                nowcodeline++;
            }
            foreach (var pos in poslist)
                if (!pos.IsStart)
                    for (int i = nodes[pos.NodeId].OneLineLeft; i < nodes[pos.NodeId].OneLineRight; i++)
                        if (codebelong[i] == -1)
                            codebelong[i] = pos.NodeId;
            for (int i = 0; i < codeslinesplit.Length; i++)
            {
                StackPanel sp = new StackPanel();
                List<TextBlock> ltb = new List<TextBlock>();
                bool allspace = true;
                int offset = linestartpos[i];
                int lastcolor = offset < codebelong.Length ? codebelong[offset] : -2, startpos = offset;
                for (int j = offset; j < linestartpos[i + 1]; j++)
                {
                    if (codebelong[j] != lastcolor || j == linestartpos[i + 1] - 1 || (allspace && j != startpos))
                    {
                        TextBlock tb = new TextBlock()
                        {
                            Text = code.Substring(startpos, j - startpos),
                            Tag = lastcolor,
                            FontFamily = new FontFamily("Courier New")
                        };
                        //tb.Background = colorlist[rnd.Next() % colorlist.Count];
                        //tb.Background = colorlist[rand];
                        tb.MouseMove += TextBlock_MouseMove;
                        rand = (rand + 1) % colorlist.Count;
                        ltb.Add(tb);
                        sp.Children.Add(tb);
                        lastcolor = codebelong[j];
                        startpos = j;
                    }
                    if (code[j] != ' ')
                        allspace = false;
                }
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 22;
                sp.HorizontalAlignment = HorizontalAlignment.Left;
                smallstackpanels.Add(sp);
                textblocks.Add(ltb);
            }
            foreach (var i in smallstackpanels)
                BigStackPanel.Children.Add(i);
            //RandBlock(ref BigStackPanel, 0, smallstackpanels.Count, 0);
        }
        int NowLeafNum = -1, MouseMoveTimes = 0;
        List<int> MouseMoveList = new List<int>();
        List<StackPanel> tempstackpanel = new List<StackPanel>();
        void ReturnOriginal()
        {
            foreach (var list in textblocks)
                foreach (var tb in list)
                {
                    tb.Background = Brushes.Transparent;
                    tb.Visibility = Visibility.Visible;
                }
            foreach (var sp in tempstackpanel)
                sp.Children.Clear();
            tempstackpanel.Clear();
            BigStackPanel.Children.Clear();
        }
        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (MouseMoveTimes == 0)
                for (int i = 0; i < TotalNodeNum; i++)
                    MouseMoveList.Add(0);
            MouseMoveTimes++;
            */

            int leaf = (int)(sender as TextBlock).Tag;
            if (leaf == -1) return;
            e.Handled = true;

            ReturnOriginal();

            List<int> chain = new List<int>
            {
                leaf
            };
            for (int i = leaf; ;)
            {
                if (nodes[i].Father == -1) break;
                chain.Add(nodes[i].Father);
                i = nodes[i].Father;
            }
            //btn.Content = MouseMoveTimes;
            double delta = 255.0 / chain.Count;
            double nowcolor = 0;
            StackPanel nowfathersp = BigStackPanel;
            int codelineleft = 0, codelineright = smallstackpanels.Count, movedspaces = 0;
            string[] colorNames = { "#CCFFFF", "#CCFFEB", "#CCFFD6", "#E0FFCC", "#FFF5CC", "#FFFFCC", "#FFEBCC", "#FFCCCC", "#FFCCE0", "#FFCCF5", "#EBCCFF", "#D6CCFF", "#E6E6E6" };
            Color[] colorScheme = colorNames.Select(s => (Color)ColorConverter.ConvertFromString(s)).ToArray();
            currentRenderLayers.Clear();
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                nowcolor += delta;
                Color c = colorScheme[(chain.Count - 1 - i) % colorScheme.Length];
                currentRenderLayers.Add(new RenderLayer(c, nodes[chain[i]].TypeName));
                //c.A = c.R = 255;
                //c.B = c.G = (byte)(255 - nowcolor);
                Brush brush = new SolidColorBrush(c);
                /*MouseMoveList[chain[i]] = MouseMoveTimes;
                foreach (var list in textblocks)
                    foreach (var tb in list)
                    {
                        if ((int)tb.Tag == chain[i])
                        {
                            Color c = new Color();
                            c.A = c.R = 255;
                            c.B = c.G = (byte)(255 - nowcolor);
                            tb.Background = new SolidColorBrush(c);
                        }
                        else if (MouseMoveList[chain[i]] != MouseMoveTimes)
                        {
                            tb.Background = Brushes.Transparent;
                        }
                    }*/
                if (nodes[chain[i]].LineNumber == -1)
                {
                    StackPanel newsp = new StackPanel();
                    tempstackpanel.Add(newsp);
                    newsp.Orientation = Orientation.Horizontal;
                    for (int j = codelineleft; j < nodes[chain[i]].Left; j++)
                        nowfathersp.Children.Add(smallstackpanels[j]);
                    nowfathersp.Children.Add(newsp);
                    for (int j = nodes[chain[i]].Right; j < codelineright; j++)
                        nowfathersp.Children.Add(smallstackpanels[j]);
                    codelineleft = nodes[chain[i]].Left;
                    codelineright = nodes[chain[i]].Right;

                    for (;;)
                    {
                        bool allspace = true;
                        for (int j = codelineleft; j < codelineright; j++)
                            if (textblocks[j].Count <= movedspaces || textblocks[j][movedspaces].Text != " ")
                                allspace = false;
                        if (!allspace) break;
                        for (int j = codelineleft; j < codelineright; j++)
                            textblocks[j][movedspaces].Visibility = Visibility.Collapsed;
                        movedspaces++;
                        TextBlock tb = new TextBlock();
                        tb.Text = " ";
                        tb.FontFamily = new FontFamily("Courier New");
                        newsp.Children.Add(tb);
                    }
                    
                    StackPanel newsp2 = new StackPanel();
                    tempstackpanel.Add(newsp2);
                    newsp.Children.Add(newsp2);
                    newsp2.Background = brush;
                    nowfathersp = newsp2;
                }
                else
                {
                    for (int j = codelineleft; j < codelineright; j++)
                        nowfathersp.Children.Add(smallstackpanels[j]);
                    codelineright = codelineleft;
                    for (int j = 0, k = 0; k < nodes[chain[i]].Right; j++)
                    {
                        if (nodes[chain[i]].Left <= k)
                        textblocks[nodes[chain[i]].LineNumber][j].Background = brush;
                        k += textblocks[nodes[chain[i]].LineNumber][j].Text.Length;
                    }
                }
            }
            for (int j = codelineleft; j < codelineright; j++)
                nowfathersp.Children.Add(smallstackpanels[j]);
        }

        List<Brush> colorlist;
        void RandBlock(ref StackPanel fasp, int start, int end, int color)
        {
            //MessageBox.Show(start + " " + end + " " + color);
            fasp.Background = colorlist[color];
            for (int i = start; i < end; i++)
                smallstackpanels[i].Background = colorlist[color];
            color++;
            color %= colorlist.Count;
            fasp.Children.Clear();
            Random rnd = new Random();
            /*
            if (start == 0 && end == smallstackpanels.Count)
                for (int i = 0; i < smallstackpanels.Count; i++)
                    smallstackpanels[i].Background = Brushes.White;
            */
            if (start + 1 == end)
            {
                fasp.Children.Add(smallstackpanels[start]);
                smallstackpanels[start].Background = colorlist[color++];
                color %= colorlist.Count;
                (smallstackpanels[start].Children[rnd.Next() % smallstackpanels[start].Children.Count] as TextBlock).Background = colorlist[color];
                return;
            }
            int left, right;
            for (;;)
            {
                left = rnd.Next() % (end - start + 1) + start;
                right = rnd.Next() % (end - start + 1) + start;
                if (left < right && left != right) break;
            }
            for (int i = start; i < left; i++)
                fasp.Children.Add(smallstackpanels[i]);
            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            RandBlock(ref sp, left, right, color);
            fasp.Children.Add(sp);
            for (int i = right; i < end; i++)
                fasp.Children.Add(smallstackpanels[i]);
        }

        private void BigStackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ReturnOriginal();
            for (int j = 0; j < smallstackpanels.Count; j++)
                BigStackPanel.Children.Add(smallstackpanels[j]);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            Random r = new Random();
            foreach (var i in smallstackpanels)
                i.Background = l[r.Next() % 3];
            foreach (var line in textblocks)
                foreach (var i in line)
                    i.Background = l[r.Next() % 3];
            */
            foreach (var l in textblocks)
                foreach (var tb in l)
                    if (Regex.IsMatch(tb.Text, @"^\s*$"))
                        if (tb.Visibility == Visibility.Collapsed)
                            tb.Visibility = Visibility.Visible;
                        else tb.Visibility = Visibility.Collapsed;
        }
    }
}
