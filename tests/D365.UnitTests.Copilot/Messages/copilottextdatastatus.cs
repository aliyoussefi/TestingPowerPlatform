#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("copilottextdatastatus")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class copilottextdatastatusRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
	
	public copilottextdatastatusRequest()
	{
		this.RequestName = "copilottextdatastatus";
	}
}

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("copilottextdatastatus")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class copilottextdatastatusResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
	
	public copilottextdatastatusResponse()
	{
	}
	
	public string response
	{
		get
		{
			if (this.Results.Contains("response"))
			{
				return ((string)(this.Results["response"]));
			}
			else
			{
				return default(string);
			}
		}
	}
}
#pragma warning restore CS1591
