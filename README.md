**(2019-02-19)** This repo is now archived as it is not being maintained. Please feel free to fork this repo if you would like to re-use it.

# LOG3900
PolyPaintPro - Team 07

[![Server Build Status](https://travis-ci.com/dizco/LOG3900.svg?token=xpqeNSHJ4NVgmZxfGuMR&branch=master)](https://travis-ci.com/dizco/LOG3900) [<img src="https://dizco.visualstudio.com/_apis/public/build/definitions/550f4e70-4933-45fa-ac7f-160f25cd27b8/2/badge"/>](https://dizco.visualstudio.com/LOG3900/_build/index?definitionId=2)

PolyPaintPro is client-server ecosystem which offers a Windows desktop app, as well as an iOS app, built with an iPad mini 4 in mind. Both apps offer collaborative drawing features and communicate through a single server. The website allows to view the public gallery.

## Install

```
git clone https://github.com/dizco/LOG3900.git
```

## Run

Refer to each project's README.

## Repo structure

This is a monolithic repository containing 4 sub-projects.

1. Server (referred to as _Serveur_) : NodeJS server
2. Thin client (referred to as _Client Léger_) : Swift iOS app
3. Fat client (referred to as _Client Lourd_) : C# WPF client
4. Website (referred to as _Site Web_) : Pure HTML/CSS/JS website

## Visit the [wiki](https://github.com/dizco/LOG3900/wiki) for contributing guide, as well as other coding conventions.

## Projects

[:diamonds: Root](./README.md)

[:point_right: Client léger](client-leger/)

[:point_right: Client lourd](client-lourd/)

[:point_right: Serveur](serveur/)

[:point_right: Site web](site-web/)
