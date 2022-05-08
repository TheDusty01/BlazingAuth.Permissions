# BlazingAuth.Permissions
This library provides easy to use, claims and policy based permission authorization for ASP.NET Core and Blazor.

You can also use this library with ASP.NET Core and without Blazor and vice versa.

## Setup
Download the latest releases from NuGet:
* **Shared:** [BlazingAuth.Permissions](https://www.nuget.org/packages/BlazingAuth.Permissions)
* **Blazor:** [BlazingAuth.Permissions.Client](https://www.nuget.org/packages/BlazingAuth.Permissions.Client)
* **ASP.NET Core:** [BlazingAuth.Permissions.Server](https://www.nuget.org/packages/BlazingAuth.Permissions.Server)


## Usage on the server
### Using the attribute
To secure an action with permissions just add the ``AuthorizePermission`` attribute above the method like this:
```cs
[AuthorizePermission("MyPermission")]
[HttpGet("GetSecretInformation")]
public IActionResult GetSecretInformation()
{
    // Code omitted for brevity
}
```

You can also specify multiple required permissions. \
Here the user needs to have ``PermissionA`` **and** ``PermissionB``.
```cs
[AuthorizePermission("MyPermission")]
[AuthorizePermission("MyOtherNeededPermission")]
[HttpGet("GetSecretInformation")]
public IActionResult GetSecretInformation()
{
    // Code omitted for brevity
}
```

If you only want the user to have one of some permissions, you can just specify multiple permissions with a single attribute. \
In this example the user needs to have ``PermissionA`` **or** ``PermissionB``.
```cs
[AuthorizePermission("PermissionA", "PermissionB")]
[HttpGet("GetSecretInformation")]
public IActionResult GetSecretInformation()
{
    // Code omitted for brevity
}
```

You can also mix permission requirements and policies. \
Now the user needs to have ``PermissionA`` **or** ``PermissionB`` **and** ``PermissionC`` aswell as fulfill the ``MyCustomPolicy`` policy.
```cs
[AuthorizePermission("PermissionA", "PermissionB")]
[AuthorizePermission("PermissionC")]
[Authorize(Policy = "MyCustomPolicy")]
[HttpGet("GetSecretInformation")]
public IActionResult GetSecretInformation()
{
    // Code omitted for brevity
}
```

### Registering
Make sure to add all ``Permission`` claims to the response:
```cs
builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add(BlazingAuthClaims.Permission.Type);
        options.ApiResources.Single().UserClaims.Add(BlazingAuthClaims.Permission.Type);
    });
```

If you want to use roles with permissions add the following:
```cs
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()   // <-- Needed
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add(BlazingAuthClaims.Permission.Type);
        options.ApiResources.Single().UserClaims.Add(BlazingAuthClaims.Permission.Type);
        // Add role to returned claims
        options.IdentityResources["openid"].UserClaims.Add("role"); 
        options.ApiResources.Single().UserClaims.Add("role");
    });
    
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");
```


## Usage on the client
### Using the attribute
You have the same functionality as on the server (see above) expect for a different syntax.

To secure an action with permissions just add the ``AuthorizePermission`` attribute. \
Only allow viewers with the PermssionA permission
```razor
@attribute [AuthorizePermission("PermssionA")]
```

You can also specify multiple required permissions. \
Here the user needs to have ``PermissionA`` **and** ``PermissionB``.
```razor
@attribute [AuthorizePermission("PermssionA")]
@attribute [AuthorizePermission("PermssionB")]
```

If you only want the user to have one of some permissions, you can just specify multiple permissions with a single attribute. \
In this example the user needs to have ``PermissionA`` **or** ``PermissionB``.
```razor
@attribute [AuthorizePermission("PermssionA", "PermssionB")]
```

You can also mix permission requirements and policies. \
Now the user needs to have ``PermissionA`` **or** ``PermissionB`` **and** ``PermissionC`` aswell as fulfill the ``MyCustomPolicy`` policy.
```razor
@attribute [AuthorizePermission("PermissionA", "PermissionB")]
@attribute [AuthorizePermission("PermissionC")]
@attribute [Authorize(Policy = "MyCustomPolicy")]
```

### Using the AuthorizePermissionView
Don't forget to add this using statement to the ``_Imports.razor`` or current file.
```razor
@using BlazingAuth.Permissions.Client
```

To conditionally show an element only when certain permissions are present, you can wrap your content inside of an ``AuthorizePermissionView`` component:
```xml
<AuthorizePermissionView AndPermissions="MyPermission">
    <p>Only users with the <b>MyPermission</b> permission can see this!</p>
</AuthorizePermissionView>
```

Obviously you can also use the ``Authorizing``, ``NotAuthorized`` and ``Authorized`` child components aswell.
```xml
<AuthorizePermissionView AndPermissions="MyPermission">
    <Authorized>
        <p>Only users with the <b>MyPermission</b> permission can see this!</p>
    </Authorized>

    <NotAuthorized>
        <p>You don't have the needed permission to see this resource :(</p>
    </NotAuthorized>

    <Authorizing>
        <p>Making Pizza and authorizing you, please stand by..</p>
    </Authorizing>
</AuthorizePermissionView>
```

To require multiple permissions you can just specify a comma separated list of permission names:
```xml
<AuthorizePermissionView AndPermissions="PermissionA,PermissionB">
    <p>Only users with PermissionA <b>AND</b> PermissionB can see this!</p>
</AuthorizePermissionView>
```

If you only want the user to have one of some permissions, you can just specify multiple permissions with the ``OrPermissions`` parameter:
```xml
<AuthorizePermissionView OrPermissions="PermissionA,PermissionB">
    <p>Only users with PermissionA <b>OR</b> PermissionB can see this!</p>
</AuthorizePermissionView>
```

Or if you want both:
```xml
<AuthorizePermissionView OrPermissions="PermissionA,PermissionB" AndPermissions="PermissionX,PermissionY">
    <p>Only users with PermissionA <b>OR</b> PermissionB <b>AND</b> PermissionX <b>AND</b> PermissionY can see this!</p>
</AuthorizePermissionView>
```

You can also conveniently use variables with multiple permissions without the hassle of quirky string building:
```cs
// The inheritance here is in not needed, do it according to your liking
class MyPermissions : BlazingAuthClaims.Permission
{
    public const string PermissionA = "PermissionA";
    public const string PermissionB = "PermissionB";
    
    public const string PermissionX = "PermissionX";
    public const string PermissionY = "PermissionY";
}
```

```xml
<AuthorizePermissionView OrPermissions="@MyPermissions.PermissionA">
    <p>Only users with @MyPermissions.PermissionA can see this!</p>
</AuthorizePermissionView>

<AuthorizePermissionView OrPermissionsList="oneOfNeededPermissions">
    <p>Only users with <b>ANY</b> permission inside of the oneOfNeededPermissions array can see this!</p>
</AuthorizePermissionView>

<AuthorizePermissionView OrPermissionsList="oneOfNeededPermissions" AndPermissionsList="allOfNeededPermissions">
    <p>Only users with <b>ANY</b> permission inside of the oneOfNeededPermissions array <b>AND</b> with <b>ALL</b> permissions inside of the allOfNeededPermissions array can see this!</p>
</AuthorizePermissionView>

@code {
    private string[] oneOfNeededPermissions = new string[]
    {
        MyPermissions.PermissionA,
        MyPermissions.PermissionB
    };

    private string[] allOfNeededPermissions = new string[]
    {
        MyPermissions.PermissionX,
        MyPermissions.PermissionY
    };
}
```

### Registering
Add this to your ``Program.cs``:
```cs
builder.Services.AddApiAuthorization()
    .AddAccountClaimsPrincipalFactory<BlazingAuthAccountClaimsPrincipalFactory>();

builder.Services.AddBlazingAuthPermissions();
```

Set the ``Resource`` parameter of ``AuthorizeRouteView`` to ``@routeData``:
```xml
<!-- Before -->
<AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">

<!-- After -->
<AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" Resource="@routeData">
```


## Sample projects
More samples can be found in the [Samples directory](/Samples).


## License
BlazingAuth.Permissions is licensed under Apache License 2.0, see [LICENSE.txt](/LICENSE.txt) for more information.