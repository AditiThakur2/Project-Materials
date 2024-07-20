using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk.Extensions;

namespace PlantPackageProject
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class PreOperationPlantCreate : PluginBase
    {
        public PreOperationPlantCreate(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(PreOperationPlantCreate))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            localPluginContext.Trace("inside execute dataverse plugin");
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;

            // TODO: Implement your custom business logic

            var plantEntity = context.InputParameters["Target"] as Entity;
            var installationRef = (EntityReference)plantEntity["contoso_contact"];
            Guid id = installationRef.Id;

            string fetchString = $"<fetch output-format='xml-platform' distinct='false' " + 
                "version='1.0' mapping='logical' " +
                "aggregate='true'>" +
                "<entity name='contoso_installationrequest'>" +
                "<attribute name='contoso_contact' alias='Count' aggregate='count' />" +
                "<filter type='and' >" +
                $"<condition attribute='contoso_contact' operator='eq' value='{id}'/>" +
                "<filter type = 'or' >" +
                "<condition attribute='contoso_installationstatus' operator='eq' value='929940000' />" +
                "<condition attribute='contoso_installationstatus' operator='eq' value='929940001' />" +
                "</filter>" +
                "</filter>" +
                "</entity>" +
                "</fetch>";

           

            var response = localPluginContext.InitiatingUserService.RetrieveMultiple(new FetchExpression(fetchString));
            int incompleteRequestCount = (int)((AliasedValue)response.Entities[0]["Count"]).Value;

            localPluginContext.Trace("Incomplete request count : " + incompleteRequestCount);


            if (incompleteRequestCount > 0)
            {
                throw new InvalidPluginExecutionException("Too many installation requests to process");
            }     

            // Check for the entity on which the plugin would be registered
            //if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            //{
            //    var entity = (Entity)context.InputParameters["Target"];

            //    // Check for entity name on which this plugin would be registered
            //    if (entity.LogicalName == "account")
            //    {

            //    }
            //}
        }
    }
}
