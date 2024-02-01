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



[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011//")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("ExtractKeyPhrases")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class ExtractKeyPhrasesRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
	
	public string text
	{
		get
		{
			if (this.Parameters.Contains("text"))
			{
				return ((string)(this.Parameters["text"]));
			}
			else
			{
				return default(string);
			}
		}
		set
		{
			this.Parameters["text"] = value;
		}
	}
	
	public string modelId
	{
		get
		{
			if (this.Parameters.Contains("modelId"))
			{
				return ((string)(this.Parameters["modelId"]));
			}
			else
			{
				return default(string);
			}
		}
		set
		{
			this.Parameters["modelId"] = value;
		}
	}
	
	public string language
	{
		get
		{
			if (this.Parameters.Contains("language"))
			{
				return ((string)(this.Parameters["language"]));
			}
			else
			{
				return default(string);
			}
		}
		set
		{
			this.Parameters["language"] = value;
		}
	}
	
	public ExtractKeyPhrasesRequest()
	{
		this.RequestName = "ExtractKeyPhrases";
		this.text = default(string);
	}
}

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011//")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("ExtractKeyPhrases")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
public partial class ExtractKeyPhrasesResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
	
	public ExtractKeyPhrasesResponse()
	{
	}
	
	public decimal countOfPhrases
	{
		get
		{
			if (this.Results.Contains("countOfPhrases"))
			{
				return ((decimal)(this.Results["countOfPhrases"]));
			}
			else
			{
				return default(decimal);
			}
		}
	}
	
	public Microsoft.Xrm.Sdk.EntityCollection phrases
	{
		get
		{
			if (this.Results.Contains("phrases"))
			{
				return ((Microsoft.Xrm.Sdk.EntityCollection)(this.Results["phrases"]));
			}
			else
			{
				return default(Microsoft.Xrm.Sdk.EntityCollection);
			}
		}
	}
}
#pragma warning restore CS1591
