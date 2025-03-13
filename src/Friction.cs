using UnityEngine;

namespace Physics
{
    public class Friction
    {
        // Référence au système physique
        private Physic physic;
        // Référence au buffer de collisions
        private CollisionBuffer collisionBuffer;

        // Constructeur : on reçoit à la fois le Physic et le CollisionBuffer
        public Friction(Physic physic)
        {
            this.physic = physic;
            this.collisionBuffer = physic.collisionBuffer;
        }

        public void Update()
        {
            // Initialiser la friction sur chaque axe
            float frictionX = 0f;
            float frictionY = 0f;

            // Parcourir toutes les collisions stockées dans CollisionBuffer
            foreach (CollisionData collision in collisionBuffer.CollisionList)
            {
                Vector2 normal = collision.Normal;
                float frictionForce = collision.Friction;

                // Calcul de l'influence de la friction sur chaque axe
                frictionX += Mathf.Abs(normal.y) * frictionForce;
                frictionY += Mathf.Abs(normal.x) * frictionForce;
            }
            frictionY *= Surfaces.VerticalFactor;

            // Récupérer la vitesse actuelle
            Vector2 currentVelocity = physic.velocity;

            // Calculer la nouvelle vitesse sur chaque axe en tenant compte de la friction
            Vector2 newVelocity = new Vector2(
                ComputeVelocityAxis(currentVelocity.x, frictionX),
                ComputeVelocityAxis(currentVelocity.y, frictionY)
            );

            // Appliquer la nouvelle vitesse
            physic.velocity = newVelocity;
        }

        // Calcule la nouvelle vitesse sur un axe en fonction de la friction appliquée
        private float ComputeVelocityAxis(float currentVelocity, float friction)
        {
            if (currentVelocity > friction)
            {
                return Mathf.Max(0, currentVelocity - friction);
            }
            else if (currentVelocity < -friction)
            {
                return Mathf.Min(0, currentVelocity + friction);
            }
            else
            {
                return 0;
            }
        }
    }
}
