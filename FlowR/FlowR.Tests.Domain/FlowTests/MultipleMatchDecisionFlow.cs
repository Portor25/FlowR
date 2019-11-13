﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR.StepLibrary.Activities;
using FlowR.StepLibrary.Decisions;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MultipleMatchDecisionFlowRequest : FlowActivityRequest<MultipleMatchDecisionFlowResponse>
    {
        public int IntValue { get; set; }
    }

    public class MultipleMatchDecisionFlowResponse : FlowResponse
    {
    }

    public class MultipleMatchDecisionFlow : FlowHandler<MultipleMatchDecisionFlowRequest, MultipleMatchDecisionFlowResponse>
    {
        public MultipleMatchDecisionFlow(IMediator mediator, IFlowLogger<MatchDecisionFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            #region Definitions

            var intValueDecision = new FlowDecisionDefinition<IntFlowValueDecision, int?>()
                .BindInput(rq => rq.SwitchValue, nameof(MatchDecisionFlowRequest.IntValue));

            #endregion

            return new FlowDefinition()
                .Check("Int_value", intValueDecision)
                .When(1).End()
                .When(1, 2, 3).End()
                .Else().End();
        }
    }
}
