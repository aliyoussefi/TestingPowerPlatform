using FakeXrmEasy.Abstractions.Plugins.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace D365.Testing.Helpers
{
    public class QuotePluginContextHelper
    {
        public Message Message { get; set; }
        public ProcessingStepStage Stage { get; set; }
        public ProcessingStepMode StepMode { get; set; }

        public QuotePluginContextHelper(Message message, ProcessingStepStage stage, ProcessingStepMode mode) {
            Message = message;
            Stage = stage;
            StepMode = mode;

        
        }
        public class Quote
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public Quote GenerateQuote()
        {
            return new Quote();
        }
    }
}
