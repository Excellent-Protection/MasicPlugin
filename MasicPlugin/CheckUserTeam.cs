using GetExcsettingWF;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasicPlugin
{
    class CheckUserTeam : CodeActivity
    {
        [RequiredArgument]
         [Input("teamId")]
        public InArgument<string> teamId { get; set; }
        [Input("UID")]
        public InArgument<string> UID { get; set; }
        [Input("BoolResult")]
        public OutArgument<bool> BoolResult { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext currentContext = context.GetExtension<IWorkflowContext>();
            IOrganizationService _service = GetCRMServiceClass.Service;

            var teamIdValue = teamId.Get<string>(context);
            var UIDValue = UID.Get<string>(context);

            try
            {
                QueryExpression query = new QueryExpression("team");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("teamid", ConditionOperator.Equal, teamIdValue));
                LinkEntity link = query.AddLink("teammembership", "teamid", "teamid");
                link.LinkCriteria.AddCondition(new ConditionExpression("systemuserid", ConditionOperator.Equal, UIDValue));
                var results = _service.RetrieveMultiple(query);

                if (results.Entities.Count > 0)
                {
                 this.BoolResult.Set(
                context,
                true);
                }
            
            }
            catch (Exception ex)
            {
              //  LogError.Error(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);

            }
            this.BoolResult.Set(
                  context,
                  false);
        }
    }
}
