using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Task_scheduler
{



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Scheduler files(*.sch)|*.sch";
            subjcets = new List<Subjcet>();
            days = new List<Day>();
            tableDays = new DataGridView[5];
            tableDays[0] = dataGridView3;
            tableDays[1] = dataGridView4;
            tableDays[2] = dataGridView5;
            tableDays[3] = dataGridView6;
            tableDays[4] = dataGridView7;

        }

        DataGridView[] tableDays;

        List<Subjcet> subjcets;
        List<Day> days;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        XmlDocument doc;
        XmlElement xRoot;
        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            //get file name from filedialog
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            toolStripStatusLabel1.Text = filename;
            //create and load xml file
            doc = new XmlDocument();
            doc.Load(filename);

            xRoot = doc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                // получаем атрибут name
                if (xnode.Attributes.Count > 0)
                {

                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null)
                        Console.WriteLine(attr.Value);
                    if (xnode.Name == "teacher")
                    {
                        dataGridView2.Rows.Add(attr.Value);
                    }
                    else
                    if (xnode.Name == "subject")
                    {
                        XmlNode cpwAttr = xnode.Attributes.GetNamedItem("cpw");
                        dataGridView1.Rows.Add(attr.Value, cpwAttr.Value);
                        subjcets.Add(new Subjcet(attr.Value, int.Parse(cpwAttr.Value)));
                    }
                    if (xnode.Name == "day")
                    {
                        XmlNode mscAttr = xnode.Attributes.GetNamedItem("msc");
                        days.Add(new Day(attr.Value,int.Parse(mscAttr.Value)));
                    }
                }

            }

            //fill days
            //for (int i = 0; i < days.Count; i++)
            //{
            //    scheduleTable.Columns.Add(days[i].Name, $"{days[i].Name}\n({days[i].TakenPlaces}/{days[i].MaxSubjectsCount})");
            //}


        }

        int maxSubjectsCount;

        public int getMaxSubjectsCount()
        {
            int max = 0;
            for (int i = 0; i < days.Count; i++)
                max = (days[i].MaxSubjectsCount > max) ? days[i].MaxSubjectsCount : max;
            return max;
        }

        private void scheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subjcets.Sort();
            //for (int i = subjcets.Count - 1; i >= 0; i--)
            //{
            //    for (int j = 0; j < subjcets[i].CountPerWeek/2; i++)
            //    {
            //        days[j].subjcets.Add(subjcets[i]);
            //        days[days.Count - 1].subjcets.Add(subjcets[i]);
            //    }
            //}
            int subindex = 0;
            int currcount = 0;
            maxSubjectsCount = getMaxSubjectsCount();
            buildSchedule(ref subindex, ref currcount);

            for (int i = 0; i < days.Count; i++)
            {
                for (int j = 0; j < days[i].subjcets.Count; j++)
                {
                    tableDays[i].Rows.Add(days[i].subjcets[j].Name);
                }
            }
        }

        private void buildSchedule(ref int subindex, ref int currcount)
        {
            for (int i = 0; i < maxSubjectsCount; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (currcount >= subjcets[subindex].CountPerWeek)
                    {
                        subindex++;
                        currcount = 0;
                    }
                    if (subindex >= subjcets.Count())
                    {
                        return;
                    }
                    if (days[j].subjcets.Count() < days[j].MaxSubjectsCount)
                    {
                        days[j].subjcets.Add(subjcets[subindex]);
                        currcount++;
                    }
                }
            }
        }

        private void teacherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name="";
            Teacher teacherDialog = new Teacher();
            if (teacherDialog.ShowDialog(this) == DialogResult.OK)
            {
                name = teacherDialog.textBox1.Text;
            }
            else
            {
                return;
            }

            XmlElement teacherElement = doc.CreateElement("teacher");
            XmlAttribute teachersName = doc.CreateAttribute("name");
            XmlText nameText = doc.CreateTextNode(name);
            teachersName.AppendChild(nameText);
            teacherElement.Attributes.Append(teachersName);
            xRoot.AppendChild(teacherElement);
            dataGridView2.Rows.Add(name);
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = "File saved to: "+filename;
                doc.Save(filename);
            }
            
        }

        private void saveFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = "File saved to: " + filename;
                doc.Save(filename);
                
            }
        }

        private void subjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "";
            string cpw = "";
            SubjectForm teacherDialog = new SubjectForm();
            if (teacherDialog.ShowDialog(this) == DialogResult.OK)
            {
                cpw = teacherDialog.textBox2.Text;
                name = teacherDialog.textBox1.Text;
            }
            else
            {
                
                return;
            }

            XmlElement teacherElement = doc.CreateElement("subject");
            XmlAttribute teachersName = doc.CreateAttribute("name");
            XmlText nameText = doc.CreateTextNode(name);
            teachersName.AppendChild(nameText);
            teacherElement.Attributes.Append(teachersName);

            XmlAttribute sujcectsCpw = doc.CreateAttribute("cpw");
            XmlText cpwText = doc.CreateTextNode(cpw);
            sujcectsCpw.AppendChild(cpwText);
            teacherElement.Attributes.Append(sujcectsCpw);
            xRoot.AppendChild(teacherElement);
            dataGridView1.Rows.Add(name,cpw);
        }
    }



    class Subjcet : IComparable<Subjcet>
    {
        public string Name { get; set; }
        public int CountPerWeek { get; set; }
        public Subjcet(string name, int cpw)
        {
            Name = name;
            CountPerWeek = cpw;
        }
        public int CompareTo(Subjcet obj) => -1*this.CountPerWeek.CompareTo(obj.CountPerWeek);
    }

    class Day
    {
        public string Name { get; set; }
        public int MaxSubjectsCount { get; set; }
        public int TakenPlaces { get; set; }
        public List<Subjcet> subjcets;
        public Day(string name, int msc)
        {
            subjcets = new List<Subjcet>();
            Name = name;
            MaxSubjectsCount = msc;
            TakenPlaces = 0;
        }
    }
}
