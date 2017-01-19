# Website
Simple CMS Launcher to create pages with content regions and display result from different applications in the regions.

Note: Website only wraps responses where the response resource is `Json` (not null) with a session.

# Sample gateway config

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

# Sample environment setup steps

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
