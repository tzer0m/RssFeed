# RssFeed

[![Deploy](https://github.com/tzer0m/RssFeed/actions/workflows/deploy.yml/badge.svg)](https://github.com/tzer0m/RssFeed/actions/workflows/deploy.yml)

A minimal, self-hosted RSS feed. Post to it via an API-key-protected endpoint, subscribe to it from any RSS reader.

## Purpose

- `POST /post` (requires an `X-API-Key` header matching the configured key) accepts a title, summary, and content, stamps it with the current UTC time, and stores it in Postgres via EF Core.
- `GET /?count=20` (max 100) returns the most recent posts as an RSS 2.0 XML feed, with full HTML content carried in a `content:encoded` element alongside the plain-text summary in `description`.

There's no UI or auth beyond the single API key — it's designed to be posted to programmatically (e.g. from another of my services) and read passively via a feed reader.

## Tech Stack

- ASP.NET Core Web API on .NET 10, with OpenAPI enabled
- EF Core + Npgsql
- `System.Xml.Linq` for building the RSS document directly, no external feed library

## Configuration

Configuration lives in `appsettings.json` (see `appsettings.git.json` for the shape, values stripped):

```json
{
  "BaseUrl": "",
  "ApiKey": "",
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

`BaseUrl` is used as the feed's `<link>` element.

## Deployment

Deployed via GitHub Actions on push to `master`, using a self-hosted runner on Tyrion. The workflow stops the `RssFeed.service` systemd unit, publishes a fresh build to `/home/tzer0m/Services/RssFeed`, and restarts the service.
