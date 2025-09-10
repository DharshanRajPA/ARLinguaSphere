# Firebase Setup (Placeholder)

1. Create a Firebase project arlinguasphere.
2. Enable Authentication (anonymous or email), Realtime Database or Firestore, Analytics, and Crashlytics.
3. Download google-services.json and place it under Assets/ or Assets/Plugins/Android/ per your chosen Unity Firebase SDK.
4. Realtime DB structure suggestion:

```
rooms/{roomId}/anchors/{anchorId} = {
  id: string,
  position: { x, y, z },
  rotation: { x, y, z, w },
  labelKey: string,
  creatorId: string,
  timestamp: number
}
```

Security rules should restrict rooms to authenticated users. Use Firebase Emulators for local dev.
