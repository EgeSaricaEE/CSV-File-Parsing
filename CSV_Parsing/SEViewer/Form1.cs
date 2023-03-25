using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Xml;
using System.Collections;
using Microsoft.VisualBasic;

namespace SEViewer
{
    public partial class Form1 : Form
    {
        struct scheduleInfo
        {
            public bool start;
            public uint timestamp;
        }
        struct runInfo
        {
            public string runname;
            public string runID;
        }
        SortedDictionary<string, List<scheduleInfo>> testData = new SortedDictionary<string, List<scheduleInfo>>();
        SortedDictionary<string, List<scheduleInfo>> testData2 = new SortedDictionary<string, List<scheduleInfo>>();
        SortedDictionary<string, List<scheduleInfo>> testData3 = new SortedDictionary<string, List<scheduleInfo>>();
        SortedDictionary<string, List<runInfo>> my_runnable3 = new SortedDictionary<string, List<runInfo>>();
        Chart chart1;
        // Chart chart1 = new Chart();
  

        public Form1()
        {
            InitializeComponent();
            //Make boxes invisible at first
            checkedListBox1.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            checkedListBox3.Visible = false;
            textBox4.Visible = false;
            checkedListBox2.Visible = false;

            listcollection.Clear();
            foreach(string str in checkedListBox1.Items)
            {
                listcollection.Add(str);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Setting the boxes into Tab Control Pages
            textBox2.Parent = tabControl1.TabPages["tabPage1"];
            checkedListBox2.Parent = tabControl1.TabPages["tabPage2"];
            checkedListBox2.Dock = DockStyle.Top;
            checkedListBox1.Dock = DockStyle.Top;
            textBox4.Dock = DockStyle.Bottom;
            textBox2.Dock = DockStyle.Bottom;
            textBox4.Parent = tabControl1.TabPages["tabPage2"];
            checkedListBox1.Parent = tabControl1.TabPages["tabPage1"];
            checkedListBox1.Height = tabPage1.Height - textBox2.Height - 5;
            checkedListBox2.Height = tabPage2.Height - textBox4.Height - 5;
            textBox1.Parent = tabControl1.TabPages["tabPage3"];
            checkedListBox3.Parent = tabControl1.TabPages["tabPage3"];
            textBox1.Dock = DockStyle.Bottom;
            checkedListBox3.Dock = DockStyle.Top;
            checkedListBox3.Height = tabPage3.Height - textBox1.Height - 5;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //This Click Event is for Opening and Parsing XML File


            var openDialog3 = new OpenFileDialog();
            openDialog3.Multiselect = false;
            openDialog3.Filter = "XML Files|*.xml";

            DialogResult result3 = openDialog3.ShowDialog(this);
            if (result3 == DialogResult.OK)
            {
                //parsing xml file
                runInfo rInfo3 = new runInfo();
                var filename3 = openDialog3.FileName;
                XmlReader reader3 = XmlReader.Create(filename3);
                while (reader3.Read())
                {
                    if ((reader3.NodeType == XmlNodeType.Element) && (reader3.Name == "runnable"))
                    {
                        if (reader3.HasAttributes)
                        {
                            rInfo3.runname = reader3.GetAttribute("name");
                            rInfo3.runID = reader3.GetAttribute("ID");
                            List<runInfo> runInfoList3 = new List<runInfo>();
                            runInfoList3.Add(rInfo3);
                            my_runnable3.Add(reader3.GetAttribute("ID"), runInfoList3);
                        }
                    }
                }
            }

        }

        private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            //Check selected chart element is a data point and set tooltip text
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                //Get selected data point
                DataPoint dataPoint = (DataPoint)e.HitTestResult.Object;

                e.Text = "Time is: " + dataPoint.XValue.ToString() + " ms";

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Width = this.Width - panel2.Width;
            panel1.Height = this.Height;
            panel2.Height = this.Height;
            panel2.Location = new Point(panel1.Width, panel2.Location.Y);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Red;
            
        }



        //Displaying the Checked Runnables on Chart
        private void button1_Click(object sender, EventArgs e)
        {
            if (null == chart1)
            {
                chart1 = new Chart();
                chart1.ChartAreas.Add("SE");
                chart1.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart1_GetToolTipText);
                this.Controls.Add(chart1);
                Legend legend = new Legend("Legend");
                chart1.Legends.Add(legend);
            }

            int series_count = chart1.Series.Count;
            for (int i = series_count; i > 0; i--)
            {
                chart1.Series.Remove(chart1.Series[i - 1]);
            }


            int count = 1;

            foreach (string item in checkedListBox1.CheckedItems)
            {
                var series = new Series(item);
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 3;
                series.Points.AddXY(0, count);
                
                foreach (scheduleInfo my_data in testData[item])
                {
                    if (my_data.start)
                    {
                        series.Points.AddXY(my_data.timestamp, count);
                        series.Points.AddXY(my_data.timestamp + 1, count + 1);
                    }
                    else
                    {
                        series.Points.AddXY(my_data.timestamp, count + 1);
                        series.Points.AddXY(my_data.timestamp + 1, count);
                    }
                }
                chart1.Series.Add(series);
                count += 2;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 400000);
                chart1.Parent = panel1;
                chart1.Dock = DockStyle.Fill;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
               

                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            }

            foreach (string item2 in checkedListBox2.CheckedItems)
            {
                var series = new Series(item2);
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 3;
                series.Points.AddXY(0, count);

                foreach (scheduleInfo my_data2 in testData2[item2])
                {
                    if (my_data2.start)
                    {
                        series.Points.AddXY(my_data2.timestamp, count);
                        series.Points.AddXY(my_data2.timestamp + 1, count + 1);
                    }
                    else
                    {
                        series.Points.AddXY(my_data2.timestamp, count + 1);
                        series.Points.AddXY(my_data2.timestamp + 1, count);
                    }
                }
                chart1.Series.Add(series);
                count += 2;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 400000);
                chart1.Parent = panel1;
                chart1.Dock = DockStyle.Fill;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            }
            foreach (string item in checkedListBox3.CheckedItems)
            {
                var series = new Series(item);
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 3;
                series.Points.AddXY(0, count);

                foreach (scheduleInfo my_data in testData3[item])
                {
                    if (my_data.start)
                    {
                        series.Points.AddXY(my_data.timestamp, count);
                        series.Points.AddXY(my_data.timestamp + 1, count + 1);
                    }
                    else
                    {
                        series.Points.AddXY(my_data.timestamp, count + 1);
                        series.Points.AddXY(my_data.timestamp + 1, count);
                    }
                }
                chart1.Series.Add(series);
                count += 2;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 400000);
                chart1.Parent = panel1;
                chart1.Dock = DockStyle.Fill;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;


                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            }
        }


        // turning textBox2 into a searchBox for checkedListBox
        List<string> listcollection = new List<string>();
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox2.Text) == false)
            {
                checkedListBox1.Items.Clear();
                foreach (string str in testData.Keys)
                {
                    if (str.StartsWith(textBox2.Text))
                    {
                        checkedListBox1.Items.Add(str);
                    }
                }
            }
        }
     
        //turning textbox into searchBox for checkedListBox2
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text) == false)
            {
                checkedListBox2.Items.Clear();
                foreach (string str in testData2.Keys)
                {
                    if (str.StartsWith(textBox4.Text))
                    {
                        checkedListBox2.Items.Add(str);
                    }
                }
            }
        }

        private void openCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get filename
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = "CSV Files|*.csv";
            DialogResult result = openDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var filename = openDialog.FileName;
                //parse .csv file
                var freader = new StreamReader(filename);
                while (!freader.EndOfStream)
                {
                    var line = freader.ReadLine();
                    string[] items = line.Split(',');
                    string name;
                    scheduleInfo schedInfo = new scheduleInfo();

                    if ((items.Length == 8))
                    {
                        if (items[4] == "0")
                            schedInfo.start = true;
                        else if (items[4] == "1")
                            schedInfo.start = false;
                        else
                            continue;
                        schedInfo.timestamp = Convert.ToUInt32(items[0]);
                        name = items[6];
                    }
                    else
                    {
                        continue;
                    }

                    if (my_runnable3.ContainsKey(items[6]))
                    {
                        foreach (runInfo runnes in my_runnable3[items[6]])
                        {
                            name = runnes.runname;
                        }

                    }

                    if (testData.ContainsKey(name))
                    {
                        List<scheduleInfo> schedInfoList;
                        testData.TryGetValue(name, out schedInfoList);
                        schedInfoList.Add(schedInfo);
                        testData[name] = schedInfoList;
                    }
                    else
                    {
                        List<scheduleInfo> schedInfoList = new List<scheduleInfo>();
                        schedInfoList.Add(schedInfo);
                        testData.Add(name, schedInfoList);
                    }

                }

                //adding runnables to checkedListBox
                foreach (var data in testData)
                {
                    checkedListBox1.Items.Add(data.Key);
                }
                //the files are opened and parsed, now we should make the boxes visible
                checkedListBox1.Visible = true;
                textBox2.Visible = true;
                button1.Visible = true;
                button2.Visible = true;

            }
            }

        private void openCSV2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //use openFileDialog to open csv file just like before
            var openDialog2 = new OpenFileDialog();
            openDialog2.Multiselect = false;
            openDialog2.Filter = "CSV Files|*.csv";
            DialogResult result2 = openDialog2.ShowDialog(this);
            if (result2 == DialogResult.OK)
            {
                var filename2 = openDialog2.FileName;
                //parse .csv file
                var freader2 = new StreamReader(filename2);
                while (!freader2.EndOfStream)
                {
                    var line2 = freader2.ReadLine();
                    string[] items2 = line2.Split(',');
                    string name2;
                    scheduleInfo schedInfo2 = new scheduleInfo();

                    if ((items2.Length == 8))
                    {
                        if (items2[4] == "0")
                            schedInfo2.start = true;
                        else if (items2[4] == "1")
                            schedInfo2.start = false;
                        else
                            continue;
                        schedInfo2.timestamp = Convert.ToUInt32(items2[0]);
                        name2 = items2[6];
                    }
                    else
                    {
                        continue;
                    }


                    if (my_runnable3.ContainsKey(items2[6]))
                    {
                        foreach (runInfo runnes2 in my_runnable3[items2[6]])
                        {
                            name2 = runnes2.runname;
                        }

                    }

                    if (testData2.ContainsKey(name2))
                    {
                        List<scheduleInfo> schedInfoList2;
                        testData2.TryGetValue(name2, out schedInfoList2);
                        schedInfoList2.Add(schedInfo2);
                        testData2[name2] = schedInfoList2;
                    }
                    else
                    {
                        List<scheduleInfo> schedInfoList2 = new List<scheduleInfo>();
                        schedInfoList2.Add(schedInfo2);
                        testData2.Add(name2, schedInfoList2);
                    }
                }
            }

            foreach (var data2 in testData2)
            {
                checkedListBox2.Items.Add(data2.Key);
            }
            textBox4.Visible = true;
            checkedListBox2.Visible = true;
            
        }

        private void openCSV3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //use openFileDialog to open csv file just like before
            var openDialog2 = new OpenFileDialog();
            openDialog2.Multiselect = false;
            openDialog2.Filter = "CSV Files|*.csv";

            DialogResult result2 = openDialog2.ShowDialog(this);
            if (result2 == DialogResult.OK)
            {
                var filename2 = openDialog2.FileName;
                //parse .csv file
                var freader2 = new StreamReader(filename2);
                while (!freader2.EndOfStream)
                {
                    var line2 = freader2.ReadLine();
                    string[] items2 = line2.Split(',');
                    string name2;
                    scheduleInfo schedInfo2 = new scheduleInfo();

                    if ((items2.Length == 8))
                    {
                        if (items2[4] == "0")
                            schedInfo2.start = true;
                        else if (items2[4] == "1")
                            schedInfo2.start = false;
                        else
                            continue;
                        schedInfo2.timestamp = Convert.ToUInt32(items2[0]);
                        name2 = items2[6];
                    }
                    else
                    {
                        continue;
                    }


                    if (my_runnable3.ContainsKey(items2[6]))
                    {
                        foreach (runInfo runnes2 in my_runnable3[items2[6]])
                        {
                            name2 = runnes2.runname;
                        }

                    }

                    if (testData3.ContainsKey(name2))
                    {
                        List<scheduleInfo> schedInfoList2;
                        testData3.TryGetValue(name2, out schedInfoList2);
                        schedInfoList2.Add(schedInfo2);
                        testData3[name2] = schedInfoList2;
                    }
                    else
                    {
                        List<scheduleInfo> schedInfoList2 = new List<scheduleInfo>();
                        schedInfoList2.Add(schedInfo2);
                        testData3.Add(name2, schedInfoList2);
                    }
                }
            }

            foreach (var data2 in testData3)
            {
                checkedListBox3.Items.Add(data2.Key);
            }
            textBox1.Visible = true;
            checkedListBox3.Visible = true;
        }

        //SearchBox
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == false)
            {
                checkedListBox3.Items.Clear();
                foreach (string str in testData3.Keys)
                {
                    if (str.StartsWith(textBox1.Text))
                    {
                        checkedListBox3.Items.Add(str);
                    }
                }
            }

        }


        //Button for uncheck
        private void button2_Click_1(object sender, EventArgs e)
        {

            foreach (int i in checkedListBox1.CheckedIndices)
            {
                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
            }

            foreach (int k in checkedListBox2.CheckedIndices)
            {
                checkedListBox2.SetItemCheckState(k, CheckState.Unchecked);
            }

            foreach (int n in checkedListBox3.CheckedIndices)
            {
                checkedListBox3.SetItemCheckState(n, CheckState.Unchecked);
            }

        }
    }
}

