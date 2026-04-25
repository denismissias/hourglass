# Hourglass

## Technologies and Tools

- .NET 7
- MySQL 8.0
- Docker

## Getting started

To start API:
1. Go to **/Hourglass** folder
2. Execute `docker-compose up -d`
- The api is running on port 3000
- Swagger link is http://localhost:3000/swagger/index.html

There is an user created in User Table:
- login: admin
- password: admin

There are 10 projects created in Project Table (Ids from 1 to 10)

## Project estimate by tasks

| Task | Description | Time to spend |
| ------ | ------ | ------ |
| Project Planning	 | Understanding the scope and panning of activities | 2h |
| Database modelling | Users, times and projects relationships | 1h |
| Project Setup | Initial AspNet core project setup, with Docker and Docker compose | 8h |
| JWT Authentication | Generate token, validate token, decode token and password encryption, with unit tests | 4h |
| POST /api/v{n}/authenticate | Authentication, with unit tests and swagger documentation | 4h |
| POST /api/v{n}/times | Time register, with unit tests and swagger documentation | 4h |
| GET /api/v{n}/users/{ID} | User get, with unit tests and swagger documentation | 4h |
| POST /api/v{n}/users | User register, with unit tests and swagger documentation | 4h |
| PUT /api/v{n}/users/{ID} | User edit, with unit tests and swagger documentation | 4h |
| GET /api/v{n}/projects | Project list, with unit tests and swagger documentation | 4h |
| GET /api/v{n}/projects/{project_id} | Project get, with unit tests and swagger documentation | 4h |
| POST /api/v{n}/projects | Project register, with unit tests and swagger documentation | 4h |
| PUT /api/v{n}/projects/{project_id} | Project edit, with unit tests and swagger documentation | 4h |
| GET /api/v{n}/times/{project_id} | Time get, with unit tests and swagger documentation | 4h |
| PUT /api/v{n}/times/{time_id} | Time edit, with unit tests and swagger documentation | 4h |
| Project documentation | Steps to execute project in README | 1h |

**Total**: 60 hours.

## Project estimate in days

**Delivery time**: 15 working days (4 working hours per day)

## Suggestions to Next Steps

- Create log structure
- CI/CD Configuration
- Handle error improvements
