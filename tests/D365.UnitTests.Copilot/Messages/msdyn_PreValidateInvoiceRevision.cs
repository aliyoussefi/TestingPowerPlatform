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
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("msdyn_PreValidateInvoiceRevision")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class msdyn_PreValidateInvoiceRevisionRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
	
	public Microsoft.Xrm.Sdk.EntityReference Target
	{
		get
		{
			if (this.Parameters.Contains("Target"))
			{
				return ((Microsoft.Xrm.Sdk.EntityReference)(this.Parameters["Target"]));
			}
			else
			{
				return default(Microsoft.Xrm.Sdk.EntityReference);
			}
		}
		set
		{
			this.Parameters["Target"] = value;
		}
	}
	
	public msdyn_PreValidateInvoiceRevisionRequest()
	{
		this.RequestName = "msdyn_PreValidateInvoiceRevision";
		this.Target = default(Microsoft.Xrm.Sdk.EntityReference);
	}
}

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("msdyn_PreValidateInvoiceRevision")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class msdyn_PreValidateInvoiceRevisionResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
	
	public msdyn_PreValidateInvoiceRevisionResponse()
	{
	}
}
#pragma warning restore CS1591
