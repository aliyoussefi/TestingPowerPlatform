using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dynamics365.UnitTest.Plugin.Framework.Plugins
{
    [DataContract(Name = "PluginExecutionContext", Namespace = "")]
    public class XrmFakedPluginExecutionContext : IPluginExecutionContext, IExecutionContext
    {
        //
        // Summary:
        //     Gets the GUID of the business unit that the user making the request, also known
        //     as the calling user, belongs to.
        [DataMember(Order = 1)]
        public Guid BusinessUnitId { get; set; }

        //
        // Summary:
        //     Gets the GUID for tracking plug-in or custom workflow activity execution.
        [DataMember(Order = 2)]
        public Guid CorrelationId { get; set; }

        //
        // Summary:
        //     Gets the current depth of execution in the call stack.
        [DataMember(Order = 3)]
        public int Depth { get; set; }

        //
        // Summary:
        //     Gets the GUID of the system user account under which the current pipeline is
        //     executing.
        [DataMember(Order = 4)]
        public Guid InitiatingUserId { get; set; }

        //
        // Summary:
        //     Gets the parameters of the request message that triggered the event that caused
        //     the plug-in to execute.
        [DataMember(Order = 5)]
        public ParameterCollection InputParameters { get; set; }

        //
        // Summary:
        //     Gets whether the plug-in is executing from the Microsoft Dynamics 365 for Microsoft
        //     Office Outlook with Offline Access client while it is offline.
        [DataMember(Order = 6)]
        public bool IsExecutingOffline { get; set; }

        //
        // Summary:
        //     Gets a value indicating if the plug-in is executing within the database transaction.
        [DataMember(Order = 7)]
        public bool IsInTransaction {
            get {
                return Stage == 20 || (Stage == 40 && Mode == 0);
            }
            set {
            }
        }

        //
        // Summary:
        //     Gets a value indicating if the plug-in is executing as a result of the Microsoft
        //     Dynamics 365 for Microsoft Office Outlook with Offline Access client transitioning
        //     from offline to online and synchronizing with the Microsoft Dynamics 365 server.
        [DataMember(Order = 8)]
        public bool IsOfflinePlayback { get; set; }

        //
        // Summary:
        //     Gets a value indicating if the plug-in is executing in the sandbox.
        [DataMember(Order = 9)]
        public int IsolationMode { get; set; }

        //
        // Summary:
        //     Gets the name of the Web service message that is being processed by the event
        //     execution pipeline.
        [DataMember(Order = 10)]
        public string MessageName { get; set; }

        //
        // Summary:
        //     Gets the mode of plug-in execution.
        [DataMember(Order = 11)]
        public int Mode { get; set; }

        //
        // Summary:
        //     Gets the date and time that the related System Job was created.
        [DataMember(Order = 12)]
        public DateTime OperationCreatedOn { get; set; }

        //
        // Summary:
        //     Gets the GUID of the related System Job.
        [DataMember(Order = 13)]
        public Guid OperationId { get; set; }

        //
        // Summary:
        //     Gets the GUID of the organization that the entity belongs to and the plug-in
        //     executes under.
        [DataMember(Order = 14)]
        public Guid OrganizationId { get; set; }

        //
        // Summary:
        //     Gets the unique name of the organization that the entity currently being processed
        //     belongs to and the plug-in executes under.
        [DataMember(Order = 15)]
        public string OrganizationName { get; set; }

        //
        // Summary:
        //     Gets the parameters of the response message after the core platform operation
        //     has completed.
        [DataMember(Order = 16)]
        public ParameterCollection OutputParameters { get; set; }

        //
        // Summary:
        //     Gets a reference to the related SdkMessageProcessingingStep or ServiceEndpoint
        [DataMember(Order = 17)]
        public EntityReference OwningExtension { get; set; }

        //
        // Summary:
        //     Gets the properties of the primary entity after the core platform operation has
        //     been completed.
        [DataMember(Order = 18)]
        public EntityImageCollection PostEntityImages { get; set; }

        //
        // Summary:
        //     Gets the properties of the primary entity before the core platform operation
        //     has begins.
        [DataMember(Order = 19)]
        public EntityImageCollection PreEntityImages { get; set; }

        //
        // Summary:
        //     Gets the GUID of the primary entity for which the pipeline is processing events.
        [DataMember(Order = 20)]
        public Guid PrimaryEntityId { get; set; }

        //
        // Summary:
        //     Gets the name of the primary entity for which the pipeline is processing events.
        [DataMember(Order = 21)]
        public string PrimaryEntityName { get; set; }

        //
        // Summary:
        //     Gets the GUID of the request being processed by the event execution pipeline.
        [DataMember(Order = 22)]
        public Guid? RequestId { get; set; }

        //
        // Summary:
        //     Gets the name of the secondary entity that has a relationship with the primary
        //     entity.
        [DataMember(Order = 23)]
        public string SecondaryEntityName { get; set; }

        //
        // Summary:
        //     Gets the custom properties that are shared between plug-ins.
        [DataMember(Order = 24)]
        public ParameterCollection SharedVariables { get; set; }

        //
        // Summary:
        //     Gets the GUID of the system user for whom the plug-in invokes web service methods
        //     on behalf of.
        [DataMember(Order = 25)]
        public Guid UserId { get; set; }

        //
        // Summary:
        //     Gets the execution context from the parent pipeline operation.
        [DataMember(Order = 26)]
        public IPluginExecutionContext ParentContext { get; set; }

        //
        // Summary:
        //     Gets the stage in the execution pipeline that a synchronous plug-in is registered
        //     for.
        [DataMember(Order = 27)]
        public int Stage { get; set; }

        //
        // Summary:
        //     Default constructor
        public XrmFakedPluginExecutionContext()
        {
            Depth = 1;
            IsExecutingOffline = false;
            MessageName = "Create";
            IsolationMode = 1;
        }

        //
        // Summary:
        //     Generates a fake plugin execution context from a previously, compressed, plugin
        //     execution, which can then be replayed
        //
        // Parameters:
        //   sSerialisedCompressedProfile:
        public static XrmFakedPluginExecutionContext FromSerialisedAndCompressedProfile(string sSerialisedCompressedProfile)
        {
            byte[] buffer = Convert.FromBase64String(sSerialisedCompressedProfile);
            using MemoryStream stream = new MemoryStream(buffer);
            using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: false);
            byte[] array = new byte[4096];
            using MemoryStream memoryStream = new MemoryStream();
            for (int num = deflateStream.Read(array, 0, array.Length); num > 0; num = deflateStream.Read(array, 0, array.Length))
            {
                memoryStream.Write(array, 0, num);
            }

            string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
            XDocument xDocument = XDocument.Parse(@string);
            string s = (from x in xDocument.Descendants().Elements()
                        where x.Name.LocalName.Equals("Context")
                        select x).FirstOrDefault()?.Value;
            XrmFakedPluginExecutionContext result = null;
            using (MemoryStream stream2 = new MemoryStream(Encoding.UTF8.GetBytes(s)))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(XrmFakedPluginExecutionContext));
                result = (XrmFakedPluginExecutionContext)dataContractSerializer.ReadObject((Stream)stream2);
            }

            return result;
        }
    }
}
