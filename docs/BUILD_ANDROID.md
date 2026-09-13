# Building NetWard for Android

NetWard is architected around the platform-independent `NetWard.Core` engine. This document details the Android mobile companion build pipeline.

---

## 1. Architecture

- **Engine:** `NetWard.Core` is 100% compliant with .NET 8.0 Android runtime and standard C# binding generators.
- **Native Android UI:** Planned for v1.0 using Kotlin / Jetpack Compose or .NET Android with material graphite theming.

---

## 2. Compiling Core for Android Target

```bash
dotnet build src/NetWard.Core/NetWard.Core.csproj -c Release
```

---

## 3. Permissions Strategy for Android

When compiling the mobile companion, only the minimum necessary permissions are requested:
- `android.permission.INTERNET` (Required to perform socket and HTTP reachability probes)
- `android.permission.ACCESS_NETWORK_STATE` (Required to detect Wi-Fi vs Cellular link status)

**No location permissions, no background microphone/camera access, and no storage tracking.**
