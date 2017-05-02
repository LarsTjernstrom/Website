# Website
Suite of two apps allowing to create surfaces with blending points and attach the result from different applications in the blending points.

Note: Website only wraps responses where the response resource is `Json` (not null) with a session.

## Requirements

The app requires a user to be signed in in order to work. This can be done with the [SignIn](https://github.com/starcounterapps/signin) app. If there are no users in the database, [create the first user](https://github.com/starcounterapps/signin#create-the-first-user). Otherwise, [sign in an existing user](https://github.com/starcounterapps/signin#sign-in)

## Features

This solution consists of two apps

1. **WebsiteProvider** - wraps the responses according to the configuration stored in the database.
2. **Website CMS** - serves the admin panel. It is used to create the configuration for WebsiteProvider in the database.

### WebsiteProvider

WebsiteProvider wraps the app responses according to the configuration stored in the database.

When the end user visits one of the wrapped URLs (or any URL, if there is a default surface), the response from that URL is wrapped in our surface.

The view-model includes all of the blending points and the filling responses from the pinned URIs.

### Website CMS

Website CMS allows to configure the wrapping of the app responses in surfaces.

CMS interface is divided into four tabs:

#### Surfaces

Surfaces are the HTML documents that contain blending points. A single surface can contain one or more blending points.

Every surface has a **Name** and a path to the **View URI** that defines the presentation of the blending points. This URL can be resolved to a static file or to a dynamic handler, to which a different app responds (try it with Content app!).

A surface wraps only the URIs that are assigned to it using **Catching rules**.

#### Blending points

Blending points define where responses can be attached in a surface. Each blending point has a **Name**.

A blending point can be **Default**, meaning that it will attach the initial response that the surface is wrapping.

#### Catching rules

Catching rules define which requests are wrapped in surfaces.

Each rule defines that a certain entry **Catch URI** should be wrapped in a certain surface.

Catching rules support single wildcard URLs. If **Catch URI** contains a wildcard (`{?}`), it will match a request that contains any value at that place in the URL. Currently are supported the following patterns:
* `/application/resource/{?}` (supports only letters)
* `/application/query?{?}` (supports letters, digits and `%` symbol)

A catching rule can be **Final**, meaning that not any other catching rule will be applied additionally. If a **final** catching rule have an empty **Catch URI** value, it will be forced on any JSON response from any Starcounter app.

#### Pinning rules

Pinning rules define additional requests from any apps and to which blending point they should be inserted.

Each rule defines that for a certain **Catch URI**, a certain **Blending point** should be filled in by **Pin URI** (a request to any Starcounter app).

Pinning rules support single wildcard URLs. If both **Catch URI** and **Pin URI** contain a wildcard (`{?}`), the value at this place in the **Catch URI** will be passed to the **Pin URI**.

If a rule has no value in the **Catch URI** column, it becomes a "catch-all" rule. This means that it is applied for any entry URL.

## Administrative tools

### `/website/cleardata`

Calling this URI deletes all the current app data (surfaces, blending points, catching rules and pinning rules).

### `/website/resetdata`

Calling thes URI replaces all the current app data (surfaces, blending points, catching rules and pinning rules) with the defaults, which are:

- **DefaultTemplate** - a surface with two blending points (TopBar, Main). The default catch-all rule uses this surface.
- **SidebarTemplate** - a surface with two blending points (Left, Right)
- **HolyGrailTemplate** - a surface with five blending points (Header, Left, Content, Right, Footer). Useful if you're building a web site
- **LauncherTemplate** - a surface that looks like the original Starcounter's Launcher app

## CSS Custom Properites

Here is the list of CSS Custom Properties used by the app for themeing

Template         | Name                      | Default   | Description
---              | ---                       | ---       | ---
HolyGrail        | `--holy-grail-chalice`    | `#ffdb3a` | Background color of header
HolyGrail        | `--holy-grail-background` | `#e6e6e6` | Background color of left and right area
HolyGrail        | `--holy-grail-foot`       | `#646464` | Background color of footer area
LauncherTemplate | `--primary-color`         | `#8a98b0` | Color of main area elements, hover color for top and left bar
LauncherTemplate | `--primary-background`    | `#333c4e` | Background color of main area
LauncherTemplate | `--secondary-color`       | `#8a98b0` | Color of top and left bar elements
LauncherTemplate | `--secondary-background`  | `#333c4e` | Background color of top and left bars

## Developer instructions

For developer instructions, go to [CONTRIBUTING](CONTRIBUTING.md).

## License

MIT
