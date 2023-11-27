using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.Testing
{
    public class FakeXrmEasyPipelineTestsBase
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;
        //protected readonly IOrganizationServiceAsync2 _servicetwo;

        public FakeXrmEasyPipelineTestsBase()
        {
            _context = MiddlewareBuilder
                            .New()

                            .AddCrud()
                            .AddFakeMessageExecutors()
                            .AddPipelineSimulation(new PipelineOptions() { UsePluginStepAudit = true })

                            .UsePipelineSimulation()
                            .UseCrud()
                            .UseMessages()
                            .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                            .Build();
            
            _service = _context.GetOrganizationService();
        }
    }
}
