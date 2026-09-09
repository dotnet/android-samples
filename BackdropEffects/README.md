---
name: ".NET for Android - Backdrop Effects"
description: "Compares backdrop and foreground render effects using the API 37.2 backdrop blur API."
page_type: sample
languages:
- csharp
products:
- dotnet-android
urlFragment: backdropeffects
---

# Backdrop Effects

This sample demonstrates backdrop render effects introduced in Android 17 QPR2
(API 37.2). An animated scene moves behind a translucent card so the difference
between filtering the backdrop and filtering the view itself is immediately
visible.

## What the sample does

- Applies a blur to pixels behind the translucent card
- Compares backdrop blur with the existing foreground render effect
- Adjusts the blur radius while the scene is animating
- Pauses the animation for close visual comparison

## Screenshots

| Backdrop effect | Foreground effect | No effect |
|---|---|---|
| ![The backdrop render effect blurs the animated scene while leaving the card content crisp](Screenshots/backdrop-effect.png) | ![The foreground render effect blurs the card and its content](Screenshots/foreground-effect.png) | ![The card and animated scene without a render effect](Screenshots/no-effect.png) |

These states were captured on an Android 17 QPR2 Beta 4 emulator at a 5 px
blur radius.

## Requirements

| Requirement | Value |
|---|---|
| Target framework | `net11.0-android37.2` |
| Device or emulator | Android 17 QPR2 / API 37.2 |
| Rendering | Hardware acceleration |
