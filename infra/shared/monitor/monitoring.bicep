metadata description = 'Creates an Application Insights instance and a Log Analytics workspace.'
param logAnalyticsName string
param applicationInsightsName string
param applicationInsightsDashboardName string = ''
param location string = resourceGroup().location
param tags object = {}

module logAnalyticsWorkspace 'br/public:avm/res/operational-insights/workspace:0.11.0' = {
  name: 'workspaceDeployment'
  params: {
    // Required parameters
    name: logAnalyticsName
    // Non-required parameters
    location: location
    tags: tags
    skuName: 'PerGB2018'
  }
}

module applicationInsights 'applicationinsights.bicep' = {
  name: 'applicationinsights'
  params: {
    name: applicationInsightsName
    location: location
    tags: tags
    dashboardName: applicationInsightsDashboardName
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.logAnalyticsWorkspaceId
  }
}

output applicationInsightsConnectionString string = applicationInsights.outputs.connectionString
output applicationInsightsId string = applicationInsights.outputs.id
output applicationInsightsInstrumentationKey string = applicationInsights.outputs.instrumentationKey
output applicationInsightsName string = applicationInsights.outputs.name
output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.outputs.logAnalyticsWorkspaceId
output logAnalyticsWorkspaceName string = logAnalyticsWorkspace.outputs.name

