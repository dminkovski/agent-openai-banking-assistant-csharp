param name string
param location string = resourceGroup().location
param tags object = {}

param identityName string
param applicationInsightsConnectionString string
param containerRegistryName string
param serviceName string = 'copilot'
param corsAcaUrl string
param exists bool

param environmentResourceId string

param imageName string = ''
param containerName string = 'main'

@description('The environment variables for the container')
param env array = []

resource apiIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
}
resource existingApp 'Microsoft.App/containerApps@2023-05-02-preview' existing = if (exists) {
  name: name
}

module containerApp 'br/public:avm/res/app/container-app:0.14.0' = {
  name: 'containerAppDeployment'
  params: {
    name: name
    location: location
    tags: union(tags, { 'azd-service-name': serviceName })
    managedIdentities: {
      value: {
        userAssignedResourceIds: [
          apiIdentity.id
        ]
      }
    }
    environmentResourceId: environmentResourceId
    containers: [
      {
        image: !empty(imageName) ? imageName : exists ? existingApp.properties.template.containers[0].image : ''
        name: 'container'
        resources: {
          cpuCoreCount: '1.0'
          memory: '2.0Gi'
          minReplicas: 1
          maxReplicas: 3
          daprEnabled: false
          daprAppId: containerName
          daprAppProtocol: 'http'
        }
        env: union(env, [
          {
            name: 'AZURE_CLIENT_ID'
            value: apiIdentity.properties.clientId
          }
          {
            name: 'DOTNET_AzureAd__ClientId'
            value: apiIdentity.properties.clientId
          }
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: applicationInsightsConnectionString
          }
          {
            name: 'API_ALLOW_ORIGINS'
            value: corsAcaUrl
          }
        ])
      }
    ]
    additionalPortMappings: [
      {
        exposedPort: 8080
        external: false
        targetPort: 8080
      }
    ]
    registries:[
      {
        server: '${containerRegistryName}.azurecr.io'
        identity: apiIdentity.id
      }
    ]
  }
}

output SERVICE_API_IDENTITY_PRINCIPAL_ID string = apiIdentity.properties.principalId
output SERVICE_API_NAME string = containerApp.outputs.name
output SERVICE_API_URI string = containerApp.outputs.fqdn
output SERVICE_API_IMAGE_NAME string = imageName
