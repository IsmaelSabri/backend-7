# Backend-7

backend-7 refers to the business layer of a web application to satisfy the purchase and sale of real estate. 

![backend-7](Email/Assets/img/Class%20Diagram1.svg)

* Users: once the request is received, the activities can be divided into two sets of parallel activities. One side returns the selected object while the other is responsible for sending the notification, the email or vice versa. Flows of activities combine to give a response with low coupling and high cohesion between modules.

* Homes: with several objects and many expected behaviors that respond to events, a reasonable simplification has been incurred without representing aggregation in the domain model

* Email: reusable solution for sending email templates.

* Gateway: A single entry point for API consumers to various backend services.


![backend-7](Email/Assets/img/colaboration%20diagram%202.png)


## Setup
0. Install dotnet 7 and docker 
1. Clone or download the repository
2. In Email folder create a file appsettings.json and set your email

![backend-7](Email/Assets/img/email%20appsettings.json.png)

3. In Homes folder create also another file appsettings.json and set these values

![backend-7](Email/Assets/img/home%20appsettings.json%20.png)

4. In linux, some files may give permissions error, in that case can run

```sh
chmod 644 metricbeat.yml
```
```sh
chmod 644 filebeat.yml
```
```sh
chmod 644 logstash.yml
```

5. Next in main folder run

```sh
docker compose up -d
```

6. Back again to Homes folder to create the tables in postgres database
```sh
dotnet ef database update mymigration
```

## Getting Started

Install myhouse-front from another repo