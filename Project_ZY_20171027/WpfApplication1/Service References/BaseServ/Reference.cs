﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApplication1.BaseServ {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://yixiubox.com:5000/", ConfigurationName="BaseServ.BaseServiceSoap")]
    public interface BaseServiceSoap {
        
        // CODEGEN: 命名空间 http://yixiubox.com:5000/ 的元素名称 HelloWorldResult 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://yixiubox.com:5000/HelloWorld", ReplyAction="*")]
        WpfApplication1.BaseServ.HelloWorldResponse HelloWorld(WpfApplication1.BaseServ.HelloWorldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://yixiubox.com:5000/HelloWorld", ReplyAction="*")]
        System.Threading.Tasks.Task<WpfApplication1.BaseServ.HelloWorldResponse> HelloWorldAsync(WpfApplication1.BaseServ.HelloWorldRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorld", Namespace="http://yixiubox.com:5000/", Order=0)]
        public WpfApplication1.BaseServ.HelloWorldRequestBody Body;
        
        public HelloWorldRequest() {
        }
        
        public HelloWorldRequest(WpfApplication1.BaseServ.HelloWorldRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class HelloWorldRequestBody {
        
        public HelloWorldRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorldResponse", Namespace="http://yixiubox.com:5000/", Order=0)]
        public WpfApplication1.BaseServ.HelloWorldResponseBody Body;
        
        public HelloWorldResponse() {
        }
        
        public HelloWorldResponse(WpfApplication1.BaseServ.HelloWorldResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://yixiubox.com:5000/")]
    public partial class HelloWorldResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string HelloWorldResult;
        
        public HelloWorldResponseBody() {
        }
        
        public HelloWorldResponseBody(string HelloWorldResult) {
            this.HelloWorldResult = HelloWorldResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BaseServiceSoapChannel : WpfApplication1.BaseServ.BaseServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BaseServiceSoapClient : System.ServiceModel.ClientBase<WpfApplication1.BaseServ.BaseServiceSoap>, WpfApplication1.BaseServ.BaseServiceSoap {
        
        public BaseServiceSoapClient() {
        }
        
        public BaseServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BaseServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BaseServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BaseServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WpfApplication1.BaseServ.HelloWorldResponse WpfApplication1.BaseServ.BaseServiceSoap.HelloWorld(WpfApplication1.BaseServ.HelloWorldRequest request) {
            return base.Channel.HelloWorld(request);
        }
        
        public string HelloWorld() {
            WpfApplication1.BaseServ.HelloWorldRequest inValue = new WpfApplication1.BaseServ.HelloWorldRequest();
            inValue.Body = new WpfApplication1.BaseServ.HelloWorldRequestBody();
            WpfApplication1.BaseServ.HelloWorldResponse retVal = ((WpfApplication1.BaseServ.BaseServiceSoap)(this)).HelloWorld(inValue);
            return retVal.Body.HelloWorldResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WpfApplication1.BaseServ.HelloWorldResponse> WpfApplication1.BaseServ.BaseServiceSoap.HelloWorldAsync(WpfApplication1.BaseServ.HelloWorldRequest request) {
            return base.Channel.HelloWorldAsync(request);
        }
        
        public System.Threading.Tasks.Task<WpfApplication1.BaseServ.HelloWorldResponse> HelloWorldAsync() {
            WpfApplication1.BaseServ.HelloWorldRequest inValue = new WpfApplication1.BaseServ.HelloWorldRequest();
            inValue.Body = new WpfApplication1.BaseServ.HelloWorldRequestBody();
            return ((WpfApplication1.BaseServ.BaseServiceSoap)(this)).HelloWorldAsync(inValue);
        }
    }
}