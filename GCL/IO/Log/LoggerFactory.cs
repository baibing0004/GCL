using System;
using System.Collections.Generic;
using System.Text;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    public class LoggerFactory {

        /// <summary>
        /// 用于设置Logger的listeners
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="listeners"></param>
        protected static Logger GetLogger(Logger logger, params ILogListener[] listeners) {
            foreach (ILogListener lis in listeners)
                logger.AddListener(lis);
            return logger;
        }

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNeedDiscript"></param>
        /// <param name="isNeedSeparator"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static Logger GetLogger(LogType type, bool isNeedDiscript, bool isNeedSeparator, params ILogListener[] listeners) {
            return GetLogger(new Logger(new LogRecord(type, true, isNeedDiscript, isNeedSeparator)), listeners);
        }

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static Logger GetLogger(LogType type, params ILogListener[] listeners) {
            return GetLogger(new Logger(new LogRecord(type)), listeners);
        }



        /// <summary>
        /// 获取Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Listener GetListener(ATrigger[] triggers, ILogResource res) {
            return new Listener(triggers, new LogRecord(), res);
        }
        /// <summary>
        /// 获取Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Listener GetListener(LogType logType, ILogResource res) {
            return GetListener(new ATrigger[] { new LogTypeTrigger(logType) }, res);
        }

        /// <summary>
        /// 获取Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Listener GetListener(LogType logType, LogType maxLogType, ILogResource res) {
            return GetListener(new ATrigger[] { new LogTypeTrigger(logType, maxLogType) }, res);
        }

        /// <summary>
        /// 获取Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Listener GetListener(LogType logType, string source, ILogResource res) {
            return GetListener(new ATrigger[] { new LogTypeTrigger(logType), new SourceTrigger(source) }, res);
        }

        /// <summary>
        /// 获取Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <param name="source"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Listener GetListener(LogType logType, LogType maxLogType, string source, ILogResource res) {
            return GetListener(new ATrigger[] { new LogTypeTrigger(logType, maxLogType), new SourceTrigger(source) }, res);
        }

        /// <summary>
        /// 获取Windows日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Listener GetWindowEventListener(LogType logType, string source, string eventName) {
            return GetListener(logType, source, new WindowEventLogResource(eventName));
        }

        /// <summary>
        /// 获取Windows日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <param name="source"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static Listener GetWindowEventListener(LogType logType, LogType maxLogType, string source, string eventName) {
            return GetListener(logType, maxLogType, source, new WindowEventLogResource(eventName));
        }

        /// <summary>
        /// 获取Windows日志Listener,这里Source同时是event名
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Listener GetWindowEventListener(LogType logType, string source) {
            return GetWindowEventListener(logType, source, source);
        }

        /// <summary>
        /// 获取Windows日志Listener,这里Source同时是event名
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Listener GetWindowEventListener(LogType logType, LogType maxLogType, string source) {
            return GetWindowEventListener(logType, maxLogType, source, source);
        }

        /// <summary>
        /// 获取桌面日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static Listener GetConsoleListener(LogType logType) {
            return GetListener(logType, new ConsoleLogResource());
        }

        /// <summary>
        /// 获取桌面日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <returns></returns>
        public static Listener GetConsoleListener(LogType logType, LogType maxLogType) {
            return GetListener(logType, maxLogType, new ConsoleLogResource());
        }

        /// <summary>
        /// 获取桌面日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Listener GetConsoleListener(LogType logType, string source) {
            return GetListener(logType, source, new ConsoleLogResource());
        }

        /// <summary>
        /// 获取桌面日志Listener
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="maxLogType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Listener GetConsoleListener(LogType logType, LogType maxLogType, string source) {
            return GetListener(logType, maxLogType, source, new ConsoleLogResource());
        }

        ///<summary>
        ///获取Logger
        ///</summary>
        ///<param name="type"></param>
        ///<param name="isNeedDiscript"></param>
        ///<param name="isNeedSeparator"></param>
        ///<param name="eventName"></param>
        ///<returns></returns>
        public static Logger GetWindowEventLogger(LogType type, bool isNeedDiscript, bool isNeedSeparator, string eventName) {
            return GetLogger(type, isNeedDiscript, isNeedSeparator, GetListener(type, new WindowEventLogResource(eventName)));
        }

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static Logger GetWindowEventLogger(LogType type, string eventName) {
            return GetLogger(type, GetListener(type, new WindowEventLogResource(eventName)));
        }

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNeedDiscript"></param>
        /// <param name="isNeedSeparator"></param>
        /// <param name="pathFormat">c:\Apply{0}.txt</param>
        /// <param name="timeFormat">{0:yyyy-MM-dd}</param>
        /// <param name="cronExpression">0 0 0 * * ?</param>
        /// <param name="encoding"></param>
        /// <param name="limitLength"></param>
        /// <returns></returns>
        public static Logger GetTextFileLogger(LogType type, bool isNeedDiscript, bool isNeedSeparator, string pathFormat, Encoding encoding, long buffer, long limitLength, string cronExpression, string timeFormat) {
            return GetLogger(type, isNeedDiscript, isNeedSeparator, GetListener(type, new TextFileLogResource(pathFormat, encoding, buffer, new LengthAndCronNameTrigger(limitLength, cronExpression, timeFormat))));
        }

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNeedDiscript"></param>
        /// <param name="isNeedSeparator"></param>
        /// <param name="pathFormat">c:\Apply{0}.txt</param>
        /// <param name="timeFormat">{0:yyyy-MM-dd}</param>
        /// <param name="cronExpression">0 0 0 * * ?</param>
        /// <param name="encoding"></param>
        /// <param name="limitLength"></param>
        /// <returns></returns>
        public static Logger GetTextFileLogger(LogType type, bool isNeedDiscript, string pathFormat, string cronExpression, string timeFormat) {
            return GetLogger(type, isNeedDiscript, false, GetListener(type, new TextFileLogResource(pathFormat, new LengthAndCronNameTrigger(cronExpression, timeFormat))));
        }

        /// <summary>
        /// 获取每日日志文件Logger 默认200M为一个文件大小
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNeedDiscript"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static Logger GetHourTextFileLogger(LogType type, bool isNeedDiscript, string pathFormat, Encoding encoding) {
            return GetLogger(type, isNeedDiscript, false, GetListener(type, new TextFileLogResource(pathFormat, encoding, new LengthAndCronNameTrigger("0 0 * * * ?", "{0:yyyyMMddHH}"))));
        }



        /// <summary>
        /// 获取每日日志文件Logger 默认200M为一个文件大小
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNeedDiscript"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static Logger GetDayTextFileLogger(LogType type, bool isNeedDiscript, string pathFormat, Encoding encoding) {
            return GetLogger(type, isNeedDiscript, false, GetListener(type, new TextFileLogResource(pathFormat, encoding, new LengthAndCronNameTrigger("0 0 0 * * ?", "{0:yyyyMMdd}"))));
        }


        /// <summary>
        /// 获取Text文本文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="pathFormat"></param>
        /// <param name="encoding"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static ILogResource GetTextFileResource(string pathFormat, Encoding encoding, string cronExpression, string timeFormat) {
            return new TextFileLogResource(pathFormat, encoding, new LengthAndCronNameTrigger(cronExpression, timeFormat));
        }

        /// <summary>
        /// 获取Text文本文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="pathFormat"></param>
        /// <param name="encoding"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static ILogResource GetTextFileResource(string pathFormat, string encoding, string cronExpression, string timeFormat) {
            return GetTextFileResource(pathFormat, Encoding.GetEncoding(encoding), cronExpression, timeFormat);
        }


        /// <summary>
        /// 获取Text文本文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="pathFormat"></param>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="limitLength"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static ILogResource GetTextFileResource(string pathFormat, Encoding encoding, long buffer, long limitLength, string cronExpression, string timeFormat) {
            return new TextFileLogResource(pathFormat, encoding, buffer, new LengthAndCronNameTrigger(limitLength, cronExpression, timeFormat));
        }

        /// <summary>
        /// 获取Text文本文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="pathFormat"></param>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="limitLength"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static ILogResource GetTextFileResource(string pathFormat, string encoding, long buffer, long limitLength, string cronExpression, string timeFormat) {
            return new TextFileLogResource(pathFormat, Encoding.GetEncoding(encoding), buffer, new LengthAndCronNameTrigger(limitLength, cronExpression, timeFormat));
        }

        /// <summary>
        /// 获取每日日志文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static ILogResource GetDayTextFileResource(string pathFormat, Encoding encoding) {
            return GetTextFileResource(pathFormat, encoding, "0 0 0 * * ?", "{0:yyyyMMdd}");
        }

        /// <summary>
        /// 获取每日日志文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static ILogResource GetDayTextFileResource(string pathFormat, string encoding) {
            return GetTextFileResource(pathFormat, encoding, "0 0 0 * * ?", "{0:yyyyMMdd}");
        }

        /// <summary>
        /// 获取每日日志文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static ILogResource GetHourTextFileResource(string pathFormat, Encoding encoding) {
            return GetTextFileResource(pathFormat, encoding, "0 0 * * * ?", "{0:yyyyMMddHH}");
        }

        /// <summary>
        /// 获取每日日志文件源 默认200M为一个文件大小
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static ILogResource GetHourTextFileResource(string pathFormat, string encoding) {
            return GetTextFileResource(pathFormat, encoding, "0 0 * * * ?", "{0:yyyyMMddHH}");
        }

    }
}
