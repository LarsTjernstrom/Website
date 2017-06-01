# Developer instructions

## How to build and run

### Build and run using CLI

1. Clone this repo to your local machine
2. Build the solution by executing `build.bat` in Windows or `./build.bat` in Git Bash
3. Run it by executing `run.bat` in Windows or `./run.bat` in Git Bash
4. Build and run SignIn and Launcher if they are not already running
5. Go to [http://localhost:8080/Website](http://localhost:8080/Website)

### Build and run using Visual Studio

1. Clone this repo to your local machine
2. Open the `.sln` file in Visual Studio
3. Build and run it using Debug in Visual Studio (**Debug** > **Start debugging** or <kbd>F5</kbd>)
4. Build and run SignIn and Launcher if they are not already running
5. Go to [http://localhost:8080/Website](http://localhost:8080/Website)

## Contributing code

To contribute code to this repository, follow the instructions in the [guidelines](https://starcounter.gitbooks.io/guidelines/content/contributing-code.html).

## How to release a package

To release the app to the warehouse, follow the instructions in the [guidelines](https://starcounter.gitbooks.io/guidelines/content/releasing-to-warehouse.html).

## Testing

### Prepare your environment

Before running the steps, you need to:

- Download and install Visual Studio 2015 to run the tests
- Download and install Java, required by Selenium Standalone Server
- Download Selenium Standalone Server and the drivers (Microsoft WebDriver (Edge), Google ChromeDriver (Chrome) and Mozilla GeckoDriver (Firefox)) using the instructions at https://docs.starcounter.io/guides/web-apps/acceptance-testing-with-selenium/
- Add path to the folder with drivers to system path on your computer

### Run the test (from Visual Studio)

1. Start Selenium Remote Driver: `java -jar selenium-server-standalone-3.*.jar`
2. Open `Website.sln` in Visual Studio and enable Test Explorer (Test > Window > Test Explorer)
3. You need to install NUnit 3 Test Adapter in VS addon window in order to see tests in Test Explorer window
3. Start the Website, WebsiteProvider_AcceptanceHelperOne, WebsiteProvider_AcceptanceHelperTwo and WebsiteProvider apps
4. Press "Run all" in Test Explorer
   - If you get an error about some packages not installed, right click on the project in Solution Explorer. Choose "Manage NuGet Packages" and click on "Restore".

### Run the test (from command line)

1. Start Selenium Remote Driver: `java -jar selenium-server-standalone-3.*.jar`
2. Build the solution (build.bat)
3. Start the WebsiteProvider.Tests runner (test.bat)