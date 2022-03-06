# TraySignalRWebApp
implement wpf tray icon application and host signalr, communicate between desktop application and web application

## TrayBackgroundApp
Wpf로 제작되는 TrayIcon Application입니다.  
자체 호스팅으로 `TrayBackgroundWeb`을 호스팅합니다.  
따라서 Web Hosting 및 SignalR을 호스팅하여 웹 페이지에서 구성되는 SingalR과 통신하게 되며 로컬에 구성된 다른 App과 통신할 수 있게 됩니다.  

## TrayBackgroundWeb
Asp.net core web api로 자제 호스팅될 서버 입니다.  
SingalR을 호스팅하게 되며 웹앱과 연결될 수 있도록 구현 됩니다.

## TrayBackgroundBlazor
메인 페이지로 사용되면 SingalR을 웹 페이지에서 구현하게 됩니다.  
정확히는 페이지에서 SignalR 클라이언트를 구현하여 `TrayBackgroundWeb`의 SignalR 서버와 연결됩니다.  

위와 같이 3가지로 구성될 경우 `Desktop App` 과 `Web page`가 상호 연결성을 갖을 수 있습니다.  
예로 파일 업로드시에 Desktop Appl을 이용한 대량 파일 업로드 프로그램을 만들경우 ActiveX와 같은 설치 프로그램에 의존하지 않게 됩니다.  
또한 Desktop App과 연결성을 계속적으로 유지하므로 웹 페이지 이벤트와 Application 이벤트가 통신할 수 있게 됩니다.  

