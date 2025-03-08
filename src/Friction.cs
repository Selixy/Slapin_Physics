using UnityEngine;

namespace Physics
{
    public class Friction
    {
        // friction value definition
        private static float frictionBase = 0.00f;
        private static float frictionIce = 0f;
        private static float frictionSticky = 0f;

        // references
        private Physic physic;

        // constructor
        public Friction(Physic physic)
        {
            this.physic = physic;
        }


        public void Update()
        {
            // definition des facteurs de friction
            float frictionX = frictionBase;
            float frictionY = frictionBase;

            // Récupère la vélocité actuelle
            Vector2 currentVelocity = physic.velocity;

            // Calcule la nouvelle vélocité pour chaque axe
            Vector2 newVelocity = new Vector2(
                ComputeVelocityAxis(currentVelocity.x, frictionX),
                ComputeVelocityAxis(currentVelocity.y, frictionY)
            );

            // La force de friction est le changement de vélocité
            physic.velocity = newVelocity;
        }

        private float ComputeVelocityAxis(float currentVelocity, float friction)
        {
            if (currentVelocity > 0) {
                return Mathf.Max(0, currentVelocity - friction);
            }
            else if (currentVelocity < 0) {
                return Mathf.Min(0, currentVelocity + friction);
            }
            else {
                return 0;
            }
        }
    }
}