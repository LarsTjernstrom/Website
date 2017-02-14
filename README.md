# Website
Suite of two apps allowing to create templates with sections and display result from different applications in the sections.

Note: Website only wraps responses where the response resource is `Json` (not null) with a session.

## Features

This solution consists of two apps

1. **WebsiteProvider** - wraps the responses according to the configuration stored in the database.
2. **Website CMS** - serves the admin panel. It is used to create the configuration for WebsiteProvider in the database.

### WebsiteProvider

WebsiteProvider wraps the app responses according to the configuration stored in the database.

When the end user visits one of the wrapped URLs (or any URL, if there is a default template), the response from that URL is wrapped in our template.

The view-model includes all of the sections and the filling responses from the mapped foreign URLs.

### Website CMS

Website CMS allows to configure the wrapping of the app responses in templates.

CMS is divided into four sections:

#### Templates

Templates are the HTML documents that contain user interface sections. A single template can contain one of more sections.

Every template has a **Name** and a path to the **Html** URI that contains the template. This URL can be resolved to a static file or to a dynamic handler, to which a different app responds (try it with Content app!).

A template can be **Default**, meaning that it will be forced on any JSON response from any Starcounter app.

If a template is not default, then it will wrap only the URIs that are assigned to it using **Maps**.

#### Sections

Sections are the visual regions of a template. Each section has a **Name**.

A section can be **Default**, meaning that it will catch the initial response that the template is wrapping.

#### Urls

Urls are the rules by which a requests is wrapped in templates.

Each rule defines that a certain entry **Url** should be wrapped in a certain template.

Url rules support single wildcard URLs. If **Url** contains a wildcard (`{?}`), it will match a request that contains any value at that place in the URL.

#### Maps

Maps are the rules by which the sections are filled in with content that comes from any app.

Each rule defines that for a certain entry **Url**, a certain **Section** should be filled in by **Foreign url** (a request to any Starcounter app).

Map rules support single wildcard URLs. If both **Url** and **Foreign url** contain a wildcard (`{?}`), the value at this place in the **Url** will be passed to the **Foreign url**.

If a rule has no value in the **Url** column, it becomes a "catch-all" rule. This means that it is applied for any entry URL.

## Sample gateway config

The following Starcounter Gateway config enables URL aliases used in the demo. Put this config to `scnetworkgateway.xml` and call `http://localhost:8181/gw/updateconf` to reload the config.

```xml
<UriAliases>
	<UriAlias>
		<HttpMethod>GET</HttpMethod>
		<FromUri>/user-profile</FromUri>
		<ToUri>/content/dynamic/userprofile</ToUri>
		<Port>8080</Port>
	</UriAlias>

  <UriAlias>
		<HttpMethod>GET</HttpMethod>
		<FromUri>/apps</FromUri>
		<ToUri>/content/dynamic/apps</ToUri>
		<Port>8080</Port>
	</UriAlias>

  <UriAlias>
		<HttpMethod>GET</HttpMethod>
		<FromUri>/apps/wanted-apps</FromUri>
		<ToUri>/content/dynamic/apps/wanted-apps</ToUri>
		<Port>8080</Port>
	</UriAlias>
</UriAliases>
```

## Sample environment setup steps

1. Apply the sample gateway config (see above)
2. Call [http://localhost:8181/gw/updateconf](http://localhost:8181/gw/updateconf) to reload the config
3. Start SignIn
4. Start Website
5. Start Content
6. Start Registration
7. Start UserProfile
8. Call [http://localhost:8080/signin/generateadminuser](http://localhost:8080/signin/generateadminuser) to generate the admin user
9. Call [http://localhost:8080/website/cms](http://localhost:8080/website/cms) to see that Website admin panel works.
  - You can sign in here.
10. Call [http://localhost:8080/content/cms](http://localhost:8080/content/cms) to see that Content admin panel works.
  - You should already be signed in.
  - You can sign out here.
11. Call [http://localhost:8080/apps](http://localhost:8080/apps) to see SignIn, Website, Content, Registration and UserProfile in concerto.

This is how it looks as of Jan 2017:

![docs/signed-out.png](docs/signed-out.png)

## Developer instructions

### How to release a package

This repo comes with a tool that automatically increments the information `package.config` (version number, version date, Starcounter dependency version) and creates a ZIP file containing the packaged app. To use it follow the below steps:

1. Make sure you have installed [Node.js](https://nodejs.org/)
2. Run `npm install` to install the tool (installs grunt, grunt-replace, grunt-bump, grunt-shell)
2. Run `grunt package` to generate a packaged version, (you can use `:minor`, `:major`, etc. as [grunt-bump](https://github.com/vojtajina/grunt-bump) does)
4. The package is created in `packages/<AppName>.zip`. Upload it to the Starcounter App Store

## License

MIT
