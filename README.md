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

The Images service uses Cloudflare R2. Configure `CloudflareR2:AccountId`,
`CloudflareR2:S3AccessId`, `CloudflareR2:S3SecretKey`, `CloudflareR2:BucketName`
and `CloudflareR2:PublicUrl` through appsettings or environment variables.
The URL must be HTTPS and point to a publicly readable R2 bucket or custom
domain. Give the R2 API credentials object read/write permissions for that bucket.
Keep credentials outside source control.

The existing `Images` table must match the new R2 model (UUID `Id`, `StorageKey`,
`AltText`, `DisplayOrder`, `IsCover`, `OwnerId`, `OwnerType` and `CreatedAt`).
The previous Cloudflare Images column migration is not applicable to this schema.
Old ImgBB or Cloudflare Images objects are not migrated automatically.

Before deploying the asynchronous Images API, manually run
[`Images/Sql/image-processing.sql`](Images/Sql/image-processing.sql) against
the Images database. Existing records default to `Ready`.

The API stores each original under `raw/` in R2 and records a `Processing` job
in PostgreSQL. A publisher sends only the image ID, R2 keys and owner metadata
to the durable RabbitMQ queue `image-processing`; publisher confirmations and
the database record allow publication to resume after a broker outage. Delivery
may happen more than once. The worker locks the database record during processing
and updates it to `Ready` or `Failed`; deletion takes the same lock. Configure
RabbitMQ with `RabbitMQ__Host`, `RabbitMQ__Port`, `RabbitMQ__VirtualHost`,
`RabbitMQ__Username` and `RabbitMQ__Password`. If credentials are omitted the
client defaults apply. The included Compose service exposes RabbitMQ only on
localhost for development. Configure production credentials outside the repository.

The worker processes one message at a time per instance (prefetch 2).
`GET /api/images/{id}` and image queries return `status`:
`Processing`, `Ready` or `Failed`. Variant URLs are null until `Ready`; the
frontend polls after upload and reports processing failures or timeouts. Originals
are removed after successful processing. Failed uploads retain their raw object
until deleted; a database error during upload can also leave an original whose
commit outcome is unknown. Configure an R2 lifecycle rule for the `raw/` prefix with a
retention period longer than the expected maximum queue outage. Configure the
public domain to deny access to `raw/` originals, which may contain metadata.
With a one-day rule, originals still waiting after one day can disappear and
those images cannot be processed. Monitor queue delays and extend retention if
an outage may last that long. Lifecycle expiration does not restrict public access.

### Images worker with Docker Compose

Create a private `.env` in the backend directory (never commit it):

```dotenv
RabbitMQ__Username=replace-with-a-dedicated-user
RabbitMQ__Password=replace-with-a-strong-password
IMAGE_WORKER_DB_CONNECTION='Host=postgres;Port=5432;Database=your-database;Username=your-user;Password=your-password'
```

Use the same database and credentials as Images, but use `postgres:5432` for
the worker because it connects over the Compose network. The worker reads
R2 settings from the existing `Images/appsettings.json` via a read-only mount;
the file must exist on the Docker host. It is excluded from the build context.
The worker is limited to 1 GiB and starts after RabbitMQ is healthy.

RabbitMQ uses the supplied user on a **new** data volume. For an existing
`rabbitmq-data` volume, configure that same user and its permissions in
RabbitMQ separately; changing the environment does not alter existing users.
Do not delete the volume to apply credentials: it contains durable messages.
Use non-guest credentials, since RabbitMQ restricts guest to loopback.

Start the services from the backend directory:

```sh
bun run docker compose up -d --build rabbitmq images-worker
```

Start Images separately with the same `RabbitMQ__Username` and
`RabbitMQ__Password` in its configuration and `RabbitMQ__Host=localhost`
(when running on the host). Do not use the worker's PostgreSQL host for the
host API. Restart Images after changing its configuration.

Uploads accept files up to 32 MiB each (128 MiB per request); the worker rejects
invalid or animated images and images larger than 40 million pixels. It
stores three WebP versions at widths up to 500, 800 and 1600 pixels for property
and other images, except profiles and energy certificates. Profile images use
two versions, up to 192 pixels for avatars and 560 pixels for the cropper.
Energy certificates use one version up to 560 pixels, served as `fullUrl` for
both the dropdown and cropper; the frontend cropper exports up to 560 pixels
when the source allows it. Small images are not enlarged by the Images service.
New profile keys use a `p2-` filename prefix; existing profile keys retain their
three versions and expose their thumbnail and full URLs as avatar and cropper
URLs. New energy certificate keys use an `e2-` filename prefix; existing keys
retain their three versions and original URLs. Deletion removes the variants
belonging to each key format. The response
includes the relevant URLs plus owner and display metadata.
Multi-file uploads save one file at a time; request errors report previously
saved images. Processing can fail after upload; clients must inspect `status`.
Deleting a property from the frontend deletes its images first;
if image deletion fails the property is retained. If property deletion fails
afterwards, its images have already been deleted and the operation can be retried.
Switching providers later requires an implementation of `IImageStorageService`
that understands existing R2 keys; changing the DI registration alone does not
migrate stored objects.

## Getting Started

Install myhouse-front from another repo