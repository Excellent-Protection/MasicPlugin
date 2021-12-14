using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace MasicPlugin
{
    public class CheckInoviceNumberExistingBeforeEdit :IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            string entityName = context.PrimaryEntityName;
            Guid recordId = context.PrimaryEntityId;

            switch (entityName)
            {

                case "new_invoice":
                var Invoice = service.Retrieve(entityName, recordId, new ColumnSet("new_name"));
                 switch (context.MessageName)
                    {
                        case "Update":
                            if (Invoice.Attributes.ContainsKey("new_name"))
                            {
                                throw new InvalidPluginExecutionException("You can not modify Invoice Fields Since Invoice Number Contain Data");
                            }
                            break;
                        case "Delete":
                            break;
                    }
                    break;
                case "new_invoicedetails":
                    var InvoiceDetail = service.Retrieve(entityName, recordId, new ColumnSet("new_invoiceheaderid"));

                    switch (context.MessageName)
                    {
                        case "Update":
                        case "Create":
                            if (InvoiceDetail.Attributes.ContainsKey("new_invoiceheaderid"))
                            {
                                var InvoiceRefrance = InvoiceDetail.Attributes["new_invoiceheaderid"] as EntityReference;
                                var invoice = service.Retrieve("new_invoice", InvoiceRefrance.Id, new ColumnSet("new_name"));
                                if (invoice.Attributes.ContainsKey("new_name"))
                                {
                                    throw new InvalidPluginExecutionException("You can not Choose This Invoice Since It's Invoice Number Contain Data");
                                }
                            }
                            break;
                        case "Delete":
                            if (InvoiceDetail.Attributes.ContainsKey("new_invoiceheaderid"))
                            {
                                var e = InvoiceDetail.Attributes["new_invoiceheaderid"] as EntityReference;
                                var invoice = service.Retrieve("new_invoice", e.Id, new ColumnSet("new_name"));
                                if (invoice.Attributes.ContainsKey("new_name"))
                                {
                                    throw new InvalidPluginExecutionException("You can not Delete This InvoiceDetail Since It's Invoice Number Contain Data");
                                }
                            }
                            break;
                    }
                    break;
            }
   
        }
        //protected override void Execute(CodeActivityContext context)
        //{
        //    try
        //    {
        //        var workflowContext = context.GetExtension<IWorkflowContext>();
        //        var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
        //        var orgService = serviceFactory.CreateOrganizationService(workflowContext.UserId);

        //        //var currentFieldName = FieldName.Get<string>(context);
        //        string entityName = workflowContext.PrimaryEntityName;
        //        Guid recordId = workflowContext.PrimaryEntityId;


        //        QueryExpression e1 = new QueryExpression(entityName);
        //        e1.Criteria.AddCondition(entityName + "Id", ConditionOperator.Equal, recordId);
        //     //   e1.ColumnSet = new ColumnSet(currentFieldName);
        //        var res = orgService.Retrieve(entityName, recordId, new ColumnSet(true));
        //        orgService.Update(res);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin." + e + "test" + e.Message);

        //    }
        //    // e[FieldName.ToString()]= e.Contains(FieldName.ToString())?new OptionSetValue(currentvalue):null;
        //    //this.TargetValue.Set(context, new OptionSetValue(value)); 



        //}

    }
   
}
