﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



using System.IO;
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://DeadAlbatross.Client", ConfigurationName="ClientImplementation")]
public interface ClientImplementation
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://DeadAlbatross.Client/ClientImplementation/Download", ReplyAction="http://DeadAlbatross.Client/ClientImplementation/DownloadResponse")]
    Stream Download(string hash);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface ClientImplementationChannel : ClientImplementation, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class ClientImplementationClient : System.ServiceModel.ClientBase<ClientImplementation>, ClientImplementation
{
    
    public ClientImplementationClient()
    {
    }
    
    public ClientImplementationClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public ClientImplementationClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ClientImplementationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ClientImplementationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public Stream Download(string hash)
    {
        return base.Channel.Download(hash);
    }
}
