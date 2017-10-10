using GCL.Db.Ni;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test {
    public class entity {
        public string Name; 
        public int Count;
    }
    class Program {
        #region Mongo
        //static void Main(string[] args)
        //{
        //    #region Mongo
        //    #region 多线程
        //    /*
        //    ThreadPool.QueueUserWorkItem(stat =>
        //        {
        //            Console.WriteLine("插入");
        //            Stopwatch sw = new Stopwatch();
        //            sw.Start();
        //            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //            var dec = middler.GetObjectByAppName("Ni", "decorator");
        //            NiTemplateDecorator deco = dec as NiTemplateDecorator;
        //            deco.Transaction = true;
        //            for (int i = 2000; i < 3000; i++)
        //            {
        //                Console.WriteLine(i + "插入");
        //                //插入
        //                deco.ExcuteNonQuery("NoSqlTest.Insert", new Hashtable
        //           {
        //             {"Id1",++i},
        //             {"Name1","eee"},
        //             {"Age1",5+i},
        //             {"Id2",++i},
        //             {"Name2","ggg"},
        //             {"Age2",6*i}
        //          });
        //                Console.WriteLine(i);
        //            }
        //            deco.Commit();
        //            Console.WriteLine("................................");
        //            new Thread(() =>
        //            {
        //                Console.WriteLine("更新");
        //                var middler1 = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //                var dec1 = middler1.GetObjectByAppName("Ni", "decorator") as NiTemplateDecorator;
        //                for (int i = 0; i < 1000; i++)
        //                {
        //                    Console.WriteLine("更新" + i);
        //                    dec1.ExcuteQuery("NoSqlTest.Update", new Hashtable
        //            {
        //                {"Id",i},
        //                {"Name","222"}
        //            });
        //                }
        //                Console.WriteLine("更新结束");
        //            }).Start();

        //            Console.WriteLine("ok");
        //            sw.Stop();
        //            Console.WriteLine(sw.ElapsedMilliseconds);
        //        }, null);
        //    Console.WriteLine("主线程");

        //    Console.WriteLine("结束了吧");
        //    Console.ReadKey(); 
        //     */
        //    #endregion

        //    //var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //    //var test = middler.GetObjectByAppName("Ni", "mongoFactory");
        //    //var dec = middler.GetObjectByAppName("Ni", "decorator");
        //    //NiTemplateDecorator deco = dec as NiTemplateDecorator;
        //    ////插入
        //    //var r1 = deco.ExcuteNonQuery("NoSqlTest.Insert", new Hashtable
        //    //{
        //    //    {"Id1",4},
        //    //    {"Name1","CustomerLevelSet"},
        //    //    {"Age1",5},
        //    //    {"Id2",5},
        //    //    {"Name2","CustomerLevelSet1"},
        //    //    {"Age2",6}
        //    //});
        //    //Console.ReadKey();
        //    //////查询
        //    //var r2 = deco.ExcuteQuery("NoSqlTest.Select", new Hashtable
        //    //{

        //    //});
        //    //Console.ReadKey();

        //    ////更新
        //    //var r3 = deco.ExcuteQuery("NoSqlTest.Update", new Hashtable
        //    //{
        //    //    {"Id",4},
        //    //    {"Name","222"}
        //    //});
        //    //Console.ReadKey();
        //    ////查询
        //    //var r4 = deco.ExcuteQuery("NoSqlTest.Select", new Hashtable
        //    //{

        //    //});
        //    //Console.ReadKey();

        //    //删除
        //    //var r5 = deco.ExcuteQuery("NoSqlTest.Delete", new Hashtable
        //    //{
        //    //    {"Id",11}
        //    //});
        //    //Console.ReadKey();
        //    //查询
        //    //var r6 = deco.ExcuteQuery("NoSqlTest.Select", new Hashtable
        //    //{

        //    //});
        //    //Console.ReadKey();

        //    ////单独查询
        //    //var r7 = deco.ExcuteQuery("NoSqlTest.Select", new Hashtable
        //    //{

        //    //});
        //    //Console.ReadKey();
        //    #endregion
        //    Thread th1 = new Thread(new ThreadStart(Run));
        //    th1.Start();
        //    th1.Join();

        //    Thread th2 = new Thread(new ThreadStart(Run1));
        //    th2.Start();
        //    th2.Join();

        //    Thread th3 = new Thread(new ThreadStart(Run2));
        //    th3.Start();
        //    th3.Join();
        //    Console.WriteLine("主线程");
        //    Console.ReadKey();
        //}
        //private static void Run()
        //{
        //    try
        //    {
        //        var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //        var dec1 = middler.GetObjectByAppName("Ni", "decorator") as NiTemplateDecorator;
        //        var res = dec1.ExcuteNonQuery("NoSqlTest.insert", new Hashtable
        //        {
        //            {"Id1",1098765},
        //            {"Name1","vvvvv"},
        //            {"Age1",33},
        //            {"Id2",190921},
        //            {"Name2","dududu2"},
        //            {"Age2",2}
        //        });
        //        Console.WriteLine("Run");
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //private static void Run1()
        //{
        //    var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //    var dec1 = middler.GetObjectByAppName("Ni", "decorator") as NiTemplateDecorator;
        //    dec1.Transaction = true;
        //    var r2 = dec1.ExcuteNonQuery("NoSqlTest.insert", new Hashtable
        //        {
        //            {"Id1",111111},
        //            {"Name1","vvvvv"},
        //            {"Age1",33},
        //            {"Id2",92111},
        //            {"Name2","dududu2"},
        //            {"Age2",2}
        //        });
        //    var res = dec1.ExcuteNonQuery("NoSqlTest.Delete", new Hashtable
        //        {
        //            {"Id",2001}
        //        });

        //    dec1.Commit();
        //    Console.WriteLine("Run1");

        //}
        //private static void Run2()
        //{
        //    try
        //    {
        //        var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
        //        var dec1 = middler.GetObjectByAppName("Ni", "decorator") as NiTemplateDecorator;
        //        var res = dec1.ExcuteNonQuery("NoSqlTest.Update", new Hashtable
        //        {
        //            {"Id",3000},
        //            {"Name",1}
        //        });
        //        Console.WriteLine("Run2");
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //} 
        #endregion
        static void Main(string[] args) {
            //var oldTime = DateTime.Now;
            //var passTime = exp.Next(oldTime);

            Console.WriteLine(GCL.Common.Tool.PostString("http://123.127.153.137:8011/module/Terminate/terminate.string", "baibing=2"));
            Console.ReadKey();
            return;
            //DataTable dt = GCL.Db.DBTool.ToTable<entity>(new entity[] { new entity { Name = "A", Count = 1 }, new entity { Name = "B", Count = 2 } });
            var ss = new GCL.Module.DESXcrypt("dQ69muDQO5Xq2I0Q2278+ZTKq/JnecDk", "wjG3Vwmk34U=");
            var word = ss.Encrypt("abc");
            Console.WriteLine(word);//36E8tNE6Yzg=
            Console.WriteLine(ss.Encrypt("abc"));
            Console.WriteLine(ss.Decrypt(ss.Encrypt("abc")));
            Console.WriteLine(ss.Decrypt(ss.Encrypt("abc")));
            Console.ReadKey();
            Console.WriteLine(GCL.Common.Tool.DownloadString("http://127.0.0.1:81", "a=2"));
            DataTable dt = GCL.Db.DBTool.ToTable<entity>(null);
            Console.WriteLine(GCL.Db.DBTool.ToJSON(dt));
            var middler1 = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
            var log = middler1.GetObjectByAppName<GCL.IO.Log.Logger>("VESH", "GCL.Project.VESH.E.Module.AModule.Logger");
            while (true) {
                log.Error("1");
                //if (passTime.CompareTo(DateTime.Now) <= 0) {
                //    string value = GCL.Common.Tool.FormatDate(oldTime);
                //    if (passTime.CompareTo(DateTime.Now) <= 0) {
                //        oldTime = passTime;
                //        value = GCL.Common.Tool.FormatDate(passTime);
                //        passTime = exp.Next(passTime);
                //    }
                //    Console.WriteLine(string.Format("{0}:{1}", GCL.Common.Tool.FormatNow(), value));
                //}
                GCL.Common.Tool.ObjectSleep(500);
            }
            return;
            var ret = GCL.Common.Tool.PostString("http://10.32.34.40/receipt/Module/receipt/getOrder.json?_n=MT&userId=101&id=110", "");
            var ret2 = GCL.Common.Tool.SerializeToString(new object[] { new object[] { new object[] { new { a = 1, b = 2 }, new { a = 3, b = 4 } } } });
            var rs = GCL.Common.Tool.Deserialize<List<List<List<IDictionary<string, string>>>>>(ret);
            return;
            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
            var tem = middler.GetObjectByAppName<NiTemplate>("Ni", "decorator");
            var r = tem.ExcuteNonQuery("Test.Insert", new Hashtable
                {
                    {"Id1",4},
                    {"Text1","3333"}
                });
            Console.ReadKey();
            ////查询
            var r2 = tem.ExcuteQuery("Test.Select", new Hashtable {

            });
            r2 = tem.ExcuteQuery("Test.Select", new Hashtable {

            });
            var r1 = tem.ExcuteNonQuery("NoSqlTest.Insert", new Hashtable
                {
                    {"Id1",4},
                    {"Name1","CustomerLevelSet"},
                    {"Age1",5},
                    {"Id2",5},
                    {"Name2","CustomerLevelSet1"},
                    {"Age2",6}
                });
            Console.WriteLine("写入完成");
            Console.WriteLine(r1.DataSet.Tables[0].Columns[0].ColumnName);
            Console.WriteLine(r1.DataSet.Tables[0].Rows[0][0]);


            ////查询
            var r3 = tem.ExcuteQuery("NoSqlTest.Select", new Hashtable {

            });
            Console.ReadKey();
            var r7 = tem.ExcuteQuery("NoSqlTest.Select", new Hashtable {

            });
            Console.ReadKey();

            //更新
            var r31 = tem.ExcuteQuery("NoSqlTest.Update", new Hashtable
            {
                {"Id",4},
                {"Name","222"}
            });
            Console.ReadKey();
            //查询
            var r4 = tem.ExcuteQuery("NoSqlTest.Select", new Hashtable {

            });
            Console.ReadKey();

            // 删除
            var r5 = tem.ExcuteQuery("NoSqlTest.Delete", new Hashtable
            {
                {"Id",11}
            });
            Console.ReadKey();
            // 查询
            var r6 = tem.ExcuteQuery("NoSqlTest.Select", new Hashtable {

            });
            Console.ReadKey();

            //单独查询





            // var res1 = temp3.ExcuteQuery("UserInfo.insert", new Hashtable
            // {
            //     {"id1",3},
            //     {"name1","test3"},
            //     {"age1",13},
            //     {"gender1","男3"},
            //     {"phone1","11023453983"},
            //     {"id2",4},
            //     {"name2","test4"},
            //     {"age2",24},
            //     {"gender2","男4"},
            //     {"phone2","11023453984"}
            // });
            // Console.WriteLine("写入完成");
            // Console.WriteLine(res.DataSet.Tables[0].Columns[0].ColumnName);
            // Console.WriteLine(res.DataSet.Tables[0].Rows[0][0]);
            // Console.ReadKey();


            // var temp = middler.GetObjectByAppName<NiTemplate>("Ni", "memcached");

            // var res1 = temp.ExcuteQuery("UserInfo.select", new Hashtable
            // {                  
            // });
            // Console.WriteLine(res1.DataSet.Tables.Count);
            // Console.WriteLine(res1.DataSet.Tables[0].Rows.Count);
            // foreach (string item in res1.DataSet.Tables[0].Rows[0].ItemArray)
            // {
            //     Console.WriteLine(item);
            // }
            // Console.ReadKey();

            //var res2 = tem.ExcuteQuery("UserInfo.remove", new Hashtable
            //{
            //});
            //Console.WriteLine("删除完成");
            //Console.ReadKey();


            // var temp2 = middler.GetObjectByAppName<NiTemplate>("Ni", "memcached");

            // var res3 = temp2.ExcuteQuery("UserInfo.select1", new Hashtable
            // {
            //     {"key1","id"},
            //     {"key2","name"},
            //     {"key3","age"},
            // });
            // Console.WriteLine(res3.DataSet.Tables.Count);
            // Console.WriteLine(res3.DataSet.Tables[0].Rows.Count);
            // foreach (string item in res3.DataSet.Tables[0].Rows[0].ItemArray)
            // {
            //     Console.WriteLine(item);
            // }
        }
    }
}
