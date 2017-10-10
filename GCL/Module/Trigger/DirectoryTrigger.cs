using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using GCL.IO;

namespace GCL.Module.Trigger {
    public class DirectoryTrigger : ATrigger {

        private int count = 0;
        private ParallelTriggerProxy ma = new ParallelTriggerProxy();
        private string filter;
        private DirectoryInfo dinfo;
        public DirectoryTrigger(string filter) {
            this.filter = IOTool.GetFileName(filter);
            dinfo = new DirectoryInfo(IOTool.GetPath(filter));

            this.ma.TriggerEvent += new GCL.Event.EventHandle(ma_TriggerEvent);
            this.ReSet();
        }

        void ma_TriggerEvent(object sender, GCL.Event.EventArg e) {
            this.CallTriggerEventSafely(this, e);
            if (sender is ATrigger) { this.ma.RemoveTrigger(sender as ATrigger); ((ATrigger)sender).Dispose(); }
            this.ReSet();
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            if (dinfo.GetFiles(filter).Length != count) {
                this.ReSet();
                return new GCL.Event.EventArg();
            } else
                this.ma.Taste(e);
            return null;
        }

        public override void ReSet() {
            count = dinfo.GetFiles(filter).Length;

            FileInfo[] infos = dinfo.GetFiles(filter);
            IDictionary idic = new Hashtable();
            foreach (FileInfo info in infos)
                idic[info.FullName] = info;
            lock (ma) {
                ATrigger[] triggers = this.ma.GetTriggers();
                //删除已经没有的文件
                foreach (ATrigger tri in triggers) {
                    if (idic[((FileTrigger)tri).File.FullName] != null) {
                        tri.ReSet();
                        idic.Remove(((FileTrigger)tri).File.FullName);
                    } else {
                        ma.RemoveTrigger(tri); tri.Dispose();
                    }
                }
                for (IDictionaryEnumerator ienum = idic.GetEnumerator(); ienum.MoveNext(); )
                    ma.AddTrigger(new FileTrigger(ienum.Value as FileInfo));
                idic.Clear();
            }
        }
    }
}