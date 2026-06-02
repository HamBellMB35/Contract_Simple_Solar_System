An interactive, educational solar system simulation built in Unity using C#. 
Features an automated ScriptableObject data pipeline, real-time unscaled simulation time management, and a highly decoupled, event-driven architecture utilizing the Single Responsibility Principle.

Decoupled Architecture: Implements a modular structure splitting raw inputs, physical flight mechanics, and UI presentation drivers via C# System.Action event streams.

Automated Data Pipeline: Utilizes runtime Resources.Load asset workflows inside custom OnValidate() editor scripts to automatically bind 3D celestial bodies to unique ScriptableObject profiles.

Independent Camera Subsystems: Features responsive camera traversal, smooth momentum acceleration, and custom viewport lens offset framing completely independent of the global time scale.
