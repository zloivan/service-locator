# Unity Service Locator Package

A lightweight and flexible Service Locator implementation for Unity projects. This package provides a simple way to manage dependencies and services across your game, supporting both global and scene-specific service containers.

## Features

- Global and scene-specific service containers
- Easy registration and retrieval of services
- Hierarchical service resolution
- Thread-safe operations
- Unity Editor integration for easy setup

## Installation

To install this package, follow these steps:

1. Open your Unity project
2. Open the Package Manager (Window > Package Manager)
3. Click the "+" button and choose "Add package from git URL"
4. Enter the following URL: `https://github.com/zloivan/service-locator.git`
5. Click "Add"

Alternatively, you can add the following line to your `manifest.json` file in the `Packages` folder of your Unity project:

```json
{
  "dependencies": {
    "com.zloivan.unity-service-locator": "https://github.com/zloivan/ServiceLocator.git"
  }
}
```

## Usage

### Setting up Service Locators

1. **Global Service Locator**:
   - Manually
     - Create an empty GameObject in your main scene
     - Add the `ServiceLocatorGlobal` component to it
   - By context menu
     - Select scene where you want to add Global Service Locator
     - **GameObject** / **ServiceLocator** / **Global**

2. **Scene Service Locator**:
    - Manually
      - Create an empty GameObject in each scene where you want to use scene-specific services
      - Add the `ServiceLocatorScene` component to it
    - By context menu
      - Select scene where you want to add Global Service Locator
      - **GameObject** / **ServiceLocator** / **Scene**

### Registering Services

You can register services in your custom `MonoBehaviour` scripts:

```csharp
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        // Get the appropriate ServiceLocator (Global or Scene)
        var serviceLocator = ServiceLocator.For(this);

        // Register services
        serviceLocator.Register<IPlayerService>(new PlayerService());
        serviceLocator.Register<IEnemyService>(new EnemyService());
    }
}
```

### Retrieving Services

To use a service in your scripts:

```csharp
public class Player : MonoBehaviour
{
    private IPlayerService _playerService;

    private void Start()
    {
        // Get the service
        ServiceLocator.For(this).Get(out _playerService);

        // Use the service
        _playerService.InitializePlayer();
    }
}
```

### Creating Custom Service Locator Bootstrappers

You can create custom bootstrappers to configure services for specific scenes or the global context:

```csharp
public class MySceneBootstrapper : ServiceLocatorBootstrapper
{
    protected override void Bootstrap()
    {
        Container.Register<IMySceneService>(new MySceneService());
        Container.Register<IAnotherService>(new AnotherService());
    }
}
```

## Debug Logs
To turn on additional debug logging, add defined symbols `DEBUG_SERVICE_LOCATOR` into your build settings

## Best Practices

1. Use interfaces for your services to maintain loose coupling
2. Register services as early as possible (e.g., in Awake() methods)
3. Use scene-specific services for functionality that should be isolated to a particular scene
4. Use the global service locator for services that need to persist across multiple scenes

## Examples
This package comes with optional downloadable examples. Examples involve switching scenes, so you're going to need to add scene `Second` to `Scenes In Build` 

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

