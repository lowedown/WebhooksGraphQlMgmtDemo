# Sitecore Authoring and Management GraphQL API

This tool will get a token from your identity server in order to use it with your GraphQL Authoring and Management API.


## Setup

1. Create a Sitecore user who is in `Sitecore/Sitecore Client Users` role. The Admin user won't work.
2. Add a client to Identity Server configuration. (see below)
3. Set URL of this app as *AllowedCorsOriginsGroup1*
4. Set token lifetime as needed

`````
<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <Sitecore>
    <IdentityServer>
      <Clients>
        <SampleMvcClient>
          <ClientId>MvcClient</ClientId>
          <ClientName>Sample MVC client</ClientName>
          <AccessTokenType>0</AccessTokenType>
          <AllowOfflineAccess>true</AllowOfflineAccess>
          <AlwaysIncludeUserClaimsInIdToken>true</AlwaysIncludeUserClaimsInIdToken>
          <AccessTokenLifetimeInSeconds>31536000</AccessTokenLifetimeInSeconds>
          <IdentityTokenLifetimeInSeconds>31536000</IdentityTokenLifetimeInSeconds>
          <AllowAccessTokensViaBrowser>true</AllowAccessTokensViaBrowser>
          <RequireConsent>false</RequireConsent>
          <RequireClientSecret>false</RequireClientSecret>
          <AllowedGrantTypes>
            <AllowedGrantType1>client_credentials</AllowedGrantType1>
            <AllowedGrantType2>hybrid</AllowedGrantType2>
          </AllowedGrantTypes>
          <RedirectUris>
            <RedirectUri1>{AllowedCorsOrigin}/signin-oidc</RedirectUri1>
          </RedirectUris>
          <PostLogoutRedirectUris>
            <PostLogoutRedirectUri1>{AllowedCorsOrigin}/signout-callback-oidc</PostLogoutRedirectUri1>
          </PostLogoutRedirectUris>
          <AllowedCorsOrigins>
            <AllowedCorsOriginsGroup1>https://localhost:7100</AllowedCorsOriginsGroup1>
          </AllowedCorsOrigins>
          <AllowedScopes>
            <AllowedScope1>openid</AllowedScope1>
            <AllowedScope2>sitecore.profile</AllowedScope2>
            <AllowedScope3>sitecore.profile.api</AllowedScope3>
          </AllowedScopes>
          <UpdateAccessTokenClaimsOnRefresh>true</UpdateAccessTokenClaimsOnRefresh>
        </SampleMvcClient>
      </Clients>
    </IdentityServer>
  </Sitecore>
</Settings>
`````

- Configure URL of your Identity Server in `appsettings.json`
- Run this MVC App. It will redirect to your identity server and ask for your credentials. After that the access token is displayed.

Details see:

https://doc.sitecore.com/xp/en/developers/103/sitecore-experience-manager/use-bearer-tokens-in-client-applications.html#UUID-93cabfaa-779d-1fbc-517f-c91053fa647d_section-5c35e68c4329a-idm46779796924496
