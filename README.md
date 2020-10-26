# Projectile Component
This is a unity component that controls a projectile

Features
- Penetration
  the projectile will get the reletive thickness of any object it hits and will only go through it if it's penetration value is greater then the thickness of the object.

- Ricochet(in development)
  the projectile will reflect off of any object if the angle hit is equal to or smaller then the set ricochet angle.
  
Usage
- download the unity pakage and import it into your project
- create a new C# class that inherits from ProjectileController
- use the overridable methods to implement your custom logic
  
Thechnologies used
- C#
- F#
