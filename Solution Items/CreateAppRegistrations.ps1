function Remove-AppRegistrationByName{
  param(
    $displayName
  )
  $queryResults = (Invoke-WebRequest -Method Get -Headers $headers -Uri "https://graph.microsoft.com/v1.0/applications?`$filter=displayName eq '$displayName'").Content | ConvertFrom-Json
  if($queryResults.value.Count -gt 0) {
      Write-Host "Found existing app registration for $displayName ($($queryResults.value[0].id)) - Deleting"
      Invoke-WebRequest -Method Delete -Headers $headers -Uri "https://graph.microsoft.com/beta/applications/$($queryResults.value[0].id)" -Body $body | Out-Null
  }
}


function New-AppRegistrationObject{
  param(
    $displayName    
  )
  New-Object PSObject -Property @{
    displayName = $displayName;
    api = New-Object PSObject -Property @{
        requestedAccessTokenVersion = 2;
        oauth2PermissionScopes = @(New-Object PSObject -Property @{
            id = [guid]::NewGuid().ToString();
            adminConsentDescription = "user_impersonation";
            adminConsentDisplayName = "user_impersonation";
            userConsentDescription= "user_impersonation";
            userConsentDisplayName = "user_impersonation";
            type = "Admin";
            value = "user_impersonation";
        });
    };
    web = New-Object PSObject -Property @{
      implicitGrantSettings = New-Object PSObject -Property @{
        enableIdTokenIssuance = $true;
        enableAccessTokenIssuance = $true;
      };
    };
    requiredResourceAccess = @();
  }
}
function Add-RequiredResourceAccess{
  param(
    $appRegistration,
    $resourceAppId,
    $resourceAccessId,
    $resourceAccessType
  )
  $appRegistration.requiredResourceAccess += @(New-Object PSObject -Property @{
    resourceAppId = $resourceAppId;
    resourceAccess = @(New-Object PSObject -Property @{
      id = $resourceAccessId;
      type = $resourceAccessType;
    })
  })
}



$apiAName = "api-a"
$apiBName = "api-b"
# Suppress progress output from Invoke-WebRequest
$global:progressPreference = 'silentlyContinue'

# Get an access token which will be needed to call the Graph API
# Requires an app registration with a Graph application permission "Application.ReadWrite.All" and a secret
$ClientID = '<CLIENT ID>'
$ClientSecret = '<CLIENT SECRET>'
$TenantID = '<TENANT ID>'
$Resource = 'https://graph.microsoft.com'

# Setup the OAuth 2.0 client credentials grant body
$authBody = @{
    client_id     = $ClientID
    client_secret = $ClientSecret
    resource      = $Resource
    grant_type    = "client_credentials"
  }

# Get the access token
$AuthResponse = Invoke-WebRequest -Uri "https://login.microsoftonline.com/$TenantID/oauth2/token" -ContentType "application/x-www-form-urlencoded" -Body $authBody -Method Post -ErrorAction Stop | ConvertFrom-Json

# Store the access token for later use
$accessToken = $AuthResponse.access_token

# Setup headers for future HTTP requests to MS Graph
$headers = @{
    "Content-Type"     = "application/json";
    "Authorization"    = "Bearer $accessToken";
    "Host"             = "graph.microsoft.com";
    "Origin"           = "https://developer.microsoft.com";
    "Referer"          = "https://developer.microsoft.com/";
    "Sdkversion"       = "GraphExplorer/4.0, graph-js/3.0.5 (featureUsage=6)";
    "Accept"           = "*/*";
}

# Remove app registrations if they already exist
Remove-AppRegistrationByName -displayName $apiAName
Remove-AppRegistrationByName -displayName $apiBName

# Create Api-A and add a secret
$jsonApiA = New-AppRegistrationObject -displayName $apiAName | ConvertTo-Json -Depth 100 -Compress
$newApiA = (Invoke-WebRequest -Method Post -Headers $headers -Uri "https://graph.microsoft.com/beta/applications/" -Body $jsonApiA).Content | ConvertFrom-Json
$apiASecret = (Invoke-WebRequest -Method Post -Headers $headers -Uri "https://graph.microsoft.com/beta/applications/$($newApiA.id)/addPassword" -Body "{'passwordCredential':{'displayName':'t-api-a','endDateTime':'2099-12-31T23:59:59.9999999Z'}}").Content | ConvertFrom-Json

# Create Api-B (No secret)
$jsonApiB = $jsonApiA = New-AppRegistrationObject -displayName $apiBName | ConvertTo-Json -Depth 100 -Compress
$newApiB = (Invoke-WebRequest -Method Post -Headers $headers -Uri "https://graph.microsoft.com/beta/applications/" -Body $jsonApiB).Content | ConvertFrom-Json

# Add Api-B as a required resource for Api-A
Add-RequiredResourceAccess -appRegistration $newApiA -resourceAppId $newApiB.appId -resourceAccessId $newApiB.api.oauth2PermissionScopes.id -resourceAccessType "Scope"
$jsonApiA = $newApiA | ConvertTo-Json -Depth 100 -Compress
$newApiA = (Invoke-WebRequest -Method Patch -Headers $headers -Uri "https://graph.microsoft.com/beta/applications/$($newApiA.id)" -Body $jsonApiA).Content | ConvertFrom-Json

# Output Summary
Write-Host "New app registrations created"
Write-Host "API A:"
Write-Host "  Client ID: $($newApiA.appId)"
Write-Host "  Client Secret: $($apiASecret.secretText)"
Write-Host "API B:"
Write-Host "  Client ID: $($newApiB.appId)"