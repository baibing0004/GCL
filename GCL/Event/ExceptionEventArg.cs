using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Event {
    public delegate void ExceptionEventHandle(object sender,ExceptionEventArg e);
    public class ExceptionEventArg:EventArg {

        public static void _ExceptionEventHandleDefault(object sender,ExceptionEventArg e) {
        }

        public static void CallExceptionEventSafely(ExceptionEventHandle handle,object sender,ExceptionEventArg e) {
            try {
                handle(sender,e);
            } catch {
            }
        }

        public static void CallEventSafely(ExceptionEventHandle handle,object sender,ExceptionEventArg e) {
            CallExceptionEventSafely(handle,sender,e);
        }
             
        /// <summary>
        /// 用于将Exception对象获得
        /// </summary>
        /// <param name="ex"></param>
        public ExceptionEventArg(Exception ex) :base(ex){            
        }

        public Exception getException() {
            return (Exception)this.GetPara(0);
        }
	
    }
}
