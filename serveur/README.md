# Serveur
PolyPaintPro - Team 07

## Install

Make sure you have Node and npm installed.

```
npm install
```

Then, you will need to create a copy of `.env.example`, and rename that copy to `.env`. This file contains any configuration specific to your setup. In particular, it is important to set a valid Mongo database url or the project will not run.


### Install Node and NPM on Mac

```
ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
brew install node
```

### Install Node and NPM on Windows

Install from [Node.js website](https://nodejs.org/en/).

Verify using

```
node -v
npm -v
```

## Run

```
npm run build
npm start
```

It is also possible to enable automatic recompile when a file changes, but you need to have run `npm run build` at least once beforehand :

```
npm run watch
```

## Test

```
npm run test
```

## Project structure

The NodeJS server supports a REST api as well as a WebSocket endpoint. It uses a remote Mongo database to store data.

Refer to the [Typescript Node Starter](https://github.com/Microsoft/TypeScript-Node-Starter) for framework issues.

## Projects

[:arrow_heading_up: Root](../README.md)

[:point_right: Client léger](../client-leger/)

[:point_right: Client lourd](../client-lourd/)

[:diamonds: Serveur](../serveur/)

[:point_right: Site web](../site-web/)
