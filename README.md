# Farmer API
This project need [auth service](https://github.com/TzyHuan/SystemAuth) issued jwt for user.

As long as the request is recieved, the `AuthorizationFilter.cs` filters will redirect to the SystemAuth url set in `appsettings.json` to check whether the jwt is valid.

## Feature
* APIs: Let [detector](https://github.com/TzyHuan/RaspberryPi_Weather) save data into sqlite, and [web](https://github.com/TzyHuan/FarmerWeb) can search historical weather data.
* Websocket: Broadcast detected values from sensors by SignalR.

## Deploy
### Prepare

[Install .NET on Linux](https://docs.microsoft.com/zh-tw/dotnet/core/install/linux)

### Set Linux service
1. Create a config
    ```bash
    sudo nano /etc/systemd/system/farmer-api.service
    ```
2. Config sample:
    ```ini
    [Unit]
    Description= FarmerAPI Web API App running on Raspberry pi

    [Service]
    WorkingDirectory=/home/pi/IoT/FarmerAPI
    ExecStart=/opt/dotnet/dotnet /home/pi/IoT/FarmerAPI/FarmerAPI.dll
    Restart=always
    # Restart service after 10 seconds if the dotnet service crashes:
    RestartSec=10
    SyslogIdentifier=dotnet-example
    User=www-data
    Environment=ASPNETCORE_ENVIRONMENT=Production
    Envirionment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

    [Install]
    WantedBy=multi-user.target
    ```
3. Enable and run the service:
    ```bash
    sudo systemctl enable farmer-api.service
    sudo systemctl start farmer-api.service
    ```