metadata description = 'Creates an Azure Container Apps environment.'
param name string
param location string = resourceGroup().location
param tags object = {}

@description('Name of the Application Insights resource')
param applicationInsightsName string = ''

@description('Specifies if Dapr is enabled')
param daprEnabled bool = false

param logAnalyticsWorkspaceId string
param applicationInsightsInstrumentationKey string

module managedEnvironment 'br/public:avm/res/app/managed-environment:0.10.0' = {
  name: 'managedEnvironmentDeployment'
  params: {
    // Required parameters
    logAnalyticsWorkspaceResourceId: logAnalyticsWorkspaceId
    logsDestination: 'log-analytics'
    name: name
    location: location
    tags: tags
    daprAIInstrumentationKey: daprEnabled && !empty(applicationInsightsName) ? applicationInsightsInstrumentationKey : ''
  }
}



output defaultDomain string = managedEnvironment.outputs.defaultDomain
output id string = managedEnvironment.outputs.resourceId
output name string = managedEnvironment.outputs.name
