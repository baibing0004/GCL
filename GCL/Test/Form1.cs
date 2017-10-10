using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Messaging;
using System.Globalization;
using PublicClass.Common;
using PublicClass.Event;
using PublicClass.Threading;
using System.IO;
using System.Collections;
using PublicClass.Module.CronExpression;
using System.Reflection;
using PublicClass.Db.Ni;
using PublicClass.IO.Log;
namespace Test {
    public partial class Form1 : Form {
        public Form1() {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            InitializeComponent();
            //Button 60
            //trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
            trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent1);
            //Control.CheckForIllegalCrossThreadCalls = false;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            //MessageBox.Show("Domain:" + e.ExceptionObject.ToString());
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            MessageBox.Show("Thread:" + e.Exception.ToString());
        }

        /// <summary>
        /// 测试是否可以通过（委托）捕捉委托方法的错误
        /// 测试结果 可以 而且还发现 声明的事件必须有事件的默认实现注册到事件上
        /// 否则会发生空引用错误 
        /// 而且调用事件时如果注册的方法抛出错误 那么事件的调用就会出现问题 这个时候 可以使用在EventArg中实现的方法CallEventSafely来安全调用屏蔽所有的错误
        /// 可见代理就是方法接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            PublicClass.Threading.ProtectThread proThread = new PublicClass.Threading.ProtectThread(this.Run);
            proThread.ExceptionThrowenEvent += new EventHandle(proThread_ExceptionThrowenEvent); //+= new EventHandle(proThread_ExceptionThrowenEvent);
            //try {
            proThread.run();
            //} catch (Exception ex) {
            //    this.labResult.Text = ex.Message;
            //}
        }

        void proThread_ExceptionThrowenEvent(object sender, EventArg e) {
            this.labResult.Text = ((ThreadEventArg)e).GetException().Message;
            throw new Exception("能否捕捉！？");
        }

        private void Run(object sender, EventArg e) {
            MessageBox.Show("代理测试 看到了吧！");
            throw new Exception("测试成功！");
        }

        /// <summary>
        /// 测试读取配置文件时是否可以加入cdata格式的数据 测试失败 但是请注意xml中的转义字符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            MessageBox.Show(System.Configuration.ConfigurationManager.AppSettings["test2"]);
        }

        private void button3_Click(object sender, EventArgs e) {
            //MessageBox.Show(string.Format("{0:0,0}", 2000));
            MessageBox.Show(System.Text.Encoding.Default.GetString(Convert.FromBase64String("PCFET0NUWVBFIGh0bWwgUFVCTElDICItLy9XM0MvL0RURCBYSFRNTCAxLjAgVHJhbnNpdGlvbmFsLy9FTiIgImh0dHA6Ly93d3cudzMub3JnL1RSL3hodG1sMS9EVEQveGh0bWwxLXRyYW5zaXRpb25hbC5kdGQiPg0KPGh0bWwgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGh0bWwiPg0KPGhlYWQ+DQo8bWV0YSBodHRwLWVxdWl2PSJDb250ZW50LVR5cGUiIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD1nYjIzMTIiIC8+DQo8dGl0bGU+ycG159PKufrE2sLKz8jNxrP2obC74buwobHEo8q9PC90aXRsZT4NCjwvaGVhZD4NCg0KPGJvZHkgc3R5bGU9ImZvbnQtc2l6ZToxNHB4O2xpbmUtaGVpZ2h0OjI0cHg7Y29sb3I6IzRjNGM0YyI+DQoNCjx0YWJsZSB3aWR0aD0iNjEwIiBib3JkZXI9IjMiIGFsaWduPSJjZW50ZXIiIGNlbGxwYWRkaW5nPSIwIiBjZWxsc3BhY2luZz0iMCIgYm9yZGVyY29sb3I9IiNlOWVlZjIiPg0KICA8dHI+DQogICAgPHRkPjxpbWcgc3JjPSJodHRwOi8vaW1hZ2VzLnNvaHUuY29tL2ZyZWVtYWlsLzIwMDltYWlsYWQvMDkxMTA2aHVpaHVhL2ltYWdlcy90aXRsZS5qcGciIHdpZHRoPSI2MTAiIGhlaWdodD0iODAiIC8+PC90ZD4NCiAgPC90cj4NCiAgPHRyPg0KICAgIDx0ZD48dGFibGUgd2lkdGg9IjU1MCIgYm9yZGVyPSIwIiBhbGlnbj0iY2VudGVyIiBjZWxscGFkZGluZz0iNSIgY2VsbHNwYWNpbmc9IjAiPg0KICAgICAgPHRyPg0KICAgICAgICA8dGQgY29sc3Bhbj0iMiI+x9ewrrXEycG159PK08O7pzo8L3RkPg0KICAgICAgPC90cj4NCiAgICAgIDx0cj4NCiAgICAgICAgPHRkIGNvbHNwYW49IjIiPqGhoaHOqsHLyMPE+r+00MW4/Le9seO4/MflzvqjrM7Sw8fCys/Izcaz9sHLobC74buwobHEo8q9oaM8L3RkPg0KICAgICAgPC90cj4NCgkgIDx0cj4NCiAgICAgICAgPHRkIGNvbHNwYW49IjIiPqGhoaHU2rvhu7DEo8q9z8KjrM7Sw8e74b2rttTNrNK708q8/rXEu9i4tM35wLTTyrz+19S2r9fps8nSu7j2u+G7sKOs1eLR+cT6tcTQxb7Nu+HP8UJCU8nPtcTUrcz7oaK4+sz70rvR+cflzvqhozwvdGQ+DQogICAgICA8L3RyPg0KDQoJICA8dHI+DQogICAgICAgIDx0ZCBjb2xzcGFuPSIyIj48c3Ryb25nIHN0eWxlPSJjb2xvcjojMmE1Y2ExIj48aW1nIHNyYz0iaHR0cDovL2ltYWdlcy5zb2h1LmNvbS9mcmVlbWFpbC8yMDA5bWFpbGFkLzA5MTEwNmh1aWh1YS9pbWFnZXMvaWNvbi5naWYiIGFsaWduPSJhYnNtaWRkbGUiLz4gwdCx7dKzPC9zdHJvbmc+oaHX1Lav1+mzybvhu7CjrL3ayqGy6dXSyrG85KOhPC90ZD4NCiAgICAgIDwvdHI+DQogICAgICA8dHI+DQogICAgICAgIDx0ZCBoZWlnaHQ9IjMyMiIgYWxpZ249ImNlbnRlciIgYmdjb2xvcj0iI2ZmZmZmZiI+PGltZyBzcmM9Imh0dHA6Ly9pbWFnZXMuc29odS5jb20vZnJlZW1haWwvMjAwOW1haWxhZC8wOTExMDZodWlodWEvaW1hZ2VzL2xpc3QuanBnIiB3aWR0aD0iNDg2IiBoZWlnaHQ9IjMyMiIgLz48L3RkPg0KICAgICA8L3RyPg0KCSANCgkgIDx0cj4NCiAgICAgICAgPHRkIGNvbHNwYW49IjIiPjxzdHJvbmcgc3R5bGU9ImNvbG9yOiMyYTVjYTEiPjxpbWcgc3JjPSJodHRwOi8vaW1hZ2VzLnNvaHUuY29tL2ZyZWVtYWlsLzIwMDltYWlsYWQvMDkxMTA2aHVpaHVhL2ltYWdlcy9pY29uLmdpZiIgYWxpZ249ImFic21pZGRsZSIvPiC2wdDF0rM8L3N0cm9uZz6hobvhu7DTyrz+yvfQzs/Uyr6jrNK7xL/By8i7o6E8L3RkPg0KICAgICAgPC90cj4NCgkgPHRyPg0KICAgICAgICA8dGQgaGVpZ2h0PSIyOTEiIGFsaWduPSJjZW50ZXIiIGJnY29sb3I9IiNmZmZmZmYiPjxpbWcgc3JjPSJodHRwOi8vaW1hZ2VzLnNvaHUuY29tL2ZyZWVtYWlsLzIwMDltYWlsYWQvMDkxMTA2aHVpaHVhL2ltYWdlcy9yZWFkLmpwZyIgd2lkdGg9IjQ4NyIgaGVpZ2h0PSIyOTEiIC8+PC90ZD4NCiAgICAgPC90cj4NCg0KICAgICAgPHRyPg0KICAgICAgICA8dGQgaGVpZ2h0PSI1MCIgY29sc3Bhbj0iMiI+oaGhocq508O3vcq9uty88rWlo6y147v308rP5MnPt721xDxpbWcgc3JjPSJodHRwOi8vaW1hZ2VzLnNvaHUuY29tL2ZyZWVtYWlsLzIwMDltYWlsYWQvMDkxMTA2aHVpaHVhL2ltYWdlcy9idXR0b24uanBnIiB3aWR0aD0iODYiIGhlaWdodD0iMjAiIC8+sLTFpby0v8mjoTwvdGQ+DQogICAgICA8L3RyPg0KICAgICAgPHRyPg0KCSAgICAgIDx0ZCBoZWlnaHQ9IjQwIiBjb2xzcGFuPSIyIj6hoaGhu7bTrcT6vavKudPDuNDK3M2ouf3V4rj2wbS907e0wKG4+M7Sw8ejoSAgIDxhIGhyZWY9Imh0dHA6Ly9nb3RvLm1haWwuc29odS5jb20vZ290by5waHA/Y29kZT0xMDA5MDMwMDA5MTEwNjExMzgyMiZydT1odHRwJTNBJTJGJTJGbWFpbC5zb2h1LmNvbSUyRm1hcHAlMkZoZWxwJTJGbWFpbCUyRmhlbHBfaHVpaHVhLmpzcCIgdGFyZ2V0PV9ibGFuaz63tMCh0uK8+z4+PC9hPjwvdGQ+DQogICAgICA8L3RyPg0KICAgICAgIDx0cj4NCiAgICAgICAgPHRkICBoZWlnaHQ9IjQwIiBjb2xzcGFuPSIyIiA+oaGhobjQ0LvE+tTEtsGxvtPKvP6joTwvdGQ+DQogICAgICA8L3RyPg0KICAgICAgIDx0cj4NCiAgICAgICAgPHRkICBoZWlnaHQ9IjYwIiBjb2xzcGFuPSIyIiBhbGlnbj0icmlnaHQiPjxpbWcgc3JjPSJodHRwOi8vaW1hZ2VzLnNvaHUuY29tL2ZyZWVtYWlsLzIwMDltYWlsYWQvMDkxMTA2aHVpaHVhL2ltYWdlcy9zb2h1bG9nby5naWYiIGFsaWduPSJhYnNtaWRkbGUiLz7L0br808q8/tbQ0MQ8L3RkPg0KICAgICAgPC90cj4NCiAgICA8L3RhYmxlPjwvdGQ+DQogIDwvdHI+DQogIDx0cj4NCiAgICA8dGQ+PGltZyBzcmM9ImltYWdlcy9ib3R0b20uanBnIiB3aWR0aD0iNjEwIiBoZWlnaHQ9IjEyIiAvPjwvdGQ+DQogIDwvdHI+DQo8L3RhYmxlPg0KPC9ib2R5Pg0KPC9odG1sPg0K")));
        }

        private void button4_Click(object sender, EventArgs e) {
            MessageBox.Show(DateTime.Parse("1999-5-8").ToString());
        }

        DataSet1 ds = new DataSet1();
        private void button5_Click(object sender, EventArgs e) {
            //MessageBox.Show(new DataSet1().cv_base.Rows.Count.ToString());
            MessageBox.Show(new Test.DataSet1TableAdapters.job_info_TestTableAdapter().GetData().Rows.Count.ToString());

            new Test.DataSet1TableAdapters.job_info_TestTableAdapter().Fill(ds.job_info_Test);
            new Test.DataSet1TableAdapters.job_occ_TestTableAdapter().Fill(ds.job_occ_Test);

            int a = ds.job_info_Test.Rows[0].GetChildRows("FK_job_occ_Test_job_info_Test").Length;
            a = ds.Relations.Count;
            //this.dgv.DataBindings.Add("DataSource",ds.job_info_Test,"job_info_Test");
            //this.dgv.DataBindings.Add("DataSource",ds,"job_info_Test");

            /*功能相同
            this.dgv.DataSource = ds;
            this.dgv.DataMember = "job_info_Test";
             */
            /*功能相同
            this.dgv.DataSource=ds.job_info_Test;
            this.dgv.DataMember=null;
             */
            DataViewManager dvm = new DataViewManager(ds);
            DataView dv = new DataView(ds.job_info_Test);
            //设置是否允许修改 
            dv.AllowEdit = false;
            //设置是否允许删除 
            //dv.AllowDelete;
            //设置是否允许添加
            //dv.AllowNew;
            //设置过滤行
            dv.RowFilter = "job_id in (1,3)";
            //排序
            dv.Sort = "job_id desc";
            this.dgv.DataSource = dvm;
            this.dgv.DataMember = "job_info_Test";
            //this.dgv.DataMember = null;
        }

        private void button6_Click(object sender, EventArgs e) {
            //因为如果DataMember为null的情况下，DataGridView显示的是事物的第一个属性 所以显示的是字符串的长度。int数组不支持
            //this.dgv.DataBindings.Add("DataSource",new string[] { "one","12","fee2" },"");
            this.dgv.DataSource = new string[] { "one", "12", "fee2" };
            this.dgv.DataMember = null;
        }

        private void button7_Click(object sender, EventArgs e) {
            //测试DataBindings的管理器(实际上分为有状态的CurryManager 和PropertyManager两种) 
            this.txtDataSet.DataBindings.Add("Text", ds.job_info_Test, "job_name");
            this.BindingContext[ds.job_info_Test].Position = 1;
        }

        private void Form1_Load(object sender, EventArgs e) {
            //DateTime now = DateTime.Now;
            //MessageBox.Show(now.Date.ToString());
            System.Globalization.Calendar cal = new GregorianCalendar();
            //System.Globalization.CultureInfo.InvariantCulture.Calendar();
        }

        /// <summary>
        /// xml测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e) {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("../../xml1.xml");
            XmlTextReader reader = new XmlTextReader("../../xml1.xml");
            while (reader.Read()) {
                string result = reader.Name + "\tT:" + reader.NodeType.ToString() + "\tC:" + reader.MoveToContent().ToString() + "\tCC:" + reader.Name;
                if (reader.NodeType == XmlNodeType.Text)
                    result += "\tV:" + reader.ReadElementString();//reader.Value;
                this.lstResult.Items.Add(result);
            }
            MessageBox.Show(doc.InnerXml);
        }

        private void button9_Click(object sender, EventArgs e) {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("../../xml1.xml");
            //SelectSingleNode是使用Xpath的方法 其GetElementsByTagName（）方法与Java中的doc.getElementsByTagName（）意义相同
            //MessageBox.Show(doc.SelectSingleNode("first/second").InnerText);
            MessageBox.Show(doc.GetElementsByTagName("first")[0].Attributes["name"].Value);
        }

        private void button10_Click(object sender, EventArgs e) {
            //重建一个XMLDocument
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            System.Xml.XmlElement root = doc.CreateElement("first");
            XmlElement second = doc.CreateElement("second");
            //不能使用Value 因为不符合DOM模型 需要加入Text节点
            second.InnerText = "第二个";
            root.AppendChild(second);
            XmlElement third = doc.CreateElement("third");
            XmlElement four = doc.CreateElement("four");
            four.InnerText = "第四个";
            third.AppendChild(four);
            root.AppendChild(third);
            doc.AppendChild(root);
            //Save功能自动将xml文件根据其encoding进行文件格式保存 但是不像JavaTransform进行转码
            doc.Save("c:/xml2.xml");
            MessageBox.Show("完成");
        }

        private void button11_Click(object sender, EventArgs e) {
            System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
            transform.Load("../../xsl1.xslt");
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("../../xml1.xml");
            transform.Transform(doc.CreateNavigator(), null, new System.IO.StreamWriter("c:/xml1.txt"));
            MessageBox.Show("完成");
        }

        private void button12_Click(object sender, EventArgs e) {
            //写入模式 数据库架构
            ds.WriteXmlSchema("c:/ds.xsd");
            ds.WriteXml("c:/ds.xml");
            DataSet ds2 = new DataSet();
            //完成Clone操作
            ds2.ReadXmlSchema("c:/ds.xsd");
            //完成COPY操作
            ds2.ReadXml("c:/ds.xml");
            this.dgv.DataSource = ds2;
        }

        private void button13_Click(object sender, EventArgs e) {
            //通过在参数类中定义结构可以串行化为一个xml文件了
            XMLTest xt = new Test.XMLTest();
            xt.A = 1;
            xt.B = 2;
            xt.C = 3;
            xt.D = 4;
            XMLTest2 xt2 = new Test.XMLTest2();
            xt2.E = 5;
            xt2.F = 6;
            xt.Test2 = xt2;
            xt.G = 20;

            //保存串行化数据
            XmlTextWriter xtw = new XmlTextWriter("c:/xt.xml", Encoding.UTF8);
            xtw.Formatting = Formatting.Indented;
            new System.Xml.Serialization.XmlSerializer(xt.GetType()).Serialize(xtw, xt);
            xtw.Close();
            MessageBox.Show("完成");

            //如何读取其串行化数据
            XmlTextReader xtr = new XmlTextReader("c:/xt.xml");
            XMLTest xt1 = (XMLTest)new System.Xml.Serialization.XmlSerializer(xt.GetType()).Deserialize(xtr);
            xtr.Close();
            MessageBox.Show(xt1.Test2.E.ToString());

        }
        System.Collections.ArrayList al = new System.Collections.ArrayList();
        private void button14_Click(object sender, EventArgs e) {
            //测试是否会返回所在位置的值 测试结果正确
            //MessageBox.Show(al.Add("1").ToString());
            //测试未初始化时引用数据是否为null 结果抛出IndexOutOfRang错误
            //MessageBox.Show(al[0].ToString());
            al.Add("2");
            al.Add("3");
            al.Add("4");
            al.Add("5");
            //测试获得Enumerator是否需要MoveNext 答需要 与Java中的Iterator一样的使用方法
            //但是不允许修改al的大小
            for (int w = al.Count - 1; w >= 0; w--) {
                al.Remove("");
                al.Remove(al[w]);
            }
        }

        private void button15_Click(object sender, EventArgs e) {
            try {
                object a = new System.Collections.Hashtable()[""];
                MessageBox.Show("无异常");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button16_Click(object sender, EventArgs e) {
            //wb.Url = new System.Uri("http://www.sohu.com");
            /*
            for(int w = 1;w <= 10;w++)
                try {
                    new System.Net.WebClient().DownloadFile("http://www.wuxiawu.com/xf/txhj/txhj23" + w.ToString("00") + ".htm","c:/" + w + ".txt");
                } catch{
                }
            */
            new System.Net.WebClient().DownloadFile("http://book.yunxiaoge.com/files/article/html/0/386/244423.html", "c:/a.txt");

        }

        private void button17_Click(object sender, EventArgs e) {
            MessageBox.Show(new PublicClass.Collections.LimitQueue(new System.Collections.Queue(), 2).Peek().ToString());
        }

        /// <summary>
        /// 测试 ' '是否与 " ".ToChars()[0] 相同，结果相同
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e) {
            MessageBox.Show("".Split(new char[] { ',' }).Length.ToString());
            //MessageBox.Show("q23 233".Split(' ')[0]);
        }
        /// <summary>
        /// 测试如果Select结果不存在是否返回数组 结果：返回而且长度为0 大小写不敏感
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e) {
            DataSet ds2 = ds.Copy();
            this.ds.Clear();
            this.ds.job_info_Test.Columns[0].AutoIncrement = false;
            int a = int.MaxValue;
            MessageBox.Show(Convert.ToString(a + 1));
            //this.ds.job_info_Test.Columns[0].DefaultValue = 0;            
            this.ds.job_info_Test.Columns[0].AutoIncrement = true;
            this.ds.job_info_Test.Addjob_info_TestRow("haha");
            this.ds.job_info_Test.Rows[0][1] = "hehe1";
            this.ds.Merge(ds2);
            this.ds.AcceptChanges();
            MessageBox.Show(this.ds.job_info_Test.Select("job_Id=3 and getDate()=getDate()").Length.ToString());
        }

        private void button20_Click(object sender, EventArgs e) {
            new Test.DataSet1TableAdapters.job_testTableAdapter().Fill(this.ds.job_test);
            MessageBox.Show(this.ds.job_test.Select("createTime <='2006-10-28'", "id desc")[0]["name"].ToString());
        }

        private void button21_Click(object sender, EventArgs e) {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            PublicClass.IO.Config.ConfigManagement.Dispose();
        }

        /// <summary>
        /// 测试HashCode操作是否比较占用内存 测试结果 是的 所以尽可能少的进行HashCode操作 但是Hashtable可能是进行了优化。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button22_Click(object sender, EventArgs e) {
            for (int w = 0; w < 10000; w++) {
                int id = w.ToString().GetHashCode();
                this.txtDataSet.Text = id.ToString();
            }
        }
        private void button23_Click(object sender, EventArgs e) {
            System.Timers.Timer timer = new System.Timers.Timer();
            MessageBox.Show(timer.AutoReset.ToString());
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 2000;
            timer.AutoReset = false;
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Invoke(new System.Timers.ElapsedEventHandler(this.timer_Elapsed1), new object[] { sender, e });
        }

        void timer_Elapsed1(object sender, System.Timers.ElapsedEventArgs e) {
            this.txtDataSet.Text = DateTime.Now.ToString("mm:ss");
            ((System.Timers.Timer)sender).Start();
        }

        private void button24_Click(object sender, EventArgs e) {
            long ID = Tool.ToLongValue(this.txtDataSet.Text);
            long Folder = 0;
            int folderParameter = 1000;
            string temp = string.Empty;
            for (int w = 0; w < 3; w++) {
                Folder = ID / (int)Math.Pow(folderParameter, w) % folderParameter;

                temp = Folder.ToString() + @"\" + temp;
            }
            this.txtDataSet.Text = temp;
            //System.IO.Directory.CreateDirectory(@"c:\1\2\3\4\5\6");            
        }

        /// <summary>
        /// 检验枚举类型是否可以向数值型转换 测试结果可以
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button25_Click(object sender, EventArgs e) {
            //MessageBox.Show(new ChinaHRSms.ReceiveMessage("1,0,2,3".Split(',')).GetReceiveType().ToString());
        }

        private void button26_Click(object sender, EventArgs e) {
            //ChinaHRSms.SendMessage msg = new ChinaHRSms.SendMessage("13520493404","");
            //msg.SetOrderNum(1);
            //msg.SetSendTime(0);
            //msg.SetSendMobile("13520493404");
            //MessageBox.Show(msg.GetSZPhone());
        }

        private void button27_Click(object sender, EventArgs e) {
            //System.IO.MemoryStream
            //DirectoryInfo din = new DirectoryInfo("C:\\Xslt");
            //MessageBox.Show(din.FullName);
            //MessageBox.Show(din.GetDirectories().Length.ToString());
            MessageBox.Show(PublicClass.IO.IOTool.GetFileName("*.xml"));
            MessageBox.Show(PublicClass.IO.IOTool.GetPath("*.xml"));
            //MessageBox.Show(Application.StartupPath);
        }

        /// <summary>
        /// 正则表达式测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button28_Click(object sender, EventArgs e) {
            //MessageBox.Show(System.Text.RegularExpressions.Regex.Replace("[momo7]3f334343[1357],578546", @"\[[^\]]*\]", "").ToString());
            //MessageBox.Show(System.Text.RegularExpressions.Regex.Matches("  1304事件 applycvid:1 ", @"\d+").Count.ToString());
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("<\\?[^(\\?>)]*\\?>");
            //for (IEnumerator ie = reg.Matches(this.txtResult.Text).GetEnumerator(); ie.MoveNext(); )
            //    MessageBox.Show(((System.Text.RegularExpressions.Match)ie.Current).Value);
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(this.txtDataSet.Text);
            for (IEnumerator ie = reg.Matches(this.txtResult.Text).GetEnumerator(); ie.MoveNext(); )
                MessageBox.Show(((System.Text.RegularExpressions.Match)ie.Current).Value);
            //MessageBox.Show(reg.Match(this.txtResult.Text).Success.ToString());
            //.Substring(10).TrimEnd()
        }

        private void button29_Click(object sender, EventArgs e) {
            System.IO.Directory.CreateDirectory(@"\\192.168.10.236\Html\newmy\xml133\");
        }

        private void button30_Click(object sender, EventArgs e) {
            //long _temp = 21474836480000;//int.MaxValue * 5000;
            //MessageBox.Show(_temp + " " + (int)(_temp / 3000));
            MessageBox.Show(Convert.ToInt16("213336").ToString());
        }

        private void button31_Click(object sender, EventArgs e) {
            string mqPath = System.Configuration.ConfigurationSettings.AppSettings["MSMQPath"];
            if (mqPath == null || mqPath == "") {
                //Mails.SendErrorMail("Apply.aspx消息队列：","消息队列的结点MSMQPath在web.config中没有配置。");
                //throw new Exception("web.config中没有对MSMQPath节点进行配置!");
                MessageBox.Show("web.config中没有对MSMQPath节点进行配置!");
            }
            //发送消息
            MessageQueue myMQ = new MessageQueue();
            //通过格式名引用队列
            myMQ.DefaultPropertiesToSend.Recoverable = true; //保存到磁盘,默认为 false。
            myMQ.Path = mqPath;
            myMQ.Formatter = new XmlMessageFormatter();
            myMQ.Send(new Entity());
        }

        private void button32_Click(object sender, EventArgs e) {
            System.Xml.XmlDocument docu = new System.Xml.XmlDocument();
            docu.Load("../../DefaultAction.xml");
            MessageBox.Show(docu.DocumentElement.SelectSingleNode("/config/action[@name=\"reboot\"]").SelectSingleNode("file[@name=\"DoExe.vbs\"]").Attributes["value"].Value);
        }

        private void button33_Click(object sender, EventArgs e) {
            MessageBox.Show(string.Format("{0}//{eef3334//}", 1));
        }

        private void button34_Click(object sender, EventArgs e) {
            //Entity entity = new Entity();
            //object c = entity;
            //Type type = c.GetType();
            MessageBox.Show(this.Analyze(typeof(TestE)));
            //((System.Reflection.PropertyInfo)type.GetMember("BB")[0]).GetSetMethod().Invoke(entity,new object[]{"20"});
            //MessageBox.Show(type.GetMethod("Test",new Type[0]).Name);//[0].MemberType.ToString());
            //type.GetMember("BB").GetType().InvokeMember("BB",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.SetProperty, null, entity, new object[] { 2 });
            //MessageBox.Show(entity.BB.ToString());
            //            type.GetField("a1").GetValue
        }

        private string Analyze(Type t) {
            StringBuilder sb = new StringBuilder();
            if (Tool.IsEnable(t.BaseType))
                sb.AppendLine("BaseType:" + t.BaseType.Name);
            if (Tool.IsEnable(t.UnderlyingSystemType))
                sb.AppendLine("UnderlyingSystemType:" + t.UnderlyingSystemType.Name);
            sb.AppendLine("Name:" + t.Name);
            sb.AppendLine("FullName:" + t.FullName);
            //sb.AppendLine("DeclaringType:" + t.DeclaringType.Name);
            //GetMembers可以获得所有公共的实例或者类的对象
            System.Reflection.MemberInfo[] mems = t.GetMembers();//(System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);           
            foreach (System.Reflection.MemberInfo info in mems) {
                sb.AppendLine("DeclaringType:" + info.DeclaringType + "\tMemberType:" + info.MemberType.ToString() + "\tName:" + info.Name + "\tIsAseemlby:" + (info.MemberType == System.Reflection.MemberTypes.Method ? ((System.Reflection.MethodInfo)info).IsAssembly.ToString() : ""));
                if (info is MethodInfo) {
                    MethodInfo info2 = (MethodInfo)info;
                    foreach (ParameterInfo info3 in info2.GetParameters())
                        sb.AppendLine("\t\tType:" + info3.ParameterType.Name + "\tName:" + info3.Name);
                }

            }
            return sb.ToString();
        }

        private void button35_Click(object sender, EventArgs e) {
            MessageBox.Show(Tool.ToStringValue(new Exception("测试").ToString()));
        }

        private void button36_Click(object sender, EventArgs e) {
            System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
            transform.Load("c:/xslt/xsl1.xslt");
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("c:/xslt/xml1.xml");
            //transform.OutputSettings//= System.Text.Encoding.GetEncoding("GB2312");
            //System.Xml.Xsl.XsltArgumentList xargs = new System.Xml.Xsl.XsltArgumentList();
            //xargs.AddParam("Encoding", null, System.Text.Encoding.GetEncoding("GB2312"));
            //transform.OutputSettings.Encoding = System.Text.Encoding.Default;
            transform.Transform(doc.CreateNavigator(), null, new System.IO.StreamWriter("c:/xslt/xml1.html"));
            MessageBox.Show("完成");
        }

        private void button37_Click(object sender, EventArgs e) {
            //MessageBox.Show("abcdef".IndexOf("D",StringComparison.OrdinalIgnoreCase).ToString());
            MessageBox.Show("abcdef".Replace("d", "E"));
        }

        private void button38_Click(object sender, EventArgs e) {
            //MessageBox.Show(new com.chinahr.my.IsWriteApplyHtml().GetCVHtml(4000000006053618,1));
            //this.txtResult.Text = new com.chinahr.my27.SkrCV().ReBuildXML.GetDataXML(4000000017949801, 0, false).InnerXml;
        }

        private void button39_Click(object sender, EventArgs e) {
            //DateTime time = DateTime.Now;
            //MessageBox.Show((time.CompareTo(DateTime.Parse(time.ToString()))).ToString());
            //MessageBox.Show(DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00")).ToString());      
            MessageBox.Show("" + (int)DateTime.Now.Date.AddDays(4).DayOfWeek);
        }

        private void button40_Click(object sender, EventArgs e) {
            //System.IO.FileInfo file = new System.IO.FileInfo("c:/111.htm");
            //MessageBox.Show(file.OpenText().CurrentEncoding.ToString());
            //System.Text.Encoding.UTF8.
            //System.IO.FileStream 

            /*
            System.IO.StreamReader reader = new System.IO.StreamReader("c:/111.htm", Encoding.UTF8);
            char[] data = new char[2];
            reader.Read(data, 0, 2);
            reader.Close();
            byte[] data1 = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            MessageBox.Show(data1.Length.ToString());
             */
            System.IO.Stream stream = new System.IO.FileStream("c:/111.htm", System.IO.FileMode.Open);
            string a = "";
            for (int w = 0; w < 20; w++)
                a += ":" + stream.ReadByte();
            stream.Close();

            MessageBox.Show(a);
        }

        private void button41_Click(object sender, EventArgs e) {
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(@"C:\Program Files\Internet Explorer\IEXPLORE.EXE");
        }

        private void button42_Click(object sender, EventArgs e) {
            TextReader reader = new StringReader("中文测试");
            TextReader reader1 = new StreamReader("c:\\b.txt", Encoding.Default);
            //。Net中Char类型可以代表一个中文!
            char[] c = new char[1];
            reader.Read(c, 0, c.Length);
            reader.Read(c, 0, c.Length);
            reader.Close();
            char[] c1 = new char[1];
            reader1.Read(c1, 0, c1.Length);
            reader1.Read(c1, 0, c1.Length);
            reader1.Close();
            MessageBox.Show(Convert.ToString(c[0] == c1[0]));
        }

        private void button43_Click(object sender, EventArgs e) {
            MessageBox.Show(Convert.ToString("1中2文3测试"[2]));
        }

        private void button44_Click(object sender, EventArgs e) {
            //失败 .Net中没有集合和集合的操作 全部自己搞定
            //ArrayList list = new ArrayList(new Hashtable());
            //list.AddRange("1,2,3,4".Split(','));
            //list.AddRange("1,2,3,4".Split(','));
            //MessageBox.Show(list.Count.ToString());
            //object a=new Hashtable()[13];
            //new Stack().Pop();抛出 System.InvalidOperationException
            //new Queue().Dequeue();   抛出 System.InvalidOperationException
            IList list = new ArrayList();
            Queue que = new Queue(list);
            que.Enqueue("1");
            que.Enqueue("2");
            que.Enqueue("3");
            que.Enqueue("4");
            list.Remove("3");
            que.Dequeue();
            MessageBox.Show(que.Dequeue().ToString());
        }

        private void button45_Click(object sender, EventArgs e) {
            ApplyEntity entity = new ApplyEntity();
            entity.Id = 2;
            MessageBox.Show(entity.ToHashtable()["@Id"].ToString());
            //Hashtable ht = new Hashtable();
            //ht["@id"] = 3;//大小写必须一致
            //MessageBox.Show(ApplyEntity.ToEntity(ht).Id.ToString());
        }

        private void button46_Click(object sender, EventArgs e) {
            //PublicClass.Collections.Pool.Pool pool = new PublicClass.Collections.Pool.Pool(new PublicClass.Collections.Pool.StackPoolStaregy());
            //PublicClass.Collections.Pool.KeyValuePool pool = new PublicClass.Collections.Pool.KeyValuePool(new PublicClass.Collections.Pool.StackPoolStaregy(), "", new IkvF());
            //pool.Set("0");
            //pool.Set("1");
            //pool.Set("2");
            //pool.Remove("2");
            //MessageBox.Show(pool.Get().ToString());
            ////object o = pool.Get();
            ////MessageBox.Show(o.ToString());

            ////MessageBox.Show(pool.Get().ToString());
            ////MessageBox.Show(pool.Get().ToString());
            ////pool.Remove(o);
            //MessageBox.Show(pool.GetSize().ToString());
            PublicClass.Collections.Pool.StackPoolStaregy s = new PublicClass.Collections.Pool.StackPoolStaregy();
            object a = "";
            s.Set(a);
            s.Set("2");
            s.Remove(a);
        }

        private void button47_Click(object sender, EventArgs e) {
            PublicClass.Collections.Pool.LRUPoolRefreshStaregy lps = new PublicClass.Collections.Pool.LRUPoolRefreshStaregy(TimeSpan.FromMilliseconds(1000));
            lps.Set("0");
            lps.Set("1");
            lps.Set("2");
            Tool.ObjectSleep(2000);
            object c = lps.AllowDel();
            lps.Remove(c);
            MessageBox.Show(lps.Get().ToString());
        }

        private void GetFiles(string dicp, IList list) {
            DirectoryInfo dic = new DirectoryInfo(dicp);
            foreach (FileInfo info in dic.GetFiles("cv*.xsl"))
                list.Add(info.FullName);
            foreach (DirectoryInfo dicinfo in dic.GetDirectories())
                GetFiles(dicinfo.FullName, list);
        }
        private void button48_Click(object sender, EventArgs e) {
            ArrayList list = new ArrayList();
            GetFiles("c:\\dds", list);
            object[] a = list.ToArray();
            foreach (object id in a)
                MessageBox.Show(id.ToString());
        }

        private void button49_Click(object sender, EventArgs e) {
            try {
                string path = "";
                int ret =
                    new com.chinahr.my1.JoinEhrDB().Join(4000000015333950, 0, 20060715014840, 20060715014840, 1, 1, 0);
                //new com.chinahr.my.IsWriteApplyHtml().IsReBuildAndGetHRDBPath(new com.chinahr.my1.JoinEhrDB().Join(4000000015333950, 0, 20060715014840, 20060715014840, 1, 1, 0), 4000000015333950, 1, out path);
                //MessageBox.Show("OK!", new com.chinahr.my27.SkrCV().g(4000000005284334, 0));
                //MessageBox.Show(ret.ToString());
                //MessageBox.Show("OK","",MessageBoxButtons.OK);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button50_Click(object sender, EventArgs e) {
            Test.com.chinahr.smscc.MessageList list = new Test.com.chinahr.smscc.MessageList();
            list.Message = "My测试";
            list.Phone = "13520493404";
            list.PhoneGroup = "";
            list.SendType = 1;
            list.SourceType = 253;
            list.Rank = 2;
            list.SourceIP = (System.Net.Dns.Resolve(System.Net.Dns.GetHostName())).AddressList[0].ToString();


            MessageBox.Show(new com.chinahr.smscc.MessageService().myMessageAdd(list, 1).ToString());
        }

        private void button51_Click(object sender, EventArgs e) {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.run));
            thread.Start();
        }

        public void run() {
            MessageBox.Show("51 线程错误测试");
            throw new Exception("51 线程错误测试");
        }

        private void button52_Click(object sender, EventArgs e) {
            throw new Exception("52 异常！");
        }

        private void button53_Click(object sender, EventArgs e) {
            EventArg ea = new EventArg(EventLevel.Comment, 10, new object[] { "1", "2" });
            ea = new EventArg(ea);
            MessageBox.Show(ea.ToStringOfPara(0));
            MessageBox.Show(ea.ToStringOfPara(1));
        }

        private void button54_Click(object sender, EventArgs e) {
            lock (key) {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.run2));
                thread.Start();
                //Tool.ObjectPulseAll(key);
                Tool.ObjectWait(key);
                MessageBox.Show("按钮" + iskey);
            }
        }

        private bool iskey = false;
        private string key = "";
        public void run2() {
            Tool.ObjectSleep(2000);
            iskey = true;
            Tool.ObjectPulseAll(key);
            MessageBox.Show("线程" + iskey);
        }

        private void button55_Click(object sender, EventArgs e) {

            MessageBox.Show(System.Text.RegularExpressions.Regex.Match("a搞笑", "^[A-Za-z]+$").Success.ToString());
        }

        private void button56_Click(object sender, EventArgs e) {
            MessageBox.Show(DateTime.Now.ToString("HH:mm:ss"));
        }

        private void TestFormatContent(object content) {
            MessageBox.Show(string.Format("{0} {1}", content));
        }

        private void button57_Click(object sender, EventArgs e) {
            string[] _n = "12,111,31,63,42,72,21,35".Split(',');
            int[] num = new int[_n.Length];
            for (int w = 0; w < _n.Length; w++)
                num[w] = Convert.ToInt32(_n[w]);

            Array.Sort(num);
            MessageBox.Show(num[GetSmallNum(21, num)].ToString());
            //StringBuilder sb = new StringBuilder();
            //foreach (int s in num)
            //    sb.Append(s).Append(',');
            //MessageBox.Show(sb.ToString().TrimEnd(','));
        }

        /// <summary>
        /// 自动取出最小值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected int GetSmallNum(int time, int[] nums) {
            if (time <= nums[0])
                return 0;
            if (time >= nums[nums.Length - 1])
                return nums.Length - 1;

            int l = 0, r, m;
            r = nums.Length;
            while (r - l > 1) {
                m = (l + r) / 2;
                if (time >= nums[m])
                    l = m;
                else
                    r = m;
            }
            return l;
        }

        private void button58_Click(object sender, EventArgs e) {
            System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
            transform.Load("c:/parse/parse.xslt");
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("c:/parse/result.xml");
            //transform.OutputSettings//= System.Text.Encoding.GetEncoding("GB2312");
            System.Xml.Xsl.XsltArgumentList xargs = new System.Xml.Xsl.XsltArgumentList();
            xargs.AddParam("ResumeId", "", 4000000001);
            xargs.AddParam("UserId", "", 4000000001);
            xargs.AddParam("Title", "", "测试");
            //transform.OutputSettings.Encoding = System.Text.Encoding.Default;
            transform.Transform(doc.CreateNavigator(), xargs, new System.IO.StreamWriter("c:/parse/result1.xml"));
            MessageBox.Show("完成");
        }

        private void button59_Click(object sender, EventArgs e) {
            this.lstResult.Items.Clear();
            CronExpression exp = new CronExpression(this.txtDataSet.Text);
#if DEBUG
            exp.NextEvent += new EventHandle(exp_NextEvent);
#endif
            MessageBox.Show(exp.Next(DateTime.Now).ToString());
        }

        void exp_NextEvent(object sender, EventArg e) {
            DateTime time = (DateTime)e.GetPara(0);
            this.lstResult.Items.Add(string.Format("{0},{1}:{2}", e.ToStringOfPara(1), e.ToStringOfPara(2), time));
            //            MessageBox.Show(time.ToString());
        }

        private void button60_Click(object sender, EventArgs e) {

            //PublicClass.Module.Trigger.CountTrigger trigger = new PublicClass.Module.Trigger.CountTrigger(5);
            //trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
            //for (int w = 0; w < 9; w++)
            //    trigger.Taste();

            //PublicClass.Module.Trigger.TimeTrigger trigger = new PublicClass.Module.Trigger.TimeTrigger(TimeSpan.FromSeconds(5));
            //trigger.TriggerEvent+=new EventHandle(trigger_TriggerEvent);
            //MessageBox.Show(trigger.PassTime.ToString());
            //PublicClass.Common.Tool.ObjectSleep(4000);
            //trigger.Taste();

            //PublicClass.Module.Trigger.CronExpressionTrigger trigger = new PublicClass.Module.Trigger.CronExpressionTrigger("0/5 * * * * ?");
            //trigger.TriggerEvent+=new EventHandle(trigger_TriggerEvent);
            //MessageBox.Show(trigger.PassTime.ToString());
            //PublicClass.Common.Tool.ObjectSleep(4000);
            //trigger.Taste();

            //PublicClass.Module.Trigger.TriggerProxy trigger = new PublicClass.Module.Trigger.TimerTriggerProxy(new PublicClass.Module.Trigger.CronExpressionTrigger("0/5 * * * * ?"), 500);
            //trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
            //trigger.SetEnable(true);

            //PublicClass.Module.Trigger.TriggerProxy trigger = new PublicClass.Module.Trigger.SerialTriggerProxy(new PublicClass.Module.Trigger.CountTrigger(5), new PublicClass.Module.Trigger.CountTrigger(2));
            //trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
            //for (int w = 0; w < 9; w++)
            //    trigger.Taste();

            //PublicClass.Module.Trigger.ParallelTriggerProxy trigger = new PublicClass.Module.Trigger.ParallelTriggerProxy();

            //trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
            ////trigger.SetEnable(true);
            //trigger.AddTrigger(new PublicClass.Module.Trigger.CountTrigger(5));

            //for (int w = 0; w < 5; w++)
            //    trigger.Taste();
            //trigger.AddTrigger(new PublicClass.Module.Trigger.CountTrigger(2));
            ////trigger.SetEnable(true);

            //for (int w = 0; w < 5; w++)
            //    trigger.Taste();


            trigger.Taste();


        }
        //PublicClass.Module.Trigger.DirectoryTrigger trigger = new PublicClass.Module.Trigger.DirectoryTrigger("*.txt");
        //PublicClass.Module.Trigger.ATrigger trigger = new PublicClass.Module.Trigger.SerialTriggerProxy(new PublicClass.Module.Trigger.CountTrigger(1),new PublicClass.Module.Trigger.FileTrigger("c:\\test.xml"));
        PublicClass.Module.Trigger.ATrigger trigger = new PublicClass.IO.Config.DirectoryConfigResource("c:\\*.xml", 1, TimeSpan.FromMinutes(1));

        void trigger_TriggerEvent(object sender, EventArg e) {
            MessageBox.Show("触发器触发!");
        }

        void trigger_TriggerEvent1(object sender, EventArg e) {
            MessageBox.Show("资源触发!\r\n" + ((PublicClass.IO.Config.AConfigResource)sender).Load());
        }
        private void button61_Click(object sender, EventArgs e) {
            //MessageBox.Show("1,".Split(',')[1].ToString());
            //PublicClass.IO.Config.ConfigManager manager = PublicClass.IO.Config.ConfigManagerFactory.GetConfigManagerFromFile(null, "c:\\base.xml");
            //manager = PublicClass.IO.Config.ConfigManagerFactory.GetConfigManagerFromDirectory(manager, "c:\\test*.xml", PublicClass.Common.Tool.DefaultEncoding, 500, TimeSpan.FromMinutes(1));
            ////manager = PublicClass.IO.Config.ConfigManagerFactory.GetConfigManagerFromFile(manager, "c:\\test1.xml");
            //MessageBox.Show(manager.GetConfig("AppSettings").GetValue("SendErrorEmailByMode").ToString());
            //manager.GetConfig("AppSettings").SetValue("test", 1);
            //manager.Update();
            MessageBox.Show(PublicClass.IO.Config.ConfigManagement.AppSettings("SendErrorEmailByMode"));
        }

        private void button62_Click(object sender, EventArgs e) {
            //MessageBox.Show(Application.ExecutablePath + ".conf");
            //LengthAndCronNameTrigger trigger = new LengthAndCronNameTrigger(10, "0 0 0 * * ?", "{0:yyyyMMddHHmmss}");
            //测试NULL
            //MessageBox.Show(trigger.Taste(null));
            //测试num
            //MessageBox.Show(trigger.Taste("c:\\test1.xml"));
            //            PublicClass.IO.Log.Logger
            //ILogResource resource = new TextFileLogResource("c:\\Apply{0}.txt", PublicClass.IO.IOTool.DefaultEncoding, 10, new LengthAndCronNameTrigger(10, "0 0 0 * * ?", "{0:yyyyMMdd}"));
            //Listener listener = new Listener(new PublicClass.Module.Trigger.ATrigger[] { new LogTypeTrigger(LogType.DEBUG, LogType.WARN), new SourceTrigger("abc") }, new LogRecord(), new ILogResource[] { resource });
            //Logger logger = new Logger(new LogRecord(LogType.TEST, true, true, true));
            //logger.Listener = listener;
            //Logger logger1 = logger;
            //logger = logger.CreateSourceLoggerDecorator("abc");
            ////, resource);
            Logger logger = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("log", "logger") as Logger;
            logger.Release("实现测试");
            logger.Error("错误测试");
            logger.Warn("警告测试");
            logger.Info("通知测试");
            logger.Debug("调试测试");
            logger.Test("测试测试");
            //logger.Close();
            //Listener listener = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("log", "listener1") as Listener;
            //listener.Init(null, null);
            //listener.Close(null, null);
            //resource.Init();
            //resource.Write(LogType.ERROR, "错误测试!");
            //resource.Write(LogType.WARN, "警告测试!");
            //resource.Write(LogType.RELEASE, "信息测试!");
            //resource.Write(LogType.INFO, "执行信息测试!");
            //resource.Close();
        }

        private void button63_Click(object sender, EventArgs e) {

            //PublicClass.Bean.BeanBandingFlags flag = (PublicClass.Bean.BeanBandingFlags)"PublicClass.Bean.BeanBandingFlags.Map";
            //证明 对于枚举类型需要设置为int值            object c = 2;            this.GetType().GetMethod("TestEnum").Invoke(this, new object[] { c });
            //System.Reflection.MethodInfo ma = typeof(Convert).GetMethod("ToInt64", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new Type[]{typeof(object)},null);
            //MessageBox.Show(Convert.ToString(ma.Invoke(null, new object[] { "23" })));
            //LinkedList<string> list = new LinkedList<string>();
            //list.AddLast("1");
            //list.AddLast("2");
            //StringBuilder sb=new StringBuilder();
            //for (IEnumerator<string> ienum = list.GetEnumerator(); ienum.MoveNext(); )
            //    sb.Append(ienum.Current).Append(",");
            //MessageBox.Show(sb.ToString());
            //MessageBox.Show(typeof(Tool).InvokeMember("GetValue", BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, new Tool(), new object[] { "", "2" }).ToString());
            MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("mq", "activemq").GetType().Name);
        }

        public void TestEnum(PublicClass.Bean.BeanBandingFlags flag) {
            MessageBox.Show("收到" + flag.ToString());
        }

        private void button64_Click(object sender, EventArgs e) {
            //string a = new dllEncrypt().EncryptString(this.txtResult.Text);
            //MessageBox.Show(a);
            //MessageBox.Show(new dllEncrypt().DecryptString(a));
            ////System.Security.Cryptography.SymmetricAlgorithm
            ////new System.Security.Cryptography.SymmetricAlgorithm
            //dllEncrypt.mCSP.GenerateIV();
            //this.txtResult.Text = Convert.ToBase64String(dllEncrypt.mCSP.IV);
            //Assembly.LoadFrom("MyProcessController.dll");
            //MessageBox.Show(Type.GetType("GeneralClassLibrary.Project.MyProcessController.TestEntity,MyProcessController") == null ? "N" : "Y");
            //MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "type") == null ? "N" : "Y");
            //MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("mpc", "BillServer") == null ? "N" : "Y");
            //MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("mpc", "formatter").ToString());
            // PublicClass.Bean.Middler.Middlement.GetObjectByAppName("mpc", "queue");

            //string[] a = new string[] { "Test.TestEntity" };
            //使用object类型不可行
            //object[] a = new object[] { "Test.TestEntity" };
            //object[] a = new Type[] { typeof(TestEntity) };
            //Assembly.LoadFrom("System.Messaging.dll");
            //ConstructorInfo info=Type.GetType("System.Messaging.XmlMessageFormatter,System.Messaging").GetConstructor(new Type[]{typeof(string[])});
            //MessageBox.Show(info.Invoke(new object[]{a}).ToString());
            //MessageBox.Show(Activator.CreateInstance(Type.GetType("System.Messaging.XmlMessageFormatter,System.Messaging"), new object[] { a }).ToString());
            //Assembly.LoadFrom("MyProcessController.dll");
            //object a = Type.GetType("GeneralClassLibrary.Project.MyProcessController.TestEntity,MyProcessController").InvokeMember("ToType", BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { new object[] { typeof(TestEntity) } });
            //MessageBox.Show(a == null ? "N" : "Y");

            //int i = 15;
            //Boss.Entities.BillMessage message = new Boss.Entities.BillMessage();
            //message.transactionID = Guid.NewGuid();
            //message.userID = i.ToString();
            //message.stampTime = DateTime.Now;
            //message.spid = i.ToString();
            //message.orgID = i.ToString();
            //message.iPAddress = i.ToString();
            //message.fee = i;
            //message.courseWareID = i.ToString();
            //message.courseID = i.ToString();
            //message.courseGroupID = i.ToString();
            //message.billingType = Boss.Entities.BillType.包次订购话单;
            //message.billingStyle = Boss.Entities.BillStyle.代计费;
            //BBill b = new BBill();
            //PublicClass.Bean.BeanTool.Transport(message, b);
            ////b.Fill(message);
            //b.stampTime = DateTime.Now.AddMonths(2);
            //MessageBox.Show(b.stampTime.Month.ToString());
            //new Boss.DAL.BillDAO().AddBill(b);
            //new Boss.DAL.BillDAO().AddLastBill(b);
            //new Boss.DAL.BillDAO().AddErrorBill(b, Boss.Entities.BillErrorType.普通错误);

            //BeanTool属性设置测试通过！
            //TestE t = new TestE();
            //t.content = "哈哈!";
            //MessageBox.Show(Analyze(typeof(TestE)).ToString());
            ////除This方式外可以使用如下方法
            ////PublicClass.Bean.BeanTool.SetPropertyValueSP(t, "content", typeof(string), "b", PublicClass.Bean.BeanBandingFlags.All;
            ////This设置使用的方法
            //PublicClass.Bean.BeanTool.SetPropertyValue(t,"", new Type[]{typeof(string),typeof(string)},new object[]{"content","b"}, PublicClass.Bean.BeanBandingFlags.This);
            //MessageBox.Show(PublicClass.Bean.BeanTool.GetPropertyValueSP(t, "content", typeof(string), "b", PublicClass.Bean.BeanBandingFlags.Map).ToString());
            //MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("mq", "type") == null ? "N" : "Y");
            //MessageBox.Show(typeof(System.Messaging.XmlMessageFormatter).AssemblyQualifiedName);
            //调用公共组件时需要这样调用
            //MessageBox.Show(Type.GetType("System.Data.Odbc.OdbcFactory,System.Data,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089") == null ? "N" : "Y");
            //MessageBox.Show(PublicClass.Bean.Middler.Middlement.GetObjectByAppName("middler", "formatter") == null ? "N" : "Y");
            //MessageBox.Show((PublicClass.Bean.Middler.Middlement.GetObjectByAppName("bean", "TestE") as TestE).Content);
            MessageBox.Show(new TestE() {
                Content = "heihei",
                Content2 = "haha"
            }.Content);
            //TestE a = new TestE();
            //PublicClass.Bean.BeanTool.SetPropertyValue(a, "Content2", new object[] { "a", "b" });
            //MessageBox.Show(a.Content);
        }


        //class BBill : Boss.Entities.ABill {
        //    protected override LCMS.Entities.Course GetCourseByID(string id) {
        //        throw new NotImplementedException();
        //    }

        //    protected override LCMS.Entities.CourseGroup GetCourseGroupByID(string id) {
        //        throw new NotImplementedException();
        //    }

        //    protected override LCMS.Entities.CourseWare GetCourseWareByID(string id) {
        //        throw new NotImplementedException();
        //    }

        //    protected override SAASCore.Organization GetOrganizationByID(string id) {
        //        throw new NotImplementedException();
        //    }

        //    protected override SAASCore.User GetUserByID(string id) {
        //        throw new NotImplementedException();
        //    }
        //}

        private void button65_Click(object sender, EventArgs e) {
            //获取基本的数据库连接 和测试 3个Factory测试完成 2个Resource测试完成 
            //IDataAbstractFactory fac = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "factory") as IDataAbstractFactory;
            //IDataResource fac = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "resource") as IDataResource;

            //DbConnection conn = fac.GetConnection();
            ////OleDB
            //conn.ConnectionString = @"Provider=SQLOLEDB.1;Password=test;Persist Security Info=True;User ID=login_user;Initial Catalog=test;Data Source=.\LXR";
            ////SQL
            //conn.ConnectionString = @"Password=test;Persist Security Info=True;User ID=login_user;Initial Catalog=test;Data Source=.\LXR";
            ////ODBC
            //conn.ConnectionString = @"Driver={sql server};server=.\LXR;database=test;uid=login_user;pwd=test;";
            //conn.Open();
            //DbCommand comm = fac.CreateCommand();
            //comm.CommandText = "SELECT * FROM [dbo].[test1]";
            //comm.Connection = conn;
            //DbDataAdapter adapter = fac.CreateAdapter();
            //adapter.SelectCommand = comm;
            //DataSet ds = new DataSet();
            //adapter.Fill(ds);
            //NiDataResult res = new NiDataResult();

            //Ni所有Command测试完成 
            ////NiFillDataCommand
            //new NiFillDataCommand().ExcuteCommand(fac, comm, res);
            ////NiQueryDataCommand
            //new NiQueryDataCommand().ExcuteCommand(fac, comm, res);
            //NiNonQueryDataCommand
            //new NiNonQueryDataCommand().ExcuteCommand(fac, comm, res);
            //NiReaderDataCommand;
            //res.ReaderEvent += new EventHandle(res_ReaderEvent);
            //new NiReaderDataCommand().ExcuteCommand(fac, comm, res);

            //while (res.Reader.Read())
            //    MessageBox.Show(res.Reader["createTime"].ToString());
            //MessageBox.Show(res.GetFirstCell().ToString());
            //res.Reader.Close();
            //fac.SetConnection(conn);
            //conn.Close();

            //测试ParameterEntity功能 测试NiDataParameters功能
            //NiDataParameters paras = new NiDataParameters();
            //paras.IsAutoCacheParameters = true;

            //测试核心方法通过
            //IDictionary idic = new Hashtable();
            //idic["@id"] = 2;
            //DbParameter[] paras2 = paras.GetParas("cache", fac, new ParameterEntity[] { GenerateParameterEntity("@a"), this.GenerateParameterEntity("@b") }, idic);
            //测试第一种变通方法通过 针对对象带或者不带属性
            //DbParameter[] paras2 = paras.GetParas("cache", fac, new ParameterEntity[] { GenerateParameterEntity("@A"), this.GenerateParameterEntity("@B") }, new TestEntity());
            //测试第二种变通方法通过 针对对象带有属性或者不带有属性
            //DbParameter[] paras2 = paras.GetParas("cache", fac, new TestEntity());
            //MessageBox.Show(paras2[0].Value.ToString());
            //MessageBox.Show(this.Analyze(typeof(TestEntity)));
            //MessageBox.Show(typeof(TestEntity).GetProperties()[0].GetCustomAttributes(typeof(NiDataParameterAttribute), true).Length > 0 ? "Y" : "N");
            NiTemplate template = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "template") as NiTemplate;
            int i = 256;
            //System.Collections.IDictionary idic = new Hashtable();

            //idic["@TransactionID"] = Guid.NewGuid().ToString();
            //idic["@UserID"] = i.ToString();
            //idic["@StampTime"] = DateTime.Now;
            //idic["@Spid"] = i.ToString();
            //idic["@OrgID"] = i.ToString();
            //idic["@IPAddress"] = i.ToString();
            //idic["@Value"] = i;
            //idic["@CourseWareID"] = i.ToString();
            //idic["@CourseID"] = i.ToString();
            //idic["@CourseGroupID"] = i.ToString();
            //idic["@BillingType"] = Boss.Entities.BillType.包次订购话单;
            //idic["@BillingStyle"] = Boss.Entities.BillStyle.代计费;
            //测试成功
            //template.ExcuteNonQuery("AddBillDetail", idic);
            //Boss.Entities.BillMessage message = new Boss.Entities.BillMessage();
            //message.transactionID = Guid.NewGuid();
            //message.userID = i.ToString();
            //message.stampTime = DateTime.Now;
            //message.spid = i.ToString();
            //message.orgID = i.ToString();
            //message.iPAddress = i.ToString();
            ////这个参数与对象不符。所以不能使用如下方法
            //message.fee = i;
            //message.courseWareID = i.ToString();
            //message.courseID = i.ToString();
            //message.courseGroupID = i.ToString();
            //message.billingType = Boss.Entities.BillType.包次订购话单;
            //message.billingStyle = Boss.Entities.BillStyle.代计费;
            //template.ExcuteNonQuery("AddBillDetail", message);
            template.Commit();
            //template.ExcuteScalar("SELECT * FROM [dbo].[test1] Where ID > @id", CommandType.Text, new ParameterEntity[] { GenerateParameterEntity("@id") }, idic);
            //template.ExcuteScalar("select count(*) from [dbo].[test1]");
            //MessageBox.Show(template.Commit().GetFirstCell(1).ToString());
            //idic.Clear();
            PublicClass.Bean.Middler.Middlement.SetObjectByAppName("ni", "template", template);
            //MessageBox.Show(new NiConfigConvert().ToConfig("<Ni><Command name=\"AddBillDetail\">      <Content>usp_Boss_AddBillDetail</Content>      <String name=\"@TransactionID\" Size=\"32\"/>      <String name=\"@UserID\" Size=\"32\"/>      <String name=\"@SPID\" Size=\"32\"/>      <String name=\"@OrgID\" Size=\"32\"/>      <String name=\"@CourseGroupID\" Size=\"32\"/>      <String name=\"@CourseID\" Size=\"32\"/>      <String name=\"@CourseWareID\" Size=\"32\"/>      <DateTime name=\"@StampTime\"/>      <Int32 name=\"@Value\"/>      <Int32 name=\"@BillingType\"/>      <String name=\"@IPAddress\" Size=\"64\"/>      <Int32 name=\"@BillingStyle\"/>    </Command></Ni>").GetValue("AddBillDetail").ToString());
            //PublicClass.IO.Config.ConfigManager ma = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "ma") as PublicClass.IO.Config.ConfigManager;

            //MessageBox.Show(ma.GetConfigValue("Ni", "AddBillDetail").ToString());
            //ma.Dispose();
        }

        ParameterEntity GenerateParameterEntity(string name) {
            ParameterEntity entity = new ParameterEntity();
            entity.DBTypeName = "Int32";
            entity.ParameterDirection = ParameterDirection.Input;
            entity.ParameterName = name;
            entity.Nullable = false;
            entity.DefaultValue = 1;
            return entity;
        }
        void res_ReaderEvent(object sender, EventArg e) {
            MessageBox.Show(((DbDataReader)e.GetPara(0))["createTime"].ToString());
        }

        private void button66_Click(object sender, EventArgs e) {
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream sw = new MemoryStream();
            formatter.Serialize(sw, new TestEntity());
            byte[] data = sw.ToArray();
            sw.Close();
            File.AppendText("").WriteLine(System.Text.Encoding.ASCII.GetString(data));

            data = System.Text.Encoding.ASCII.GetBytes(System.Text.Encoding.ASCII.GetString(data));
            sw = new MemoryStream();
            sw.Write(data, 0, data.Length);
            sw.Seek(0, SeekOrigin.Begin);
            MessageBox.Show((formatter.Deserialize(sw) as TestEntity).Boc.ToString());
            sw.Close();
        }

        private void button67_Click(object sender, EventArgs e) {
            ////算法修改
            for (int w = 0; w < 1000000; w++) {
                NiTemplate template = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("ni", "template") as NiTemplate;
                IDictionary idic = new Hashtable();
                idic["@text"] = CreateNewID2();
                template.ExcuteNonQuery("AddLog", idic);
                PublicClass.Bean.Middler.Middlement.SetObjectByAppName("ni", "template", template);
            }
            MessageBox.Show("OK!");
            //MessageBox.Show(CreateNewID2());
        }

        public static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\W");
        protected string CreateNewID2() {
            string v = CreateNewID3(CreateNewID());
            //while (regex.Matches(v).Count > 0) { MessageBox.Show(v, "", MessageBoxButtons.OK, MessageBoxIcon.Error); v = CreateNewID3(CreateNewID()); }
            return v;
        }
        protected string CreateNewID3(string a) {
            byte[] data = Encoding.ASCII.GetBytes(a);
            StringBuilder sb = new StringBuilder();
            byte c = 255;
            foreach (byte b in data) {
                sb.Append((b ^ c).ToString());
                c = b;
            }
            return sb.ToString().Substring(3, 16);
        }
        protected string CreateNewID() {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private void button68_Click(object sender, EventArgs e) {
            //XmlTextReader reader = new XmlTextReader("c:/XMLFile1.xml");
            //XmlValidatingReader v = new XmlValidatingReader(reader);
            //v.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(v_ValidationEventHandler);
            //v.ValidationType = ValidationType.Schema;
            //while (v.Read()) ;
            //v.Close();
            //XmlDocument doc = new XmlDocument();
            //doc.Load("c:\\XMLFile1.xml");
            //MessageBox.Show(Tool.GetValue(doc.SchemaInfo,"N").ToString());
            PublicClass.IO.XmlValidate validate = new PublicClass.IO.XmlValidate("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Config>  <ConfigConverts xmlns=\"http://tempuri.org/config.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://tempuri.org/config.xsd config.xsd\">    <ConfigConvert name=\"ConfigConverts\" type=\"PublicClass.IO.Config.ConfigConvert\" dll=\"\" />    <ConfigConvert name=\"AppSettings\" type=\"PublicClass.IO.Config.AppSettingsConfigConvert\" dll=\"\" />    <ConfigConvert name=\"ConnectionStrings\" type=\"PublicClass.IO.Config.ConnectionStringsConvert\" dll=\"\" />    <ConfigConvert name=\"Middler\" type=\"PublicClass.Bean.Middler.NetMiddlerConfigConvert\" dll=\"\" />  </ConfigConverts></Config>", ValidationType.Schema);
            validate.ValidationEvent += new EEventHandle<System.Xml.Schema.ValidationEventArgs>(validate_ValidationEvent);
            MessageBox.Show(validate.Validate().ToString());
        }

        void validate_ValidationEvent(object sender, System.Xml.Schema.ValidationEventArgs e) {
            MessageBox.Show(e.Message);
        }

        void v_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e) {
            MessageBox.Show(e.Message);
        }

        private void button69_Click(object sender, EventArgs e) {
            PublicClass.Module.DESXcrypt.MSCP.GenerateKey();
            PublicClass.Module.DESXcrypt.MSCP.GenerateIV();
            this.txtResult.Text = Convert.ToBase64String(PublicClass.Module.DESXcrypt.MSCP.Key) + ":" + Convert.ToBase64String(PublicClass.Module.DESXcrypt.MSCP.IV);
        }

        private void bun_TryParse_Click(object sender, EventArgs e) {
            int ss = 0;
            //MessageBox.Show(int.TryParse(DBNull.Value.ToString(),out ss)?"1":"0");
            //测试输入字符串时是否会报错，测试结果 输入不是数字的字符串 空字符串 null 都未报错，正确判断是否为数字.
            string[] sss = new string[0];//null
            foreach (string s in sss) MessageBox.Show(s); //in 中不能是 null
            MessageBox.Show(new ArrayList().ToArray(typeof(string)).Length + "");
        }

        private void button70_Click(object sender, EventArgs e) {
            MessageBox.Show("....".TrimEnd('.'));
        }

        private void button71_Click(object sender, EventArgs e) {
            StringBuilder sb = new StringBuilder();
            foreach (string s in System.IO.File.ReadAllLines("d:/del.txt")) {
                //sb.AppendFormat("{0}:{1}", s, GeneralClassLibrary.Module.DESXcrypt.DESDeCode(s, "TR08vZUI"));
                sb.AppendFormat("{0}:{1}", s, new PublicClass.Module.DESXcrypt("TR08vZUI","").Decrypt(s));
                sb.AppendLine();
            }
            Clipboard.SetData(DataFormats.UnicodeText, sb.ToString());
            MessageBox.Show(sb.ToString());
        }

        private void button72_Click(object sender, EventArgs e) {
            int[] array = new int[] { 1, 22, 4, 6, 7, 43, 4, 7, 8, 49, 56, 575, 768, 973, 35, 44, 23, 36, 6666, 4, 3, 34 };
            array = GBsort(array);
            StringBuilder sb = new StringBuilder();
            foreach (int w in array) {
                sb.Append(w).Append(";");
            }
            MessageBox.Show(sb.ToString());
            sb.Remove(0, sb.Length);
            sb = null;
        }

        public int[] GBsort(int[] array) {
            int len = (int)Math.Pow(2, Math.Ceiling(Math.Log(array.Length, 2)));
            int size = 2;
            while (size <= len) {
                for (int w = 0; w < array.Length; w += size) {
                    int _size = (w + size) > array.Length ? array.Length - w : size;
                    int l = w, r = l + size / 2;
                    int[] c = new int[_size];
                    for (int q = 0; q < _size; q++) {
                        if (r >= (w + _size)) {
                            c[q] = array[l];
                            l++;
                        } else if (l >= (w + size / 2)) {
                            c[q] = array[r];
                            r++;
                        } else if (array[l] < array[r]) {
                            c[q] = array[l];
                            l++;
                        } else {
                            c[q] = array[r];
                            r++;
                        }
                    }
                    //完成复制工作
                    for (int q = 0; q < _size; q++) array[w + q] = c[q];
                }
                size *= 2;
            }
            return array;
        }

        public int[] KSsort(int[] array) {
            Queue queue = new Queue();
            queue.Enqueue(new int[] { 0, array.Length - 1 });
            while (queue.Count > 0) {
                bool l2r = true;
                int[] _data = queue.Dequeue() as int[];
                int l = _data[0], r = _data[1];
                while (l != r) {
                    if (array[l] < array[r]) {
                        if (l2r) r--; else l++;
                    } else {
                        int temp = array[l];
                        array[l] = array[r];
                        array[r] = temp;
                        if (l2r) l++; else r--;
                        l2r = !l2r;
                    }
                }
                if (l - _data[0] > 1) queue.Enqueue(new int[] { _data[0], l - 1 });
                if (_data[1] - l > 1) queue.Enqueue(new int[] { l + 1, _data[1] });
            }
            return array;
        }
    }

    [Serializable]
    class TestEntity {
        [NiDataParameterAttribute("@a", ParameterDirection.Output)]
        public int A { get; set; }
        [NiDataParameterAttribute("@b")]
        public int B { get; set; }

        public int ABC { get; set; }

        private int boc = 112;
        public int Boc {
            get { return boc; }
            set { boc = value; }
        }

        public TestEntity() {
            this.A = 20;
            this.B = 21;
            this.ABC = 30;
        }
    }
    public class IkvF : PublicClass.Collections.Pool.IPoolValueFactory {

        #region IPoolValueFactory Members

        int num = 0;
        public object CreateObject(object e) {
            return "" + num++;
        }

        public void CloseObject(object obj) {
        }

        #endregion
    }
    public class ApplyEntity {
        private int m_id;

        public int Id {
            get { return m_id; }
            set { m_id = value; }
        }

        public static ApplyEntity ToEntity(System.Collections.IDictionary idic) {
            ApplyEntity entity = new ApplyEntity();
            for (System.Collections.IDictionaryEnumerator ide = idic.GetEnumerator(); ide.MoveNext(); ) {
                try {
                    System.Reflection.PropertyInfo info = (entity.GetType().GetProperty(ide.Key.ToString().Substring(1), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.IgnoreCase));
                    if (info != null)
                        info.SetValue(entity, ide.Value, null);
                } catch (System.ArgumentNullException) {
                } catch (System.ArgumentException) {
                } catch (System.FieldAccessException) {
                } catch (System.Reflection.TargetException) {
                }
            }
            return entity;
        }

        public System.Collections.IDictionary ToDictionary(System.Collections.IDictionary idic) {
            idic.Clear();
            System.Reflection.PropertyInfo[] infos = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);

            foreach (System.Reflection.PropertyInfo info in infos) {
                try {
                    idic["@" + info.Name] = info.GetValue(this, null);
                } catch (System.NotSupportedException) {
                } catch (System.ArgumentException) {
                } catch (System.FieldAccessException) {
                } catch (System.Reflection.TargetException) {
                }
            }
            return idic;
        }

        public System.Collections.Hashtable ToHashtable() {
            return ToDictionary(new System.Collections.Hashtable()) as System.Collections.Hashtable;
        }
    }
    public class Entity {
        private int a;
        private string b;
        public int a1;

        public string BB {
            get { return b; }
            set { b = value; }
        }

        public string Test(int b) {
            return b.ToString();
        }
        public string Test(String a) {
            return b.ToString();
        }

        public string this[int id] {
            get {
                switch (id) {
                    case 1:
                        return a.ToString();
                    default:
                    case 2:
                        return b;
                }
            }
            set {
                switch (id) {
                    case 1:
                        this.a = int.Parse(value);
                        break;
                    case 2:
                        this.b = value;
                        break;
                }
            }
        }

        public static string Test2() {
            return "Test2";
        }
    }
    public class TestE {
        public string content;
        public string Content {
            get { return content; }
            set { content = value; }
        }
        public string Content2 {
            get { return content; }
            set { content = value; }
        }
        public string GetContent() {
            return content;
        }
        public void SetContent(string content) {
            this.content = content;
        }
        public void SetContent2(string content, string a) {
            this.content = content;
        }
        public void SetContent3(string[] content) {
            this.content = content[0];
        }

        public string Get(string t) {
            return content;
        }

        public void Set(string content) {
            this.content = content;
        }

        public string this[string value1] {
            get { return content; }
            set { content = value; }
        }
    }
}