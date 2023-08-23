/*
 Task 4: Update Related Records Workflow
Create a workflow that triggers on the creation of a new Opportunity.
The workflow should automatically update the related Account's "Last Opportunity Close Date" field with
the estimatedclose date of the new Opportunity created.
 */
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace CustomWorkflow
{
    public class UpdateAccountLastOpportunityCloseDate : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            try
            {
                // Get the Opportunity record being created
                Entity opportunity = service.Retrieve(workflowContext.PrimaryEntityName, workflowContext.PrimaryEntityId, new ColumnSet( "estimatedclosedate", "parentaccountid"));

                // Check if the Opportunity has a related Account (Customer)
                if (opportunity.Contains("parentaccountid") && opportunity["parentaccountid"] is EntityReference accountReference)
                {
                    // Update the Last Opportunity Close Date on the related Account
                    Entity account = new Entity("account");
                    account.Id = accountReference.Id;
                    account["new_lastopportunityclosedate"] = opportunity.GetAttributeValue<DateTime>("estimatedclosedate");
                    service.Update(account);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and log error messages
                ITracingService tracingService = context.GetExtension<ITracingService>();
                tracingService.Trace("An error occurred: " + ex.Message);
            }
        }
    }
}


