﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.296
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.296 版自动生成。
// 
#pragma warning disable 1591

namespace Test.com.chinahr.my27 {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SkrCVSoap", Namespace="http://tempuri.org/")]
    public partial class SkrCV : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetSkrCvCountOperationCompleted;
        
        private System.Threading.SendOrPostCallback ViewCVOperationCompleted;
        
        private System.Threading.SendOrPostCallback ReferJobOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetViewXmlPathStrLangOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetViewXmlPathOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetXmlPathForEhr5OperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDataXMLOperationCompleted;
        
        private System.Threading.SendOrPostCallback ReBuildXMLOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateViewXmlByResumeIdOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateViewXmlByResumeIdStrLangOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPropStrViewXmlPathOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SkrCV() {
            this.Url = global::Test.Properties.Settings.Default.Test_com_chinahr_my27_SkrCV;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetSkrCvCountCompletedEventHandler GetSkrCvCountCompleted;
        
        /// <remarks/>
        public event ViewCVCompletedEventHandler ViewCVCompleted;
        
        /// <remarks/>
        public event ReferJobCompletedEventHandler ReferJobCompleted;
        
        /// <remarks/>
        public event GetViewXmlPathStrLangCompletedEventHandler GetViewXmlPathStrLangCompleted;
        
        /// <remarks/>
        public event GetViewXmlPathCompletedEventHandler GetViewXmlPathCompleted;
        
        /// <remarks/>
        public event GetXmlPathForEhr5CompletedEventHandler GetXmlPathForEhr5Completed;
        
        /// <remarks/>
        public event GetDataXMLCompletedEventHandler GetDataXMLCompleted;
        
        /// <remarks/>
        public event ReBuildXMLCompletedEventHandler ReBuildXMLCompleted;
        
        /// <remarks/>
        public event UpdateViewXmlByResumeIdCompletedEventHandler UpdateViewXmlByResumeIdCompleted;
        
        /// <remarks/>
        public event UpdateViewXmlByResumeIdStrLangCompletedEventHandler UpdateViewXmlByResumeIdStrLangCompleted;
        
        /// <remarks/>
        public event GetPropStrViewXmlPathCompletedEventHandler GetPropStrViewXmlPathCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSkrCvCount", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int GetSkrCvCount(string UID, string Sep, long UserID) {
            object[] results = this.Invoke("GetSkrCvCount", new object[] {
                        UID,
                        Sep,
                        UserID});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void GetSkrCvCountAsync(string UID, string Sep, long UserID) {
            this.GetSkrCvCountAsync(UID, Sep, UserID, null);
        }
        
        /// <remarks/>
        public void GetSkrCvCountAsync(string UID, string Sep, long UserID, object userState) {
            if ((this.GetSkrCvCountOperationCompleted == null)) {
                this.GetSkrCvCountOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSkrCvCountOperationCompleted);
            }
            this.InvokeAsync("GetSkrCvCount", new object[] {
                        UID,
                        Sep,
                        UserID}, this.GetSkrCvCountOperationCompleted, userState);
        }
        
        private void OnGetSkrCvCountOperationCompleted(object arg) {
            if ((this.GetSkrCvCountCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSkrCvCountCompleted(this, new GetSkrCvCountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ViewCV", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int ViewCV(long CV_ID) {
            object[] results = this.Invoke("ViewCV", new object[] {
                        CV_ID});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void ViewCVAsync(long CV_ID) {
            this.ViewCVAsync(CV_ID, null);
        }
        
        /// <remarks/>
        public void ViewCVAsync(long CV_ID, object userState) {
            if ((this.ViewCVOperationCompleted == null)) {
                this.ViewCVOperationCompleted = new System.Threading.SendOrPostCallback(this.OnViewCVOperationCompleted);
            }
            this.InvokeAsync("ViewCV", new object[] {
                        CV_ID}, this.ViewCVOperationCompleted, userState);
        }
        
        private void OnViewCVOperationCompleted(object arg) {
            if ((this.ViewCVCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ViewCVCompleted(this, new ViewCVCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ReferJob", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int ReferJob(string CurrIndustry, string CurrJobCategory, short Experience, short Degree, string ResidenceState, decimal JobID, System.DateTime JobCreateDate, string EhrEmail, int type, decimal MemID, System.DateTime JobEndDate) {
            object[] results = this.Invoke("ReferJob", new object[] {
                        CurrIndustry,
                        CurrJobCategory,
                        Experience,
                        Degree,
                        ResidenceState,
                        JobID,
                        JobCreateDate,
                        EhrEmail,
                        type,
                        MemID,
                        JobEndDate});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void ReferJobAsync(string CurrIndustry, string CurrJobCategory, short Experience, short Degree, string ResidenceState, decimal JobID, System.DateTime JobCreateDate, string EhrEmail, int type, decimal MemID, System.DateTime JobEndDate) {
            this.ReferJobAsync(CurrIndustry, CurrJobCategory, Experience, Degree, ResidenceState, JobID, JobCreateDate, EhrEmail, type, MemID, JobEndDate, null);
        }
        
        /// <remarks/>
        public void ReferJobAsync(string CurrIndustry, string CurrJobCategory, short Experience, short Degree, string ResidenceState, decimal JobID, System.DateTime JobCreateDate, string EhrEmail, int type, decimal MemID, System.DateTime JobEndDate, object userState) {
            if ((this.ReferJobOperationCompleted == null)) {
                this.ReferJobOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReferJobOperationCompleted);
            }
            this.InvokeAsync("ReferJob", new object[] {
                        CurrIndustry,
                        CurrJobCategory,
                        Experience,
                        Degree,
                        ResidenceState,
                        JobID,
                        JobCreateDate,
                        EhrEmail,
                        type,
                        MemID,
                        JobEndDate}, this.ReferJobOperationCompleted, userState);
        }
        
        private void OnReferJobOperationCompleted(object arg) {
            if ((this.ReferJobCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ReferJobCompleted(this, new ReferJobCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetViewXmlPathStrLang", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetViewXmlPathStrLang(long resumeid, string lang) {
            object[] results = this.Invoke("GetViewXmlPathStrLang", new object[] {
                        resumeid,
                        lang});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetViewXmlPathStrLangAsync(long resumeid, string lang) {
            this.GetViewXmlPathStrLangAsync(resumeid, lang, null);
        }
        
        /// <remarks/>
        public void GetViewXmlPathStrLangAsync(long resumeid, string lang, object userState) {
            if ((this.GetViewXmlPathStrLangOperationCompleted == null)) {
                this.GetViewXmlPathStrLangOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetViewXmlPathStrLangOperationCompleted);
            }
            this.InvokeAsync("GetViewXmlPathStrLang", new object[] {
                        resumeid,
                        lang}, this.GetViewXmlPathStrLangOperationCompleted, userState);
        }
        
        private void OnGetViewXmlPathStrLangOperationCompleted(object arg) {
            if ((this.GetViewXmlPathStrLangCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetViewXmlPathStrLangCompleted(this, new GetViewXmlPathStrLangCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetViewXmlPath", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetViewXmlPath(long resumeid, int lang) {
            object[] results = this.Invoke("GetViewXmlPath", new object[] {
                        resumeid,
                        lang});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetViewXmlPathAsync(long resumeid, int lang) {
            this.GetViewXmlPathAsync(resumeid, lang, null);
        }
        
        /// <remarks/>
        public void GetViewXmlPathAsync(long resumeid, int lang, object userState) {
            if ((this.GetViewXmlPathOperationCompleted == null)) {
                this.GetViewXmlPathOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetViewXmlPathOperationCompleted);
            }
            this.InvokeAsync("GetViewXmlPath", new object[] {
                        resumeid,
                        lang}, this.GetViewXmlPathOperationCompleted, userState);
        }
        
        private void OnGetViewXmlPathOperationCompleted(object arg) {
            if ((this.GetViewXmlPathCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetViewXmlPathCompleted(this, new GetViewXmlPathCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetXmlPathForEhr5", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetXmlPathForEhr5(long cvid, byte ehrLang) {
            object[] results = this.Invoke("GetXmlPathForEhr5", new object[] {
                        cvid,
                        ehrLang});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetXmlPathForEhr5Async(long cvid, byte ehrLang) {
            this.GetXmlPathForEhr5Async(cvid, ehrLang, null);
        }
        
        /// <remarks/>
        public void GetXmlPathForEhr5Async(long cvid, byte ehrLang, object userState) {
            if ((this.GetXmlPathForEhr5OperationCompleted == null)) {
                this.GetXmlPathForEhr5OperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetXmlPathForEhr5OperationCompleted);
            }
            this.InvokeAsync("GetXmlPathForEhr5", new object[] {
                        cvid,
                        ehrLang}, this.GetXmlPathForEhr5OperationCompleted, userState);
        }
        
        private void OnGetXmlPathForEhr5OperationCompleted(object arg) {
            if ((this.GetXmlPathForEhr5Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetXmlPathForEhr5Completed(this, new GetXmlPathForEhr5CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDataXML", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Xml.XmlNode GetDataXML(long resumeId, byte lang, bool isText) {
            object[] results = this.Invoke("GetDataXML", new object[] {
                        resumeId,
                        lang,
                        isText});
            return ((System.Xml.XmlNode)(results[0]));
        }
        
        /// <remarks/>
        public void GetDataXMLAsync(long resumeId, byte lang, bool isText) {
            this.GetDataXMLAsync(resumeId, lang, isText, null);
        }
        
        /// <remarks/>
        public void GetDataXMLAsync(long resumeId, byte lang, bool isText, object userState) {
            if ((this.GetDataXMLOperationCompleted == null)) {
                this.GetDataXMLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDataXMLOperationCompleted);
            }
            this.InvokeAsync("GetDataXML", new object[] {
                        resumeId,
                        lang,
                        isText}, this.GetDataXMLOperationCompleted, userState);
        }
        
        private void OnGetDataXMLOperationCompleted(object arg) {
            if ((this.GetDataXMLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDataXMLCompleted(this, new GetDataXMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ReBuildXML", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool ReBuildXML(long resumeId, byte lang) {
            object[] results = this.Invoke("ReBuildXML", new object[] {
                        resumeId,
                        lang});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void ReBuildXMLAsync(long resumeId, byte lang) {
            this.ReBuildXMLAsync(resumeId, lang, null);
        }
        
        /// <remarks/>
        public void ReBuildXMLAsync(long resumeId, byte lang, object userState) {
            if ((this.ReBuildXMLOperationCompleted == null)) {
                this.ReBuildXMLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReBuildXMLOperationCompleted);
            }
            this.InvokeAsync("ReBuildXML", new object[] {
                        resumeId,
                        lang}, this.ReBuildXMLOperationCompleted, userState);
        }
        
        private void OnReBuildXMLOperationCompleted(object arg) {
            if ((this.ReBuildXMLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ReBuildXMLCompleted(this, new ReBuildXMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateViewXmlByResumeId", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] UpdateViewXmlByResumeId(long[] resumeids, int[] langs) {
            object[] results = this.Invoke("UpdateViewXmlByResumeId", new object[] {
                        resumeids,
                        langs});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void UpdateViewXmlByResumeIdAsync(long[] resumeids, int[] langs) {
            this.UpdateViewXmlByResumeIdAsync(resumeids, langs, null);
        }
        
        /// <remarks/>
        public void UpdateViewXmlByResumeIdAsync(long[] resumeids, int[] langs, object userState) {
            if ((this.UpdateViewXmlByResumeIdOperationCompleted == null)) {
                this.UpdateViewXmlByResumeIdOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateViewXmlByResumeIdOperationCompleted);
            }
            this.InvokeAsync("UpdateViewXmlByResumeId", new object[] {
                        resumeids,
                        langs}, this.UpdateViewXmlByResumeIdOperationCompleted, userState);
        }
        
        private void OnUpdateViewXmlByResumeIdOperationCompleted(object arg) {
            if ((this.UpdateViewXmlByResumeIdCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateViewXmlByResumeIdCompleted(this, new UpdateViewXmlByResumeIdCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateViewXmlByResumeIdStrLang", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] UpdateViewXmlByResumeIdStrLang(long[] resumeids, string[] langs) {
            object[] results = this.Invoke("UpdateViewXmlByResumeIdStrLang", new object[] {
                        resumeids,
                        langs});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void UpdateViewXmlByResumeIdStrLangAsync(long[] resumeids, string[] langs) {
            this.UpdateViewXmlByResumeIdStrLangAsync(resumeids, langs, null);
        }
        
        /// <remarks/>
        public void UpdateViewXmlByResumeIdStrLangAsync(long[] resumeids, string[] langs, object userState) {
            if ((this.UpdateViewXmlByResumeIdStrLangOperationCompleted == null)) {
                this.UpdateViewXmlByResumeIdStrLangOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateViewXmlByResumeIdStrLangOperationCompleted);
            }
            this.InvokeAsync("UpdateViewXmlByResumeIdStrLang", new object[] {
                        resumeids,
                        langs}, this.UpdateViewXmlByResumeIdStrLangOperationCompleted, userState);
        }
        
        private void OnUpdateViewXmlByResumeIdStrLangOperationCompleted(object arg) {
            if ((this.UpdateViewXmlByResumeIdStrLangCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateViewXmlByResumeIdStrLangCompleted(this, new UpdateViewXmlByResumeIdStrLangCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetPropStrViewXmlPath", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetPropStrViewXmlPath(long[] resumeids, string[] langs) {
            object[] results = this.Invoke("GetPropStrViewXmlPath", new object[] {
                        resumeids,
                        langs});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetPropStrViewXmlPathAsync(long[] resumeids, string[] langs) {
            this.GetPropStrViewXmlPathAsync(resumeids, langs, null);
        }
        
        /// <remarks/>
        public void GetPropStrViewXmlPathAsync(long[] resumeids, string[] langs, object userState) {
            if ((this.GetPropStrViewXmlPathOperationCompleted == null)) {
                this.GetPropStrViewXmlPathOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPropStrViewXmlPathOperationCompleted);
            }
            this.InvokeAsync("GetPropStrViewXmlPath", new object[] {
                        resumeids,
                        langs}, this.GetPropStrViewXmlPathOperationCompleted, userState);
        }
        
        private void OnGetPropStrViewXmlPathOperationCompleted(object arg) {
            if ((this.GetPropStrViewXmlPathCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPropStrViewXmlPathCompleted(this, new GetPropStrViewXmlPathCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetSkrCvCountCompletedEventHandler(object sender, GetSkrCvCountCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSkrCvCountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSkrCvCountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void ViewCVCompletedEventHandler(object sender, ViewCVCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ViewCVCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ViewCVCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void ReferJobCompletedEventHandler(object sender, ReferJobCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ReferJobCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ReferJobCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetViewXmlPathStrLangCompletedEventHandler(object sender, GetViewXmlPathStrLangCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetViewXmlPathStrLangCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetViewXmlPathStrLangCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetViewXmlPathCompletedEventHandler(object sender, GetViewXmlPathCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetViewXmlPathCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetViewXmlPathCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetXmlPathForEhr5CompletedEventHandler(object sender, GetXmlPathForEhr5CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetXmlPathForEhr5CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetXmlPathForEhr5CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetDataXMLCompletedEventHandler(object sender, GetDataXMLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDataXMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDataXMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Xml.XmlNode Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Xml.XmlNode)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void ReBuildXMLCompletedEventHandler(object sender, ReBuildXMLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ReBuildXMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ReBuildXMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void UpdateViewXmlByResumeIdCompletedEventHandler(object sender, UpdateViewXmlByResumeIdCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UpdateViewXmlByResumeIdCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UpdateViewXmlByResumeIdCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void UpdateViewXmlByResumeIdStrLangCompletedEventHandler(object sender, UpdateViewXmlByResumeIdStrLangCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UpdateViewXmlByResumeIdStrLangCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UpdateViewXmlByResumeIdStrLangCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetPropStrViewXmlPathCompletedEventHandler(object sender, GetPropStrViewXmlPathCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPropStrViewXmlPathCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPropStrViewXmlPathCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591