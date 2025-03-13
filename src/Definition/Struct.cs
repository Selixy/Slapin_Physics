using UnityEngine;

namespace Physics
{
    public struct CollisionData
    {
        public Vector2 Normal;  // Normale au point de collision
        public float Friction;  // Facteur de friction
        public float Adherence; // Constructeur pour initialiser la structure
        public CollisionData(Vector2 normal, float friction, float adherence)
        {
            this.Normal = normal;
            this.Friction = friction;
            this.Adherence = adherence;
        }
    }
}