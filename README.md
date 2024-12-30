# DoDdns
Update an existing domain record managed by DigitalOcean. The DigitalOcean API reference is here: https://docs.digitalocean.com/reference/api/api-reference/#tag/Domain-Records.

## Configuration Requirements
The following keys need to be set in one of the [C# generic host default configuration providers](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder#host-builder-settings):

| Key                       | Description                                                                                                                           | Example               |
| ---                       | ---                                                                                                                                   | ---                   |
| TargetDomain              | The base domain                                                                                                                       | example.com           |
| TargetHostName            | The subdomain to update                                                                                                               | shop.example.com      |
| DigitalOceanAccessToken   | Your DigitalOcean access token with domain read and update access (Managed from https://cloud.digitalocean.com/account/api/tokens)    |                       |
| GetPublicIpUrl            | The URL of an HTTP GET endpoint that will resolve the public IP address of the machine the application is running from                | http://ipinfo.io/ip   |
| UpdatePeriodMinutes       | How often to check that the public IP address of the machine the application is running from matches the domain record                | 15                    |
