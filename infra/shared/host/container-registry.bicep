metadata description = 'Creates an Azure Container Registry.'
param name string
param location string = resourceGroup().location
param tags object = {}

@description('Indicates whether admin user is enabled')
param adminUserEnabled bool = false

@description('Indicates whether anonymous pull is enabled')
param anonymousPullEnabled bool = false

@description('Azure ad authentication as arm policy settings')
param azureADAuthenticationAsArmPolicy object = {
  status: 'enabled'
}

@description('Indicates whether data endpoint is enabled')
param dataEndpointEnabled bool = false

@description('Export policy settings')
param exportPolicy object = {
  status: 'enabled'
}

@description('Options for bypassing network rules')
param networkRuleBypassOptions string = 'AzureServices'

@description('Public network access setting')
param publicNetworkAccess string = 'Enabled'

@description('Quarantine policy settings')
param quarantinePolicy object = {
  status: 'disabled'
}

@description('Retention policy settings')
param retentionPolicy object = {
  days: 7
  status: 'disabled'
}

@description('Scope maps setting')
param scopeMaps array = []

@description('SKU settings')
param sku object = {
  name: 'Basic'
}

@description('Soft delete policy settings')
param softDeletePolicy object = {
  retentionDays: 7
  status: 'disabled'
}

@description('Trust policy settings')
param trustPolicy object = {
  type: 'Notary'
  status: 'disabled'
}

@description('Zone redundancy setting')
param zoneRedundancy string = 'Disabled'

@description('The log analytics workspace ID used for logging and monitoring')
param workspaceId string = ''

module registry 'br/public:avm/res/container-registry/registry:0.9.0' = {
  name: 'registryDeployment'
  params: {
    name: name
    location: location
    tags: tags
    acrSku: sku.name
    acrAdminUserEnabled: adminUserEnabled
    anonymousPullEnabled: anonymousPullEnabled
    dataEndpointEnabled: dataEndpointEnabled
    networkRuleBypassOptions: networkRuleBypassOptions
    quarantinePolicyStatus: quarantinePolicy.status
    trustPolicyStatus: trustPolicy.status
    retentionPolicyDays: retentionPolicy.days
    retentionPolicyStatus: retentionPolicy.status
    exportPolicyStatus: exportPolicy.status
    azureADAuthenticationAsArmPolicyStatus: azureADAuthenticationAsArmPolicy.status
    softDeletePolicyDays: softDeletePolicy.retentionDays
    softDeletePolicyStatus: softDeletePolicy.status
    publicNetworkAccess: publicNetworkAccess
    zoneRedundancy: zoneRedundancy
    scopeMaps: scopeMaps
    diagnosticSettings: [
      {
        logCategoriesAndGroups:[
          {
            category: 'ContainerRegistryRepositoryEvents'
            enabled: true
          }
          {
            category: 'ContainerRegistryLoginEvents'
            enabled: true
          }
        ]
        metricCategories: [
          {
            category: 'AllMetrics'
            enabled: true
          }
        ]
        name: 'registry-diagnostics'
        workspaceResourceId: workspaceId
      }
    ]
  }
}

output id string = registry.outputs.resourceId
output loginServer string = registry.outputs.loginServer
output name string = registry.outputs.name
